using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.StringAccessor;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security;
using System.Xml;
using ZLinq;

namespace SWF.Core.ImageAccessor
{

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

        internal static readonly string[] IMAGE_FILE_EXTENSION_LIST = GetImageFileExtensionList();
        private static readonly string[] RETAIN_EXIF_IMAGE_FORMAT = GetRetainExifImageFormat();
        public static readonly Size EMPTY_SIZE = Size.Empty;
        public static readonly Bitmap EMPTY_IMAGE = new(1, 1);

        private const int FILE_READ_BUFFER_SIZE = 16 * 1024;

        private static IShellApplication GetSell()
        {
            var type = Type.GetTypeFromProgID("Shell.Application")
                ?? throw new InvalidOperationException("Shell.Applicationを取得できませんでした。");

            var obj = Activator.CreateInstance(type)
                ?? throw new InvalidOperationException("Shell.Applicationを取得できませんでした。");

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
            using (TimeMeasuring.Run(false, "ImageUtil.GetImageFilesArray"))
            {
                ArgumentNullException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

                try
                {
                    if (!FileUtil.CanAccess(directoryPath))
                    {
                        return [];
                    }

                    var root = new DirectoryInfo(directoryPath);

                    return root
                        .Children()
                        .OfType<FileInfo>()
                        .Where(static file => FileUtil.CanAccess(file.FullName) && IsImageFile(file.FullName))
                        .Select(static file => file.FullName)
                        .ToArray();
                }
                catch (Exception ex) when (
                    ex is ArgumentNullException ||
                    ex is SecurityException ||
                    ex is ArgumentException ||
                    ex is PathTooLongException)
                {
                    return [];
                }
            }
        }

        public static DateTime GetTakenDate(string filePath)
        {
            ArgumentNullException.ThrowIfNull(filePath, nameof(filePath));

            try
            {
                return MetadataExtractorUtil.GetTakenDate(filePath);
            }
            catch (Exception ex) when (
                ex is MetadataExtractor.ImageProcessingException ||
                ex is IOException)
            {
                throw new ImageUtilException($"撮影日を取得できませんでした。", filePath, ex);
            }
        }

        public static string GetFirstImageFilePath(string directoryPath)
        {
            using (TimeMeasuring.Run(false, "ImageUtil.GetFirstImageFilePath"))
            {
                ArgumentNullException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

                try
                {
                    if (!FileUtil.CanAccess(directoryPath))
                    {
                        return string.Empty;
                    }

                    var root = new DirectoryInfo(directoryPath);

                    var imageFile = root
                        .Children()
                        .OfType<FileInfo>()
                        .OrderBy(static file => file.FullName, NaturalStringComparer.WINDOWS)
                        .FirstOrDefault(static file =>
                        {
                            return FileUtil.CanAccess(file.FullName) && IsImageFile(file.FullName);
                        });

                    if (imageFile is null)
                    {
                        return string.Empty;
                    }

                    return imageFile.FullName;
                }
                catch (Exception ex) when (
                    ex is ArgumentNullException ||
                    ex is SecurityException ||
                    ex is ArgumentException ||
                    ex is PathTooLongException)
                {
                    return string.Empty;
                }
            }
        }

        internal static OpenCvSharp.Mat Resize(Bitmap srcBmp, float width, float height)
        {
            ArgumentNullException.ThrowIfNull(srcBmp, nameof(srcBmp));

            return OpenCVUtil.Resize(srcBmp, width, height);
        }

        internal static async ValueTask<Size> GetImageSize(string filePath)
        {
            using (TimeMeasuring.Run(false, "ImageUtil.GetImageSize"))
            {
                ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

                try
                {
                    return await GetImageSizeWithVarious(filePath).WithConfig();
                }
                catch (ImageUtilException ex)
                {
                    Log.GetLogger().Error(ex);

                    using (var bmp = await ReadImageFile(filePath).WithConfig())
                    {
                        return bmp.Size;
                    }
                }
            }
        }

