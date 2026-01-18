using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Entities;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Collections.Concurrent;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// ファイルをフォルダで検索します。
    /// </summary>

    public sealed class FilesGetByDirectoryJob
        : AbstractTwoWayJob<FilesGetByDirectoryParameter, DirectoryGetResult>
    {
        private const int MAX_DEGREE_OF_PARALLELISM = 8;

        protected override async ValueTask Execute(FilesGetByDirectoryParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            if (string.IsNullOrEmpty(param.DirectoryPath))
            {
                this.Callback(new DirectoryGetResult()
                {
                    DirectoryPath = string.Empty,
                    DirectoryState = DirectoryStateParameter.EMPTY,
                    FileInfoList = [],
                });

                return;
            }

            var result = new DirectoryGetResult
            {
                DirectoryPath = param.DirectoryPath
            };

            var files = FileUtil.GetFileSystemEntriesArray(param.DirectoryPath);
            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ConcurrentBag<FileShallowInfoEntity>();

            using (Measuring.Time(true, "FilesGetByDirectoryJob Parallel.ForEachAsync"))
            {
                using (var cts = new CancellationTokenSource())
                {
                    try
                    {
                        await Parallel.ForEachAsync(
                            files,
                            new ParallelOptions
                            {
                                CancellationToken = cts.Token,
                                MaxDegreeOfParallelism = MAX_DEGREE_OF_PARALLELISM,
                            },
                            async (file, token) =>
                            {
                                token.ThrowIfCancellationRequested();

                                if (this.IsJobCancel)
                                {
                                    await cts.CancelAsync().False();
                                    return;
                                }

                                try
                                {
                                    var info = await getInfoLogic.Get(file, param.IsGetThumbnail).False();
                                    if (!info.IsEmpty)
                                    {
                                        infoList.Add(info);
                                    }
                                }
                                catch (FileUtilException ex)
                                {
                                    this.WriteErrorLog(ex);
                                }
                                catch (ObjectDisposedException)
                                {
                                    await cts.CancelAsync().False();
                                    return;
                                }
                            }).False();
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                }
            }

            var con = await Instance<IFileInfoDao>.Value.Connect().False();
            try
            {
                var getDirectoryStateLogic = new DirectoryStateGetLogic(this);
                var directoryState = await getDirectoryStateLogic.Execute(con, param.DirectoryPath).False();
                result.FileInfoList = [.. infoList];
                result.DirectoryState = directoryState;
            }
            finally
            {
                await con.DisposeAsync().False();
            }

            this.Callback(result);
        }
    }
}
