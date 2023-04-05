using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using WinApi;
using System.Drawing.Drawing2D;
using Shell32;
using System.Runtime.InteropServices;

namespace SWF.Common
{
    public static class ImageUtil
    {
        private static readonly IList<string> _imageFileExtensionList = getImageFileExtensionList();
        private static readonly EncoderParameter _encorderParameter = new EncoderParameter(Encoder.Quality, 50L);
        private static readonly ImageCodecInfo _jpegCodecInfo = ImageCodecInfo.GetImageEncoders().Single(info => info.FormatID == ImageFormat.Jpeg.Guid);
        private static readonly ImageConverter _imageConverter = new ImageConverter();
        private static readonly dynamic _shell = Activator.CreateInstance(Type.GetTypeFromProgID("Shell.Application"));

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
                throw new ArgumentNullException("img");
            }

            using (MemoryStream mes = new MemoryStream())
            {
                EncoderParameters eps = new EncoderParameters(1);
                eps.Param[0] = _encorderParameter;
                img.Save(mes, _jpegCodecInfo, eps);
                byte[] buffer = new byte[mes.Length];
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
                throw new ArgumentNullException("bf");
            }

            var mes = new MemoryStream(bf);
            return Image.FromStream(mes, false, false);
        }

        ///// <summary>
        ///// ビットマップオブジェクトをバイナリに変換します。
        ///// </summary>
        ///// <param name="bmp">ビットマップオブジェクト</param>
        ///// <returns></returns>
        //public static byte[] ToBinary(Bitmap bmp)
        //{
        //    if (bmp == null)
        //    {
        //        throw new ArgumentNullException("bmp");
        //    }

        //    BitmapData bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
        //    IntPtr bitmapPtr = bd.Scan0;
        //    byte[] bf = new byte[bmp.Width * bmp.Height * 3];
        //    Marshal.Copy(bitmapPtr, bf, 0, bmp.Width * bmp.Height * 3);
        //    bmp.UnlockBits(bd);

        //    return bf;
        //}

        ///// <summary>
        ///// バッファから、ビットマップオブジェクトを作成します。
        ///// </summary>
        ///// <param name="bf"></param>
        ///// <param name="w"></param>
        ///// <param name="h"></param>
        ///// <param name="pf"></param>
        ///// <returns></returns>
        //public static Bitmap ToBitmap(byte[] bf, int w, int h, PixelFormat pf)
        //{
        //    if (bf == null)
        //    {
        //        throw new ArgumentNullException("bf");
        //    }

        //    Bitmap bmp = new Bitmap(w, h, pf);
        //    BitmapData bd = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, pf);
        //    Marshal.Copy(bf, 0, bd.Scan0, bf.Length);
        //    bmp.UnlockBits(bd);
        //    return bmp;
        //}

        ///// <summary>
        ///// ビットマップのクローンを作成します。
        ///// </summary>
        ///// <param name="bmp">クローンするビットマップ。</param>
        ///// <returns>クローンしたビットマップ。</returns>
        ///// <exception cref="ArgumentNullException"></exception>
        //public static Bitmap CreateCloneBitmap(Bitmap bmp)
        //{
        //    if (bmp == null)
        //    {
        //        throw new ArgumentNullException(nameof(bmp));
        //    }

        //    var bf = ToBinary(bmp);
        //    return ToBitmap(bf, bmp.Width, bmp.Height, bmp.PixelFormat);
        //}

        /// <summary>
        /// 画像ファイルのサイズを取得します。
        /// </summary>
        /// <param name="filePath">取得するファイルのパス。</param>
        /// <returns>取得した画像サイズ。</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Size GetImageSize(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            //var folder = _shell.NameSpace(Path.GetDirectoryName(filePath));
            //var item = folder.ParseName(Path.GetFileName(filePath));

            //var v = folder.GetDetailsOf(item, 31).Split(('x'));

            //var wText = v[0];
            //var hText = v[1];
            //var w = int.Parse(wText.Substring(1).Trim());
            //var h = int.Parse(hText.Substring(0, hText.Length - 1).Trim());

            //return new Size(w, h);

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

            //IntPtr imgPtr = IntPtr.Zero;
            //int result = WinApiMembers.GdipLoadImageFromFile(filePath, ref imgPtr);
            //if (result != 0)
            //{
            //    throw new Exception();
            //}

            //object obj = typeof(Bitmap).InvokeMember("FromGDIplus",
            //                                         BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod,
            //                                         null,
            //                                         null,
            //                                         new object[] { imgPtr });

            //return (Bitmap)obj;

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
                throw new ArgumentNullException("bmp");
            }

            if (bmp.PixelFormat != PixelFormat.Format32bppArgb)
            {
                throw new ArgumentException(string.Format("ピクセルフォーマットが{0}ではありません。", PixelFormat.Format32bppArgb));
            }

            int w = bmp.Width;
            int h = bmp.Height;

            if (x < 0 || x > w - 1)
            {
                throw new ArgumentOutOfRangeException("x");
            }

            if (y < 0 || y > h - 1)
            {
                throw new ArgumentOutOfRangeException("y");
            }

            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, w, h),
                                         ImageLockMode.ReadOnly,
                                         PixelFormat.Format32bppArgb);

            try
            {
                unsafe
                {
                    byte* p = (byte*)(void*)bd.Scan0;
                    p += (y * w + x) * 4;
                    int a = p[3];
                    int r = p[2];
                    int g = p[1];
                    int b = p[0];
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
                throw new ArgumentNullException("bmp");
            }

            if (bmp.PixelFormat != PixelFormat.Format32bppArgb)
            {
                throw new ArgumentException(string.Format("ピクセルフォーマットが{0}ではありません。", PixelFormat.Format32bppArgb));
            }

            int w = bmp.Width;
            int h = bmp.Height;

            using (GraphicsPath path = new GraphicsPath())
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                path.AddRectangle(rect);
                BitmapData bd = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                try
                {
                    unsafe
                    {
                        byte* p = (byte*)(void*)bd.Scan0;

                        for (int y = 0; y < h; ++y)
                        {
                            for (int x = 0; x < w; ++x)
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
            List<string> exList = new List<string>();
            ImageCodecInfo[] encs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo enc in encs)
            {
                string[] exs = enc.FilenameExtension.Replace("*", string.Empty).Split(';');
                exList.AddRange(exs);
            }

            return exList;
        }
    }
}
