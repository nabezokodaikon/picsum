using System.Diagnostics;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows")]
    public static class ThumbnailUtil
    {
        /// <summary>
        /// サムネイルを作成します。
        /// </summary>
        /// <param name="srcImg">作成元の画像</param>
        /// <param name="thumbWidth">作成するサムネイルの幅</param>
        /// <param name="thumbHeight">作成するサムネイルの高さ</param>
        /// <returns>サムネイル</returns>
        public static Image CreateThumbnail(Image srcImg, int thumbWidth, int thumbHeight)
        {
            var sw = Stopwatch.StartNew();

            ArgumentNullException.ThrowIfNull(srcImg, nameof(srcImg));

            int w, h;
            if (Math.Max(srcImg.Width, srcImg.Height) <= Math.Min(thumbWidth, thumbHeight))
            {
                w = srcImg.Width;
                h = srcImg.Height;
            }
            else
            {
                var scale = Math.Min(thumbWidth / (double)srcImg.Width, thumbHeight / (double)srcImg.Height);
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

            var thumb = ImageUtil.Resize((Bitmap)srcImg, w, h);

            sw.Stop();
            //Console.WriteLine($"[{Thread.CurrentThread.Name}] ThumbnailUtil.CreateThumbnail: {sw.ElapsedMilliseconds} ms");

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
            ArgumentNullException.ThrowIfNull(g, nameof(g));
            ArgumentNullException.ThrowIfNull(thumb, nameof(thumb));

            const int offset = 16;

            var scale = Math.Min(rect.Width / thumb.Width, rect.Height / thumb.Height);

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

            var x = rect.X + (rect.Width - w) / 2f;
            var y = rect.Y + (rect.Height - h) / 2f;

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
            ArgumentNullException.ThrowIfNull(g, nameof(g));
            ArgumentNullException.ThrowIfNull(thumb, nameof(thumb));
            ArgumentNullException.ThrowIfNull(icon, nameof(icon));

            DrawFileThumbnail(g, thumb, rect);
            g.DrawImage(icon, new RectangleF(rect.X + 2, rect.Bottom - icon.Height, icon.Width, icon.Height));
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
            ArgumentNullException.ThrowIfNull(g, nameof(g));
            ArgumentNullException.ThrowIfNull(thumb, nameof(thumb));
            ArgumentNullException.ThrowIfNull(icon, nameof(icon));

            AdjustDrawFileThumbnail(g, thumb, rect);
            g.DrawImage(icon, new RectangleF(rect.X + 2, rect.Bottom - icon.Height, icon.Width, icon.Height));
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
                g.DrawImage(icon, new RectangleF(x, y, w, h));
            }
        }
    }
}
