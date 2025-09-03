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

        protected override async ValueTask Execute()
        {
            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ConcurrentBag<FileShallowInfoEntity>();
            var dtos = await this.GetBookmarks().WithConfig();

            using (TimeMeasuring.Run(true, "BookmarksGetJob Parallel.ForEachAsync"))
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
                                        dto.FilePath, true, dto.RegistrationDate).WithConfig();
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

        private async ValueTask<BookmarkDto[]> GetBookmarks()
        {
            await using (var con = await Instance<IFileInfoDao>.Value.Connect().WithConfig())
            {
                var getBookmarkLogic = new BookmarksGetLogic(this);
                return await getBookmarkLogic.Execute(con).WithConfig();
            }
        }
    }
}
