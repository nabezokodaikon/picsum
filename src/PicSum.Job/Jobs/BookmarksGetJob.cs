using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Dto;
using PicSum.Job.Entities;
using PicSum.Job.Logics;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Collections.Concurrent;

namespace PicSum.Job.Jobs
{

    public sealed class BookmarksGetJob
        : AbstractTwoWayJob<ListResult<FileShallowInfoEntity>>
    {
        private const int MAX_DEGREE_OF_PARALLELISM = 8;

        protected override ValueTask Execute()
        {
            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ConcurrentBag<FileShallowInfoEntity>();
            var dtos = this.GetBookmarks();

            using (TimeMeasuring.Run(true, "BookmarksGetJob Parallel.ForEach"))
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
                                        dto.FilePath, true, dto.RegistrationDate);
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
                    catch (OperationCanceledException)
                    {
                        return ValueTask.CompletedTask;
                    }
                }
            }

            this.Callback([.. infoList]);

            return ValueTask.CompletedTask;
        }

        private BookmarkDto[] GetBookmarks()
        {
            using (var con = Instance<IFileInfoDB>.Value.Connect())
            {
                var getBookmarkLogic = new BookmarksGetLogic(this);
                return getBookmarkLogic.Execute(con);
            }
        }
    }
}
