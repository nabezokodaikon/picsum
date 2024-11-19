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

        // 1MB画像に最適化されたパラメータ
        private const int OPTIMAL_CHUNK_SIZE = 131072; // 128KB (32K pixels) per chunk
        private const int MIN_PARALLEL_CHUNK_SIZE = 32768; // 32KB threshold for parallelization
        private static readonly int OPTIMAL_THREAD_COUNT = Math.Min(4, Environment.ProcessorCount);

        // L1/L2キャッシュに合わせたプリフェッチ距離
        private const int PREFETCH_DISTANCE = 128; // bytes

        public static Bitmap ReadImageFile(Stream stream)
        {
            using (var image = new MagickImage(stream))
            {
                ConfigureImage(image);

                using (var pixels = image.GetPixelsUnsafe())
                {
                    var width = (int)image.Width;
                    var height = (int)image.Height;
                    var pixelsBytes = pixels.ToByteArray(PixelMapping.RGBA);

                    if (pixelsBytes == null)
                    {
                        throw new InvalidOperationException("Failed to get pixel data");
                    }

                    return CreateBitmapFromPixels(pixelsBytes, width, height);
                }
            }
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
                        // 1MB未満の場合は並列処理を使用しない
                        if (pixelsBytes.Length < MIN_PARALLEL_CHUNK_SIZE * 2)
                        {
                            ConvertPixelFormat(src, dst, pixelsBytes.Length);
                        }
                        else
                        {
                            ConvertPixelFormatParallel(src, dst, pixelsBytes.Length);
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

        private static unsafe void ConvertPixelFormatParallel(byte* src, byte* dst, int totalLength)
        {
            // チャンク数の計算（最適なチャンクサイズを基に）
            var chunksCount = Math.Min(OPTIMAL_THREAD_COUNT,
                (totalLength + OPTIMAL_CHUNK_SIZE - 1) / OPTIMAL_CHUNK_SIZE);

            // 実際のチャンクサイズを計算（32バイトアライメント）
            var chunkSize = ((totalLength / chunksCount + 31) & ~31);

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = chunksCount
            };

            Parallel.For(0, chunksCount, options, i =>
            {
                var start = i * chunkSize;
                var length = (i == chunksCount - 1) ? totalLength - start : chunkSize;
                ConvertPixelFormat(src + start, dst + start, length);
            });
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
            var i = 0;

            // 32バイトアライメント
            var alignOffset = (int)((nint)dst & 31);
            if (alignOffset > 0)
            {
                alignOffset = 32 - alignOffset;
                ConvertRgbaToBgraScalar(src, dst, alignOffset);
                i = alignOffset;
            }

            // メインループ with プリフェッチ
            for (; i <= length - 128; i += 128)
            {
                // L1キャッシュへのプリフェッチ
                Sse.Prefetch0(src + i + PREFETCH_DISTANCE);

                // 4回のAVX2処理をまとめて実行（32バイト * 4 = 128バイト）
                for (var j = 0; j < 128; j += 32)
                {
                    var rgba = Avx.LoadVector256(src + i + j);
                    var bgra = Avx2.Shuffle(rgba, SHUFFLE_AVX2);
                    Avx.Store(dst + i + j, bgra);
                }
            }

            // 残りの32バイトブロックを処理
            for (; i <= length - 32; i += 32)
            {
                var rgba = Avx.LoadVector256(src + i);
                var bgra = Avx2.Shuffle(rgba, SHUFFLE_AVX2);
                Avx.Store(dst + i, bgra);
            }

            // 残りのピクセルをSSE2で処理
            if (USE_SSE2 && i <= length - 16)
            {
                var rgba = Sse2.LoadVector128(src + i);
                var bgra = Ssse3.Shuffle(rgba, SHUFFLE_SSE2);
                Sse2.Store(dst + i, bgra);
                i += 16;
            }

            // 最後の数ピクセルをスカラー処理
            ConvertRgbaToBgraScalar(src + i, dst + i, length - i);
        }

        private static unsafe void ConvertRgbaToBgraSse2(byte* src, byte* dst, int length)
        {
            var i = 0;

            // 16バイトアライメント
            var alignOffset = (int)((nint)dst & 15);
            if (alignOffset > 0)
            {
                alignOffset = 16 - alignOffset;
                ConvertRgbaToBgraScalar(src, dst, alignOffset);
                i = alignOffset;
            }

            // メインループ with プリフェッチ（64バイト = キャッシュライン）
            for (; i <= length - 64; i += 64)
            {
                Sse.Prefetch0(src + i + PREFETCH_DISTANCE);

                // 4回のSSE2処理をまとめて実行
                for (int j = 0; j < 64; j += 16)
                {
                    var rgba = Sse2.LoadVector128(src + i + j);
                    var bgra = Ssse3.Shuffle(rgba, SHUFFLE_SSE2);
                    Sse2.Store(dst + i + j, bgra);
                }
            }

            // 残りの16バイトブロックを処理
            for (; i <= length - 16; i += 16)
            {
                var rgba = Sse2.LoadVector128(src + i);
                var bgra = Ssse3.Shuffle(rgba, SHUFFLE_SSE2);
                Sse2.Store(dst + i, bgra);
            }

            ConvertRgbaToBgraScalar(src + i, dst + i, length - i);
        }

        private static unsafe void ConvertRgbaToBgraScalar(byte* src, byte* dst, int length)
        {
            var i = 0;

            // キャッシュライン（64バイト）に合わせたループアンローリング
            for (; i <= length - 64; i += 64)
            {
                // 16ピクセル（64バイト）を一度に処理
                for (var j = 0; j < 64; j += 4)
                {
                    dst[i + j + 0] = src[i + j + 2];
                    dst[i + j + 1] = src[i + j + 1];
                    dst[i + j + 2] = src[i + j + 0];
                    dst[i + j + 3] = src[i + j + 3];
                }
            }

            // 残りのピクセル処理
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
