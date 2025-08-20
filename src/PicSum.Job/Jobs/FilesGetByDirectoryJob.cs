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

        protected override ValueTask Execute(FilesGetByDirectoryParameter param)
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

                return ValueTask.CompletedTask;
            }

            var result = new DirectoryGetResult
            {
                DirectoryPath = param.DirectoryPath
            };

            var files = FileUtil.GetFileSystemEntriesArray(param.DirectoryPath);
            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ConcurrentBag<FileShallowInfoEntity>();

            using (TimeMeasuring.Run(true, "FilesGetByDirectoryJob Parallel.ForEach"))
            {
                using (var cts = new CancellationTokenSource())
                {
                    try
                    {
                        Parallel.ForEach(
                            files,
                            new ParallelOptions
                            {
                                CancellationToken = cts.Token,
                                MaxDegreeOfParallelism = MAX_DEGREE_OF_PARALLELISM,
                            },
                            file =>
                            {
                                if (this.IsJobCancel)
                                {
                                    cts.Cancel();
                                    cts.Token.ThrowIfCancellationRequested();
                                }

                                try
                                {
                                    var info = getInfoLogic.Get(file);
                                    if (!info.IsEmpty)
                                    {
                                        infoList.Add(info);
                                    }
                                }
                                catch (FileUtilException ex)
                                {
                                    this.WriteErrorLog(ex);
                                }
                            });
                    }
                    catch (OperationCanceledException)
                    {
                        return ValueTask.CompletedTask;
                    }
                }
            }

            using (var con = Instance<IFileInfoDB>.Value.Connect())
            {
                var getDirectoryStateLogic = new DirectoryStateGetLogic(this);
                var directoryState = getDirectoryStateLogic.Execute(con, param.DirectoryPath);
                result.FileInfoList = [.. infoList];
                result.DirectoryState = directoryState;
            }

            this.Callback(result);

            return ValueTask.CompletedTask;
        }
    }
}
