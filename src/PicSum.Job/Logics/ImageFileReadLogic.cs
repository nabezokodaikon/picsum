using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Drawing;

namespace PicSum.Job.Logics
{
    internal sealed class ImageFileReadLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
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

                thumbnail = (isError) ?
                        null :
                        ThumbnailUtil.CreateThumbnail(image, thumbnailSize, imageSizeMode);
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

        internal ImageFileReadResult CreateEmptyResult(
            string filePath, bool isMain, bool hasSub, Size imageSize)
        {
            return new()
            {
                IsMain = isMain,
                HasSub = hasSub,
                Image = new()
                {
                    FilePath = filePath,
                    Thumbnail = null,
                    Image = new CvImage(imageSize),
                    Size = imageSize,
                    IsEmpty = true,
                    IsError = false,
                }
            };
        }

        internal CvImage ReadImageFile(string filePath)
        {
            try
            {
                return ImageFileCacher.GetCvImage(filePath);
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
                return ImageFileSizeCacher.Get(filePath).Size;
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