        private static async ValueTask<Size> GetImageSizeWithVarious(string filePath)
        {
            using (TimeMeasuring.Run(false, "ImageUtil.GetImageSizeWithVarious"))
            {
                ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

                try
                {
                    if (IsIconFile(filePath))
                    {
                        return GetImageSizeWithShell(filePath);
                    }

                    var data = await File.ReadAllBytesAsync(filePath).WithConfig();
                    using var ms = new MemoryStream(data);

                    if (IsSvgFile(filePath))
                    {
                        return SvgUtil.GetImageSize(ms);
                    }

                    var formatName = await SixLaborsUtil.DetectFormat(ms);

                    if (IsAvifFile(formatName))
                    {
                        return LibHeifSharpUtil.GetImageSize(ms);
                    }
                    else if (IsBmpFile(formatName))
                    {
                        var size = BitmapUtil.GetImageSize(ms);
                        if (size == EMPTY_SIZE)
                        {
                            throw new ImageUtilException($"画像サイズを取得できませんでした。", filePath);
                        }

                        return size;
                    }
                    else if (IsGifFile(formatName))
                    {
                        return await SixLaborsUtil.GetImageSize(ms).WithConfig();
                    }
                    else if (IsHeicFile(formatName))
                    {
                        return LibHeifSharpUtil.GetImageSize(ms);
                    }
                    else if (IsHeifFile(formatName))
                    {
                        return LibHeifSharpUtil.GetImageSize(ms);
                    }
                    else if (IsJpegFile(formatName))
                    {
                        return JpegUtil.GetImageSize(ms);
                    }
                    else if (IsPngFile(formatName))
                    {
                        var size = PngUtil.GetImageSize(ms);
                        if (size == EMPTY_SIZE)
                        {
                            throw new ImageUtilException($"画像サイズを取得できませんでした。", filePath);
                        }

                        return size;
                    }
                    else if (IsWebpFile(formatName))
                    {
                        return await SixLaborsUtil.GetImageSize(ms).WithConfig();
                    }
                    else
                    {
                        throw new ImageUtilException(
                            $"未対応の画像ファイルが指定されました。", filePath);
                    }
                }
                catch (Exception ex) when (
                    ex is ArgumentNullException ||
                    ex is ArgumentException ||
                    ex is NotSupportedException ||
                    ex is FileNotFoundException ||
                    ex is SecurityException ||
                    ex is DirectoryNotFoundException ||
                    ex is PathTooLongException ||
                    ex is EndOfStreamException ||
                    ex is ObjectDisposedException ||
                    ex is IOException ||
                    ex is UnauthorizedAccessException ||
                    ex is COMException ||
                    ex is XmlException ||
                    ex is InvalidDataException ||
                    ex is SixLabors.ImageSharp.InvalidImageContentException ||
                    ex is SixLabors.ImageSharp.UnknownImageFormatException ||
                    ex is LibHeifSharp.HeifException)
                {
                    throw new ImageUtilException($"画像サイズを取得できませんでした。", filePath, ex);
                }
            }
        }

        private static Size GetImageSizeWithShell(string filePath)
        {
            using (TimeMeasuring.Run(false, "ImageUtil.GetImageSizeWithShell"))
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
                        throw new ImageUtilException($"画像サイズを取得できませんでした。", filePath);
                    }

                    var v = details.Split(('x'));
                    if (v.Length != 2)
                    {
                        throw new ImageUtilException($"画像サイズを取得できませんでした。", filePath);
                    }

                    var wText = v[0];
                    var hText = v[1];

                    if (!int.TryParse(wText[1..].Trim(), out int w))
                    {
                        throw new ImageUtilException($"画像サイズを取得できませんでした。", filePath);
                    }

                    if (!int.TryParse(hText[..^1].Trim(), out int h))
                    {
                        throw new ImageUtilException($"画像サイズを取得できませんでした。", filePath);
                    }

