using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SWF.Common
{
    public static class ThumbnailUtil
    {
        private const int SHADOW_OFFSET = 1;
        private const int FRAME_OFFSET = 1;
        private static readonly SolidBrush _shadowBrush = new SolidBrush(Color.FromArgb(64, Color.Black));
        private static readonly SolidBrush _frameBrush = new SolidBrush(Color.White);

        /// <summary>
        /// サムネイルを作成します。
        /// </summary>
        /// <param name="srcBmp">作成元の画像</param>
        /// <param name="thumbWidth">作成するサムネイルの幅</param>
        /// <param name="thumbHeight">作成するサムネイルの高さ</param>
        /// <returns>サムネイル</returns>
        public static Image CreateThumbnail(Image srcBmp, int thumbWidth, int thumbHeight)
        {
            if (srcBmp == null)
            {
                throw new ArgumentNullException("srcBmp");
            }

            int offset = (SHADOW_OFFSET + FRAME_OFFSET) * 2;

            int tw = thumbWidth - offset;
            int th = thumbHeight - offset;

            int w, h;
            if (Math.Max(srcBmp.Width, srcBmp.Height) <= Math.Min(tw, th))
            {
                w = srcBmp.Width;
                h = srcBmp.Height;
            }
            else
            {
                double scale = Math.Min(tw / (double)srcBmp.Width, th / (double)srcBmp.Height);
                w = (int)(srcBmp.Width * scale);
                h = (int)(srcBmp.Height * scale);
            }

            Image thumb = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(thumb))
            {
                g.InterpolationMode = InterpolationMode.Low;
                g.SmoothingMode = SmoothingMode.HighSpeed;
                using (Image temp = srcBmp.GetThumbnailImage(w, h, () => false, IntPtr.Zero))
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
            float w = thumb.Width;
            float h = thumb.Height;
            float x = rect.X + (rect.Width - w) / 2f;
            float y = rect.Y + (rect.Height - h) / 2f;

            int frameOffset = FRAME_OFFSET * 2;

            g.FillRectangle(_shadowBrush,
                            x,
                            y,
                            w + frameOffset,
                            h + frameOffset);

            g.FillRectangle(_frameBrush,
                            x - FRAME_OFFSET,
                            y - FRAME_OFFSET,
                            w + frameOffset,
                            h + frameOffset);

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
            float scale = Math.Min(rect.Width / thumb.Width, rect.Height / thumb.Height);
            float w = thumb.Width * scale - (SHADOW_OFFSET + FRAME_OFFSET) * 2;
            float h = thumb.Height * scale - (SHADOW_OFFSET + FRAME_OFFSET) * 2;
            float x = rect.X + (rect.Width - w) / 2f;
            float y = rect.Y + (rect.Height - h) / 2f;

            int frameOffset = FRAME_OFFSET * 2;

            g.FillRectangle(_shadowBrush,
                            x,
                            y,
                            w + frameOffset,
                            h + frameOffset);

            g.FillRectangle(_frameBrush,
                            x - FRAME_OFFSET,
                            y - FRAME_OFFSET,
                            w + frameOffset,
                            h + frameOffset);

            g.DrawImage(thumb, x, y, w, h);
        }

        /// <summary>
        /// フォルダのサムネイルを描画します。
        /// </summary>
        /// <param name="g"></param>
        /// <param name="thumb"></param>
        /// <param name="rect"></param>
        /// <param name="icon"></param>
        public static void DrawFolderThumbnail(Graphics g, Image thumb, RectangleF rect, Image icon)
        {
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
        public static void AdjustDrawFolderThumbnail(Graphics g, Image thumb, RectangleF rect, Image icon)
        {
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
                throw new ArgumentNullException("g");
            }

            if (icon == null)
            {
                throw new ArgumentNullException("icon");
            }

            if (Math.Max(icon.Width, icon.Height) <= Math.Min(rect.Width, rect.Height))
            {
                float w = icon.Width;
                float h = icon.Height;
                float x = rect.X + (rect.Width - w) / 2f;
                float y = rect.Y + (rect.Height - h) / 2f;
                g.DrawImage(icon, new RectangleF(x, y, w, h));
            }
            else
            {
                float scale = Math.Min(rect.Width / icon.Width, rect.Height / icon.Height);
                float w = icon.Width * scale;
                float h = icon.Width * scale;
                float x = rect.X + (rect.Width - w) / 2f;
                float y = rect.Y + (rect.Height - h) / 2f;
                g.DrawImage(icon, new RectangleF(x, y, w, h));
            }
        }
    }
}
