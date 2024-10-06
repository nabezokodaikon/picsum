using PicSum.Core.Base.Conf;
using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using System.Diagnostics;
using System.Drawing;

namespace PicSum.Job.Jobs
{
    public sealed class ImageFileReadJob
        : AbstractTwoWayJob<ImageFileReadParameter, ImageFileGetResult>
    {
        private static void ExeptionHandler(ImageFileGetResult result)
        {
            if (result.Image != null)
            {
                if (result.Image.Image != null)
                {
                    result.Image.Image.Dispose();
                    result.Image.Image = null;
                }

                if (result.Image.Thumbnail != null)
                {
                    result.Image.Thumbnail.Dispose();
                    result.Image.Thumbnail = null;
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

            this.CheckCancel();

            var mainFilePath = parameter.FilePathList[parameter.CurrentIndex];
            var mainSize = this.GetImageSize(mainFilePath);
            this.CheckCancel();

            if (parameter.ImageDisplayMode != ImageDisplayMode.Single
                && mainSize != ImageUtil.EMPTY_SIZE
                && mainSize.Width < mainSize.Height)
            {
                var subtIndex = parameter.CurrentIndex + 1;
                if (subtIndex > parameter.FilePathList.Count - 1)
                {
                    subtIndex = 0;
                }

                var subFilePath = parameter.FilePathList[subtIndex];
                var subSize = this.GetImageSize(subFilePath);
                this.CheckCancel();

                if (subSize != ImageUtil.EMPTY_SIZE
                    && subSize.Width < subSize.Height)
                {
                    var mainResult = this.CreateResult(
                        mainFilePath, true, true, parameter.ThumbnailSize, parameter.ImageSizeMode);
                    this.Callback(mainResult);

                    var subResult = this.CreateResult(
                        subFilePath, false, true, parameter.ThumbnailSize, parameter.ImageSizeMode);
                    this.Callback(subResult);
                }
                else
                {
                    var result = this.CreateResult(
                        mainFilePath, true, false, parameter.ThumbnailSize, parameter.ImageSizeMode);
                    this.Callback(result);
                }
            }
            else
            {
                var result = this.CreateResult(
                    mainFilePath, true, false, parameter.ThumbnailSize, parameter.ImageSizeMode);
                this.Callback(result);
            }
        }

        private ImageFileGetResult CreateResult(
            string filePath, bool isMain, bool hasSub, int thumbnailSize, ImageSizeMode imageSizeMode)
        {
            var sw = Stopwatch.StartNew();
            Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageFileReadJob.CreateResult Start IsMain: {isMain}");

            var result = new ImageFileGetResult();

            try
            {
                var thumbLogic = new ThumbnailGetLogic(this);

                var image = this.ReadImageFile(filePath);
                var isSuccess = image != CvImage.EMPTY;
                this.CheckCancel();

                image.CreateMat();
                this.CheckCancel();

                result.IsMain = isMain;
                result.HasSub = hasSub;
                result.Image = new()
                {
                    FilePath = filePath,
                    Thumbnail = (isSuccess) ?
                        thumbLogic.CreateThumbnail(image, thumbnailSize, imageSizeMode) :
                        null,
                    Image = image,
                    IsError = !isSuccess,
                };
                this.CheckCancel();
            }
            catch (JobCancelException)
            {
                ExeptionHandler(result);
                throw;
            }
            catch (Exception)
            {
                ExeptionHandler(result);
                throw;
            }
            finally
            {
                sw.Stop();
                Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageFileReadJob.CreateResult: {sw.ElapsedMilliseconds} ms");
            }

            return result;
        }

        private CvImage ReadImageFile(string filePath)
        {
            try
            {
                return ImageFileCacheUtil.GetCvImage(filePath);
            }
            catch (FileUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.ID, ex));
                return CvImage.EMPTY;
            }
            catch (ImageUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.ID, ex));
                return CvImage.EMPTY;
            }
        }

        private Size GetImageSize(string filePath)
        {
            try
            {
                return ImageUtil.GetImageInfo(filePath).Size;
            }
            catch (FileUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.ID, ex));
                return ImageUtil.EMPTY_SIZE;
            }
            catch (ImageUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.ID, ex));
                return ImageUtil.EMPTY_SIZE;
            }
        }
    }
}
