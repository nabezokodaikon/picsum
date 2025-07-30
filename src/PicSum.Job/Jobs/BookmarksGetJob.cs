using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Dto;
using PicSum.Job.Entities;
using PicSum.Job.Logics;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Collections.Concurrent;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class BookmarksGetJob
        : AbstractTwoWayJob<ListResult<FileShallowInfoEntity>>
    {
        protected override Task Execute()
        {
            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ConcurrentBag<FileShallowInfoEntity>();
            var dtos = this.GetBookmarks();

            using (TimeMeasuring.Run(true, "BookmarksGetJob FileShallowInfoGetLogic"))
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
                    catch (OperationCanceledException) { }
                }
            }

            this.Callback([.. infoList]);

            return Task.CompletedTask;
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
