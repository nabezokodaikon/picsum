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
        private static readonly bool USE_AVX2 = Avx.X64.IsSupported;

        private static readonly Vector256<byte> SHUFFLE = Vector256.Create(
            2, 1, 0, 3,  // First pixel
            6, 5, 4, 7,  // Second pixel
            10, 9, 8, 11,  // Third pixel
            14, 13, 12, 15,  // Fourth pixel
            18, 17, 16, 19,  // Fifth pixel
            22, 21, 20, 23,  // Sixth pixel
            26, 25, 24, 27,  // Seventh pixel
            30, 29, 28, 31   // Eighth pixel
        ).AsByte();

        public static Bitmap ReadImageFile(Stream stream)
        {
            using (var image = new MagickImage(stream))
            {
                // 画質設定
                image.Quality = 90;
                image.Depth = 8;  // 8-bit/channel
                image.Format = MagickFormat.Rgba;

                using (var pixels = image.GetPixels())
                {
                    var width = (int)image.Width;
                    var height = (int)image.Height;

                    // メモリ最適化のためにピクセルデータを直接取得
                    var pixelsBytes = pixels.ToByteArray(PixelMapping.RGBA);
                    if (pixelsBytes == null)
                    {
                        throw new InvalidOperationException();
                    }

                    var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                    var rect = new Rectangle(0, 0, width, height);
                    var bitmapData = bitmap.LockBits(
                        rect,
                        ImageLockMode.WriteOnly,
                        PixelFormat.Format32bppArgb);

                    try
                    {
                        unsafe
                        {
                            fixed (byte* src = pixelsBytes)
                            {
                                var dst = (byte*)bitmapData.Scan0;
                                var stride = bitmapData.Stride;

                                // SIMD操作が可能な場合は利用
                                if (USE_AVX2 && width >= 8)
                                {
                                    // AVX2を使用した高速変換
                                    ConvertRgbaToBgraAvx2(src, dst, pixelsBytes.Length);
                                }
                                else
                                {
                                    // 通常の変換
                                    for (var i = 0; i < pixelsBytes.Length; i += 4)
                                    {
                                        dst[i + 0] = src[i + 2]; // B
                                        dst[i + 1] = src[i + 1]; // G
                                        dst[i + 2] = src[i + 0]; // R
                                        dst[i + 3] = src[i + 3]; // A
                                    }
                                }
                            }
                        }

                        return bitmap;
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }
                }
            }

        }

        private static unsafe void ConvertRgbaToBgraAvx2(byte* src, byte* dst, int length)
        {
            // 32バイト（8ピクセル）ずつ処理
            var vectorSize = 32;
            var i = 0;
            for (; i <= length - vectorSize; i += vectorSize)
            {
                var rgba = Avx.LoadVector256(src + i);
                var bgra = Avx2.Shuffle(rgba, SHUFFLE);
                Avx.Store(dst + i, bgra);
            }

            // 残りのピクセルを処理
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
