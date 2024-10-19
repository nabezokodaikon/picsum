using Microsoft.WindowsAPICodePack.Shell;
using SWF.Core.Base;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows")]
    public static class ThumbnailUtil
    {
        private static readonly EncoderParameter ENCORDER_PARAMETER = new(Encoder.Quality, 80L);
        private static readonly ImageCodecInfo COMPRESS_CODEC_INFO = ImageCodecInfo.GetImageEncoders().Single(info => info.FormatID == ImageFormat.Jpeg.Guid);

        /// <summary>
        /// イメージオブジェクトを圧縮したバイナリに変換します。
        /// </summary>
        /// <param name="img">イメージオブジェクト</param>
        /// <returns></returns>
        public static byte[] ToCompressionBinary(Image img)
        {
            ArgumentNullException.ThrowIfNull(img, nameof(img));

            using (TimeMeasuring.Run(false, "ThumbnailUtil.ToCompressionBinary"))
            using (var mes = new MemoryStream())
            {
                var eps = new EncoderParameters(1);
                eps.Param[0] = ENCORDER_PARAMETER;
                img.Save(mes, COMPRESS_CODEC_INFO, eps);
                return mes.ToArray();
            }
        }

        /// <summary>
        /// バイト配列からイメージオブジェクトを取得します。
        /// </summary>
        /// <param name="bf">バイト配列</param>
        /// <returns>イメージオブジェクト</returns>
        public static Bitmap ToImage(byte[] bf)
        {
            ArgumentNullException.ThrowIfNull(bf, nameof(bf));

            using (TimeMeasuring.Run(false, "ThumbnailUtil.ToImage"))
            using (var mes = new MemoryStream(bf, 0, bf.Length, false, true))
            {
                try
                {
                    return (Bitmap)Bitmap.FromStream(mes, false, true);
                }
                catch (OutOfMemoryException ex)
                {
                    throw new ImageUtilException("メモリが不足しています。", ex);
                }
            }
        }

        public static Bitmap GetThumbnail(string filePath, Size srcSize, int thumbWidth, int thumbHeight)
        {
            using (TimeMeasuring.Run(true, "ThumbnailUtil.GetThumbnail"))
            {
                int w, h;
                if (Math.Max(srcSize.Width, srcSize.Height) <= Math.Min(thumbWidth, thumbHeight))
                {
                    w = srcSize.Width;
                    h = srcSize.Height;
                }
                else
                {
                    var scale = Math.Min(thumbWidth / (float)srcSize.Width, thumbHeight / (float)srcSize.Height);
                    w = (int)(srcSize.Width * scale);
                    h = (int)(srcSize.Height * scale);
                }

                if (w < 1)
                {
                    w = 1;
                }

                if (h < 1)
                {
                    h = 1;
                }

                using (var shellFile = ShellFile.FromFilePath(filePath))
                using (var thumbnail = shellFile.Thumbnail.LargeBitmap)
                {
                    return new Bitmap(thumbnail, new Size(w, h));
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
        public static Image CreateThumbnail(Image srcImg, int thumbWidth, int thumbHeight)
        {
            ArgumentNullException.ThrowIfNull(srcImg, nameof(srcImg));

            using (TimeMeasuring.Run(true, "ThumbnailUtil.CreateThumbnail"))
            {
                int w, h;
                if (Math.Max(srcImg.Width, srcImg.Height) <= Math.Min(thumbWidth, thumbHeight))
                {
                    w = srcImg.Width;
                    h = srcImg.Height;
                }
                else
                {
                    var scale = Math.Min(thumbWidth / (float)srcImg.Width, thumbHeight / (float)srcImg.Height);
                    w = (int)(srcImg.Width * scale);
                    h = (int)(srcImg.Height * scale);
                }

                if (w < 1)
                {
                    w = 1;
                }

                if (h < 1)
                {
                    h = 1;
                }

                return ImageUtil.Resize((Bitmap)srcImg, w, h);
            }
        }

        public static Bitmap CreateThumbnail(CvImage srcImg, int thumbSize, ImageSizeMode sizeMode)
        {
            ArgumentNullException.ThrowIfNull(srcImg, nameof(srcImg));

            if (thumbSize < 0)
            {
                ArgumentOutOfRangeException.ThrowIfNegative(thumbSize, nameof(thumbSize));
            }

            using (TimeMeasuring.Run(true, "ThumbnailUtil.CreateThumbnail"))
            {
                var scale = Math.Min(thumbSize / (float)srcImg.Width, thumbSize / (float)srcImg.Height);
                var w = srcImg.Width * scale;
                var h = srcImg.Height * scale;

                var destImg = new Bitmap((int)w, (int)h);
                using (var g = Graphics.FromImage(destImg))
                {
                    if (sizeMode == ImageSizeMode.Original)
                    {
                        g.SmoothingMode = SmoothingMode.None;
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.CompositingQuality = CompositingQuality.HighSpeed;
                        g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                        g.CompositingMode = CompositingMode.SourceOver;

                        srcImg.DrawResizeImage(g, new RectangleF(0, 0, w, h));
                    }
                }

                return destImg;
            }
        }

        /// <summary>
        /// ファイルのサムネイルを描画します。
        /// </summary>
        /// <param name="g"></param>
        /// <param name="thumb"></param>
        /// <param name="rect"></param>
        public static void DrawFileThumbnail(Graphics g, Image thumb, RectangleF rect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));
            ArgumentNullException.ThrowIfNull(thumb, nameof(thumb));

            const int offset = 16;

            float w;
            float h;
            if (thumb.Width > thumb.Height)
            {
                var scale = thumb.Height / (float)thumb.Width;
                w = thumb.Width - offset;
                h = thumb.Height - (offset * scale);
            }
            else
            {
                var scale = thumb.Width / (float)thumb.Height;
                w = thumb.Width - (offset * scale);
                h = thumb.Height - offset;
            }

            var x = rect.X + (rect.Width - w) / 2f;
            var y = rect.Y + (rect.Height - h) / 2f;

            g.DrawImage(
                thumb,
                new RectangleF(x, y, w, h),
                new RectangleF(0, 0, thumb.Width, thumb.Height),
                GraphicsUnit.Pixel);
        }

        /// <summary>
        /// ファイルのサムネイルを調整して描画します。
        /// </summary>
        /// <param name="g"></param>
        /// <param name="thumb"></param>
        /// <param name="destRect"></param>
        public static void AdjustDrawFileThumbnail(Graphics g, Image thumb, RectangleF destRect, SizeF srcSize)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));
            ArgumentNullException.ThrowIfNull(thumb, nameof(thumb));

            const int offset = 16;

            float scale;
            if (destRect.Width > srcSize.Width || destRect.Height > srcSize.Height)
            {
                scale = Math.Min(1, Math.Min(destRect.Width / thumb.Width, destRect.Height / thumb.Height));
            }
            else
            {
                scale = Math.Min(destRect.Width / thumb.Width, destRect.Height / thumb.Height);
            }

            float w;
            float h;
            if (thumb.Width > thumb.Height)
            {
                w = thumb.Width * scale - offset;
                h = thumb.Height * scale - (offset * (thumb.Height / (float)thumb.Width));
            }
            else
            {
                w = thumb.Width * scale - (offset * (thumb.Width / (float)thumb.Height));
                h = thumb.Height * scale - offset;
            }

            var x = destRect.X + (destRect.Width - w) / 2f;
            var y = destRect.Y + (destRect.Height - h) / 2f;

            g.DrawImage(
                thumb,
                new RectangleF(x, y, w, h),
                new RectangleF(0, 0, thumb.Width, thumb.Height),
                GraphicsUnit.Pixel);
        }

        /// <summary>
        /// フォルダのサムネイルを描画します。
        /// </summary>
        /// <param name="g"></param>
        /// <param name="thumb"></param>
        /// <param name="rect"></param>
        /// <param name="icon"></param>
        public static void DrawDirectoryThumbnail(Graphics g, Image thumb, RectangleF rect, Image icon)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));
            ArgumentNullException.ThrowIfNull(thumb, nameof(thumb));
            ArgumentNullException.ThrowIfNull(icon, nameof(icon));

            DrawFileThumbnail(g, thumb, rect);
            g.DrawImage(
                icon,
                new RectangleF(rect.X + 2, rect.Bottom - icon.Height, icon.Width, icon.Height),
                new RectangleF(0, 0, icon.Width, icon.Height),
                GraphicsUnit.Pixel);
        }

        /// <summary>
        /// フォルダのサムネイルを調整して描画します。
        /// </summary>
        /// <param name="g"></param>
        /// <param name="thumb"></param>
        /// <param name="destRect"></param>
        /// <param name="icon"></param>
        public static void AdjustDrawDirectoryThumbnail(Graphics g, Image thumb, RectangleF destRect, SizeF srcSize, Image icon)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));
            ArgumentNullException.ThrowIfNull(thumb, nameof(thumb));
            ArgumentNullException.ThrowIfNull(icon, nameof(icon));

            AdjustDrawFileThumbnail(g, thumb, destRect, srcSize);
            g.DrawImage(
                icon,
                new RectangleF(destRect.X + 2, destRect.Bottom - icon.Height, icon.Width, icon.Height),
                new RectangleF(0, 0, icon.Width, icon.Height),
                GraphicsUnit.Pixel);
        }

        /// <summary>
        /// アイコンを描画します。
        /// </summary>
        /// <param name="g">グラフィックオブジェクト</param>
        /// <param name="icon">アイコン</param>
        /// <param name="rect">描画領域</param>
        public static void DrawIcon(Graphics g, Image icon, RectangleF rect)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));
            ArgumentNullException.ThrowIfNull(icon, nameof(icon));

            if (Math.Max(icon.Width, icon.Height) <= Math.Min(rect.Width, rect.Height))
            {
                var w = icon.Width;
                var h = icon.Height;
                var x = rect.X + (rect.Width - w) / 2f;
                var y = rect.Y + (rect.Height - h) / 2f;
                g.DrawImage(icon, new RectangleF(x, y, w, h));
            }
            else
            {
                var scale = Math.Min(rect.Width / icon.Width, rect.Height / icon.Height);
                var w = icon.Width * scale;
                var h = icon.Width * scale;
                var x = rect.X + (rect.Width - w) / 2f;
                var y = rect.Y + (rect.Height - h) / 2f;
                g.DrawImage(
                    icon,
                    new RectangleF(x, y, w, h),
                    new RectangleF(0, 0, icon.Width, icon.Height),
                    GraphicsUnit.Pixel);
            }
        }
    }
}
