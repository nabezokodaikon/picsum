using PicSum.Core.Base.Conf;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Logics;
using PicSum.Task.Entities;
using PicSum.Task.Paramters;
using PicSum.Task.Results;
using SWF.Common;
using System;
using System.Runtime.Versioning;

namespace PicSum.Task.Tasks
{
    /// <summary>
    /// 画像ファイルを読込みます。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class GetImageFileTask
        : AbstractTwoWayTask<GetImageFileParameter, GetImageFileResult>
    {
        private static void ExeptionHandler(GetImageFileResult result)
        {
            bool isDisposed = false;

            if (result.Image1 != null)
            {
                if (result.Image1.Image != null)
                {
                    result.Image1.Image.Dispose();
                    result.Image1.Image = null;
                    isDisposed = true;
                }

                if (result.Image1.Thumbnail != null)
                {
                    result.Image1.Thumbnail.Dispose();
                    result.Image1.Thumbnail = null;
                    isDisposed = true;
                }
            }

            if (result.Image2 != null)
            {
                if (result.Image2.Image != null)
                {
                    result.Image2.Image.Dispose();
                    result.Image2.Image = null;
                    isDisposed = true;
                }

                if (result.Image2.Thumbnail != null)
                {
                    result.Image2.Thumbnail.Dispose();
                    result.Image2.Thumbnail = null;
                    isDisposed = true;
                }
            }

            if (isDisposed)
            {
                GC.Collect();
            }
        }

        protected override void Execute(GetImageFileParameter parameter)
        {
            var result = new GetImageFileResult();
            var logic = new GetImageFileLogic(this);
            var currentFilePath = parameter.FilePathList[parameter.CurrentIndex];

            try
            {
                this.CheckCancel();
                var img1 = ImageUtil.ReadImageFile(currentFilePath);

                if (parameter.ImageDisplayMode != ImageDisplayMode.Single &&
                    img1.Width < img1.Height)
                {
                    var nextIndex = parameter.CurrentIndex + 1;
                    if (nextIndex > parameter.FilePathList.Count - 1)
                    {
                        nextIndex = 0;
                    }

                    this.CheckCancel();
                    var nextFilePath = parameter.FilePathList[nextIndex];
                    var srcImg2Size = ImageUtil.GetImageSize(nextFilePath);
                    if (srcImg2Size.Width < srcImg2Size.Height)
                    {
                        this.CheckCancel();
                        var img2 = ImageUtil.ReadImageFile(nextFilePath);

                        this.CheckCancel();
                        result.Image1 = new ImageFileEntity();
                        result.Image1.FilePath = currentFilePath;
                        result.Image1.Image = img1;
                        result.Image1.Thumbnail = logic.CreateThumbnail(result.Image1.Image, parameter.ThumbnailSize, parameter.ImageSizeMode);

                        this.CheckCancel();
                        result.Image2 = new ImageFileEntity();
                        result.Image2.FilePath = nextFilePath;      
                        result.Image2.Image = img2;
                        result.Image2.Thumbnail = logic.CreateThumbnail(result.Image2.Image, parameter.ThumbnailSize, parameter.ImageSizeMode);
                    }
                    else
                    {
                        this.CheckCancel();
                        result.Image1 = new ImageFileEntity();
                        result.Image1.FilePath = currentFilePath;
                        result.Image1.Image = img1;
                        result.Image1.Thumbnail = logic.CreateThumbnail(result.Image1.Image, parameter.ThumbnailSize, parameter.ImageSizeMode);
                    }
                }
                else
                {
                    this.CheckCancel();
                    result.Image1 = new ImageFileEntity();
                    result.Image1.FilePath = currentFilePath;                    
                    result.Image1.Image = img1;
                    result.Image1.Thumbnail = logic.CreateThumbnail(result.Image1.Image, parameter.ThumbnailSize, parameter.ImageSizeMode);
                }
            }
            catch (ImageUtilException ex)
            {
                ExeptionHandler(result);
                throw new TaskException(this.ID, ex);
            }
            catch (TaskCancelException)
            {
                ExeptionHandler(result);
                throw;
            }

            this.Callback(result);
        }
    }
}
