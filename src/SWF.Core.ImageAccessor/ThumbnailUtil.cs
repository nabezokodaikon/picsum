using SWF.Core.Base;

namespace SWF.Core.ImageAccessor
{

    public static class ThumbnailUtil
    {
        public const string THUMBNAIL_BUFFER_FILE_EXTENSION = ".thumbnail";
        public const int THUMBNAIL_MAXIMUM_SIZE = 400;
        public const int THUMBNAIL_MINIMUM_SIZE = 96;

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

        public static OpenCvSharp.Mat ToImage(byte[] bf)
        {
            ArgumentNullException.ThrowIfNull(bf, nameof(bf));

            using (Measuring.Time(false, "ThumbnailUtil.ToImage"))
            {
                try
                {
                    return OpenCVUtil.ReadImageFileToMat(bf);
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

        public static void DrawFileThumbnail(Control control, Graphics g, CvImage thumb, RectangleF destRect, SizeF srcSize)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));
            ArgumentNullException.ThrowIfNull(thumb, nameof(thumb));

            const int offset = 16;

            float scale;
            if (destRect.Width > srcSize.Width || destRect.Height > srcSize.Height)
            {
                var displayScale = WindowUtil.GetCurrentWindowScale(control);
                scale = Math.Min(
                    displayScale,
                    Math.Min(destRect.Width / srcSize.Width, destRect.Height / srcSize.Height));
            }
            else
            {
                scale = Math.Min(destRect.Width / srcSize.Width, destRect.Height / srcSize.Height);
            }

            var w = srcSize.Width * scale;
            var h = srcSize.Height * scale;
            if (w > h && w > offset)
            {
                w -= offset;
                h -= offset * (srcSize.Height / (float)srcSize.Width);
            }
            else if (w <= h && h > offset)
            {
                w -= offset * (srcSize.Width / (float)srcSize.Height);
                h -= offset;
            }

            var x = destRect.X + (destRect.Width - w) / 2f;
            var y = destRect.Y + (destRect.Height - h) / 2f;

            thumb.DrawResizeImage(g, new RectangleF(x, y, w, h));
        }

        public static void DrawDirectoryThumbnail(Control control, Graphics g, CvImage thumb, RectangleF destRect, SizeF srcSize, Image icon)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));
            ArgumentNullException.ThrowIfNull(thumb, nameof(thumb));
            ArgumentNullException.ThrowIfNull(icon, nameof(icon));

            DrawFileThumbnail(control, g, thumb, destRect, srcSize);

            var destIconSize = new SizeF(destRect.Width * 0.5f, destRect.Height * 0.5f);
            var destIconRect = new RectangleF(
                destRect.X + 2, destRect.Bottom - destIconSize.Height,
                destIconSize.Width, destIconSize.Height);

            g.DrawImage(
                icon,
                destIconRect,
                new RectangleF(0, 0, icon.Width, icon.Height),
                GraphicsUnit.Pixel);
        }

        public static void DrawDirectoryThumbnail(Control control, Graphics g, CvImage thumb, RectangleF destRect, SizeF srcSize, IconImage icon)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));
            ArgumentNullException.ThrowIfNull(thumb, nameof(thumb));
            ArgumentNullException.ThrowIfNull(icon, nameof(icon));

            DrawFileThumbnail(control, g, thumb, destRect, srcSize);

            var destIconSize = new SizeF(destRect.Width * 0.5f, destRect.Height * 0.5f);
            var destIconRect = new RectangleF(
                destRect.X + 2, destRect.Bottom - destIconSize.Height,
                destIconSize.Width, destIconSize.Height);

            icon.Draw(g, destIconRect);
        }

        /// <summary>
        /// アイコンを描画します。
        /// </summary>
        /// <param name="g">グラフィックオブジェクト</param>
        /// <param name="icon">アイコン</param>
        /// <param name="rect">描画領域</param>
        public static void DrawIcon(Control control, Graphics g, IconImage icon, RectangleF rect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));
            ArgumentNullException.ThrowIfNull(icon, nameof(icon));

            var displayScale = WindowUtil.GetCurrentWindowScale(control);
            var displayScaleWidth = icon.Width * displayScale;
            var displayScaleHeight = icon.Height * displayScale;
            if (Math.Max(displayScaleWidth, displayScaleHeight) <= Math.Min(rect.Width, rect.Height))
            {
                var w = displayScaleWidth;
                var h = displayScaleHeight;
                var x = rect.X + (rect.Width - w) / 2f;
                var y = rect.Y + (rect.Height - h) / 2f;
                icon.Draw(g, new RectangleF(x, y, w, h));
            }
            else
            {
                var scale = Math.Min(rect.Width / icon.Width, rect.Height / icon.Height);
                var w = icon.Width * scale;
                var h = icon.Width * scale;
                var x = rect.X + (rect.Width - w) / 2f;
                var y = rect.Y + (rect.Height - h) / 2f;
                icon.Draw(g, new RectangleF(x, y, w, h));
            }
        }
    }
}
