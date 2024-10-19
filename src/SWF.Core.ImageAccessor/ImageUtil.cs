using SWF.Core.Base;
using SWF.Core.FileAccessor;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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

        private static readonly int FILE_READ_BUFFER_SIZE = 80 * 1024;
        private static readonly dynamic SHELL = GetSell();

        private static object GetSell()
        {
            var type = Type.GetTypeFromProgID("Shell.Application")
                ?? throw new NullReferenceException("Shell.Applicationを取得できませんでした。");

            var obj = Activator.CreateInstance(type)
                ?? throw new NullReferenceException("Shell.Applicationを取得できませんでした。");

            return obj;
        }

        internal static Bitmap Resize(Bitmap srcBmp, int width, int height)
        {
            ArgumentNullException.ThrowIfNull(srcBmp, nameof(srcBmp));

            return OpenCVUtil.Resize(srcBmp, width, height);
        }

        internal static byte[] BitmapToBuffer(Bitmap bitmap)
        {
            ArgumentNullException.ThrowIfNull(bitmap, nameof(bitmap));

            using (TimeMeasuring.Run(true, "ImageUtil.BitmapToBuffer"))
            {
                BitmapData? bmpData = null;

                try
                {
                    var bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                    var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                    bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);

                    var stride = bmpData.Stride;
                    var bufferSize = stride * bitmap.Height;
                    var pixelBuffer = new byte[bufferSize];

                    unsafe
                    {
                        var srcPtr = (byte*)bmpData.Scan0;
                        fixed (byte* destPtr = pixelBuffer)
                        {
                            Buffer.MemoryCopy(srcPtr, destPtr, bufferSize, bufferSize);
                        }
                    }

                    return pixelBuffer;
                }
                finally
                {
                    if (bmpData != null)
                    {
                        bitmap.UnlockBits(bmpData);
                        bmpData = null;
                    }
                }
            }
        }

        internal static Bitmap BufferToBitmap(byte[] rawBytes, int width, int height, PixelFormat pixelFormat)
        {
            ArgumentNullException.ThrowIfNull(rawBytes, nameof(rawBytes));

            using (TimeMeasuring.Run(true, "ImageUtil.BufferToBitmap"))
            {
                var bitmap = new Bitmap(width, height, pixelFormat);
                BitmapData? bmpData = null;

                try
                {
                    var rect = new Rectangle(0, 0, width, height);
                    bmpData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, pixelFormat);

                    var bytesPerPixel = Image.GetPixelFormatSize(pixelFormat) / 8;
                    var stride = bmpData.Stride;
                    var bufferSize = stride * height;

                    unsafe
                    {
                        fixed (byte* srcPtr = rawBytes)
                        {
                            var destPtr = (byte*)bmpData.Scan0;
                            Buffer.MemoryCopy(srcPtr, destPtr, bufferSize, rawBytes.Length);
                        }
                    }

                    return bitmap;
                }
                finally
                {
                    if (bmpData != null)
                    {
                        bitmap.UnlockBits(bmpData);
                        bmpData = null;
                    }
                }
            }
        }

        internal static byte[] BitmapToBufferFor8bpp(Bitmap bitmap)
        {
            ArgumentNullException.ThrowIfNull(bitmap, nameof(bitmap));

            if (bitmap.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                throw new ArgumentException("Pixel format must be 8bppIndexed.");
            }

            using (TimeMeasuring.Run(true, "ImageUtil.BitmapToBufferFor8bpp"))
            {
                BitmapData? bmpData = null;

                try
                {
                    var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                    bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
                    var stride = bmpData.Stride;
                    var bufferSize = stride * bitmap.Height;
                    var pixelBuffer = new byte[bufferSize];

                    unsafe
                    {
                        var srcPtr = (byte*)bmpData.Scan0;
                        fixed (byte* destPtr = pixelBuffer)
                        {
                            Buffer.MemoryCopy(srcPtr, destPtr, bufferSize, bufferSize);
                        }
                    }

                    return pixelBuffer;
                }
                finally
                {
                    if (bmpData != null)
                    {
                        bitmap.UnlockBits(bmpData);
                        bmpData = null;
                    }
                }
            }
        }

        internal static byte[] BitmapToBufferFor4bpp(Bitmap bitmap)
        {
            ArgumentNullException.ThrowIfNull(bitmap, nameof(bitmap));

            if (bitmap.PixelFormat != PixelFormat.Format4bppIndexed)
            {
                throw new ArgumentException("Pixel format must be 4bppIndexed.");
            }

            using (TimeMeasuring.Run(true, "ImageUtil.BitmapToBufferFor4bpp"))
            {
                BitmapData? bmpData = null;

                try
                {
                    var bitmapData = bitmap.LockBits(
                        new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        ImageLockMode.ReadOnly,
                        bitmap.PixelFormat);

                    var widthInBytes = (bitmap.Width + 1) / 2;
                    var stride = bitmapData.Stride;
                    var pixelData = new byte[bitmap.Height * widthInBytes];

                    unsafe
                    {
                        var ptr = (byte*)bitmapData.Scan0;

                        for (var y = 0; y < bitmap.Height; y++)
                        {
                            var row = ptr + y * stride;
                            for (var x = 0; x < widthInBytes; x++)
                            {
                                pixelData[y * widthInBytes + x] = row[x];
                            }
                        }
                    }

                    return pixelData;
                }
                finally
                {
                    if (bmpData != null)
                    {
                        bitmap.UnlockBits(bmpData);
                        bmpData = null;
                    }
                }
            }
        }

        internal static Bitmap BufferToBitmapFor8bpp(byte[] rawBytes, int width, int height)
        {
            ArgumentNullException.ThrowIfNull(rawBytes, nameof(rawBytes));

            using (TimeMeasuring.Run(true, "ImageUtil.BufferToBitmapFor8bpp"))
            {
                var bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                BitmapData? bmpData = null;

                try
                {
                    var palette = bitmap.Palette;
                    for (var i = 0; i < 256; i++)
                    {
                        palette.Entries[i] = Color.FromArgb(i, i, i);
                    }
                    bitmap.Palette = palette;

                    var rect = new Rectangle(0, 0, width, height);
                    bmpData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

                    var stride = bmpData.Stride;
                    var bufferSize = stride * height;

                    unsafe
                    {
                        fixed (byte* srcPtr = rawBytes)
                        {
                            Buffer.MemoryCopy(srcPtr, (byte*)bmpData.Scan0, bufferSize, rawBytes.Length);
                        }
                    }

                    return bitmap;
                }
                finally
                {
                    if (bmpData != null)
                    {
                        bitmap.UnlockBits(bmpData);
                        bmpData = null;
                    }
                }
            }
        }

        internal static Bitmap BufferToBitmapFor4bpp(byte[] rawBytes, int width, int height)
        {
            ArgumentNullException.ThrowIfNull(rawBytes, nameof(rawBytes));

            using (TimeMeasuring.Run(true, "ImageUtil.BufferToBitmapFor4bpp"))
            {
                var bitmap = new Bitmap(width, height, PixelFormat.Format4bppIndexed);
                BitmapData? bmpData = null;

                try
                {
                    var palette = bitmap.Palette;
                    for (var i = 0; i < 16; i++)
                    {
                        palette.Entries[i] = Color.FromArgb(i * 17, i * 17, i * 17);
                    }
                    bitmap.Palette = palette;

                    bmpData = bitmap.LockBits(
                        new Rectangle(0, 0, width, height),
                        ImageLockMode.WriteOnly,
                        bitmap.PixelFormat);

                    var stride = bmpData.Stride;

                    unsafe
                    {
                        var ptr = (byte*)bmpData.Scan0;
                        for (var y = 0; y < height; y++)
                        {
                            var row = ptr + y * stride;
                            for (var x = 0; x < width / 2; x++)
                            {
                                row[x] = rawBytes[y * (width / 2) + x];
                            }
                        }
                    }

                    return bitmap;
                }
                finally
                {
                    if (bmpData != null)
                    {
                        bitmap.UnlockBits(bmpData);
                        bmpData = null;
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
        internal static Size GetImageSize(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            using (TimeMeasuring.Run(true, "ImageUtil.GetImageSize"))
            {
                try
                {
                    using (var fs = new FileStream(filePath,
                        FileMode.Open, FileAccess.Read, FileShare.Read, 64, FileOptions.SequentialScan))
                    {
                        var formatName = SixLaborsUtil.DetectFormat(fs);

                        if (FileUtil.IsWebpFile(formatName))
                        {
                            //return SixLaborsUtil.GetImageSize(fs);
                        }
                        if (FileUtil.IsAvifFile(formatName))
                        {
                            return LibHeifSharpUtil.GetImageSize(fs);
                        }
                        else if (FileUtil.IsHeifFile(formatName))
                        {
                            return LibHeifSharpUtil.GetImageSize(fs);
                        }
                        else if (FileUtil.IsJpegFile(formatName))
                        {
                            var size = GetJpegSize(fs);
                            if (size != EMPTY_SIZE)
                            {
                                return size;
                            }
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
        }

        public static Bitmap ReadImageFile(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            try
            {
                if (FileUtil.IsSvgFile(filePath))
                {
                    using (TimeMeasuring.Run(true, "ImageUtil.ReadImageFile Svg"))
                    {
                        return SvgUtil.ReadImageFile(filePath);
                    }
                }

                var sw = Stopwatch.StartNew();
                using (var fs = new FileStream(filePath,
                    FileMode.Open, FileAccess.Read, FileShare.Read, FILE_READ_BUFFER_SIZE, FileOptions.SequentialScan))
                {
                    sw.Stop();
                    ConsoleUtil.Write($"ImageUtil.ReadImageFile new FileStream: {sw.ElapsedMilliseconds} ms");

                    if (FileUtil.IsIconFile(filePath))
                    {
                        using (TimeMeasuring.Run(true, "ImageUtil.ReadImageFile Icon"))
                        using (var icon = new Icon(fs))
                        {
                            return ConvertIfGrayscale(icon.ToBitmap(), fs);
                        }
                    }

                    var formatName = SixLaborsUtil.DetectFormat(fs);

                    if (FileUtil.IsWebpFile(formatName))
                    {
                        using (TimeMeasuring.Run(true, "ImageUtil.ReadImageFile Webp"))
                        {
                            return ConvertIfGrayscale(OpenCVUtil.ReadImageFile(fs), fs);
                        }
                    }
                    else if (FileUtil.IsAvifFile(formatName))
                    {
                        using (TimeMeasuring.Run(true, "ImageUtil.ReadImageFile Avif"))
                        {
                            return ConvertIfGrayscale(SixLaborsUtil.ReadImageFile(fs), fs);
                        }
                    }
                    else if (FileUtil.IsHeifFile(formatName))
                    {
                        using (TimeMeasuring.Run(true, "ImageUtil.ReadImageFile Heif"))
                        {
                            return ConvertIfGrayscale(SixLaborsUtil.ReadImageFile(fs), fs);
                        }
                    }
                    else if (FileUtil.IsJpegFile(formatName))
                    {
                        using (TimeMeasuring.Run(true, "ImageUtil.ReadImageFile Jpeg"))
                        {
                            var bmp = ConvertIfGrayscale((Bitmap)Bitmap.FromStream(fs, false, true), fs);
                            return LoadBitmapCorrectOrientation(bmp);
                        }
                    }
                    else if (FileUtil.IsBmpFile(formatName))
                    {
                        using (TimeMeasuring.Run(true, "ImageUtil.ReadImageFile Bitmap"))
                        {
                            return ConvertIfGrayscale((Bitmap)Bitmap.FromStream(fs, false, true), fs);
                        }
                    }
                    else if (FileUtil.IsPngFile(formatName))
                    {
                        using (TimeMeasuring.Run(true, "ImageUtil.ReadImageFile Png"))
                        {
                            return ConvertIfGrayscale((Bitmap)Bitmap.FromStream(fs, false, true), fs);
                        }
                    }
                    else if (FileUtil.IsImageFile(filePath))
                    {
                        using (TimeMeasuring.Run(true, "ImageUtil.ReadImageFile Other"))
                        {
                            return ConvertIfGrayscale((Bitmap)Bitmap.FromStream(fs, false, true), fs);
                        }
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

        public static Bitmap LoadBitmapCorrectOrientation(Bitmap bitmap)
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
