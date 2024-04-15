using PicSum.Core.Base.Conf;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Entities;
using PicSum.Task.Logics;
using PicSum.Task.Paramters;
using PicSum.Task.Results;
using SWF.Common;
using System;
using System.Drawing;
using System.Runtime.Versioning;

namespace PicSum.Task.Tasks
{
    /// <summary>
    /// 画像ファイルを読込みます。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class ImageFileReadTask
        : AbstractTwoWayTask<ImageFileReadParameter, ImageFileGetResult>
    {
        private static void ExeptionHandler(ImageFileGetResult result)
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

        protected override void Execute(ImageFileReadParameter parameter)
        {
            var result = new ImageFileGetResult();
            var logic = new ImageFileReadLogic(this);
            var currentFilePath = parameter.FilePathList[parameter.CurrentIndex];

            try
            {
                this.CheckCancel();
                var isImg1Success = this.ReadImageFile(currentFilePath, out var img1);
                if (parameter.ImageDisplayMode != ImageDisplayMode.Single
                    && img1 != null
                    && img1.Width < img1.Height)
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
                        result.Image1 = new()
                        {
                            FilePath = currentFilePath,
                            Thumbnail = (isImg1Success) ?
                                logic.CreateThumbnail(img1, parameter.ThumbnailSize, parameter.ImageSizeMode) :
                                null,
                            Image = img1,
                            IsError = !isImg1Success,
                        };
                        this.CheckCancel();

                        var isImg2Success = this.ReadImageFile(nextFilePath, out var img2);
                        result.Image2 = new()
                        {
                            FilePath = nextFilePath,
                            Image = img2,
                            Thumbnail = (isImg2Success) ?
                                logic.CreateThumbnail(img2, parameter.ThumbnailSize, parameter.ImageSizeMode) :
                                null,
                            IsError = !isImg2Success,
                        };
                        this.CheckCancel();
                    }
                    else
                    {
                        result.Image1 = new()
                        {
                            FilePath = currentFilePath,
                            Thumbnail = (isImg1Success) ?
                                logic.CreateThumbnail(img1, parameter.ThumbnailSize, parameter.ImageSizeMode) :
                                null,
                            Image = img1,
                            IsError = !isImg1Success,
                        };
                        this.CheckCancel();
                    }
                }
                else
                {
                    result.Image1 = new()
                    {
                        FilePath = currentFilePath,
                        Thumbnail = (isImg1Success) ?
                            logic.CreateThumbnail(img1, parameter.ThumbnailSize, parameter.ImageSizeMode) :
                            null,
                        Image = img1,
                        IsError = !isImg1Success,
                    };
                    this.CheckCancel();
                }
            }
            catch (TaskCancelException)
            {
                ExeptionHandler(result);
                throw;
            }

            this.Callback(result);
        }

        private bool ReadImageFile(string filePath, out Bitmap bmp)
        {
            bmp = null;
            try
            {
                bmp = ImageUtil.ReadImageFile(filePath);
                return true;
            }
            catch (ImageUtilException ex)
            {
                WriteErrorLog(new TaskException(this.ID, ex));
                bmp?.Dispose();
                bmp = null;
                return false;
            }
        }
    }
}
