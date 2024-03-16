using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SWF.Common
{
    public static class ThumbnailUtil
    {
        private const int SHADOW_OFFSET = 2;
        private const int FRAME_OFFSET = 1;
        private static readonly Pen SHADOW_PEN = new Pen(Color.FromArgb(32, Color.Black));
        private static readonly Pen FRAME_PEN = new Pen(Color.FromArgb(64, Color.White));

        /// <summary>
        /// サムネイルを作成します。
        /// </summary>
        /// <param name="srcImg">作成元の画像</param>
        /// <param name="thumbWidth">作成するサムネイルの幅</param>
        /// <param name="thumbHeight">作成するサムネイルの高さ</param>
        /// <returns>サムネイル</returns>
        public static Image CreateThumbnail(Image srcImg, int thumbWidth, int thumbHeight)
        {
            if (srcImg == null)
            {
                throw new ArgumentNullException(nameof(srcImg));
            }

            var offset = (SHADOW_OFFSET + FRAME_OFFSET) * 2;

            var tw = thumbWidth - offset;
            var th = thumbHeight - offset;

            int w, h;
            if (Math.Max(srcImg.Width, srcImg.Height) <= Math.Min(tw, th))
            {
                w = srcImg.Width;
                h = srcImg.Height;
            }
            else
            {
                var scale = Math.Min(tw / (double)srcImg.Width, th / (double)srcImg.Height);
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

            var thumb = new Bitmap(w, h);
            using (var g = Graphics.FromImage(thumb))
            {
                g.InterpolationMode = InterpolationMode.Low;
                g.SmoothingMode = SmoothingMode.HighSpeed;
                using (var temp = srcImg.GetThumbnailImage(w, h, () => false, IntPtr.Zero))
                {
                    g.DrawImage(temp, 0, 0, w, h);
                }
            }

            return thumb;
        }

        /// <summary>
        /// ファイルのサムネイルを描画します。
        /// </summary>
        /// <param name="g"></param>
        /// <param name="thumb"></param>
        /// <param name="rect"></param>
        public static void DrawFileThumbnail(Graphics g, Image thumb, RectangleF rect)
        {
            if (g == null)
            {
                throw new ArgumentNullException(nameof(g));
            }

            if (thumb == null)
            {
                throw new ArgumentNullException(nameof(thumb));
            }

            var w = thumb.Width;
            var h = thumb.Height;
            var x = rect.X + (rect.Width - w) / 2f;
            var y = rect.Y + (rect.Height - h) / 2f;

            g.DrawRectangle(SHADOW_PEN,
                            x - SHADOW_OFFSET,
                            y - SHADOW_OFFSET,
                            w + SHADOW_OFFSET * 2,
                            h + SHADOW_OFFSET * 2);

            g.DrawRectangle(FRAME_PEN,
                            x - FRAME_OFFSET,
                            y - FRAME_OFFSET,
                            w + FRAME_OFFSET * 2,
                            h + FRAME_OFFSET * 2);

            g.DrawImage(thumb, x, y, w, h);
        }

        /// <summary>
        /// ファイルのサムネイルを調整して描画します。
        /// </summary>
        /// <param name="g"></param>
        /// <param name="thumb"></param>
        /// <param name="rect"></param>
        public static void AdjustDrawFileThumbnail(Graphics g, Image thumb, RectangleF rect)
        {
            if (g == null)
            {
                throw new ArgumentNullException(nameof(g));
            }

            if (thumb == null)
            {
                throw new ArgumentNullException(nameof(thumb));
            }

            var scale = Math.Min(rect.Width / thumb.Width, rect.Height / thumb.Height);
            var w = thumb.Width * scale - (SHADOW_OFFSET + FRAME_OFFSET) * 2;
            var h = thumb.Height * scale - (SHADOW_OFFSET + FRAME_OFFSET) * 2;
            var x = rect.X + (rect.Width - w) / 2f;
            var y = rect.Y + (rect.Height - h) / 2f;

            var shadowOffset = SHADOW_OFFSET * scale;
            var frameOffset = FRAME_OFFSET * scale;

            g.DrawRectangle(SHADOW_PEN,
                            x - shadowOffset,
                            y - shadowOffset,
                            w + shadowOffset * 2,
                            h + shadowOffset * 2); ;

            g.DrawRectangle(FRAME_PEN,
                            x - frameOffset,
                            y - frameOffset,
                            w + frameOffset * 2,
                            h + frameOffset * 2);

            g.DrawImage(thumb, x, y, w, h);
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
            if (g == null)
            {
                throw new ArgumentNullException(nameof(g));
            }

            if (thumb == null)
            {
                throw new ArgumentNullException(nameof(thumb));
            }

            if (icon == null)
            {
                throw new ArgumentNullException(nameof(icon));
            }

            DrawFileThumbnail(g, thumb, rect);
            g.DrawImage(icon, new RectangleF(rect.X, rect.Bottom - icon.Height, icon.Width, icon.Height));
        }

        /// <summary>
        /// フォルダのサムネイルを調整して描画します。
        /// </summary>
        /// <param name="g"></param>
        /// <param name="thumb"></param>
        /// <param name="rect"></param>
        /// <param name="icon"></param>
        public static void AdjustDrawDirectoryThumbnail(Graphics g, Image thumb, RectangleF rect, Image icon)
        {
            if (g == null)
            {
                throw new ArgumentNullException(nameof(g));
            }

            if (thumb == null)
            {
                throw new ArgumentNullException(nameof(thumb));
            }

            if (icon == null)
            {
                throw new ArgumentNullException(nameof(icon));
            }

            AdjustDrawFileThumbnail(g, thumb, rect);
            g.DrawImage(icon, new RectangleF(rect.X, rect.Bottom - icon.Height, icon.Width, icon.Height));
        }

        /// <summary>
        /// アイコンを描画します。
        /// </summary>
        /// <param name="g">グラフィックオブジェクト</param>
        /// <param name="icon">アイコン</param>
        /// <param name="rect">描画領域</param>
        public static void DrawIcon(Graphics g, Image icon, RectangleF rect)
        {
            if (g == null)
            {
                throw new ArgumentNullException(nameof(g));
            }

            if (icon == null)
            {
                throw new ArgumentNullException(nameof(icon));
            }

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
                g.DrawImage(icon, new RectangleF(x, y, w, h));
            }
        }
    }
}
