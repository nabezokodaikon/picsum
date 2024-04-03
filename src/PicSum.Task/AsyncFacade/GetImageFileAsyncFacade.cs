using PicSum.Core.Base.Conf;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;
using PicSum.Task.Result;
using SWF.Common;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.Versioning;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// 画像ファイルを読込みます。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class GetImageFileAsyncFacade
        : TwoWayFacadeBase<GetImageFileParameter, GetImageFileResult>
    {
        public override void Execute(GetImageFileParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var result = new GetImageFileResult();
            var logic = new GetImageFileAsyncLogic(this);
            var currentFilePath = param.FilePathList[param.CurrentIndex];

            try
            {
                this.CheckCancel();
                var img1 = ImageUtil.ReadImageFile(currentFilePath);
                this.CheckCancel();

                if (param.ImageDisplayMode != ImageDisplayMode.Single &&
                    img1.Width < img1.Height)
                {
                    var nextIndex = param.CurrentIndex + 1;
                    if (nextIndex > param.FilePathList.Count - 1)
                    {
                        nextIndex = 0;
                    }

                    var nextFilePath = param.FilePathList[nextIndex];
                    var srcImg2Size = ImageUtil.GetImageSize(nextFilePath);
                    if (srcImg2Size.Width < srcImg2Size.Height)
                    {
                        this.CheckCancel();
                        var img2 = ImageUtil.ReadImageFile(nextFilePath);
                        this.CheckCancel();

                        var drawSize = new Size((int)(param.DrawSize.Width / 2f), param.DrawSize.Height);

                        result.Image1 = new ImageFileEntity();
                        result.Image1.FilePath = currentFilePath;

                        this.CheckCancel();
                        result.Image1.Image = img1;
                        result.Image1.ImageScale = logic.GetImageScale(img1, param.ImageSizeMode, drawSize);
                        this.CheckCancel();
                        result.Image1.Thumbnail = logic.CreateThumbnail(result.Image1.Image, param.ThumbnailSize, param.ImageSizeMode);
                        this.CheckCancel();


                        result.Image2 = new ImageFileEntity();
                        result.Image2.FilePath = nextFilePath;

                        this.CheckCancel();
                        result.Image2.Image = img2;
                        result.Image2.ImageScale = logic.GetImageScale(img2, param.ImageSizeMode, drawSize);
                        this.CheckCancel();
                        result.Image2.Thumbnail = logic.CreateThumbnail(result.Image2.Image, param.ThumbnailSize, param.ImageSizeMode);
                        this.CheckCancel();
                    }
                    else
                    {
                        result.Image1 = new ImageFileEntity();
                        result.Image1.FilePath = currentFilePath;

                        this.CheckCancel();
                        result.Image1.Image = img1;
                        result.Image1.ImageScale = logic.GetImageScale(img1, param.ImageSizeMode, param.DrawSize);
                        this.CheckCancel();
                        result.Image1.Thumbnail = logic.CreateThumbnail(result.Image1.Image, param.ThumbnailSize, param.ImageSizeMode);
                        this.CheckCancel();
                    }
                }
                else
                {
                    result.Image1 = new ImageFileEntity();
                    result.Image1.FilePath = currentFilePath;
                    this.CheckCancel();
                    result.Image1.Image = img1;
                    result.Image1.ImageScale = logic.GetImageScale(img1, param.ImageSizeMode, param.DrawSize);
                    this.CheckCancel();
                    result.Image1.Thumbnail = logic.CreateThumbnail(result.Image1.Image, param.ThumbnailSize, param.ImageSizeMode);
                    this.CheckCancel();
                }

                result.ReadImageFileException = null;
            }
            catch (ImageUtilException ex)
            {
                this.exeptionHandler(result);
                result.ReadImageFileException = ex;
            }
            catch (FileNotFoundException ex)
            {
                this.exeptionHandler(result);
                result.ReadImageFileException = new ImageUtilException(ex);
            }

            this.OnCallback(result);
        }

        private void exeptionHandler(GetImageFileResult result)
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
    }
}
