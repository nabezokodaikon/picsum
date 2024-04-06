using PicSum.Core.Base.Conf;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;
using PicSum.Task.Result;
using SWF.Common;
using System;
using System.Runtime.Versioning;

namespace PicSum.Task.AsyncTask
{
    /// <summary>
    /// 画像ファイルを読込みます。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class GetImageFileAsyncTask
        : AbstractAsyncTask<GetImageFileParameter, GetImageFileResult>
    {
        protected override void Execute(GetImageFileParameter parameter)
        {
            var result = new GetImageFileResult();
            var logic = new GetImageFileAsyncLogic(this);
            var currentFilePath = parameter.FilePathList[parameter.CurrentIndex];

            try
            {
                this.CheckCancel();
                var img1 = ImageUtil.ReadImageFile(currentFilePath);
                this.CheckCancel();

                if (parameter.ImageDisplayMode != ImageDisplayMode.Single &&
                    img1.Width < img1.Height)
                {
                    var nextIndex = parameter.CurrentIndex + 1;
                    if (nextIndex > parameter.FilePathList.Count - 1)
                    {
                        nextIndex = 0;
                    }

                    var nextFilePath = parameter.FilePathList[nextIndex];
                    var srcImg2Size = ImageUtil.GetImageSize(nextFilePath);
                    if (srcImg2Size.Width < srcImg2Size.Height)
                    {
                        this.CheckCancel();
                        var img2 = ImageUtil.ReadImageFile(nextFilePath);
                        this.CheckCancel();

                        result.Image1 = new ImageFileEntity();
                        result.Image1.FilePath = currentFilePath;

                        this.CheckCancel();
                        result.Image1.Image = img1;
                        this.CheckCancel();
                        result.Image1.Thumbnail = logic.CreateThumbnail(result.Image1.Image, parameter.ThumbnailSize, parameter.ImageSizeMode);
                        this.CheckCancel();

                        result.Image2 = new ImageFileEntity();
                        result.Image2.FilePath = nextFilePath;

                        this.CheckCancel();
                        result.Image2.Image = img2;
                        this.CheckCancel();
                        result.Image2.Thumbnail = logic.CreateThumbnail(result.Image2.Image, parameter.ThumbnailSize, parameter.ImageSizeMode);
                        this.CheckCancel();
                    }
                    else
                    {
                        result.Image1 = new ImageFileEntity();
                        result.Image1.FilePath = currentFilePath;

                        this.CheckCancel();
                        result.Image1.Image = img1;
                        this.CheckCancel();
                        result.Image1.Thumbnail = logic.CreateThumbnail(result.Image1.Image, parameter.ThumbnailSize, parameter.ImageSizeMode);
                        this.CheckCancel();
                    }
                }
                else
                {
                    result.Image1 = new ImageFileEntity();
                    result.Image1.FilePath = currentFilePath;
                    this.CheckCancel();
                    result.Image1.Image = img1;
                    this.CheckCancel();
                    result.Image1.Thumbnail = logic.CreateThumbnail(result.Image1.Image, parameter.ThumbnailSize, parameter.ImageSizeMode);
                    this.CheckCancel();
                }

                result.TaskException = null;
            }
            catch (ImageUtilException ex)
            {
                this.ExeptionHandler(result);
                result.TaskException = new TaskException(this.ID, ex);
            }
            catch (TaskCancelException)
            {
                this.ExeptionHandler(result);
                throw;
            }

            this.ThenAction(result);
        }

        private void ExeptionHandler(GetImageFileResult result)
        {
            try
            {
                if (result.Image1 != null)
                {
                    if (result.Image1.Image != null)
                    {
                        result.Image1.Image.Dispose();
                        result.Image1.Image = null;
                    }

                    if (result.Image1.Thumbnail != null)
                    {
                        result.Image1.Thumbnail.Dispose();
                        result.Image1.Thumbnail = null;
                    }
                }

                if (result.Image2 != null)
                {
                    if (result.Image2.Image != null)
                    {
                        result.Image2.Image.Dispose();
                        result.Image2.Image = null;
                    }

                    if (result.Image2.Thumbnail != null)
                    {
                        result.Image2.Thumbnail.Dispose();
                        result.Image2.Thumbnail = null;
                    }
                }
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
