using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Entities;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class FavoriteDirectoriesGetJob
        : AbstractTwoWayJob<FavoriteDirectoriesGetParameter, ListResult<FileShallowInfoEntity>>
    {
        protected override async ValueTask Execute(FavoriteDirectoriesGetParameter param)
        {
            var dtos = await this.GetOrCreateFileList().WithConfig();
            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new List<FileShallowInfoEntity>();

            using (TimeMeasuring.Run(true, "FavoriteDirectoriesGetJob ForEach"))
            {
                foreach (var dto in dtos)
                {
                    this.ThrowIfJobCancellationRequested();

                    if (infoList.Count >= param.Count)
                    {
                        break;
                    }

                    if (dto.Value is null
                        || FileUtil.IsSystemRoot(dto.Value)
                        || FileUtil.IsExistsDrive(dto.Value))
                    {
                        continue;
                    }

                    try
                    {
                        var info = await getInfoLogic.Get(dto.Value, true).WithConfig();
                        if (info != FileShallowInfoEntity.EMPTY)
                        {
                            infoList.Add(info);
                        }
                    }
                    catch (FileUtilException ex)
                    {
                        this.WriteErrorLog(ex);
                    }
                }
            }

            this.Callback([.. infoList]);
        }

        private async ValueTask<SingleValueDto<string>[]> GetOrCreateFileList()
        {
            await using (var con = await Instance<IFileInfoDB>.Value.Connect().WithConfig())
            {
                var logic = new FavoriteDirectoriesGetLogic(this);
                var dtos = logic.Execute(con);
                if (dtos.Length > 0)
                {
                    return dtos;
                }
            }

            await using (var con = await Instance<IFileInfoDB>.Value.ConnectWithTransaction().WithConfig())
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

            return await this.GetOrCreateFileList();
        }
    }
}
