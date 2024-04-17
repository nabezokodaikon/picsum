using PicSum.Core.Base.Conf;
using PicSum.Core.Job.AsyncJob;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// 画像ファイル読込ロジック
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class ImageFileReadLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public Bitmap CreateThumbnail(Image srcImg, int thumbSize, ImageSizeMode sizeMode)
        {
            ArgumentNullException.ThrowIfNull(srcImg, nameof(srcImg));

            if (thumbSize < 0)
            {
                ArgumentOutOfRangeException.ThrowIfNegative(thumbSize, nameof(thumbSize));
            }

            var scale = Math.Min(thumbSize / (double)srcImg.Width, thumbSize / (double)srcImg.Height);
            var w = (int)(srcImg.Width * scale);
            var h = (int)(srcImg.Height * scale);

            var destImg = new Bitmap(w, h);
            using (var g = Graphics.FromImage(destImg))
            {
                g.InterpolationMode = InterpolationMode.Low;
                g.SmoothingMode = SmoothingMode.HighSpeed;
                if (sizeMode == ImageSizeMode.Original)
                {
                    using (var thumb = srcImg.GetThumbnailImage(w, h, () => false, IntPtr.Zero))
                    {
                        g.DrawImage(thumb, 0, 0, w, h);
                    }
                }
                else
                {
                    g.FillRectangle(Brushes.Yellow, new Rectangle(0, 0, w, h));
                }
            }

            return destImg;
        }
    }
}
