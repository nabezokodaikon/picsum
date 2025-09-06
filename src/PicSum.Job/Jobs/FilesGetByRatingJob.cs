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
                                MaxDegreeOfParallelism = MAX_DEGREE_OF_PARALLELISM,
                            },
                            async (dto, _) =>
                            {
                                if (this.IsJobCancel)
                                {
                                    await cts.CancelAsync().False();
                                    cts.Token.ThrowIfCancellationRequested();
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
