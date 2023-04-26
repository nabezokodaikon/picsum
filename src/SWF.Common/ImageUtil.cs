using System;
using System.CodeDom;
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
        public static readonly Size EMPTY_SIZE = new Size(-1, -1);
        internal static readonly IList<string> IMAGE_FILE_EXTENSION_LIST = ImageUtil.GetImageFileExtensionList();

        private static readonly EncoderParameter ENCORDER_PARAMETER = new EncoderParameter(Encoder.Quality, 100L);
        private static readonly ImageCodecInfo PNG_CODEC_INFO = ImageCodecInfo.GetImageEncoders().Single(info => info.FormatID == ImageFormat.Png.Guid);
        private static readonly dynamic SHELL = Activator.CreateInstance(Type.GetTypeFromProgID("Shell.Application"));

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
                eps.Param[0] = ImageUtil.ENCORDER_PARAMETER;
                img.Save(mes, ImageUtil.PNG_CODEC_INFO, eps);
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
        /// 画像をリサイズします。
        /// </summary>
        /// <param name="srcImg"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Image ResizeImage(Bitmap srcImg, double scale)
        {
            if (srcImg == null)
            {
                throw new ArgumentNullException(nameof(srcImg));
            }

            var w = (int)(srcImg.Width * scale);
            var h = (int)(srcImg.Height * scale);
            var destImg = new Bitmap(w, h);
            using (var g = Graphics.FromImage(destImg))
            {
                g.DrawImage(srcImg, 0, 0, w, h);
                return destImg;
            }
        }

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

            var directory = ImageUtil.SHELL.NameSpace(Path.GetDirectoryName(filePath));
            var item = directory.ParseName(Path.GetFileName(filePath));
            var deteils = directory.GetDetailsOf(item, 31);
            if (string.IsNullOrWhiteSpace(deteils))
            {
                return ImageUtil.EMPTY_SIZE;
            }

            var v = deteils.Split(('x'));
            if (v.Length != 2)
            {
                return ImageUtil.EMPTY_SIZE;
            }

            var wText = v[0];
            var hText = v[1];

            if (!int.TryParse(wText.Substring(1).Trim(), out int w))
            {
                return ImageUtil.EMPTY_SIZE;
            }

            if (!int.TryParse(hText.Substring(0, hText.Length - 1).Trim(), out int h))
            {
                return ImageUtil.EMPTY_SIZE;
            }

            if (w < 1 || h < 1)
            {
                return ImageUtil.EMPTY_SIZE;
            }

            return new System.Drawing.Size(w, h);
        }

        /// <summary>
        /// 画像ファイルを読込みます。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Bitmap ReadImageFile(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var destImg = new Bitmap(fs);
                return destImg;
            }
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
                throw new ArgumentException(string.Format("ピクセルフォーマットが'{0}'ではありません。", PixelFormat.Format32bppArgb));
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
        /// 指定した色を除いた画像の領域を取得します。
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
                throw new ArgumentException(string.Format("ピクセルフォーマットが'{0}'ではありません。", System.Drawing.Imaging.PixelFormat.Format32bppArgb));
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
        private static IList<string> GetImageFileExtensionList()
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
