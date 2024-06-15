using PicSum.Core.Base.Conf;
using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Logics;
using PicSum.Job.Paramters;
using PicSum.Job.Results;
using SWF.Common;
using System.Drawing;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// 画像ファイルを読込みます。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class ImageFileReadJob
        : AbstractTwoWayJob<ImageFileReadParameter, ImageFileGetResult>
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
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            if (parameter.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(parameter));
            }

            var result = new ImageFileGetResult();
            var imageLogic = new ImageFileReadLogic(this);
            var thumbLogic = new ThumbnailGetLogic(this);
            var currentFilePath = parameter.FilePathList[parameter.CurrentIndex];

            try
            {
                this.CheckCancel();
                var img1 = this.ReadImageFile(currentFilePath, imageLogic);
                if (parameter.ImageDisplayMode != ImageDisplayMode.Single
                    && img1 != ImageUtil.EMPTY_IMAGE
                    && img1.Width < img1.Height)
                {
                    var nextIndex = parameter.CurrentIndex + 1;
                    if (nextIndex > parameter.FilePathList.Count - 1)
                    {
                        nextIndex = 0;
                    }

                    var nextFilePath = parameter.FilePathList[nextIndex];
                    var srcImg2Size = this.GetImageSize(nextFilePath);
                    if (srcImg2Size.Width < srcImg2Size.Height)
                    {
                        result.Image1 = new()
                        {
                            FilePath = currentFilePath,
                            Thumbnail = thumbLogic.CreateThumbnail(img1, parameter.ThumbnailSize, parameter.ImageSizeMode),
                            Image = img1,
                            IsError = false,
                        };
                        this.CheckCancel();

                        var img2 = this.ReadImageFile(nextFilePath, imageLogic);
                        var isImg2Success = img2 != ImageUtil.EMPTY_IMAGE;
                        result.Image2 = new()
                        {
                            FilePath = nextFilePath,
                            Image = img2,
                            Thumbnail = (isImg2Success) ?
                                thumbLogic.CreateThumbnail(img2, parameter.ThumbnailSize, parameter.ImageSizeMode) :
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
                            Thumbnail = thumbLogic.CreateThumbnail(img1, parameter.ThumbnailSize, parameter.ImageSizeMode),
                            Image = img1,
                            IsError = false,
                        };
                        this.CheckCancel();
                    }
                }
                else
                {
                    var isImg1Success = img1 != ImageUtil.EMPTY_IMAGE;
                    result.Image1 = new()
                    {
                        FilePath = currentFilePath,
                        Thumbnail = (isImg1Success) ?
                            thumbLogic.CreateThumbnail(img1, parameter.ThumbnailSize, parameter.ImageSizeMode) :
                            null,
                        Image = img1,
                        IsError = !isImg1Success,
                    };
                    this.CheckCancel();
                }
            }
            catch (JobCancelException)
            {
                ExeptionHandler(result);
                throw;
            }

            this.Callback(result);
        }

        private Bitmap ReadImageFile(string filePath, ImageFileReadLogic logic)
        {
            try
            {
                return logic.Execute(filePath);
            }
            catch (FileUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.ID, ex));
                return ImageUtil.EMPTY_IMAGE;
            }
            catch (ImageUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.ID, ex));
                return ImageUtil.EMPTY_IMAGE;
            }
        }

        private Size GetImageSize(string filePath)
        {
            try
            {
                return ImageUtil.GetImageSize(filePath);
            }
            catch (ImageUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.ID, ex));
                return ImageUtil.EMPTY_SIZE;
            }
        }
    }
}
