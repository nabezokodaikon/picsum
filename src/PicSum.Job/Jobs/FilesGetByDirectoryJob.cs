using PicSum.Job.Entities;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
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
        protected override void Execute(FilesGetByDirectoryParameter param)
        {
            if (string.IsNullOrEmpty(param.DirectoryPath))
            {
                this.Callback(new DirectoryGetResult()
                {
                    DirectoryPath = string.Empty,
                    DirectoryState = DirectoryStateParameter.EMPTY,
                    FileInfoList = [],
                });

                return;
            }

            var result = new DirectoryGetResult
            {
                DirectoryPath = param.DirectoryPath
            };

            string[] fileList;
            var getFilesLogic = new FilesAndSubDirectoriesGetLogic(this);
            try
            {
                fileList = getFilesLogic.Execute(param.DirectoryPath);
            }
            catch (FileUtilException ex)
            {
                throw new JobException(this.ID, ex);
            }

            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ListEntity<FileShallowInfoEntity>();
            foreach (var file in fileList)
            {
                this.CheckCancel();

                try
                {
                    var info = getInfoLogic.Execute(file, param.IsGetThumbnail);
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

            var getDirectoryStateLogic = new DirectoryStateGetLogic(this);
            var directoryState = getDirectoryStateLogic.Execute(param.DirectoryPath);

            result.FileInfoList = [.. infoList];
            result.DirectoryState = directoryState;

            this.Callback(result);
        }
    }
}
