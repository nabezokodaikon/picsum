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

        protected override ValueTask Execute(FavoriteDirectoriesGetParameter param)
        {
            var dtos = this.GetOrCreateFileList();
            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ConcurrentBag<(long ViewCount, FileShallowInfoEntity info)>();

            using (TimeMeasuring.Run(true, "FavoriteDirectoriesGetJob FileShallowInfoGetLogic"))
            {
                foreach (var dto in dtos)
                {
                    if (this.IsJobCancel)
                    {
                        break;
                    }

                    if (infoList.Count >= param.Count)
                    {
                        break;
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
                }
            }

            this.Callback([.. infoList
                .AsEnumerable()
                .OrderBy(info => info.info.FilePath)
                .OrderByDescending(info => info.ViewCount)
                .Take(param.Count)
                .Select(info => info.info)]);

            return ValueTask.CompletedTask;
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

                var addFileMaster = new FileMasterAddLogic(this);
                var incrementDirectoryViewCounter = new DirectoryViewCounterIncrementLogic(this);

                foreach (var dirPath in FileUtil.GetSubDirectoriesArray(parentDir, true))
                {
                    this.ThrowIfJobCancellationRequested();

                    if (!incrementDirectoryViewCounter.Execute(con, dirPath))
                    {
                        addFileMaster.Execute(con, dirPath);
                        incrementDirectoryViewCounter.Execute(con, dirPath);
                    }
                }

                con.Commit();
            }

            return this.GetOrCreateFileList();
        }
    }
}
