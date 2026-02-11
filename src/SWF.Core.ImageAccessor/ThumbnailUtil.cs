using SkiaSharp;
using SWF.Core.Base;

namespace SWF.Core.ImageAccessor
{

    public static class ThumbnailUtil
    {
        public const string THUMBNAIL_BUFFER_FILE_EXTENSION = ".thumbnail";
        public const int THUMBNAIL_MAXIMUM_SIZE = 400;
        public const int THUMBNAIL_MINIMUM_SIZE = 90;

        /// <summary>
        /// イメージオブジェクトを圧縮したバイナリに変換します。
        /// </summary>
        /// <param name="img">イメージオブジェクト</param>
        /// <returns></returns>
        public static byte[] ToCompressionBinary(OpenCvSharp.Mat img)
        {
            ArgumentNullException.ThrowIfNull(img, nameof(img));

            using (Measuring.Time(false, "ThumbnailUtil.ToCompressionBinary"))
            {
                return OpenCVUtil.ToCompressionBinary(img);
            }
        }

        public static OpenCvSharp.Mat ReadBufferToMat(byte[] bf)
        {
            ArgumentNullException.ThrowIfNull(bf, nameof(bf));

            using (Measuring.Time(false, "ThumbnailUtil.ReadImageBuffer"))
            {
                try
                {
                    return OpenCVUtil.ReadBuffer(bf);
                }
                catch (OutOfMemoryException ex)
                {
                    throw new ImageUtilException("メモリが不足しています。", ex);
                }
            }
        }

        public static SKImage ReadBufferToSKImage(byte[] bf)
        {
            ArgumentNullException.ThrowIfNull(bf, nameof(bf));

            using (Measuring.Time(false, "ThumbnailUtil.ReadSKImageBuffer"))
            {
                try
                {
                    return SkiaUtil.ReadBuffer(bf);
                }
                catch (OutOfMemoryException ex)
                {
                    throw new ImageUtilException("メモリが不足しています。", ex);
                }
            }
        }

        /// <summary>
        /// サムネイルを作成します。
        /// </summary>
        /// <param name="srcImg">作成元の画像</param>
        /// <param name="thumbWidth">作成するサムネイルの幅</param>
        /// <param name="thumbHeight">作成するサムネイルの高さ</param>
        /// <returns>サムネイル</returns>
        public static OpenCvSharp.Mat CreateThumbnail(Bitmap srcImg, int thumbWidth, int thumbHeight)
        {
            ArgumentNullException.ThrowIfNull(srcImg, nameof(srcImg));

            using (Measuring.Time(false, "ThumbnailUtil.CreateThumbnail"))
            {
                float w, h;
                if (Math.Max(srcImg.Width, srcImg.Height) <= Math.Min(thumbWidth, thumbHeight))
                {
                    w = srcImg.Width;
                    h = srcImg.Height;
                }
                else
                {
                    var scale = Math.Min(thumbWidth / (float)srcImg.Width, thumbHeight / (float)srcImg.Height);
                    w = srcImg.Width * scale;
                    h = srcImg.Height * scale;
                }

                if (w < 1)
                {
                    w = 1;
                }

                if (h < 1)
                {
                    h = 1;
                }

                return ImageUtil.Resize(srcImg, w, h);
            }
        }

        private static SKRectI GetDrawFileThumbnailRect(
            SKRectI destRect,
            Size srcSize,
            float displayScale)
        {
            float scale;
            if (destRect.Width > srcSize.Width || destRect.Height > srcSize.Height)
            {
                scale = Math.Min(
                    displayScale,
                    Math.Min(destRect.Width / (float)srcSize.Width, destRect.Height / (float)srcSize.Height));
            }
            else
            {
                scale = Math.Min(destRect.Width / (float)srcSize.Width, destRect.Height / (float)srcSize.Height);
            }

            const int OFFSET = 16;
            var w = srcSize.Width * scale;
            var h = srcSize.Height * scale;
            if (w > h && w > OFFSET)
            {
                w -= OFFSET;
                h -= OFFSET * (srcSize.Height / (float)srcSize.Width);
            }
            else if (w <= h && h > OFFSET)
            {
                w -= OFFSET * (srcSize.Width / (float)srcSize.Height);
                h -= OFFSET;
            }

            var x = destRect.Left + (destRect.Width - w) / 2f;
            var y = destRect.Top + (destRect.Height - h) / 2f;

            return SKRectI.Create((int)x, (int)y, (int)w, (int)h);
        }

