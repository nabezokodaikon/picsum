using PicSum.Job.Common;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SkiaSharp;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Drawing;

namespace PicSum.Job.Logics
{

    internal sealed class ImageFileReadLogic(IJob job)
        : AbstractLogic(job)
    {
        internal async ValueTask<ImageFileReadResult> CreateResult(
            int index,
            string filePath,
            bool isMain,
            bool hasSub,
            float zoomValue,
            float thumbnailSize,
            ImageSizeMode sizeMode)
        {
            SkiaImage? image = null;
            SKImage? thumbnail = null;

            try
            {
                using (Measuring.Time(false, $"ImageFileReadLogic.CreateResult"))
                {
                    image = await this.ReadImageFile(filePath, zoomValue).False();

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

        internal async ValueTask<ImageFileReadResult> CreateLoadingResult(
            int index, string filePath, bool isMain, bool hasSub, Size imageSize, float zoomValue)
        {
            var thumbnail = await this.GetThumbnail(filePath, imageSize, zoomValue).False();
            var isEmpty = thumbnail.IsEmpry;
            var image = isEmpty ? new SkiaImage(filePath, imageSize, zoomValue) : thumbnail;

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

        internal async ValueTask<Size> GetImageSize(string filePath)
        {
            try
            {
                var cache = await Instance<IImageFileSizeCacher>.Value.GetOrCreate(filePath).False();
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

        internal async ValueTask<int> GetNextIndex(ImageFileReadParameter parameter)
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
                var currentImageSize = await this.GetImageSize(currentFilePath).False();
                if (currentImageSize != ImageUtil.EMPTY_SIZE
                    && currentImageSize.Width <= currentImageSize.Height)
                {
                    var nextIndex = currentIndex + 1;
                    if (nextIndex > maximumIndex)
                    {
                        nextIndex = 0;
                    }

                    var nextFilePath = files[nextIndex];
                    var nextImageSize = await this.GetImageSize(nextFilePath).False();
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

        internal async ValueTask<int> GetPreviewIndex(ImageFileReadParameter parameter)
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
                var prevImageSize1 = await this.GetImageSize(prevFilePath1).False();
                if (prevImageSize1 != ImageUtil.EMPTY_SIZE
                    && prevImageSize1.Width <= prevImageSize1.Height)
                {
                    var prevIndex2 = prevIndex1 - 1;
                    if (prevIndex2 < 0)
                    {
                        prevIndex2 = maximumIndex;
                    }

                    var prevFilePath2 = files[prevIndex2];
                    var prevImageSize2 = await this.GetImageSize(prevFilePath2).False();
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

        private async ValueTask<SkiaImage> ReadImageFile(string filePath, float zoomValue)
        {
            try
            {
                using (Measuring.Time(false, "ImageFileReadLogic.ReadImageFile Get Cache"))
                {
                    var image = await Instance<IImageFileCacher>.Value.GetSKCache(filePath, zoomValue).False();
                    if (!image.IsEmpry)
                    {
                        return image;
                    }
                }

                using (Measuring.Time(false, "ImageFileReadLogic.ReadImageFile Read File"))
                {
                    using (var bmp = await ImageUtil.ReadImageFile(filePath).False())
                    {
                        return new SkiaImage(
                            filePath, SkiaImageUtil.ToSKImage(bmp), zoomValue);
                    }
                }
            }
            catch (Exception ex) when (
                ex is FileUtilException ||
                ex is ImageUtilException)
            {
                this.WriteErrorLog(ex);
                return SkiaImage.EMPTY;
            }
        }

        private async ValueTask<SkiaImage> GetThumbnail(
            string filePath, Size imageSize, float zoomValue)
        {
            using (Measuring.Time(false, "ImageFileReadLogic.GetThumbnail"))
            {
                var cache = await Instance<IThumbnailCacher>.Value.GetCache(filePath).False();
                if (!cache.IsEmpry
                    && cache.ThumbnailBuffer != null)
                {
                    return new SkiaImage(
                        filePath,
                        ThumbnailUtil.ReadSKImageBuffer(cache.ThumbnailBuffer),
                        imageSize,
                        zoomValue);
                }
                else
                {
                    return SkiaImage.EMPTY;
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
