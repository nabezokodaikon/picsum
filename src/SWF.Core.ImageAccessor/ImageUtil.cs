using NLog;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Xml;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class ImageUtil
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly Size EMPTY_SIZE = System.Drawing.Size.Empty;
        public static readonly Bitmap EMPTY_IMAGE = new(1, 1);

        private static readonly int FILE_READ_BUFFER_SIZE = 16384;

        private static IShellApplication GetSell()
        {
            var type = Type.GetTypeFromProgID("Shell.Application")
                ?? throw new NullReferenceException("Shell.Applicationを取得できませんでした。");

            var obj = Activator.CreateInstance(type)
                ?? throw new NullReferenceException("Shell.Applicationを取得できませんでした。");

            return (IShellApplication)obj;
        }

        public static bool HasTransparentPixels(Bitmap bitmap)
        {
            ArgumentNullException.ThrowIfNull(bitmap, nameof(bitmap));
            if (bitmap.PixelFormat == PixelFormat.Format32bppArgb)
            {
                BitmapData? bitmapData = null;
                try
                {
                    bitmapData = bitmap.LockBits(
                        new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        ImageLockMode.ReadOnly,
                        PixelFormat.Format32bppArgb);
                    var ptr = bitmapData.Scan0;
                    var byteCount = bitmapData.Stride * bitmap.Height;
                    unsafe
                    {
                        var pixelPtr = (byte*)ptr.ToPointer();
                        for (var i = 0; i < byteCount; i += 4)
                        {
                            var alpha = pixelPtr[i + 3];
                            if (alpha == 0)
                            {
                                return true;
                            }
                        }
                    }
                }
                finally
                {
                    if (bitmapData != null)
                    {
                        bitmap.UnlockBits(bitmapData);
                    }
                }
            }
            return false;
        }

        internal static Bitmap Resize(Bitmap srcBmp, int width, int height)
        {
            ArgumentNullException.ThrowIfNull(srcBmp, nameof(srcBmp));

            return OpenCVUtil.Resize(srcBmp, width, height);
        }

        /// <summary>
        /// 画像ファイルのサイズを取得します。
        /// </summary>
        /// <param name="filePath">取得するファイルのパス。</param>
        /// <returns>取得した画像サイズ。</returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static Size GetImageSize(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            using (TimeMeasuring.Run(false, "ImageUtil.GetImageSize"))
            {
                try
                {
                    using (var fs = new FileStream(filePath,
                        FileMode.Open, FileAccess.Read, FileShare.Read, 64, FileOptions.SequentialScan))
                    {
                        var formatName = SixLaborsUtil.DetectFormat(fs);

                        if (FileUtil.IsWebpFile(formatName))
                        {
                            return SixLaborsUtil.GetImageSize(fs);
                        }
                        if (FileUtil.IsAvifFile(formatName))
                        {
                            return LibHeifSharpUtil.GetImageSize(fs);
                        }
                        else if (FileUtil.IsHeifFile(formatName))
                        {
                            return LibHeifSharpUtil.GetImageSize(fs);
                        }
                        else if (FileUtil.IsGifFile(formatName))
                        {
                            return SixLaborsUtil.GetImageSize(fs);
                        }
                        else if (FileUtil.IsJpegFile(formatName))
                        {
                            return SixLaborsUtil.GetImageSize(fs);
                        }
                        else if (FileUtil.IsPngFile(formatName))
                        {
                            var size = GetPngSize(fs);
                            if (size != EMPTY_SIZE)
                            {
                                return size;
                            }
                        }
                        else if (FileUtil.IsBmpFile(formatName))
                        {
                            var size = GetBmpSize(fs);
                            if (size != EMPTY_SIZE)
                            {
                                return size;
                            }
                        }
                    }
                }
                catch (ArgumentNullException ex)
                {
                    throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
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
                catch (EndOfStreamException ex)
                {
                    throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
                }
                catch (ObjectDisposedException ex)
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
                catch (LibHeifSharp.HeifException ex)
                {
                    throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
                }

                if (FileUtil.IsImageFile(filePath))
                {
                    IShellApplication? shell = null;
                    try
                    {
                        shell = GetSell();
                        var directory = shell.NameSpace(FileUtil.GetParentDirectoryPath(filePath));
                        var item = directory.ParseName(Path.GetFileName(filePath));
                        var details = directory.GetDetailsOf(item, 31);
                        if (string.IsNullOrWhiteSpace(details))
                        {
                            throw new ImageUtilException(CreateFileAccessErrorMessage(filePath));
                        }

                        var v = details.Split(('x'));
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
                    catch (COMException)
                    {
                        using (var bmp = ReadImageFile(filePath))
                        {
                            return bmp.Size;
                        }
                    }
                    finally
                    {
                        if (shell != null)
                        {
                            Marshal.ReleaseComObject(shell);
                            shell = null;
                        }
                    }
                }
                else
                {
                    throw new ArgumentException(
                        $"画像ファイル以外のファイルが指定されました。'{filePath}'", nameof(filePath));
                }
            }
        }

        public static Bitmap ReadImageFile(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFile"))
            {
                try
                {
                    return ReadImageFileFromFileStream(filePath);
                }
                catch (ImageUtilException ex)
                {
                    Logger.Error(ex);
                    return ReadImageFileFromFilePath(filePath);
                }
            }
        }

        private static Bitmap ReadImageFileFromFileStream(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            try
            {
                using (var fs = new FileStream(filePath,
                    FileMode.Open, FileAccess.Read, FileShare.Read, FILE_READ_BUFFER_SIZE, FileOptions.SequentialScan))
                {
                    if (FileUtil.IsIconFile(filePath))
                    {
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileFromFileStream: Icon"))
                        using (var icon = new Icon(fs))
                        {
                            return ConvertIfGrayscale(icon.ToBitmap(), fs);
                        }
                    }
                    else if (FileUtil.IsSvgFile(filePath))
                    {
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileFromFileStream: Svg"))
                        {
                            return MagickUtil.ReadImageFile(fs);
                        }
                    }

                    var formatName = SixLaborsUtil.DetectFormat(fs);
                    if (FileUtil.IsAvifFile(formatName))
                    {
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileFromFileStream: Avif"))
                        {
                            return ConvertIfGrayscale(SixLaborsUtil.ReadImageFile(fs), fs);
                        }
                    }
                    else if (FileUtil.IsBmpFile(formatName))
                    {
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileFromFileStream: Bmp"))
                        {
                            return ConvertIfGrayscale((Bitmap)Bitmap.FromStream(fs, false, true), fs);
                        }
                    }
                    else if (FileUtil.IsGifFile(formatName))
                    {
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileFromFileStream: Gif"))
                        {
                            return ConvertIfGrayscale((Bitmap)Bitmap.FromStream(fs, false, true), fs);
                        }
                    }
                    else if (FileUtil.IsHeifFile(formatName))
                    {
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileFromFileStream: Heif"))
                        {
                            return ConvertIfGrayscale(MagickUtil.ReadImageFile(fs), fs);
                        }
                    }
                    else if (FileUtil.IsJpegFile(formatName))
                    {
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileFromFileStream: Jpeg"))
                        {
                            var bmp = ConvertIfGrayscale(OpenCVUtil.ReadImageFile(fs), fs);
                            return LoadBitmapCorrectOrientation(bmp);
                        }
                    }
                    else if (FileUtil.IsPngFile(formatName))
                    {
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileFromFileStream: Png"))
                        {
                            return ConvertIfGrayscale((Bitmap)Bitmap.FromStream(fs, false, true), fs);
                        }
                    }
                    else if (FileUtil.IsWebpFile(formatName))
                    {
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileFromFileStream: Webp"))
                        {
                            return ConvertIfGrayscale(OpenCVUtil.ReadImageFile(fs), fs);
                        }
                    }
                    else
                    {
                        throw new ImageUtilException(CreateFileAccessErrorMessage(filePath));
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
            catch (ImageMagick.MagickException ex)
            {
                throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (OutOfMemoryException ex)
            {
                throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
        }

        private static Bitmap ReadImageFileFromFilePath(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            try
            {
                var format = MagickUtil.DetectFormat(filePath);
                switch (format)
                {
                    case ImageMagick.MagickFormat.Avif:
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileFromFilePath: Avif"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Bmp:
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileFromFilePath: Bmp"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Gif:
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileFromFilePath: Gif"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Heic:
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileFromFilePath: Heic"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Heif:
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileFromFilePath: Heif"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Icon:
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileFromFilePath: Icon"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Jpeg:
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileFromFilePath: Jpeg"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Png:
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileFromFilePath: Png"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Svg:
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileFromFilePath: Svg"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.WebP:
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileFromFilePath: WebP"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    default:
                        throw new ImageUtilException(CreateFileAccessErrorMessage(filePath));
                }
            }
            catch (ImageMagick.MagickException ex)
            {
                throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
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
                BitmapData? bd = null;

                try
                {
                    bd = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                    unsafe
                    {
                        var p = (byte*)(void*)bd.Scan0;

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
                    if (bd != null)
                    {
                        bmp.UnlockBits(bd);
                        bd = null;
                    }
                }

                return new Region(path);
            }
        }

        private static Bitmap LoadBitmapCorrectOrientation(Bitmap bitmap)
        {
            ArgumentNullException.ThrowIfNull(bitmap, nameof(bitmap));

            const int ExifOrientationID = 0x112;
            if (bitmap.PropertyIdList.Contains(ExifOrientationID))
            {
                var prop = bitmap.GetPropertyItem(ExifOrientationID);
                if (prop == null || prop.Value == null)
                {
                    return bitmap;
                }

                var orientationValue = BitConverter.ToUInt16(prop.Value, 0);

                var rotateFlipType = RotateFlipType.RotateNoneFlipNone;
                switch (orientationValue)
                {
                    case 3:
                        rotateFlipType = RotateFlipType.Rotate180FlipNone;
                        break;
                    case 6:
                        rotateFlipType = RotateFlipType.Rotate90FlipNone;
                        break;
                    case 8:
                        rotateFlipType = RotateFlipType.Rotate270FlipNone;
                        break;
                }

                if (rotateFlipType != RotateFlipType.RotateNoneFlipNone)
                {
                    bitmap.RotateFlip(rotateFlipType);
                }
            }

            return bitmap;
        }

        private static Size GetJpegSize(FileStream fs)
        {
            using (var reader = new BinaryReader(fs))
            {
                if (reader.ReadByte() == 0xFF && reader.ReadByte() == 0xD8)
                {
                    while (fs.Position < fs.Length)
                    {
                        if (reader.ReadByte() == 0xFF)
                        {
                            var marker = reader.ReadByte();
                            if (marker >= 0xC0 && marker <= 0xC3)
                            {
                                fs.Seek(3, SeekOrigin.Current);
                                var height = reader.ReadUInt16();
                                var width = reader.ReadUInt16();
                                return new Size(width, height);
                            }
                            else
                            {
                                var length = reader.ReadUInt16();
                                fs.Seek(length - 2, SeekOrigin.Current);
                            }
                        }
                    }
                }
            }

            return EMPTY_SIZE;
        }

        private static Size GetPngSize(FileStream fs)
        {
            using (var reader = new BinaryReader(fs))
            {
                var pngSignature = reader.ReadBytes(8);
                var expectedPngSignature = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };
                if (!CompareByteArrays(pngSignature, expectedPngSignature))
                {
                    return EMPTY_SIZE;
                }

                reader.ReadInt32();
                var ihdrChunkType = reader.ReadBytes(4);
                var ihdrChunkTypeStr = System.Text.Encoding.ASCII.GetString(ihdrChunkType);
                if (ihdrChunkTypeStr != "IHDR")
                {
                    return EMPTY_SIZE;
                }

                var width = ReadBigEndianInt32(reader);
                var height = ReadBigEndianInt32(reader);
                return new Size(width, height);
            }
        }

        private static bool CompareByteArrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            for (var i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        private static int ReadBigEndianInt32(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(4);
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        private static Size GetBmpSize(FileStream fs)
        {
            using (var reader = new BinaryReader(fs))
            {
                var bmpSignature = reader.ReadBytes(2);
                if (bmpSignature[0] != 'B' || bmpSignature[1] != 'M')
                {
                    return EMPTY_SIZE;
                }

                reader.BaseStream.Seek(18, SeekOrigin.Begin);

                var width = reader.ReadInt32();
                var height = reader.ReadInt32();
                return new Size(width, Math.Abs(height));
            }
        }

        private static Bitmap ConvertIfGrayscale(Bitmap bmp, Stream fs)
        {
            if (bmp.PixelFormat == PixelFormat.Format4bppIndexed
                || bmp.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                using (bmp)
                {
                    var convBmp = OpenCVUtil.ReadImageFile(fs);
                    return convBmp;
                }
            }
            else
            {
                return bmp;
            }
        }

        private static string CreateFileAccessErrorMessage(string path)
        {
            return $"'{path}'へのアクセスに失敗しました。";
        }
    }
}
