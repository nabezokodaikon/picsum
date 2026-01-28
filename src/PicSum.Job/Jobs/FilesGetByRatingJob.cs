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
    /// ファイルを評価値で検索します。
    /// </summary>

    public sealed class FilesGetByRatingJob
        : AbstractTwoWayJob<FilesGetByRatingParameter, ListResult<FileShallowInfoEntity>>
    {
        protected override async ValueTask Execute(FilesGetByRatingParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ConcurrentBag<FileShallowInfoEntity>();
            var dtos = await this.GetFiles(param.RatingValue).False();

            using (Measuring.Time(true, "FilesGetByRatingJob Parallel.ForEachAsync"))
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
                                MaxDegreeOfParallelism = Math.Min(dtos.Length, AppConstants.MAX_DEGREE_OF_PARALLELISM),
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

        private async ValueTask<FileByRatingDto[]> GetFiles(int ratingValue)
        {
            using (Measuring.Time(true, "FilesGetByRatingJob.GetFiles"))
            {
                var con = await Instance<IFileInfoDao>.Value.Connect().False();
                try
                {
                    var logic = new FilesGetByRatingLogic(this);
                    return await logic.Execute(con, ratingValue).False();
                }
                finally
                {
                    await con.DisposeAsync().False();
                }
            }
        }
    }
}
