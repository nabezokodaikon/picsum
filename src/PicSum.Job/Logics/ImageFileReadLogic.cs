using OpenCvSharp.Extensions;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
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
            int index, string filePath, bool isMain, bool hasSub, float zoomValue)
        {
            var image = CvImage.EMPTY;

            try
            {
                image = this.ReadImageFileFromCache(filePath, zoomValue);

                this.CheckCancel();

                return new ImageFileReadResult()
                {
                    Index = index,
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

        internal ImageFileReadResult CreateThumbnailResult(
            int index, string filePath, bool isMain, bool hasSub, float zoomValue)
        {
            var image = CvImage.EMPTY;

            try
            {
                image = this.ReadImageFile(filePath, zoomValue);

                this.CheckCancel();

                return new ImageFileReadResult()
                {
                    Index = index,
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
            int index, string filePath, bool isMain, bool hasSub, Size imageSize)
        {
            return new()
            {
                Index = index,
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

        internal CvImage ReadImageFile(string filePath, float zoomValue)
        {
            try
            {
                using (var bmp = ImageUtil.ReadImageFile(filePath))
                {
                    if (zoomValue == AppConstants.DEFAULT_ZOOM_VALUE)
                    {
                        return new CvImage(
                            filePath, bmp.ToMat(), bmp.PixelFormat);
                    }
                    else
                    {
                        return new CvImage(
                            filePath, OpenCVUtil.Zoom(bmp, zoomValue, OpenCvSharp.InterpolationFlags.Area), bmp.PixelFormat);
                    }
                }
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

        internal CvImage ReadImageFileFromCache(string filePath, float zoomValue)
        {
            try
            {
                return Instance<IImageFileCacher>.Value.GetCvImage(filePath, zoomValue);
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

        internal Size GetImageSize(string filePath, float zoomValue)
        {
            try
            {
                var size = Instance<IImageFileSizeCacher>.Value.GetOrCreate(filePath).Size;
                return new Size((int)(size.Width * zoomValue), (int)(size.Height * zoomValue));
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

        internal float GetThumbnailScale(float thumbnailSize, Size imageSize)
        {
            return Math.Min(
                thumbnailSize / (float)imageSize.Width,
                thumbnailSize / (float)imageSize.Height);
        }

        internal int GetNextIndex(ImageFileReadParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            if (parameter.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(parameter));
            }

            var files = parameter.FilePathList;
            var currentIndex = parameter.CurrentIndex;
            var maximumIndex = files.Length - 1;

            if (parameter.IsForceSingle
                || parameter.ImageDisplayMode == ImageDisplayMode.Single)
            {
                if (currentIndex == maximumIndex)
                {
                    return 0;
                }
                else
                {
                    return currentIndex + 1;
                }
            }
            else
            {
                var currentFilePath = files[currentIndex];
                var currentImageSize = this.GetImageSize(currentFilePath, parameter.ZoomValue);
                if (currentImageSize != ImageUtil.EMPTY_SIZE
                    && currentImageSize.Width <= currentImageSize.Height)
                {
                    var nextIndex = currentIndex + 1;
                    if (nextIndex > maximumIndex)
                    {
                        nextIndex = 0;
                    }

                    var nextFilePath = files[nextIndex];
                    var nextImageSize = this.GetImageSize(nextFilePath, parameter.ZoomValue);
                    if (nextImageSize != ImageUtil.EMPTY_SIZE
                        && nextImageSize.Width <= nextImageSize.Height)
                    {
                        if (nextIndex == maximumIndex)
                        {
                            return 0;
                        }
                        else
                        {
                            return nextIndex + 1;
                        }
                    }
                    else
                    {
                        return nextIndex;
                    }
                }
                else
                {
                    if (currentIndex == maximumIndex)
                    {
                        return 0;
                    }
                    else
                    {
                        return currentIndex + 1;
                    }
                }
            }
        }

        internal int GetPreviewIndex(ImageFileReadParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            if (parameter.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(parameter));
            }

            var files = parameter.FilePathList;
            var currentIndex = parameter.CurrentIndex;
            var maximumIndex = files.Length - 1;

            if (parameter.IsForceSingle
                || parameter.ImageDisplayMode == ImageDisplayMode.Single)
            {
                if (currentIndex == 0)
                {
                    return maximumIndex;
                }
                else
                {
                    return currentIndex - 1;
                }
            }
            else
            {
                var prevIndex1 = currentIndex - 1;
                if (prevIndex1 < 0)
                {
                    prevIndex1 = maximumIndex;
                }

                var prevFilePath1 = files[prevIndex1];
                var prevImageSize1 = this.GetImageSize(prevFilePath1, parameter.ZoomValue);
                if (prevImageSize1 != ImageUtil.EMPTY_SIZE
                    && prevImageSize1.Width <= prevImageSize1.Height)
                {
                    var prevIndex2 = prevIndex1 - 1;
                    if (prevIndex2 < 0)
                    {
                        prevIndex2 = maximumIndex;
                    }

                    var prevFilePath2 = files[prevIndex2];
                    var prevImageSize2 = this.GetImageSize(prevFilePath2, parameter.ZoomValue);
                    if (prevImageSize2 != ImageUtil.EMPTY_SIZE
                        && prevImageSize2.Width <= prevImageSize2.Height)
                    {
                        return prevIndex2;
                    }
                    else
                    {
                        return prevIndex1;
                    }
                }
                else
                {
                    return prevIndex1;
                }
            }
        }
    }
}
