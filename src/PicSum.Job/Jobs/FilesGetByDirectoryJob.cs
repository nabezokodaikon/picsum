using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Entities;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// ファイルをフォルダで検索します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class FilesGetByDirectoryJob
        : AbstractTwoWayJob<FilesGetByDirectoryParameter, DirectoryGetResult>
    {
        protected override Task Execute(FilesGetByDirectoryParameter param)
        {
            if (string.IsNullOrEmpty(param.DirectoryPath))
            {
                this.Callback(new DirectoryGetResult()
                {
                    DirectoryPath = string.Empty,
                    DirectoryState = DirectoryStateParameter.EMPTY,
                    FileInfoList = [],
                });

                return Task.CompletedTask;
            }

            var result = new DirectoryGetResult
            {
                DirectoryPath = param.DirectoryPath
            };

            var files = FileUtil.GetFileSystemEntriesArray(param.DirectoryPath);
            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ListEntity<FileShallowInfoEntity>(files.Length);
            foreach (var file in files)
            {
                this.CheckCancel();

                try
                {
                    var info = getInfoLogic.Get(file, param.IsGetThumbnail);
                    if (info != FileShallowInfoEntity.EMPTY)
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

            using (var con = Instance<IFileInfoDB>.Value.Connect())
            {
                var getDirectoryStateLogic = new DirectoryStateGetLogic(this);
                var directoryState = getDirectoryStateLogic.Execute(con, param.DirectoryPath);
                result.FileInfoList = [.. infoList];
                result.DirectoryState = directoryState;
            }

            this.Callback(result);

            return Task.CompletedTask;
        }
    }
}
