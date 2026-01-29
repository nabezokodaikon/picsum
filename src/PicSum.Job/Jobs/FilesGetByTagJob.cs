using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Dto;
using PicSum.Job.Entities;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Collections.Concurrent;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// ファイルをタグで検索します。
    /// </summary>

    public sealed class FilesGetByTagJob
        : AbstractTwoWayJob<FilesGetByTagParameter, ListResult<FileShallowInfoEntity>>
    {
        protected override async ValueTask Execute(FilesGetByTagParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            if (string.IsNullOrEmpty(param.Tag))
            {
                throw new NotSupportedException("タグが設定されていません。");
            }

            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ConcurrentBag<FileShallowInfoEntity>();
            var dtos = await this.GetFiles(param.Tag).False();

            using (Measuring.Time(true, "FilesGetByTagJob Parallel.ForEachAsync"))
            {
                using (var cts = new CancellationTokenSource())
                {
                    try
                    {
                        await Parallel.ForEachAsync(
                            dtos,
                            new ParallelOptions
                            {
                                CancellationToken = cts.Token,
                                MaxDegreeOfParallelism = AppConstants.GetHeavyMaxDegreeOfParallelism(dtos),
                            },
                            async (dto, token) =>
                            {
                                token.ThrowIfCancellationRequested();

                                if (this.IsJobCancel)
                                {
                                    await cts.CancelAsync().False();
                                    return;
                                }

                                try
                                {
                                    var info = await getInfoLogic.Get(
                                        dto.FilePath, param.IsGetThumbnail, dto.RegistrationDate).False();
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

            this.Callback([.. infoList]);
        }

        private async ValueTask<FileByTagDto[]> GetFiles(string tag)
        {
            using (Measuring.Time(true, "FilesGetByTagJob.GetFiles"))
            {
                var con = await Instance<IFileInfoDao>.Value.Connect().False();
                try
                {
                    var logic = new FilesGetByTagLogic(this);
                    return await logic.Execute(con, tag).False();
                }
                finally
                {
                    await con.DisposeAsync().False();
                }
            }
        }
    }
}
