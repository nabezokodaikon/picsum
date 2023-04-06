using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace SWF.Common
{
    public static class ImageUtil
    {
        private static readonly IList<string> _imageFileExtensionList = getImageFileExtensionList();
        private static readonly EncoderParameter _encorderParameter = new EncoderParameter(Encoder.Quality, 50L);
        private static readonly ImageCodecInfo _jpegCodecInfo = ImageCodecInfo.GetImageEncoders().Single(info => info.FormatID == ImageFormat.Jpeg.Guid);
        private static readonly ImageConverter _imageConverter = new ImageConverter();

        /// <summary>
        /// 画像ファイルの拡張子リスト
        /// </summary>
        public static IList<string> ImageFileExtensionList
        {
            get
            {
                return _imageFileExtensionList;
            }
        }

        /// <summary>
        /// イメージオブジェクトを圧縮したバイナリに変換します。
        /// </summary>
        /// <param name="img">イメージオブジェクト</param>
        /// <returns></returns>
        public static byte[] ToCompressionBinary(Image img)
        {
            if (img == null)
            {
                throw new ArgumentNullException(nameof(img));
            }

            using (var mes = new MemoryStream())
            {
                var eps = new EncoderParameters(1);
                eps.Param[0] = _encorderParameter;
                img.Save(mes, _jpegCodecInfo, eps);
                var buffer = new byte[mes.Length];
                mes.Position = 0;
                mes.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        /// <summary>
        /// バイト配列からイメージオブジェクトを取得します。
        /// </summary>
        /// <param name="bf">バイト配列</param>
        /// <returns>イメージオブジェクト</returns>
        public static Image ToImage(byte[] bf)
        {
            if (bf == null)
            {
                throw new ArgumentNullException(nameof(bf));
            }

            var mes = new MemoryStream(bf);
            var img = Image.FromStream(mes, false, false);
            return img;
        }

        /// <summary>
        /// ビットマップオブジェクトをバイナリに変換します。
        /// </summary>
        /// <param name="img">画像オブジェクト。</param>
        /// <returns></returns>
        public static byte[] ToBinary(Image img)
        {
            if (img == null)
            {
                throw new ArgumentNullException(nameof(img));
            }

            using (var bmp = new Bitmap(img))
            {
                var bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                var bitmapPtr = bd.Scan0;
                var len = bmp.Width * bmp.Height * 3;
                var bf = new byte[len];
                Marshal.Copy(bitmapPtr, bf, 0, len);
                bmp.UnlockBits(bd);

                return bf;
            }
        }

        public static Stream ToStream(Image img, ImageFormat format)
        {
            if (img == null)
            {
                throw new ArgumentNullException(nameof(img));
            }

            var stream = new MemoryStream();
            img.Save(stream, format);
            stream.Position = 0;
            return stream;
        }

        public static Image ResizeImage(string filePath, Image srcImg, double scale)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (srcImg == null)
            {
                throw new ArgumentNullException(nameof(srcImg));
            }

            Console.WriteLine("ResizeImage");
            var sw = Stopwatch.StartNew();

            using (var src = new Mat(filePath))
            {
                if (src.Width < 1 || src.Height < 1)
                {
                    return null;
                }

                using (var resize = new Mat())
                {
                    Cv2.Resize(src, resize, new OpenCvSharp.Size(), scale, scale, InterpolationFlags.Area);

                    using (var ms = resize.ToMemoryStream())
                    {
                        var destImg = Bitmap.FromStream(ms);
                        sw.Stop();
                        Console.WriteLine("[{0}] ResizeImage", sw.ElapsedMilliseconds);
                        return destImg;
                    }
                }
            }
        }

        /// <summary>
        /// 画像ファイルのサイズを取得します。
        /// </summary>
        /// <param name="filePath">取得するファイルのパス。</param>
        /// <returns>取得した画像サイズ。</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static System.Drawing.Size GetImageSize(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            using (var img = ReadImageFile(filePath))
            {
                return img.Size;
            }
        }

        /// <summary>
        /// 画像ファイルを読込みます。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Image ReadImageFile(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Bitmap.FromStream(fs, false, false);
        }

        /// <summary>
        /// ポイント値をピクセル値に変換します。
        /// </summary>
        /// <param name="points">Font.SizeInPoints</param>
        /// <param name="dpiY">Graphics.DpiY</param>
        /// <returns></returns>
        public static float PointsToPixels(float points, float dpiY)
        {
            return points * dpiY / 72f;
        }

        /// <summary>
        /// ビットマップの指定座標の色を取得します。
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Color GetPixel(Bitmap bmp, int x, int y)
        {
            if (bmp == null)
            {
                throw new ArgumentNullException(nameof(bmp));
            }

            if (bmp.PixelFormat != PixelFormat.Format32bppArgb)
            {
                throw new ArgumentException(string.Format("ピクセルフォーマットが{0}ではありません。", System.Drawing.Imaging.PixelFormat.Format32bppArgb));
            }

            var w = bmp.Width;
            var h = bmp.Height;

            if (x < 0 || x > w - 1)
            {
                throw new ArgumentOutOfRangeException("x");
            }

            if (y < 0 || y > h - 1)
            {
                throw new ArgumentOutOfRangeException("y");
            }

            var bd = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                unsafe
                {
                    byte* p = (byte*)(void*)bd.Scan0;
                    p += (y * w + x) * 4;
                    var a = p[3];
                    var r = p[2];
                    var g = p[1];
                    var b = p[0];
                    return Color.FromArgb(a, r, g, b);
                }
            }
            finally
            {
                bmp.UnlockBits(bd);
            }
        }

        /// <summary>
        /// 透過色を除いた画像の領域を取得します。
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Region GetRegion(Bitmap bmp)
        {
            if (bmp == null)
            {
                throw new ArgumentNullException(nameof(bmp));
            }

            return GetRegion(bmp, Color.Transparent);
        }

        /// <summary>
        /// 透過色を除いた画像の領域を取得します。
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="transparent"></param>
        /// <returns></returns>
        public static Region GetRegion(Bitmap bmp, Color transparent)
        {
            if (bmp == null)
            {
                throw new ArgumentNullException(nameof(bmp));
            }

            if (bmp.PixelFormat != PixelFormat.Format32bppArgb)
            {
                throw new ArgumentException(string.Format("ピクセルフォーマットが{0}ではありません。", System.Drawing.Imaging.PixelFormat.Format32bppArgb));
            }

            var w = bmp.Width;
            var h = bmp.Height;

            using (var path = new GraphicsPath())
            {
                var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                path.AddRectangle(rect);
                var bd = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                try
                {
                    unsafe
                    {
                        byte* p = (byte*)(void*)bd.Scan0;

                        for (var y = 0; y < h; ++y)
                        {
                            for (var x = 0; x < w; ++x)
                            {
                                if (p[3] == transparent.A &&
                                    p[2] == transparent.R &&
                                    p[1] == transparent.G &&
                                    p[0] == transparent.B)
                                {
                                    path.AddRectangle(new Rectangle(x, y, 1, 1));
                                }

                                p += 4;
                            }
                        }
                    }
                }
                finally
                {
                    bmp.UnlockBits(bd);
                }

                return new Region(path);
            }
        }

        /// <summary>
        /// 画像ファイルの拡張子リストを取得します。
        /// </summary>
        /// <remarks>リスト内の各項目には、ピリオド + 英大文字 * n の文字列(.XXX)が格納されます。</remarks>
        /// <returns></returns>
        private static IList<string> getImageFileExtensionList()
        {
            var exList = new List<string>();
            var encs = ImageCodecInfo.GetImageEncoders();
            foreach (var enc in encs)
            {
                var exs = enc.FilenameExtension.Replace("*", string.Empty).Split(';');
                exList.AddRange(exs);
            }

            return exList;
        }
    }
}
