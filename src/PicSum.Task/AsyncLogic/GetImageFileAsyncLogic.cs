using PicSum.Core.Base.Conf;
using PicSum.Core.Task.AsyncTask;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// 画像ファイル読込ロジック
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class GetImageFileAsyncLogic
        : AbstractAsyncLogic
    {
        public GetImageFileAsyncLogic(AbstractAsyncFacade facade)
            : base(facade)
        {

        }

        public float GetImageScale(Bitmap bmp, ImageSizeMode sizeMode, Size drawSize)
        {
            if (bmp == null)
            {
                throw new ArgumentNullException(nameof(bmp));
            }

            if (sizeMode == ImageSizeMode.Original ||
                sizeMode == ImageSizeMode.FitOnlyBigImage && bmp.Width <= drawSize.Width && bmp.Height <= drawSize.Height)
            {
                return 1.0f;
            }
            else
            {
                var scale = Math.Min(drawSize.Width / (float)bmp.Width, drawSize.Height / (float)bmp.Height);
                return scale;
            }
        }

        public Bitmap CreateThumbnail(Image srcImg, int thumbSize, ImageSizeMode sizeMode)
        {
            if (srcImg == null)
            {
                throw new ArgumentNullException(nameof(srcImg));
            }

            if (thumbSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(thumbSize));
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
