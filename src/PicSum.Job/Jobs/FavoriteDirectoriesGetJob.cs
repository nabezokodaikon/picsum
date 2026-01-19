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

            var dtos = await this.GetOrCreateFileList().False();
            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new List<FileShallowInfoEntity>();

            using (Measuring.Time(true, "FavoriteDirectoriesGetJob ForEach"))
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
                        var info = await getInfoLogic.Get(dto.Value, true).False();
                        if (!info.IsEmpty)
                        {
                            infoList.Add(info);
                        }
                    }
                    catch (FileUtilException ex)
                    {
                        this.WriteErrorLog(ex);
                        continue;
                    }
                    catch (ObjectDisposedException)
                    {
                        continue;
                    }
                }
            }

            this.Callback([.. infoList]);
        }

        private async ValueTask<SingleValueDto<string>[]> GetOrCreateFileList()
        {
            var con = await Instance<IFileInfoDao>.Value.Connect().False();
            try
            {
                var logic = new FavoriteDirectoriesGetLogic(this);
                var dtos = await logic.Execute(con).False();
                if (dtos.Length > 0)
                {
                    return dtos;
                }
            }
            finally
            {
                await con.DisposeAsync().False();
            }

            var transactionConnection = await Instance<IFileInfoDao>.Value.ConnectWithTransaction().False();
            try
            {
                var parentDir = FileUtil.GetParentDirectoryPath(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));

                var addFileMaster = new FileMasterAddLogic(this);
                var incrementDirectoryViewCounter = new DirectoryViewCounterIncrementLogic(this);

                foreach (var dirPath in FileUtil.GetSubDirectoriesArray(parentDir, true))
                {
                    this.ThrowIfJobCancellationRequested();

                    if (!await incrementDirectoryViewCounter.Execute(transactionConnection, dirPath).False())
                    {
                        await addFileMaster.Execute(transactionConnection, dirPath).False();
                        await incrementDirectoryViewCounter.Execute(transactionConnection, dirPath).False();
                    }
                }
            }
            finally
            {
                await transactionConnection.DisposeAsync().False();
            }

            return await this.GetOrCreateFileList().False();
        }
    }
}
