using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
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
        protected override void Execute(ImageFileGetByDirectoryParameter param)
        {

            var result = new ImageFilesGetByDirectoryResult();

            try
            {
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

                var getFilesLogic = new FilesAndSubDirectoriesGetLogic(this);
                var filePathList = getFilesLogic.Execute(result.DirectoryPath);

                result.FilePathList = [];
                foreach (var filePath in filePathList)
                {
                    if (FileUtil.IsImageFile(filePath))
                    {
                        result.FilePathList.Add(filePath);
                    }
                }

                if (result.FilePathList.Contains(param.FilePath))
                {
                    result.SelectedFilePath = param.FilePath;
                }
                else
                {
                    result.SelectedFilePath = string.Empty;
                }
            }
            catch (FileUtilException ex)
            {
                throw new JobException(this.ID, ex);
            }

            this.Callback(result);
        }
    }
}
