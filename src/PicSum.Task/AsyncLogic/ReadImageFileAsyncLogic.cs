using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using PicSum.Core.Base.Conf;
using PicSum.Core.Data.FileAccessor;
using PicSum.Core.Task.AsyncTask;
using SWF.Common;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// 画像ファイル読込ロジック
    /// </summary>
    internal class ReadImageFileAsyncLogic : AsyncLogicBase
    {
        public ReadImageFileAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public Image CreateImage(string filePath, Bitmap srcBmp, ImageSizeMode sizeMode, Size drawSize)
        {
            if (srcBmp == null)
            {
                throw new ArgumentNullException("srcImg");
            }

            if (sizeMode == ImageSizeMode.Original ||
                sizeMode == ImageSizeMode.FitOnlyBigImage && srcBmp.Width <= drawSize.Width && srcBmp.Height <= drawSize.Height)
            {
                return ImageUtil.CreateCloneBitmap(srcBmp);
            }
            else
            {
                double scale = Math.Min(drawSize.Width / (double)srcBmp.Width, drawSize.Height / (double)srcBmp.Height);
                int w = (int)(srcBmp.Width * scale);
                int h = (int)(srcBmp.Height * scale);

                Bitmap destImg = new Bitmap(w, h);
                using (Graphics g = Graphics.FromImage(destImg))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.DrawImage(srcBmp, new Rectangle(0, 0, w, h), new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), GraphicsUnit.Pixel);
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

            double scale = Math.Min(thumbSize / (double)srcImg.Width, thumbSize / (double)srcImg.Height);
            int w = (int)(srcImg.Width * scale);
            int h = (int)(srcImg.Height * scale);

            Bitmap destImg = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(destImg))
            {
                g.InterpolationMode = InterpolationMode.Low;
                g.SmoothingMode = SmoothingMode.HighSpeed;
                if (sizeMode == ImageSizeMode.Original)
                {
                    using (Image thumb = srcImg.GetThumbnailImage(w, h, () => false, IntPtr.Zero))
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
