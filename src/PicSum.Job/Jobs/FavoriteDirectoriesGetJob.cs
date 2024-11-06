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
        protected override void Execute(FavoriteDirectoriesGetParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            var fileList = this.GetOrCreateFileList();

            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ListResult<FileShallowInfoEntity>();
            foreach (var file in fileList)
            {
                this.CheckCancel();

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
                    var info = getInfoLogic.Execute(file, true);
                    if (info != null)
                    {
                        infoList.Add(info);
                    }
                }
                catch (FileUtilException ex)
                {
                    this.WriteErrorLog(new JobException(this.ID, ex));
                    continue;
                }
            }

            this.Callback(infoList);
        }

        private IList<string> GetOrCreateFileList()
        {
            var logic = new FavoriteDirectoriesGetLogic(this);
            var fileList = logic.Execute()
                .Where(_ => FileUtil.CanAccess(_) && FileUtil.IsDirectory(_))
                .ToArray();
            if (fileList.Length > 0)
            {
                return fileList;
            }

            using (var tran = Instance<IFileInfoDB>.Value.BeginTransaction())
            {
                var parentDir = FileUtil.GetParentDirectoryPath(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));

                foreach (var dirPath in FileUtil.GetSubDirectories(parentDir)
                    .Where(_ => FileUtil.CanAccess(_) && FileUtil.IsDirectory(_)))
                {
                    this.CheckCancel();

                    var incrementDirectoryViewCounter = new DirectoryViewCounterIncrementLogic(this);
                    if (!incrementDirectoryViewCounter.Execute(dirPath))
                    {
                        var updateFileMaster = new FileMastercUpdateLogic(this);
                        if (!updateFileMaster.Execute(dirPath))
                        {
                            var addFileMaster = new FileMasterAddLogic(this);
                            addFileMaster.Execute(dirPath);
                        }

                        var addDirectoryViewCounter = new DirectoryViewCounterAddLogic(this);
                        addDirectoryViewCounter.Execute(dirPath);
                    }
                }

                tran.Commit();
            }

            return this.GetOrCreateFileList();
        }
    }
}
