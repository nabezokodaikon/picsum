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
    public sealed class GetImageFileTask
        : AbstractTwoWayTask<GetImageFileParameter, GetImageFileResult>
    {
        private static void ExeptionHandler(GetImageFileResult result)
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

        protected override void Execute(GetImageFileParameter parameter)
        {
            var result = new GetImageFileResult();
            var logic = new GetImageFileLogic(this);
            var currentFilePath = parameter.FilePathList[parameter.CurrentIndex];

            try
            {
                this.CheckCancel();
                var isImg1Error = this.ReadImageFile(currentFilePath, out var img1);

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
                        var isImg2Error = this.ReadImageFile(nextFilePath, out var img2);

                        this.CheckCancel();
                        result.Image1 = new()
                        {
                            FilePath = currentFilePath,
                            Image = img1,
                            Thumbnail = logic.CreateThumbnail(img1, parameter.ThumbnailSize, parameter.ImageSizeMode),
                            IsError = isImg1Error,
                        };

                        this.CheckCancel();
                        result.Image2 = new()
                        {
                            FilePath = nextFilePath,
                            Image = img2,
                            Thumbnail = logic.CreateThumbnail(img2, parameter.ThumbnailSize, parameter.ImageSizeMode),
                            IsError = isImg2Error,
                        };
                    }
                    else
                    {
                        this.CheckCancel();
                        result.Image1 = new()
                        {
                            FilePath = currentFilePath,
                            Image = img1,
                            Thumbnail = logic.CreateThumbnail(img1, parameter.ThumbnailSize, parameter.ImageSizeMode),
                            IsError = isImg1Error,
                        };
                    }
                }
                else
                {
                    this.CheckCancel();
                    result.Image1 = new()
                    {
                        FilePath = currentFilePath,
                        Image = img1,
                        Thumbnail = logic.CreateThumbnail(img1, parameter.ThumbnailSize, parameter.ImageSizeMode),
                        IsError = isImg1Error
                    };
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
                return false;
            }
            catch (ImageUtilException ex)
            {
                const int size = 512;
                WriteErrorLog(ex);
                bmp?.Dispose();
                bmp = ImageUtil.CreateErrorImage(size);
                return true;
            }
        }
    }
}
