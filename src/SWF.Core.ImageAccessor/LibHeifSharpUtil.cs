using LibHeifSharp;
using System.Drawing.Imaging;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal static class LibHeifSharpUtil
    {
        private static readonly bool useAvx2 = Avx2.IsSupported;

        private static readonly ParallelOptions parallelOptions
             = new()
             {
                 MaxDegreeOfParallelism = Environment.ProcessorCount
             };

        private static readonly Vector256<byte> shuffle = Vector256.Create(
            2, 1, 0, 3,  // First pixel
            6, 5, 4, 7,  // Second pixel
            10, 9, 8, 11,  // Third pixel
            14, 13, 12, 15,  // Fourth pixel
            18, 17, 16, 19,  // Fifth pixel
            22, 21, 20, 23,  // Sixth pixel
            26, 25, 24, 27,  // Seventh pixel
            30, 29, 28, 31   // Eighth pixel
        ).AsByte();

        private static readonly HeifDecodingOptions decodingOptions = new()
        {
            ConvertHdrToEightBit = true,
            IgnoreTransformations = false,
        };

        public static Size GetImageSize(Stream fs)
        {
            ArgumentNullException.ThrowIfNull(fs, nameof(fs));

            using (var context = new HeifContext(fs))
            using (var primaryImageHandle = context.GetPrimaryImageHandle())
            {
                return new Size(primaryImageHandle.Width, primaryImageHandle.Height);
            }
        }

        public static unsafe Bitmap ReadImageFile(Stream fs)
        {
            using (var context = new HeifContext(fs))
            using (var handle = context.GetPrimaryImageHandle())
            using (var heifImage = handle.Decode(HeifColorspace.Rgb, HeifChroma.InterleavedRgba32, decodingOptions))
            {
                var width = heifImage.Width;
                var height = heifImage.Height;
                var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                var bitmapData = bitmap.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppArgb);
                try
                {
                    var plane = heifImage.GetPlane(HeifChannel.Interleaved);
                    var srcPtr = (byte*)plane.Scan0;
                    var dstPtr = (byte*)bitmapData.Scan0;
                    var srcStride = plane.Stride;
                    var dstStride = bitmapData.Stride;

                    // SIMD操作のための定数を準備
                    if (useAvx2 && width >= 8) // AVX2が使用可能で、幅が8ピクセル以上の場合
                    {
                        // 並列処理で各行を変換
                        Parallel.For(0, height, parallelOptions, y =>
                        {
                            var srcRow = srcPtr + y * srcStride;
                            var dstRow = dstPtr + y * dstStride;
                            var x = 0;

                            // 8ピクセルずつ処理
                            for (; x <= width - 8; x += 8)
                            {
                                var rgba = Avx.LoadVector256(srcRow);
                                var bgra = Avx2.Shuffle(rgba, shuffle);
                                Avx.Store(dstRow, bgra);

                                srcRow += 32; // 8 pixels * 4 bytes
                                dstRow += 32;
                            }

                            // 残りのピクセルを通常の方法で処理
                            for (; x < width; x++)
                            {
                                dstRow[0] = srcRow[2]; // B
                                dstRow[1] = srcRow[1]; // G
                                dstRow[2] = srcRow[0]; // R
                                dstRow[3] = srcRow[3]; // A
                                srcRow += 4;
                                dstRow += 4;
                            }
                        });
                    }
                    else // SIMD非対応の場合は従来の処理
                    {
                        Parallel.For(0, height, y =>
                        {
                            var srcRow = srcPtr + y * srcStride;
                            var dstRow = dstPtr + y * dstStride;
                            for (var x = 0; x < width; x++)
                            {
                                dstRow[0] = srcRow[2];
                                dstRow[1] = srcRow[1];
                                dstRow[2] = srcRow[0];
                                dstRow[3] = srcRow[3];
                                srcRow += 4;
                                dstRow += 4;
                            }
                        });
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
}

