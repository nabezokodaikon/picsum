using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// ファイルの深い情報取得非同期ジョブ
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class FileDeepInfoGetJob
        : AbstractTwoWayJob<FileDeepInfoGetParameter, FileDeepInfoGetResult>
    {
        protected override void Execute(FileDeepInfoGetParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            if (param.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(param));
            }

            var result = new FileDeepInfoGetResult
            {
                FilePathList = param.FilePathList
            };

            if (param.FilePathList.Count == 1)
            {
                try
                {
                    var getInfoLogic = new FileDeepInfoGetLogic(this);
                    var filePath = param.FilePathList[0];
                    result.FileInfo = getInfoLogic.Execute(filePath, param.ThumbnailSize);
                }
                catch (FileUtilException ex)
                {
                    throw new JobException(this.ID, ex);
                }
                catch (ImageUtilException ex)
                {
                    throw new JobException(this.ID, ex);
                }
            }

            if (param.FilePathList.Count <= 997)
            {
                var logic = new FilesTagInfoGetLogic(this);
                result.TagInfoList = logic.Execute(result.FilePathList);
            }
            else
            {
                result.TagInfoList = [];
            }

            this.CheckCancel();

            this.Callback(result);
        }
    }
}