        public static void CacheFileThumbnail(
            OpenCVImage thumb,
            SKRectI destRect,
            Size srcSize,
            float displayScale)
        {
            ArgumentNullException.ThrowIfNull(thumb, nameof(thumb));

            var rect = GetDrawFileThumbnailRect(destRect, srcSize, displayScale);
            thumb.CacheResizeThumbnail(rect);
        }

        public static void DrawFileThumbnail(
            SKCanvas canvas,
            SKPaint paint,
            OpenCVImage thumb,
            SKRectI destRect,
            Size srcSize,
            float displayScale)
        {
            ArgumentNullException.ThrowIfNull(canvas, nameof(canvas));
            ArgumentNullException.ThrowIfNull(paint, nameof(paint));
            ArgumentNullException.ThrowIfNull(thumb, nameof(thumb));

            var rect = GetDrawFileThumbnailRect(destRect, srcSize, displayScale);
            thumb.DrawResizeThumbnail(canvas, paint, rect);
        }

        public static void DrawDirectoryThumbnail(
            SKCanvas canvas,
            SKPaint paint,
            OpenCVImage thumb,
            SKRectI destRect,
            Size srcSize,
            IconImage icon,
            float displayScale)
        {
            ArgumentNullException.ThrowIfNull(canvas, nameof(canvas));
            ArgumentNullException.ThrowIfNull(paint, nameof(paint));
            ArgumentNullException.ThrowIfNull(thumb, nameof(thumb));
            ArgumentNullException.ThrowIfNull(icon, nameof(icon));

            DrawFileThumbnail(canvas, paint, thumb, destRect, srcSize, displayScale);

            var destIconSize = new SKSizeI(
                (int)(destRect.Width * 0.5f),
                (int)(destRect.Height * 0.5f));
            var x = destRect.Left + 2;
            var y = destRect.Bottom - destIconSize.Height;
            var destIconRect = SKRectI.Create(
                x, y,
                destIconSize.Width, destIconSize.Height);

            icon.Draw(canvas, paint, destIconRect);
        }

        public static void DrawIcon(
            SKCanvas canvas,
            SKPaint paint,
            IconImage icon,
            SKRectI rect,
            float displayScale)
        {
            ArgumentNullException.ThrowIfNull(canvas, nameof(canvas));
            ArgumentNullException.ThrowIfNull(paint, nameof(paint));
            ArgumentNullException.ThrowIfNull(icon, nameof(icon));

            var displayScaleWidth = icon.Width * displayScale;
            var displayScaleHeight = icon.Height * displayScale;
            if (Math.Max(displayScaleWidth, displayScaleHeight) <= Math.Min(rect.Width, rect.Height))
            {
                var w = displayScaleWidth;
                var h = displayScaleHeight;
                var x = rect.Left + (rect.Width - w) / 2f;
                var y = rect.Top + (rect.Height - h) / 2f;
                icon.Draw(canvas, paint, SKRectI.Create((int)x, (int)y, (int)w, (int)h));
            }
            else
            {
                var scale = Math.Min(rect.Width / (float)icon.Width, rect.Height / (float)icon.Height);
                var w = icon.Width * scale;
                var h = icon.Width * scale;
                var x = rect.Left + (rect.Width - w) / 2f;
                var y = rect.Top + (rect.Height - h) / 2f;
                icon.Draw(canvas, paint, SKRectI.Create((int)x, (int)y, (int)w, (int)h));
            }
        }
    }
}
