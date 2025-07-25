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
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class FavoriteDirectoriesGetJob
        : AbstractTwoWayJob<FavoriteDirectoriesGetParameter, ListResult<FileShallowInfoEntity>>
    {
        private const int MAX_DEGREE_OF_PARALLELISM = 8;

        protected override Task Execute(FavoriteDirectoriesGetParameter param)
        {
            var dtos = this.GetOrCreateFileList();
            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ConcurrentBag<(long ViewCount, FileShallowInfoEntity info)>();

            using (TimeMeasuring.Run(true, "FavoriteDirectoriesGetJob FileShallowInfoGetLogic"))
            {
                using (var cts = new CancellationTokenSource())
                {
                    try
                    {
                        Parallel.ForEach(
                            dtos.AsEnumerable()
                                .OrderBy(dto => dto.DirectoryPath)
                                .OrderByDescending(dto => dto.ViewCount),
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

                                if (infoList.Count >= param.Count)
                                {
                                    cts.Cancel();
                                    cts.Token.ThrowIfCancellationRequested();
                                }

                                try
                                {
                                    var info = getInfoLogic.Get(dto.DirectoryPath, true);
                                    if (info != FileShallowInfoEntity.EMPTY)
                                    {
                                        infoList.Add((dto.ViewCount, info));
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

            this.Callback([.. infoList
                .AsEnumerable()
                .OrderBy(info => info.info.FilePath)
                .OrderByDescending(info => info.ViewCount)
                .Take(param.Count)
                .Select(info => info.info)]);

            return Task.CompletedTask;
        }

        private FavoriteDirecotryDto[] GetOrCreateFileList()
        {
            using (var con = Instance<IFileInfoDB>.Value.Connect())
            {
                var logic = new FavoriteDirectoriesGetLogic(this);
                var dtos = logic.Execute(con);
                if (dtos.Length > 0)
                {
                    return dtos;
                }
            }

            using (var con = Instance<IFileInfoDB>.Value.ConnectWithTransaction())
            {
                var parentDir = FileUtil.GetParentDirectoryPath(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));

                foreach (var dirPath in FileUtil.GetSubDirectoriesArray(parentDir, true))
                {
                    this.ThrowIfJobCancellationRequested();

                    var incrementDirectoryViewCounter = new DirectoryViewCounterIncrementLogic(this);
                    if (!incrementDirectoryViewCounter.Execute(con, dirPath))
                    {
                        var updateFileMaster = new FileMastercUpdateLogic(this);
                        if (!updateFileMaster.Execute(con, dirPath))
                        {
                            var addFileMaster = new FileMasterAddLogic(this);
                            addFileMaster.Execute(con, dirPath);
                        }

                        var addDirectoryViewCounter = new DirectoryViewCounterAddLogic(this);
                        addDirectoryViewCounter.Execute(con, dirPath);
                    }
                }

                con.Commit();
            }

            return this.GetOrCreateFileList();
        }
    }
}
