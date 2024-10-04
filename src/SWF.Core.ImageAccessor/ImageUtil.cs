using SWF.Core.FileAccessor;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO.MemoryMappedFiles;
using System.Runtime.Versioning;
using System.Security;
using System.Xml;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows")]
    public static class ImageUtil
    {
        public static readonly Size EMPTY_SIZE = Size.Empty;
        public static readonly Bitmap EMPTY_IMAGE = new(1, 1);

        private static readonly int BUFFER_SIZE = 64 * 1024;
        private static readonly EncoderParameter ENCORDER_PARAMETER = new(Encoder.Quality, 100L);
        private static readonly ImageCodecInfo PNG_CODEC_INFO = ImageCodecInfo.GetImageEncoders().Single(info => info.FormatID == ImageFormat.Png.Guid);
        private static readonly dynamic SHELL = Activator.CreateInstance(Type.GetTypeFromProgID("Shell.Application"));

        /// <summary>
        /// イメージオブジェクトを圧縮したバイナリに変換します。
        /// </summary>
        /// <param name="img">イメージオブジェクト</param>
        /// <returns></returns>
        public static byte[] ToCompressionBinary(Image img)
        {
            ArgumentNullException.ThrowIfNull(img, nameof(img));

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
        public static Bitmap ToImage(byte[] bf)
        {
            ArgumentNullException.ThrowIfNull(bf, nameof(bf));

            using (var mes = new MemoryStream(bf))
            {
                try
                {
                    var img = Bitmap.FromStream(mes, false, true);
                    return (Bitmap)img;
                }
                catch (OutOfMemoryException ex)
                {
                    throw new ImageUtilException("メモリが不足しています。", ex);
                }
            }
        }

        internal static Bitmap Resize(Bitmap bmp, int width, int height)
        {
            ArgumentNullException.ThrowIfNull(bmp, nameof(bmp));

            return OpenCVUtil.Resize(bmp, width, height);
        }

        public static ImageInfoCache GetImageInfo(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return ImageInfoCacheUtil.GetImageInfo(filePath);
        }

        /// <summary>
        /// 画像ファイルのサイズを取得します。
        /// </summary>
        /// <param name="filePath">取得するファイルのパス。</param>
        /// <returns>取得した画像サイズ。</returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static Size GetImageSize(string filePath)
        {
            var sw = Stopwatch.StartNew();

            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            try
            {
                try
                {
                    using (var fs = new FileStream(filePath,
                        FileMode.Open, FileAccess.Read, FileShare.Read, 64, FileOptions.SequentialScan))
                    {
                        var formatName = $".{SixLaborsUtil.DetectFormat(fs).Name.ToUpperInvariant()}";

                        if (FileUtil.IsWebpFile(formatName))
                        {
                            //return SixLaborsUtil.GetImageSize(filePath);
                        }
                        if (FileUtil.IsAvifFile(formatName))
                        {
                            return LibHeifSharpUtil.GetImageSize(filePath);
                        }
                        else if (FileUtil.IsHeifFile(formatName))
                        {
                            return LibHeifSharpUtil.GetImageSize(filePath);
                        }
                    }
                }
                catch (ArgumentException ex)
                {
                    throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
                }
                catch (NotSupportedException ex)
                {
                    throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
                }
                catch (FileNotFoundException ex)
                {
                    throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
                }
                catch (SecurityException ex)
                {
                    throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
                }
                catch (DirectoryNotFoundException ex)
                {
                    throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
                }
                catch (PathTooLongException ex)
                {
                    throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
                }
                catch (IOException ex)
                {
                    throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
                }
                catch (SixLabors.ImageSharp.InvalidImageContentException ex)
                {
                    throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
                }
                catch (SixLabors.ImageSharp.UnknownImageFormatException ex)
                {
                    throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
                }

                if (FileUtil.IsImageFile(filePath))
                {
                    var directory = ImageUtil.SHELL.NameSpace(Path.GetDirectoryName(filePath));
                    var item = directory.ParseName(Path.GetFileName(filePath));
                    var deteils = directory.GetDetailsOf(item, 31);
                    if (string.IsNullOrWhiteSpace(deteils))
                    {
                        throw new ImageUtilException(CreateFileAccessErrorMessage(filePath));
                    }

                    var v = deteils.Split(('x'));
                    if (v.Length != 2)
                    {
                        throw new ImageUtilException(CreateFileAccessErrorMessage(filePath));
                    }

                    var wText = v[0];
                    var hText = v[1];

                    if (!int.TryParse(wText.Substring(1).Trim(), out int w))
                    {
                        throw new ImageUtilException(CreateFileAccessErrorMessage(filePath));
                    }

                    if (!int.TryParse(hText.Substring(0, hText.Length - 1).Trim(), out int h))
                    {
                        throw new ImageUtilException(CreateFileAccessErrorMessage(filePath));
                    }

                    if (w < 1 || h < 1)
                    {
                        throw new ImageUtilException(CreateFileAccessErrorMessage(filePath));
                    }

                    return new Size(w, h);
                }
                else
                {
                    throw new ArgumentException(
                        $"画像ファイル以外のファイルが指定されました。'{filePath}'", nameof(filePath));
                }
            }
            finally
            {
                sw.Stop();
                Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageUtil.GetImageSize: {sw.ElapsedMilliseconds} ms");
            }
        }

        public static Bitmap ReadImageFile(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            try
            {
                Stopwatch sw;

                if (FileUtil.IsSvgFile(filePath))
                {
                    sw = Stopwatch.StartNew();
                    var bmp = SvgUtil.ReadImageFile(filePath);
                    sw.Stop();
                    Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageUtil.ReadImageFile Svg file: {sw.ElapsedMilliseconds} ms");
                    return bmp;
                }

                sw = Stopwatch.StartNew();
                using (var fs = new FileStream(filePath,
                    FileMode.Open, FileAccess.Read, FileShare.Read, BUFFER_SIZE, FileOptions.SequentialScan))
                {
                    sw.Stop();
                    Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageUtil.ReadImageFile new FileStream: {sw.ElapsedMilliseconds} ms");

                    if (FileUtil.IsIconFile(filePath))
                    {
                        using (var icon = new Icon(fs))
                        {
                            sw = Stopwatch.StartNew();
                            var bmp = icon.ToBitmap();
                            sw.Stop();
                            Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageUtil.ReadImageFile Icon file: {sw.ElapsedMilliseconds} ms");
                            return bmp;
                        }
                    }

                    var formatName = $".{SixLaborsUtil.DetectFormat(fs).Name.ToUpperInvariant()}";

                    if (FileUtil.IsWebpFile(formatName))
                    {
                        sw = Stopwatch.StartNew();
                        var bmp = SkiaSharpUtil.ReadImageFile(fs);
                        sw.Stop();
                        Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageUtil.ReadImageFile Webp file: {sw.ElapsedMilliseconds} ms");
                        return bmp;
                    }
                    else if (FileUtil.IsAvifFile(formatName))
                    {
                        sw = Stopwatch.StartNew();
                        var bmp = SixLaborsUtil.ReadImageFile(fs);
                        sw.Stop();
                        Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageUtil.ReadImageFile Avif file: {sw.ElapsedMilliseconds} ms");
                        return bmp;
                    }
                    else if (FileUtil.IsHeifFile(formatName))
                    {
                        sw = Stopwatch.StartNew();
                        var bmp = SixLaborsUtil.ReadImageFile(fs);
                        sw.Stop();
                        Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageUtil.ReadImageFile Heif file: {sw.ElapsedMilliseconds} ms");
                        return bmp;
                    }
                    else if (FileUtil.IsJpegFile(formatName))
                    {
                        sw = Stopwatch.StartNew();
                        var bmp = (Bitmap)Bitmap.FromStream(fs, false, true);
                        sw.Stop();
                        Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageUtil.ReadImageFile Jpeg file: {sw.ElapsedMilliseconds} ms");
                        return bmp;
                    }
                    else if (FileUtil.IsBmpFile(formatName))
                    {
                        sw = Stopwatch.StartNew();
                        var bmp = (Bitmap)Bitmap.FromStream(fs, false, true);
                        sw.Stop();
                        Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageUtil.ReadImageFile Bitmap file: {sw.ElapsedMilliseconds} ms");
                        return bmp;
                    }
                    else if (FileUtil.IsPngFile(formatName))
                    {
                        sw = Stopwatch.StartNew();
                        var bmp = (Bitmap)Bitmap.FromStream(fs, false, true);
                        sw.Stop();
                        Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageUtil.ReadImageFile Png file: {sw.ElapsedMilliseconds} ms");

                        if (bmp.PixelFormat == PixelFormat.Format8bppIndexed)
                        {
                            using (bmp)
                            {
                                var convBmp = OpenCVUtil.Convert(fs);
                                return convBmp;
                            }
                        }
                        else
                        {
                            return bmp;
                        }

                    }
                    else if (FileUtil.IsImageFile(filePath))
                    {
                        sw = Stopwatch.StartNew();
                        var bmp = (Bitmap)Bitmap.FromStream(fs, false, true);
                        sw.Stop();
                        Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageUtil.ReadImageFile Other file: {sw.ElapsedMilliseconds} ms");
                        return bmp;
                    }
                    else
                    {
                        throw new ArgumentException(
                            $"画像ファイル以外のファイルが指定されました。'{filePath}'", nameof(filePath));
                    }
                }
            }
            catch (ArgumentException ex)
            {
                throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (NotSupportedException ex)
            {
                throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (FileNotFoundException ex)
            {
                throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (SecurityException ex)
            {
                throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (PathTooLongException ex)
            {
                throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (IOException ex)
            {
                throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (XmlException ex)
            {
                throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (SixLabors.ImageSharp.InvalidImageContentException ex)
            {
                throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (SixLabors.ImageSharp.UnknownImageFormatException ex)
            {
                throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (LibHeifSharp.HeifException ex)
            {
                throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (OutOfMemoryException ex)
            {
                throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
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
            ArgumentNullException.ThrowIfNull(bmp, nameof(bmp));

            if (bmp.PixelFormat != PixelFormat.Format32bppArgb)
            {
                throw new ArgumentException($"ピクセルフォーマットが'{PixelFormat.Format32bppArgb}'ではありません。");
            }

            var w = bmp.Width;
            var h = bmp.Height;

            if (x < 0 || x > w - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            if (y < 0 || y > h - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(y));
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
            ArgumentNullException.ThrowIfNull(bmp, nameof(bmp));

            if (bmp.PixelFormat != PixelFormat.Format32bppArgb)
            {
                throw new ArgumentException($"ピクセルフォーマットが'{PixelFormat.Format32bppArgb}'ではありません。");
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

        private static string CreateFileAccessErrorMessage(string path)
        {
            return $"'{path}'を開けませんでした。";
        }
    }
}
