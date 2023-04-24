using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using PicSum.Core.Base.Conf;
using PicSum.Core.Task.AsyncTask;
using SWF.Common;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// 画像ファイル読込ロジック
    /// </summary>
    internal class GetImageFileAsyncLogic : AsyncLogicBase
    {
        public GetImageFileAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public Image CreateImage(string filePath, Bitmap srcImg, ImageSizeMode sizeMode, Size drawSize)
        {
            if (srcImg == null)
            {
                throw new ArgumentNullException("srcImg");
            }

            if (sizeMode == ImageSizeMode.Original ||
                sizeMode == ImageSizeMode.FitOnlyBigImage && srcImg.Width <= drawSize.Width && srcImg.Height <= drawSize.Height)
            {
                var destImg = (Image)srcImg.Clone();
                return destImg;
            }
            else
            {
                var scale = Math.Min(drawSize.Width / (double)srcImg.Width, drawSize.Height / (double)srcImg.Height);
                var destImg = ImageUtil.ResizeImage(srcImg, scale);
                if (destImg == null)
                {
                    throw new ImageUtilException(filePath);
                }

                return destImg;
            }
        }

        public Image CreateThumbnail(Image srcImg, int thumbSize, ImageSizeMode sizeMode)
        {
            if (srcImg == null)
            {
                throw new ArgumentNullException("srcImg");
            }

            if (thumbSize < 0)
            {
                throw new ArgumentOutOfRangeException("thumbSize");
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
