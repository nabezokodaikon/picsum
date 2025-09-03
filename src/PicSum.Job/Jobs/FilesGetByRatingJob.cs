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
        private const int MAX_DEGREE_OF_PARALLELISM = 8;

        protected override async ValueTask Execute(FilesGetByRatingParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ConcurrentBag<FileShallowInfoEntity>();
            var dtos = await this.GetFiles(param.RatingValue).WithConfig();

            using (TimeMeasuring.Run(true, "FilesGetByRatingJob Parallel.ForEachAsync"))
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
                                MaxDegreeOfParallelism = MAX_DEGREE_OF_PARALLELISM,
                            },
                            async (dto, _) =>
                            {
                                if (this.IsJobCancel)
                                {
                                    await cts.CancelAsync().WithConfig();
                                    cts.Token.ThrowIfCancellationRequested();
                                }

                                try
                                {
                                    var info = await getInfoLogic.Get(
                                        dto.FilePath, param.IsGetThumbnail, dto.RegistrationDate).WithConfig();
                                    if (!info.IsEmpty)
                                    {
                                        infoList.Add(info);
                                    }
                                }
                                catch (FileUtilException ex)
                                {
                                    this.WriteErrorLog(ex);
                                }
                            }).WithConfig();
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
            await using (var con = await Instance<IFileInfoDao>.Value.Connect().WithConfig())
            {
                var logic = new FilesGetByRatingLogic(this);
                return await logic.Execute(con, ratingValue).WithConfig();
            }
        }
    }
}
