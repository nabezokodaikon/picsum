using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// フォルダ内の画像ファイルを検索します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ImageFilesGetByDirectoryJob
        : AbstractTwoWayJob<ImageFileGetByDirectoryParameter, ImageFilesGetByDirectoryResult>
    {
        protected override ValueTask Execute(ImageFileGetByDirectoryParameter param)
        {
            var result = new ImageFilesGetByDirectoryResult();

            if (FileUtil.IsExistsDirectory(param.FilePath))
            {
                result.DirectoryPath = param.FilePath;
            }
            else if (FileUtil.IsExistsFile(param.FilePath))
            {
                result.DirectoryPath = FileUtil.GetParentDirectoryPath(param.FilePath);
            }
            else
            {
                throw new ArgumentException("ファイルまたはフォルダのパスではありません。", nameof(param));
            }

            result.FilePathList = ImageUtil.GetImageFilesArray(result.DirectoryPath);
            if (Array.IndexOf(result.FilePathList, param.FilePath) < 0)
            {
                result.SelectedFilePath = string.Empty;
            }
            else
            {
                result.SelectedFilePath = param.FilePath;
            }

            this.Callback(result);

            return ValueTask.CompletedTask;
        }
    }
}
