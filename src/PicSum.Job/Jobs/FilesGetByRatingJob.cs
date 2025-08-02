using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Dto;
using PicSum.Job.Entities;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Collections.Concurrent;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// ファイルを評価値で検索します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class FilesGetByRatingJob
        : AbstractTwoWayJob<FilesGetByRatingParameter, ListResult<FileShallowInfoEntity>>
    {
        protected override ValueTask Execute(FilesGetByRatingParameter param)
        {
            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ConcurrentBag<FileShallowInfoEntity>();
            var dtos = this.GetFiles(param.RatingValue);

            using (TimeMeasuring.Run(true, "FilesGetByRatingJob FileShallowInfoGetLogic"))
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
                                MaxDegreeOfParallelism = dtos.Length,
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
                                    if (info != FileShallowInfoEntity.EMPTY)
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
                    catch (OperationCanceledException) { }
                }
            }

            this.Callback([.. infoList]);

            return ValueTask.CompletedTask;
        }

        private FileByRatingDto[] GetFiles(int ratingValue)
        {
            using (var con = Instance<IFileInfoDB>.Value.Connect())
            {
                var logic = new FilesGetByRatingLogic(this);
                return logic.Execute(con, ratingValue);
            }
        }
    }
}
