using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Entities;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class FavoriteDirectoriesGetJob
        : AbstractTwoWayJob<FavoriteDirectoriesGetParameter, ListResult<FileShallowInfoEntity>>
    {
        protected override Task Execute(FavoriteDirectoriesGetParameter param)
        {

            var fileList = this.GetOrCreateFileList();

            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ListResult<FileShallowInfoEntity>();
            foreach (var file in fileList)
            {
                this.ThrowIfJobCancellationRequested();

                if (infoList.Count >= param.Count)
                {
                    break;
                }

                if (param.IsOnlyDirectory
                    && string.IsNullOrEmpty(file))
                {
                    continue;
                }

                try
                {
                    var info = getInfoLogic.Get(file, true);
                    if (info != FileShallowInfoEntity.EMPTY)
                    {
                        infoList.Add(info);
                    }
                }
                catch (FileUtilException ex)
                {
                    this.WriteErrorLog(ex);
                    continue;
                }
            }

            this.Callback(infoList);

            return Task.CompletedTask;
        }

        private string[] GetOrCreateFileList()
        {
            using (var con = Instance<IFileInfoDB>.Value.Connect())
            {
                var logic = new FavoriteDirectoriesGetLogic(this);
                var fileList = logic.Execute(con);
                if (fileList.Length > 0)
                {
                    return fileList;
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
