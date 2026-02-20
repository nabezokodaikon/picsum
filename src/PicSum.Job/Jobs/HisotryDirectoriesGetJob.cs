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

    public sealed class HisotryDirectoriesGetJob
        : AbstractTwoWayJob<ListResult<FileShallowInfoEntity>>
    {
        protected override async ValueTask Execute()
        {
            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ConcurrentBag<FileShallowInfoEntity>();
            var dtos = await this.GetHisotryDirectories().False();

            using (Measuring.Time(true, "BookmarksGetJob Parallel.ForEachAsync"))
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
                                        dto.DirectoryPath, true, dto.ViewDate).False();
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

        private async ValueTask<DirectoryViewHistoryDto[]> GetHisotryDirectories()
        {
            using (Measuring.Time(true, "HisotryDirectoriesGetJob.GetHisotryDirectories"))
            {
                var con = await Instance<IFileInfoDao>.Value.Connect().False();
                try
                {
                    var logic = new DirectoryViewHistoryGetLogic(this);
                    return await logic.Execute(con).False();
                }
                finally
                {
                    await con.DisposeAsync().False();
                }
            }
        }
    }
}
