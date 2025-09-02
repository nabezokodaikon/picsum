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

        protected override ValueTask Execute(FilesGetByRatingParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ConcurrentBag<FileShallowInfoEntity>();
            var dtos = this.GetFiles(param.RatingValue);

            using (TimeMeasuring.Run(true, "FilesGetByRatingJob Parallel.ForEach"))
            {
                using (var cts = new CancellationTokenSource())
                {
                    try
                    {
                        Parallel.ForEach(
                            dtos,
                            new ParallelOptions
                            {
                                CancellationToken = cts.Token,
                                MaxDegreeOfParallelism = MAX_DEGREE_OF_PARALLELISM,
                            },
                            dto =>
                            {
                                if (this.IsJobCancel)
                                {
                                    cts.Cancel();
                                    cts.Token.ThrowIfCancellationRequested();
                                }

                                try
                                {
                                    var info = getInfoLogic.Get(
                                        dto.FilePath, param.IsGetThumbnail, dto.RegistrationDate);
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

            this.Callback([.. infoList]);

            return ValueTask.CompletedTask;
        }

        private FileByRatingDto[] GetFiles(int ratingValue)
        {
            using (var con = Instance<IFileInfoDao>.Value.Connect())
            {
                var logic = new FilesGetByRatingLogic(this);
                return logic.Execute(con, ratingValue);
            }
        }
    }
}
