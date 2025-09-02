using PicSum.Job.Common;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Drawing;

namespace PicSum.Job.Logics
{

    internal sealed class ImageFileReadLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        internal ImageFileReadResult CreateResult(
            int index,
            string filePath,
            bool isMain,
            bool hasSub,
            float zoomValue,
            float thumbnailSize,
            ImageSizeMode sizeMode)
        {
            CvImage? image = null;
            Bitmap? thumbnail = null;

            try
            {
                using (TimeMeasuring.Run(false, $"ImageFileReadLogic.CreateResult"))
                {
                    image = this.ReadImageFile(filePath, zoomValue);

                    this.ThrowIfJobCancellationRequested();

                    if (sizeMode == ImageSizeMode.Original)
                    {
                        var thumbnailScale = this.GetThumbnailScale(thumbnailSize, image.Size);
                        thumbnail = image.CreateScaleImage(thumbnailScale);
                    }

                    this.ThrowIfJobCancellationRequested();

                    return new ImageFileReadResult()
                    {
                        Index = index,
                        IsMain = isMain,
                        HasSub = hasSub,
                        Image = new()
                        {
                            FilePath = filePath,
                            Image = image,
                            Thumbnail = thumbnail,
                            IsEmpty = false,
                            IsError = image.IsEmpry,
                        },
                    };
                }
            }
            catch (JobCancelException)
            {
                image?.Dispose();
                thumbnail?.Dispose();
                throw;
            }
        }

        internal ImageFileReadResult CreateLoadingResult(
            int index, string filePath, bool isMain, bool hasSub, Size imageSize, float zoomValue)
        {
            var thumbnail = this.GetThumbnail(filePath, imageSize, zoomValue);
            var isEmpty = thumbnail.IsEmpry;
            var image = isEmpty ? new CvImage(filePath, imageSize, zoomValue) : thumbnail;

            return new()
            {
                Index = index,
                IsMain = isMain,
                HasSub = hasSub,
                Image = new()
                {
                    FilePath = filePath,
                    Image = image,
                    IsEmpty = isEmpty,
                    IsError = false,
                }
            };
        }

        internal Size GetImageSize(string filePath)
        {
            try
            {
                var cache = Instance<IImageFileSizeCacher>.Value.GetOrCreate(filePath);
                return cache.Size;
            }
            catch (Exception ex) when (
                ex is FileUtilException ||
                ex is ImageUtilException)
            {
                this.WriteErrorLog(ex);
                return ImageUtil.EMPTY_SIZE;
            }
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
                || parameter.DisplayMode == ImageDisplayMode.Single)
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
                var currentImageSize = this.GetImageSize(currentFilePath);
                if (currentImageSize != ImageUtil.EMPTY_SIZE
                    && currentImageSize.Width <= currentImageSize.Height)
                {
                    var nextIndex = currentIndex + 1;
                    if (nextIndex > maximumIndex)
                    {
                        nextIndex = 0;
                    }

                    var nextFilePath = files[nextIndex];
                    var nextImageSize = this.GetImageSize(nextFilePath);
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
                || parameter.DisplayMode == ImageDisplayMode.Single)
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
                var prevImageSize1 = this.GetImageSize(prevFilePath1);
                if (prevImageSize1 != ImageUtil.EMPTY_SIZE
                    && prevImageSize1.Width <= prevImageSize1.Height)
                {
                    var prevIndex2 = prevIndex1 - 1;
                    if (prevIndex2 < 0)
                    {
                        prevIndex2 = maximumIndex;
                    }

                    var prevFilePath2 = files[prevIndex2];
                    var prevImageSize2 = this.GetImageSize(prevFilePath2);
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

        private CvImage ReadImageFile(string filePath, float zoomValue)
        {
            try
            {
                using (TimeMeasuring.Run(false, "ImageFileReadLogic.ReadImageFile Get Cache"))
                {
                    var image = Instance<IImageFileCacher>.Value.GetCache(filePath, zoomValue);
                    if (!image.IsEmpry)
                    {
                        return image;
                    }
                }

                using (TimeMeasuring.Run(false, "ImageFileReadLogic.ReadImageFile Read File"))
                {
                    using (var bmp = ImageUtil.ReadImageFile(filePath))
                    {
                        return new CvImage(
                            filePath, OpenCVUtil.ToMat(bmp), zoomValue);
                    }
                }
            }
            catch (Exception ex) when (
                ex is FileUtilException ||
                ex is ImageUtilException)
            {
                this.WriteErrorLog(ex);
                return CvImage.EMPTY;
            }
        }

        private CvImage GetThumbnail(string filePath, Size imageSize, float zoomValue)
        {
            using (TimeMeasuring.Run(false, "ImageFileReadLogic.GetThumbnail"))
            {
                var cache = Instance<IThumbnailCacher>.Value.GetCache(filePath);
                if (!cache.IsEmpry
                    && cache.ThumbnailBuffer != null)
                {
                    return new CvImage(
                        filePath,
                        ThumbnailUtil.ToImage(cache.ThumbnailBuffer),
                        imageSize,
                        zoomValue);
                }
                else
                {
                    return CvImage.EMPTY;
                }
            }
        }

        private float GetThumbnailScale(float thumbnailSize, SizeF imageSize)
        {
            return Math.Min(
                thumbnailSize / imageSize.Width,
                thumbnailSize / imageSize.Height);
        }
    }
}
