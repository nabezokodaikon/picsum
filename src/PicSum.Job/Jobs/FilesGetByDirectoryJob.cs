using PicSum.Job.Entities;
using PicSum.Job.Logics;
using PicSum.Job.Results;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// ファイルをフォルダで検索します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class FilesGetByDirectoryJob
        : AbstractTwoWayJob<ValueParameter<string>, DirectoryGetResult>
    {
        protected override void Execute(ValueParameter<string> param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            var result = new DirectoryGetResult
            {
                DirectoryPath = param.Value
            };

            IList<string> fileList;
            var getFilesLogic = new FilesAndSubDirectoriesGetLogic(this);
            try
            {
                fileList = getFilesLogic.Execute(param.Value);
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

            var getDirectoryStateLogic = new DirectoryStateGetLogic(this);
            var directoryState = getDirectoryStateLogic.Execute(param.Value);

            result.FileInfoList = infoList;
            result.DirectoryState = directoryState;

            this.Callback(result);
        }
    }
}
