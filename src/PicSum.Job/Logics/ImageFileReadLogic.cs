using PicSum.Core.Base.Conf;
using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Results;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using System.Diagnostics;
using System.Drawing;

namespace PicSum.Job.Logics
{
    public sealed class ImageFileReadLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public static ImageFileReadResult CreateEmptyResult(
            string filePath, bool isMain, bool hasSub, ImageSizeMode imageSizeMode, Size imageSize)
        {
            var sw = Stopwatch.StartNew();

            var image = new CvImage(imageSize);

            sw.Stop();
            Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageFileReadLogic.CreateEmptyResult: {sw.ElapsedMilliseconds} ms");

            return new()
            {
                IsMain = isMain,
                HasSub = hasSub,
                Image = new()
                {
                    FilePath = filePath,
                    Thumbnail = null,
                    Image = image,
                    Size = imageSize,
                    IsEmpty = true,
                    IsError = false,
                }
            };
        }

        internal ImageFileReadResult CreateResult(
            string filePath, bool isMain, bool hasSub, int thumbnailSize, ImageSizeMode imageSizeMode, Size imageSize)
        {
            CvImage? image = null;
            Bitmap? thumbnail = null;

            try
            {
                image = this.ReadImageFile(filePath);
                var isError = image == CvImage.EMPTY;
                this.CheckCancel();

                image.CreateMat();
                this.CheckCancel();

                thumbnail = (isError) ?
                        null :
                        ThumbnailGetLogic.CreateThumbnail(image, thumbnailSize, imageSizeMode);
                this.CheckCancel();

                return new ImageFileReadResult()
                {
                    IsMain = isMain,
                    HasSub = hasSub,
                    Image = new()
                    {
                        FilePath = filePath,
                        Thumbnail = thumbnail,
                        Image = image,
                        Size = imageSize,
                        IsEmpty = false,
                        IsError = isError,
                    },
                };
            }
            catch (JobCancelException)
            {
                image?.Dispose();
                thumbnail?.Dispose();
                throw;
            }
        }

        internal CvImage ReadImageFile(string filePath)
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

        internal Size GetImageSize(string filePath)
        {
            try
            {
                return ImageFileSizeCacheUtil.Get(filePath).Size;
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