                    if (w < 1 || h < 1)
                    {
                        throw new ImageUtilException($"画像サイズを取得できませんでした。", filePath);
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

        public static bool CanRetainExifImageFormat(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var ex = FileUtil.GetExtensionFastStack(filePath);
            return RETAIN_EXIF_IMAGE_FORMAT.Any(_ => StringUtil.CompareFilePath(_, ex));
        }

        public static async ValueTask<Bitmap> ReadImageFile(string filePath)
        {
            using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFile"))
            {
                ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

                try
                {
                    return await ReadImageFileWithVarious(filePath).WithConfig();
                }
                catch (ImageUtilException ex)
                {
                    Log.GetLogger().Error(ex);

                    return ReadImageFileWithImageMagick(filePath);
                }
            }
        }

        private static async ValueTask<Bitmap> ReadImageFileWithVarious(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            try
            {
                var data = await File.ReadAllBytesAsync(filePath).WithConfig();
                using var ms = new MemoryStream(data);

                if (IsIconFile(filePath))
                {
                    using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileWithVarious: Icon"))
                    using (var icon = new Icon(ms))
                    {
                        return ConvertIfGrayscale(icon.ToBitmap(), ms);
                    }
                }
                else if (IsSvgFile(filePath))
                {
                    using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileWithVarious: Svg"))
                    {
                        return SvgUtil.ReadImageFile(ms);
                    }
                }

                var formatName = await SixLaborsUtil.DetectFormat(ms);
                if (IsAvifFile(formatName))
                {
                    using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileWithVarious: Avif"))
                    {
                        return await SixLaborsUtil.ReadImageFile(ms);
                    }
                }
                else if (IsBmpFile(formatName))
                {
                    using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileWithVarious: Bmp"))
                    {
                        return ConvertIfGrayscale((Bitmap)Bitmap.FromStream(ms, false, true), ms);
                    }
                }
                else if (IsGifFile(formatName))
                {
                    using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileWithVarious: Gif"))
                    {
                        return ConvertIfGrayscale((Bitmap)Bitmap.FromStream(ms, false, true), ms);
                    }
                }
                else if (IsHeicFile(formatName))
                {
                    using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileWithVarious: Heic"))
                    {
                        return ConvertIfGrayscale(MagickUtil.ReadImageFile(ms, ImageMagick.MagickFormat.Heic), ms);
                    }
                }
                else if (IsHeifFile(formatName))
                {
                    using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileWithVarious: Heif"))
                    {
                        return ConvertIfGrayscale(MagickUtil.ReadImageFile(ms, ImageMagick.MagickFormat.Heif), ms);
                    }
                }
                else if (IsJpegFile(formatName))
                {
                    using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileWithVarious: Jpeg"))
                    {
                        return ConvertIfGrayscale(JpegUtil.ReadImageFile(ms), ms);
                    }
                }
                else if (IsPngFile(formatName))
                {
                    using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileWithVarious: Png"))
                    {
                        return ConvertIfGrayscale((Bitmap)Bitmap.FromStream(ms, false, true), ms);
                    }
                }
                else if (IsWebpFile(formatName))
                {
                    using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileWithVarious: Webp"))
                    {
                        return ConvertIfGrayscale(OpenCVUtil.ReadImageFile(ms), ms);
                    }
                }
                else
                {
                    throw new ImageUtilException(
                        $"未対応の画像ファイルが指定されました。", filePath);
                }
            }
            catch (Exception ex) when (
                ex is ArgumentException ||
                ex is NotSupportedException ||
                ex is FileNotFoundException ||
                ex is SecurityException ||
                ex is DirectoryNotFoundException ||
                ex is PathTooLongException ||
                ex is IOException ||
                ex is UnauthorizedAccessException ||
                ex is XmlException ||
                ex is SixLabors.ImageSharp.InvalidImageContentException ||
                ex is SixLabors.ImageSharp.UnknownImageFormatException ||
                ex is LibHeifSharp.HeifException ||
                ex is ImageMagick.MagickException ||
                ex is OutOfMemoryException ||
                ex is InvalidOperationException)
            {
                throw new ImageUtilException($"画像ファイルを読み込めませんでした。", filePath, ex);
            }
        }

        private static Bitmap ReadImageFileWithImageMagick(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            try
            {
                var format = MagickUtil.DetectFormat(filePath);
                switch (format)
                {
                    case ImageMagick.MagickFormat.Avif:
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileWithImageMagick: Avif"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Bmp:
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileWithImageMagick: Bmp"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Gif:
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileWithImageMagick: Gif"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Heic:
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileWithImageMagick: Heic"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Heif:
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileWithImageMagick: Heif"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Icon:
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileWithImageMagick: Icon"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Jpeg:
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileWithImageMagick: Jpeg"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Png:
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileWithImageMagick: Png"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.Svg:
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileWithImageMagick: Svg"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    case ImageMagick.MagickFormat.WebP:
                        using (TimeMeasuring.Run(false, "ImageUtil.ReadImageFileWithImageMagick: WebP"))
                        {
                            return MagickUtil.ReadImageFile(filePath);
                        }
                    default:
                        throw new ImageUtilException(
                            $"未対応の画像ファイルが指定されました。", filePath);
                }
            }
            catch (Exception ex) when (
                ex is ImageMagick.MagickException ||
                ex is ArgumentException)
            {
                throw new ImageUtilException($"画像ファイルを読み込めませんでした。", filePath, ex);
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

        private static Bitmap ConvertIfGrayscale(Bitmap bmp, Stream fs)
        {
            ArgumentNullException.ThrowIfNull(bmp, nameof(bmp));
            ArgumentNullException.ThrowIfNull(fs, nameof(fs));

            if (bmp.PixelFormat == PixelFormat.Format4bppIndexed
                || bmp.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                using (TimeMeasuring.Run(false, "ImageUtil.ConvertIfGrayscale"))
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

        private static string[] GetRetainExifImageFormat()
        {
            return
            [
                AVIF_FILE_EXTENSION,
                JPEG_FILE_EXTENSION,
                JPG_FILE_EXTENSION,
                HEIC_FILE_EXTENSION,
                HEIF_FILE_EXTENSION,
                WEBP_FILE_EXTENSION
            ];
        }
    }
}
