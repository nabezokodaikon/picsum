using OpenCvSharp;
using OpenCvSharp.Extensions;
using SWF.Core.Base;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class OpenCVUtil
    {
        private static readonly ImageEncodingParam WEBP_QUALITY
            = new(ImwriteFlags.WebPQuality, 70);

        private static Bitmap ToBitmap(Mat mat)
        {
            using (TimeMeasuring.Run(false, "OpenCVUtil.ToBitmap"))
            {
                return mat.ToBitmap();
            }
        }

        private static unsafe Bitmap ToBitmapFast(Mat sourceMat)
        {
            using (TimeMeasuring.Run(false, "OpenCVBitmapConverter.ToBitmapFast"))
            {
                // Matの深さとチャンネル数を取得
                MatType matType = sourceMat.Type();
                int depth = matType.Depth;
                int channels = matType.Channels;

                // Bitmapのピクセルフォーマットを決定
                var bmpFormat = depth switch
                {
                    // 8ビット符号なし整数
                    MatType.CV_8U => channels switch
                    {
                        1 => PixelFormat.Format8bppIndexed,
                        3 => PixelFormat.Format24bppRgb,
                        4 => PixelFormat.Format32bppArgb,
                        _ => throw new NotSupportedException($"Unsupported Mat channels for 8-bit unsigned: {channels}"),
                    },
                    // 16ビット符号なし整数 (例: Depthマップ)
                    MatType.CV_16U => throw new NotSupportedException("Unsupported Mat depth (CV_16U). Consider converting to 8-bit first."),// 一般的なBitmapフォーマットには直接対応しないため、変換が必要になることが多い
                                                                                                                                             // あるいは、Format48bppRgbなどの高ビット深度フォーマットを検討
                                                                                                                                             // 32ビット浮動小数点数 (例: 特徴量マップ)
                    MatType.CV_32F => throw new NotSupportedException("Unsupported Mat depth (CV_32F). Consider converting to 8-bit first."),// 同上
                    _ => throw new NotSupportedException($"Unsupported Mat depth: {depth}"),
                };

                // 新しいBitmapを作成
                var displayBitmap = new Bitmap(sourceMat.Width, sourceMat.Height, bmpFormat);

                // 8bppIndexedの場合、パレットを設定する
                if (bmpFormat == PixelFormat.Format8bppIndexed)
                {
                    ColorPalette palette = displayBitmap.Palette;
                    for (int i = 0; i < 256; i++)
                    {
                        palette.Entries[i] = Color.FromArgb(i, i, i);
                    }
                    displayBitmap.Palette = palette;
                }

                // Bitmapのピクセルデータをロック
                BitmapData bmpData = displayBitmap.LockBits(
                    new System.Drawing.Rectangle(0, 0, displayBitmap.Width, displayBitmap.Height),
                    ImageLockMode.WriteOnly,
                    displayBitmap.PixelFormat);

                try
                {
                    // Matのデータポインタ
                    IntPtr matDataPtr = (IntPtr)sourceMat.DataPointer;
                    // Bitmapのデータポインタ
                    IntPtr bmpDataPtr = bmpData.Scan0;

                    // Matの行ごとのバイト数 (ストライド)
                    long matStride = sourceMat.Step(); // Step()はlongを返す
                                                       // Bitmapの行ごとのバイト数 (ストライド、パディングが含まれる場合がある)
                    long bmpStride = bmpData.Stride;

                    // コピーするバイト数（各行でコピーするMatの有効データバイト数）
                    long bytesPerRow = sourceMat.Cols * channels * sourceMat.ElemSize1(); // sourceMat.Width * channels * BytesPerPixel

                    // ストライドが同じなら、Mat全体を一括コピーするのが最も効率的
                    if (matStride == bmpStride)
                    {
                        Buffer.MemoryCopy(
                            (void*)matDataPtr, // ソースの開始ポインタ
                            (void*)bmpDataPtr, // デスティネーションの開始ポインタ
                            bmpStride * sourceMat.Height, // デスティネーションの合計バッファサイズ（少なくともこれだけは確保されているはず）
                            matStride * sourceMat.Height  // ソースからコピーするバイト数（Matの全データサイズ）
                        );
                    }
                    else
                    {
                        // ストライドが異なる場合、行ごとにコピー
                        for (int y = 0; y < sourceMat.Height; y++)
                        {
                            Buffer.MemoryCopy(
                                (void*)(matDataPtr + y * matStride), // ソースの行開始ポインタ
                                (void*)(bmpDataPtr + y * bmpStride), // デスティネーションの行開始ポインタ
                                bytesPerRow, // コピーするバイト数（Matの有効データ行のバイト数）
                                bytesPerRow // ソースからコピーするバイト数 (MemoryCopyの引数として)
                            );
                        }
                    }

                    return displayBitmap;
                }
                finally
                {
                    // Bitmapのロックを解除
                    displayBitmap.UnlockBits(bmpData);
                }
            }
        }

        public static Mat ToMat(Bitmap bmp)
        {
            using (TimeMeasuring.Run(false, "OpenCVUtil.ToMat"))
            {
                return bmp.ToMat();
            }
        }

        public static Bitmap Resize(Mat srcMat, float newWidth, float newHeight, InterpolationFlags flag)
        {
            ArgumentNullException.ThrowIfNull(srcMat, nameof(srcMat));

            using (TimeMeasuring.Run(false, "OpenCVUtil.Resize By Mat"))
            {
                var size = new OpenCvSharp.Size(newWidth, newHeight);
                using (var destMat = new Mat(size, srcMat.Type()))
                {
                    Cv2.Resize(srcMat, destMat, size, 0, 0, flag);
                    return ToBitmapFast(destMat);
                }
            }
        }

        public static Mat Resize(Bitmap srcBmp, float width, float height, InterpolationFlags flag)
        {
            ArgumentNullException.ThrowIfNull(srcBmp, nameof(srcBmp));

            using (TimeMeasuring.Run(false, "OpenCVUtil.Resize By Bitmap"))
            {
                var size = new OpenCvSharp.Size(width, height);
                using (var srcMat = ToMat(srcBmp))
                {
                    var destMat = new Mat(size, srcMat.Type());
                    Cv2.Resize(srcMat, destMat, size, 0, 0, flag);
                    return destMat;
                }
            }
        }

        public static Bitmap ReadImageFile(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));

            using (TimeMeasuring.Run(false, "OpenCVUtil.ReadImageFile"))
            {
                stream.Seek(0, SeekOrigin.Begin);
                using (var mat = Mat.FromStream(stream, ImreadModes.Unchanged))
                {
                    return ToBitmapFast(mat);
                }
            }
        }

        public static Mat ReadImageFileToMat(byte[] bf)
        {
            ArgumentNullException.ThrowIfNull(bf, nameof(bf));

            using (TimeMeasuring.Run(false, "OpenCVUtil.ReadImageFileToMat"))
            {
                return Cv2.ImDecode(bf, ImreadModes.Unchanged);
            }
        }

        public static byte[] ToCompressionBinary(Mat mat)
        {
            Cv2.ImEncode(".webp", mat, out var bf, WEBP_QUALITY);
            return bf;
        }
    }
}
