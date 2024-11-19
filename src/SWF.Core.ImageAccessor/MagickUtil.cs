using ImageMagick;
using System.Drawing.Imaging;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal static class MagickUtil
    {
        private static readonly bool USE_AVX_X64 = Avx.X64.IsSupported;
        private static readonly bool USE_SSE2 = Sse2.IsSupported;
        private static readonly Vector256<byte> SHUFFLE_AVX2 = Vector256.Create(
            2, 1, 0, 3, 6, 5, 4, 7, 10, 9, 8, 11, 14, 13, 12, 15,
            18, 17, 16, 19, 22, 21, 20, 23, 26, 25, 24, 27, 30, 29, 28, 31
        ).AsByte();
        private static readonly Vector128<byte> SHUFFLE_SSE2 = Vector128.Create(
            2, 1, 0, 3, 6, 5, 4, 7, 10, 9, 8, 11, 14, 13, 12, 15
        ).AsByte();

        public static Bitmap ReadImageFile(Stream stream)
        {
            using var image = new MagickImage(stream);
            // 画質設定を一括で行う
            ConfigureImage(image);

            using var pixels = image.GetPixelsUnsafe();
            var width = (int)image.Width;
            var height = (int)image.Height;
            var pixelsBytes = pixels.ToByteArray(PixelMapping.RGBA);

            if (pixelsBytes == null)
            {
                throw new InvalidOperationException("Failed to get pixel data");
            }            

            return CreateBitmapFromPixels(pixelsBytes, width, height);
        }

        private static void ConfigureImage(MagickImage image)
        {
            image.Quality = 90;
            image.Depth = 8;
            image.Format = MagickFormat.Rgba;
        }

        private static Bitmap CreateBitmapFromPixels(byte[] pixelsBytes, int width, int height)
        {
            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var rect = new Rectangle(0, 0, width, height);
            var bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            try
            {
                unsafe
                {
                    fixed (byte* src = pixelsBytes)
                    {
                        var dst = (byte*)bitmapData.Scan0;
                        ConvertPixelFormat(src, dst, pixelsBytes.Length);
                    }
                }
                return bitmap;
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        private static unsafe void ConvertPixelFormat(byte* src, byte* dst, int length)
        {
            if (USE_AVX_X64 && length >= 32)
            {
                ConvertRgbaToBgraAvx2(src, dst, length);
            }
            else if (USE_SSE2 && length >= 16)
            {
                ConvertRgbaToBgraSse2(src, dst, length);
            }
            else
            {
                ConvertRgbaToBgraScalar(src, dst, length);
            }
        }

        private static unsafe void ConvertRgbaToBgraAvx2(byte* src, byte* dst, int length)
        {
            int i = 0;
            // Process 32 bytes (8 pixels) at a time
            for (; i <= length - 32; i += 32)
            {
                var rgba = Avx.LoadVector256(src + i);
                var bgra = Avx2.Shuffle(rgba, SHUFFLE_AVX2);
                Avx.Store(dst + i, bgra);
            }

            // Handle remaining bytes with SSE2 if possible
            if (USE_SSE2)
            {
                for (; i <= length - 16; i += 16)
                {
                    var rgba = Sse2.LoadVector128(src + i);
                    var bgra = Ssse3.Shuffle(rgba, SHUFFLE_SSE2);
                    Sse2.Store(dst + i, bgra);
                }
            }

            // Handle remaining pixels
            ConvertRgbaToBgraScalar(src + i, dst + i, length - i);
        }

        private static unsafe void ConvertRgbaToBgraSse2(byte* src, byte* dst, int length)
        {
            int i = 0;
            // Process 16 bytes (4 pixels) at a time
            for (; i <= length - 16; i += 16)
            {
                var rgba = Sse2.LoadVector128(src + i);
                var bgra = Ssse3.Shuffle(rgba, SHUFFLE_SSE2);
                Sse2.Store(dst + i, bgra);
            }

            // Handle remaining pixels
            ConvertRgbaToBgraScalar(src + i, dst + i, length - i);
        }

        private static unsafe void ConvertRgbaToBgraScalar(byte* src, byte* dst, int length)
        {
            // Unrolled loop for better performance
            int i = 0;
            for (; i <= length - 16; i += 16)
            {
                // Process 4 pixels at once
                dst[i + 0] = src[i + 2];
                dst[i + 1] = src[i + 1];
                dst[i + 2] = src[i + 0];
                dst[i + 3] = src[i + 3];

                dst[i + 4] = src[i + 6];
                dst[i + 5] = src[i + 5];
                dst[i + 6] = src[i + 4];
                dst[i + 7] = src[i + 7];

                dst[i + 8] = src[i + 10];
                dst[i + 9] = src[i + 9];
                dst[i + 10] = src[i + 8];
                dst[i + 11] = src[i + 11];

                dst[i + 12] = src[i + 14];
                dst[i + 13] = src[i + 13];
                dst[i + 14] = src[i + 12];
                dst[i + 15] = src[i + 15];
            }

            // Handle remaining pixels
            for (; i < length; i += 4)
            {
                dst[i + 0] = src[i + 2];
                dst[i + 1] = src[i + 1];
                dst[i + 2] = src[i + 0];
                dst[i + 3] = src[i + 3];
            }
        }
    }
}
