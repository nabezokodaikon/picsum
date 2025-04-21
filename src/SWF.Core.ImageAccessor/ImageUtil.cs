using NLog;
using SWF.Core.Base;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Xml.Linq;
using ZLinq;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class ImageUtil
    {
        internal const string AVIF_FILE_EXTENSION = ".avif";
        internal const string BMP_FILE_EXTENSION = ".bmp";
        internal const string GIF_FILE_EXTENSION = ".gif";
        internal const string HEIC_FILE_EXTENSION = ".heic";
        internal const string HEIF_FILE_EXTENSION = ".heif";
        internal const string ICON_FILE_EXTENSION = ".ico";
        internal const string JPEG_FILE_EXTENSION = ".jpeg";
        internal const string JPG_FILE_EXTENSION = ".jpg";
        internal const string PNG_FILE_EXTENSION = ".png";
        internal const string SVG_FILE_EXTENSION = ".svg";
        internal const string WEBP_FILE_EXTENSION = ".webp";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        internal static readonly string[] IMAGE_FILE_EXTENSION_LIST = GetImageFileExtensionList();
        public static readonly Size EMPTY_SIZE = Size.Empty;
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

        public static bool IsImageFile(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var ex = FileUtil.GetExtensionFastStack(filePath);
            return IMAGE_FILE_EXTENSION_LIST.Any(_ => StringUtil.CompareFilePath(_, ex));
        }

        public static string[] GetImageFilesArray(string directoryPath)
        {
            using (TimeMeasuring.Run(false, "GetImageFilesArray"))
            {
                ArgumentNullException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

                try
                {
                    var root = new DirectoryInfo(directoryPath);

                    return root
                        .Children()
                        .OfType<FileInfo>()
                        .Select(fi => fi.FullName)
                        .Where(file => FileUtil.CanAccess(file) && IsImageFile(file))
                        .ToArray();
                }
                catch (ArgumentNullException)
                {
                    return [];
                }
                catch (SecurityException)
                {
                    return [];
                }
                catch (ArgumentException)
                {
                    return [];
                }
                catch (PathTooLongException)
                {
                    return [];
                }
            }
        }

        public static string GetFirstImageFilePath(string directoryPath)
        {
            using (TimeMeasuring.Run(false, "GetFirstImageFilePath"))
            {
                ArgumentNullException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

                try
                {
                    var root = new DirectoryInfo(directoryPath);

                    var imageFile = root
                        .Children()
                        .OfType<FileInfo>()
                        .Select(fi => fi.FullName)
                        .OrderBy(file => file, NaturalStringComparer.Windows)
                        .FirstOrDefault(file =>
                        {
                            return FileUtil.CanAccess(file) && IsImageFile(file);
                        });

                    if (string.IsNullOrEmpty(imageFile))
                    {
                        return string.Empty;
                    }

                    return imageFile;
                }
                catch (ArgumentNullException)
                {
                    return string.Empty;
                }
                catch (SecurityException)
                {
                    return string.Empty;
                }
                catch (ArgumentException)
                {
                    return string.Empty;
                }
                catch (PathTooLongException)
                {
                    return string.Empty;
                }
            }
        }

        internal static Bitmap Resize(Bitmap srcBmp, int width, int height)
        {
            ArgumentNullException.ThrowIfNull(srcBmp, nameof(srcBmp));

            return OpenCVUtil.Resize(srcBmp, width, height);
        }

        internal static Size GetImageSize(string filePath)
        {
            using (TimeMeasuring.Run(false, "GetImageSize"))
            {
                ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

                try
                {
                    return GetImageSizeWithVarious(filePath);
                }
                catch (ImageUtilException ex)
                {
                    Logger.Error(ex);

                    using (var bmp = ReadImageFile(filePath))
                    {
                        return bmp.Size;
                    }
                }
            }
        }

        private static Size GetImageSizeWithVarious(string filePath)
        {
            using (TimeMeasuring.Run(false, "GetImageSizeWithVarious"))
            {
                ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

                try
                {
                    if (IsIconFile(filePath))
                    {
                        return GetImageSizeWithShell(filePath);
                    }
                    else if (IsSvgFile(filePath))
                    {
                        var size = GetSvgSize(filePath);
                        if (size == EMPTY_SIZE)
                        {
                            throw new ImageUtilException(CreateFileAccessErrorMessage(filePath));
                        }

                        return size;
                    }

                    using (var fs = new FileStream(filePath,
                         FileMode.Open, FileAccess.Read, FileShare.Read, 64, FileOptions.SequentialScan))
                    {
                        var formatName = SixLaborsUtil.DetectFormat(fs);

                        if (IsAvifFile(formatName))
                        {
                            return LibHeifSharpUtil.GetImageSize(fs);
                        }
                        else if (IsBmpFile(formatName))
                        {
                            var size = GetBmpSize(fs);
                            if (size == EMPTY_SIZE)
                            {
                                throw new ImageUtilException(CreateFileAccessErrorMessage(filePath));
                            }

                            return size;
                        }
                        else if (IsGifFile(formatName))
                        {
                            return SixLaborsUtil.GetImageSize(fs);
                        }
                        else if (IsHeicFile(formatName))
                        {
                            return LibHeifSharpUtil.GetImageSize(fs);
                        }
                        else if (IsHeifFile(formatName))
                        {
                            return LibHeifSharpUtil.GetImageSize(fs);
                        }
                        else if (IsJpegFile(formatName))
                        {
                            return SixLaborsUtil.GetImageSize(fs);
                        }
                        else if (IsPngFile(formatName))
                        {
                            var size = GetPngSize(fs);
                            if (size == EMPTY_SIZE)
                            {
                                throw new ImageUtilException(CreateFileAccessErrorMessage(filePath));
                            }

                            return size;
                        }
                        else if (IsWebpFile(formatName))
                        {
                            return SixLaborsUtil.GetImageSize(fs);
                        }
                        else
                        {
                            throw new ImageUtilException(
                                $"未対応の画像ファイルが指定されました。'{filePath}'");
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
                catch (COMException ex)
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
            }
        }

        private static Size GetImageSizeWithShell(string filePath)
        {
            using (TimeMeasuring.Run(false, "GetImageSizeWithShell"))
            {
                ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

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
                finally
                {
                    if (shell != null)
                    {
                        Marshal.ReleaseComObject(shell);
                        shell = null;
                    }
                }
            }
        }

        public static Bitmap ReadImageFile(string filePath)
        {
            using (TimeMeasuring.Run(false, "ReadImageFile"))
            {
                ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

                try
                {
                    return ReadImageFileWithVarious(filePath);
                }
                catch (ImageUtilException ex)
                {
                    Logger.Error(ex);

                    return ReadImageFileFromImageMagick(filePath);
                }
            }
        }

        private static Bitmap ReadImageFileWithVarious(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            try
            {
                using (var fs = new FileStream(filePath,
                    FileMode.Open, FileAccess.Read, FileShare.Read, FILE_READ_BUFFER_SIZE, FileOptions.SequentialScan))
                {
                    if (IsIconFile(filePath))
                    {
                        using (TimeMeasuring.Run(false, "ReadImageFileFromFileStream: Icon"))
                        using (var icon = new Icon(fs))
                        {
                            return ConvertIfGrayscale(icon.ToBitmap(), fs);
                        }
                    }
                    else if (IsSvgFile(filePath))
                    {
                        using (TimeMeasuring.Run(false, "ReadImageFileFromFileStream: Svg"))
                        {
                            return MagickUtil.ReadImageFile(fs, ImageMagick.MagickFormat.Svg);
                        }
                    }

                    var formatName = SixLaborsUtil.DetectFormat(fs);
                    if (IsAvifFile(formatName))
                    {
                        using (TimeMeasuring.Run(false, "ReadImageFileFromFileStream: Avif"))
                        {
                            return ConvertIfGrayscale(SixLaborsUtil.ReadImageFile(fs), fs);
                        }
                    }
                    else if (IsBmpFile(formatName))
                    {
                        using (TimeMeasuring.Run(false, "ReadImageFileFromFileStream: Bmp"))
                        {
                            return ConvertIfGrayscale((Bitmap)Bitmap.FromStream(fs, false, true), fs);
                        }
                    }
                    else if (IsGifFile(formatName))
                    {
                        using (TimeMeasuring.Run(false, "ReadImageFileFromFileStream: Gif"))
                        {
                            return ConvertIfGrayscale((Bitmap)Bitmap.FromStream(fs, false, true), fs);
                        }
                    }
                    else if (IsHeicFile(formatName))
                    {
                        using (TimeMeasuring.Run(false, "ReadImageFileFromFileStream: Heic"))
                        {
                            return ConvertIfGrayscale(MagickUtil.ReadImageFile(fs, ImageMagick.MagickFormat.Heic), fs);
                        }
                    }
                    else if (IsHeifFile(formatName))
                    {
                        using (TimeMeasuring.Run(false, "ReadImageFileFromFileStream: Heif"))
                        {
                            return ConvertIfGrayscale(MagickUtil.ReadImageFile(fs, ImageMagick.MagickFormat.Heif), fs);
                        }
                    }
                    else if (IsJpegFile(formatName))
                    {
                        using (TimeMeasuring.Run(false, "ReadImageFileFromFileStream: Jpeg"))
                        {
                            var bmp = ConvertIfGrayscale(OpenCVUtil.ReadImageFile(fs), fs);
                            return LoadBitmapCorrectOrientation(bmp);
                        }
                    }
                    else if (IsPngFile(formatName))
                    {
                        using (TimeMeasuring.Run(false, "ReadImageFileFromFileStream: Png"))
                        {
                            return ConvertIfGrayscale((Bitmap)Bitmap.FromStream(fs, false, true), fs);
                        }
                    }
                    else if (IsWebpFile(formatName))
                    {
                        using (TimeMeasuring.Run(false, "ReadImageFileFromFileStream: Webp"))
                        {
                            return ConvertIfGrayscale(OpenCVUtil.ReadImageFile(fs), fs);
                        }
                    }
                    else
                    {
                        throw new ImageUtilException(
                            $"未対応の画像ファイルが指定されました。'{filePath}'");
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

        private static Bitmap ReadImageFileFromImageMagick(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            try
            {
                var format = MagickUtil.DetectFormat(filePath);
                switch (format)
                {
                    case ImageMagick.MagickFormat.Avif:
                        using (TimeMeasuring.Run(false, "ReadImageFileFromFilePath: Avif"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Bmp:
                        using (TimeMeasuring.Run(false, "ReadImageFileFromFilePath: Bmp"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Gif:
                        using (TimeMeasuring.Run(false, "ReadImageFileFromFilePath: Gif"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Heic:
                        using (TimeMeasuring.Run(false, "ReadImageFileFromFilePath: Heic"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Heif:
                        using (TimeMeasuring.Run(false, "ReadImageFileFromFilePath: Heif"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Icon:
                        using (TimeMeasuring.Run(false, "ReadImageFileFromFilePath: Icon"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Jpeg:
                        using (TimeMeasuring.Run(false, "ReadImageFileFromFilePath: Jpeg"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Png:
                        using (TimeMeasuring.Run(false, "ReadImageFileFromFilePath: Png"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Svg:
                        using (TimeMeasuring.Run(false, "ReadImageFileFromFilePath: Svg"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.WebP:
                        using (TimeMeasuring.Run(false, "ReadImageFileFromFilePath: WebP"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    default:
                        throw new ImageUtilException(
                            $"未対応の画像ファイルが指定されました。'{filePath}'");
                }
            }
            catch (ImageMagick.MagickException ex)
            {
                throw new ImageUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (ArgumentException ex)
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
            ArgumentNullException.ThrowIfNull(fs, nameof(fs));

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

        private static Size GetSvgSize(string filePath)
        {
            ArgumentNullException.ThrowIfNull(filePath, nameof(filePath));

            var svgDoc = XDocument.Load(filePath);
            var svgElement = svgDoc.Root;
            if (svgElement != null && svgElement.Name.LocalName == "svg")
            {
                var widthStr = svgElement.Attribute("width")?.Value;
                if (string.IsNullOrEmpty(widthStr))
                {
                    return EMPTY_SIZE;
                }

                var width = StringUtil.ToInt(StringUtil.ExtractNumbers(widthStr));
                if (!width.HasValue)
                {
                    return EMPTY_SIZE;
                }

                var heightStr = svgElement.Attribute("height")?.Value;
                if (string.IsNullOrEmpty(heightStr))
                {
                    return EMPTY_SIZE;
                }

                var height = StringUtil.ToInt(StringUtil.ExtractNumbers(heightStr));
                if (!height.HasValue)
                {
                    return EMPTY_SIZE;
                }

                return new Size(width.Value, height.Value);
            }

            return EMPTY_SIZE;
        }

        private static Size GetPngSize(Stream fs)
        {
            ArgumentNullException.ThrowIfNull(fs, nameof(fs));

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
            ArgumentNullException.ThrowIfNull(a, nameof(a));
            ArgumentNullException.ThrowIfNull(b, nameof(b));

            if (a.Length != b.Length)
            {
                return false;
            }

            for (var i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static int ReadBigEndianInt32(BinaryReader reader)
        {
            ArgumentNullException.ThrowIfNull(reader, nameof(reader));

            var bytes = reader.ReadBytes(4);
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        private static Size GetBmpSize(Stream fs)
        {
            ArgumentNullException.ThrowIfNull(fs, nameof(fs));

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
            ArgumentNullException.ThrowIfNull(bmp, nameof(bmp));
            ArgumentNullException.ThrowIfNull(fs, nameof(fs));

            if (bmp.PixelFormat == PixelFormat.Format4bppIndexed
                || bmp.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                using (TimeMeasuring.Run(true, "ConvertIfGrayscale"))
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

        private static bool IsAvifFile(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var ex = FileUtil.GetExtensionFastStack(filePath);
            return StringUtil.CompareFilePath(ex, AVIF_FILE_EXTENSION);
        }

        private static bool IsBmpFile(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var ex = FileUtil.GetExtensionFastStack(filePath);
            return StringUtil.CompareFilePath(ex, BMP_FILE_EXTENSION);
        }

        private static bool IsGifFile(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var ex = FileUtil.GetExtensionFastStack(filePath);
            return StringUtil.CompareFilePath(ex, GIF_FILE_EXTENSION);
        }

        private static bool IsHeicFile(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var ex = FileUtil.GetExtensionFastStack(filePath);
            return StringUtil.CompareFilePath(ex, HEIC_FILE_EXTENSION);
        }

        private static bool IsHeifFile(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var ex = FileUtil.GetExtensionFastStack(filePath);
            return StringUtil.CompareFilePath(ex, HEIF_FILE_EXTENSION);
        }

        private static bool IsIconFile(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var ex = FileUtil.GetExtensionFastStack(filePath);
            return StringUtil.CompareFilePath(ex, ICON_FILE_EXTENSION);
        }

        private static bool IsJpegFile(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var ex = FileUtil.GetExtensionFastStack(filePath);
            return StringUtil.CompareFilePath(ex, JPG_FILE_EXTENSION)
                || StringUtil.CompareFilePath(ex, JPEG_FILE_EXTENSION);
        }

        private static bool IsPngFile(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var ex = FileUtil.GetExtensionFastStack(filePath);
            return StringUtil.CompareFilePath(ex, PNG_FILE_EXTENSION);
        }

        private static bool IsSvgFile(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var ex = FileUtil.GetExtensionFastStack(filePath);
            return StringUtil.CompareFilePath(ex, SVG_FILE_EXTENSION);
        }

        private static bool IsWebpFile(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var ex = FileUtil.GetExtensionFastStack(filePath);
            return StringUtil.CompareFilePath(ex, WEBP_FILE_EXTENSION);
        }

        private static string[] GetImageFileExtensionList()
        {
            return
            [
                AVIF_FILE_EXTENSION,
                BMP_FILE_EXTENSION,
                GIF_FILE_EXTENSION,
                ICON_FILE_EXTENSION,
                JPEG_FILE_EXTENSION,
                JPG_FILE_EXTENSION,
                HEIC_FILE_EXTENSION,
                HEIF_FILE_EXTENSION,
                PNG_FILE_EXTENSION,
                SVG_FILE_EXTENSION,
                WEBP_FILE_EXTENSION
            ];
        }

        private static string CreateFileAccessErrorMessage(string path)
        {
            return $"'{path}'へのアクセスに失敗しました。";
        }
    }
}
