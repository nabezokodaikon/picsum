using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Drawing;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class ImageFileReadLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        internal ImageFileReadResult CreateResult(
            string filePath, bool isMain, bool hasSub)
        {
            var image = CvImage.EMPTY;

            try
            {
                image = this.ReadImageFile(filePath);

                this.CheckCancel();

                return new ImageFileReadResult()
                {
                    IsMain = isMain,
                    HasSub = hasSub,
                    Image = new()
                    {
                        FilePath = filePath,
                        Image = image,
                        IsEmpty = false,
                        IsError = image == CvImage.EMPTY,
                    },
                };
            }
            catch (JobCancelException)
            {
                image.Dispose();
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
                    Image = new CvImage(filePath, imageSize),
                    IsEmpty = true,
                    IsError = false,
                }
            };
        }

        internal CvImage ReadImageFile(string filePath)
        {
            try
            {
                return Instance<IImageFileCacher>.Value.GetCvImage(filePath);
            }
            catch (FileUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.Job.ID, ex));
                return CvImage.EMPTY;
            }
            catch (ImageUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.Job.ID, ex));
                return CvImage.EMPTY;
            }
        }

        internal Size GetImageSize(string filePath)
        {
            try
            {
                return Instance<IImageFileSizeCacher>.Value.GetOrCreate(filePath).Size;
            }
            catch (FileUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.Job.ID, ex));
                return ImageUtil.EMPTY_SIZE;
            }
            catch (ImageUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.Job.ID, ex));
                return ImageUtil.EMPTY_SIZE;
            }
        }
    }
}
