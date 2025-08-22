using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Entities;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.FileAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{

    public sealed class FavoriteDirectoriesGetJob
        : AbstractTwoWayJob<FavoriteDirectoriesGetParameter, ListResult<FileShallowInfoEntity>>
    {
        protected override async ValueTask Execute(FavoriteDirectoriesGetParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

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
                        if (!info.IsEmpty)
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
            await using (var con = await Instance<IFileInfoDao>.Value.Connect().WithConfig())
            {
                var logic = new FavoriteDirectoriesGetLogic(this);
                var dtos = await logic.Execute(con).WithConfig();
                if (dtos.Length > 0)
                {
                    return dtos;
                }
            }

            await using (var con = await Instance<IFileInfoDao>.Value.ConnectWithTransaction().WithConfig())
            {
                var parentDir = FileUtil.GetParentDirectoryPath(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));

                var addFileMaster = new FileMasterAddLogic(this);
                var incrementDirectoryViewCounter = new DirectoryViewCounterIncrementLogic(this);

                foreach (var dirPath in FileUtil.GetSubDirectoriesArray(parentDir, true))
                {
                    this.ThrowIfJobCancellationRequested();

                    if (!await incrementDirectoryViewCounter.Execute(con, dirPath).WithConfig())
                    {
                        await addFileMaster.Execute(con, dirPath).WithConfig();
                        await incrementDirectoryViewCounter.Execute(con, dirPath).WithConfig();
                    }
                }

                await con.Commit().WithConfig();
            }

            return await this.GetOrCreateFileList().WithConfig();
        }
    }
}
