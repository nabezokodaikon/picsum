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

        public static OpenCvSharp.Mat ReadImageBuffer(byte[] bf)
        {
            ArgumentNullException.ThrowIfNull(bf, nameof(bf));

            using (Measuring.Time(false, "ThumbnailUtil.ReadImageBuffer"))
            {
                try
                {
                    return OpenCVUtil.ReadImageBuffer(bf);
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

        public static void DrawFileThumbnail(Graphics g, CvImage thumb, Rectangle destRect, Size srcSize, float displayScale)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));
            ArgumentNullException.ThrowIfNull(thumb, nameof(thumb));

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

            var x = destRect.X + (destRect.Width - w) / 2f;
            var y = destRect.Y + (destRect.Height - h) / 2f;

            thumb.DrawResizeThumbnail(g, new Rectangle((int)x, (int)y, (int)w, (int)h));
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

            return new SKRectI((int)x, (int)y, (int)(x + w), (int)(y + h));
        }

        public static void CacheFileThumbnail(
            CvImage thumb,
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
            CvImage thumb,
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

        public static void DrawDirectoryThumbnail(Graphics g, CvImage thumb, Rectangle destRect, Size srcSize, Image icon, float displayScale)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));
            ArgumentNullException.ThrowIfNull(thumb, nameof(thumb));
            ArgumentNullException.ThrowIfNull(icon, nameof(icon));

            DrawFileThumbnail(g, thumb, destRect, srcSize, displayScale);

            var destIconSize = new Size((int)(destRect.Width * 0.5f), (int)(destRect.Height * 0.5f));
            var destIconRect = new Rectangle(
                destRect.X + 2, destRect.Bottom - destIconSize.Height,
                destIconSize.Width, destIconSize.Height);

            g.DrawImage(
                icon,
                destIconRect,
                new Rectangle(0, 0, icon.Width, icon.Height),
                GraphicsUnit.Pixel);
        }

        public static void DrawDirectoryThumbnail(
            SKCanvas canvas,
            SKPaint paint,
            CvImage thumb,
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
            var destIconRect = new SKRectI(
                x, y,
                x + destIconSize.Width, y + destIconSize.Height);

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
                icon.Draw(canvas, paint, new SKRectI((int)x, (int)y, (int)(x + w), (int)(y + h)));
            }
            else
            {
                var scale = Math.Min(rect.Width / (float)icon.Width, rect.Height / (float)icon.Height);
                var w = icon.Width * scale;
                var h = icon.Width * scale;
                var x = rect.Left + (rect.Width - w) / 2f;
                var y = rect.Top + (rect.Height - h) / 2f;
                icon.Draw(canvas, paint, new SKRectI((int)x, (int)y, (int)(x + w), (int)(y + h)));
            }
        }
    }
}
