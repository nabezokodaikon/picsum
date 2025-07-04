using SWF.Core.ConsoleAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal static class JpegUtil
    {
        public static Bitmap ReadImageFile(Stream fs)
        {
            ArgumentNullException.ThrowIfNull(fs, nameof(fs));

            using (TimeMeasuring.Run(false, "JpegUtil.ReadImageFile"))
            {
                using (var reader = new BinaryReader(fs, System.Text.Encoding.UTF8, true))
                {
                    var bytes = reader.ReadBytes((int)fs.Length - 1);
                    var orientation = GetOrientation(bytes);
                    var rotateFlipType = GetRotateFlipType(orientation);
                    if (rotateFlipType != RotateFlipType.RotateNoneFlipNone)
                    {
                        var bmp = (Bitmap)Bitmap.FromStream(fs, false, true);
                        bmp.RotateFlip(rotateFlipType);
                        return bmp;
                    }
                    else
                    {
                        return (Bitmap)Bitmap.FromStream(fs, false, true);
                    }
                }
            }
        }

        public static Size GetImageSize(FileStream fs)
        {
            ArgumentNullException.ThrowIfNull(fs, nameof(fs));

            using (TimeMeasuring.Run(false, "JpegUtil.GetImageSize"))
            {
                using (var reader = new BinaryReader(fs, System.Text.Encoding.UTF8, true))
                {
                    var bytes = reader.ReadBytes((int)fs.Length - 1);

                    var imageSize = GetImageSize(bytes);
                    var orientation = GetOrientation(bytes);

                    // Orientationに基づいて縦横を調整
                    var size = ShouldSwapDimensions(orientation)
                        ? new Size(imageSize.Height, imageSize.Width)
                        : imageSize;

                    return size;
                }
            }
        }

        private static RotateFlipType GetRotateFlipType(int orientation)
        {
            return orientation switch
            {
                3 => RotateFlipType.Rotate180FlipNone,
                6 => RotateFlipType.Rotate90FlipNone,
                8 => RotateFlipType.Rotate270FlipNone,
                _ => RotateFlipType.RotateNoneFlipNone,
            };
        }

        private static Size GetImageSize(byte[] bytes)
        {
            int pos = 2; // SOI (Start of Image) をスキップ

            while (pos < bytes.Length - 1)
            {
                // マーカーを探す
                if (bytes[pos] != 0xFF)
                {
                    pos++;
                    continue;
                }

                var marker = bytes[pos + 1];
                pos += 2;

                // SOF (Start of Frame) マーカーをチェック
                if ((marker >= 0xC0 && marker <= 0xC3) ||
                    (marker >= 0xC5 && marker <= 0xC7) ||
                    (marker >= 0xC9 && marker <= 0xCB) ||
                    (marker >= 0xCD && marker <= 0xCF))
                {
                    // SOFセグメント内の画像サイズを読み取り
                    pos += 3; // セグメント長（2バイト）+ サンプル精度（1バイト）をスキップ

                    int height = (bytes[pos] << 8) | bytes[pos + 1];
                    int width = (bytes[pos + 2] << 8) | bytes[pos + 3];

                    return new Size(width, height);
                }

                // 他のセグメントはスキップ
                if (pos + 1 < bytes.Length)
                {
                    int segmentLength = (bytes[pos] << 8) | bytes[pos + 1];
                    pos += segmentLength;
                }
            }

            throw new InvalidDataException("Invalid JPEG format");
        }

        private static int GetOrientation(byte[] bytes)
        {
            int pos = 2; // SOI をスキップ

            while (pos < bytes.Length - 1)
            {
                if (bytes[pos] != 0xFF)
                {
                    pos++;
                    continue;
                }

                var marker = bytes[pos + 1];
                pos += 2;

                // APP1 (EXIF) マーカーをチェック
                if (marker == 0xE1)
                {
                    if (pos + 1 >= bytes.Length) break;

                    int segmentLength = (bytes[pos] << 8) | bytes[pos + 1];
                    pos += 2;

                    // "Exif\0\0" をチェック
                    if (pos + 6 < bytes.Length &&
                        bytes[pos] == 0x45 && bytes[pos + 1] == 0x78 &&
                        bytes[pos + 2] == 0x69 && bytes[pos + 3] == 0x66 &&
                        bytes[pos + 4] == 0x00 && bytes[pos + 5] == 0x00)
                    {
                        return ParseExifOrientation(bytes, pos + 6, segmentLength - 8);
                    }

                    pos += segmentLength - 2;
                }
                else
                {
                    // 他のセグメントをスキップ
                    if (pos + 1 < bytes.Length)
                    {
                        int segmentLength = (bytes[pos] << 8) | bytes[pos + 1];
                        pos += segmentLength;
                    }
                }
            }

            return 1; // デフォルトのOrientation
        }

        private static int ParseExifOrientation(byte[] bytes, int exifStart, int exifLength)
        {
            if (exifLength < 8) return 1;

            // TIFF ヘッダーをチェック
            bool isLittleEndian;
            if (bytes[exifStart] == 0x49 && bytes[exifStart + 1] == 0x49)
            {
                isLittleEndian = true;
            }
            else if (bytes[exifStart] == 0x4D && bytes[exifStart + 1] == 0x4D)
            {
                isLittleEndian = false;
            }
            else
            {
                return 1; // 無効なTIFFヘッダー
            }

            // IFD0 オフセットを取得
            int ifd0Offset = ReadInt32(bytes, exifStart + 4, isLittleEndian);
            if (ifd0Offset >= exifLength - 2) return 1;

            // IFD0 エントリ数を取得
            int numEntries = ReadInt16(bytes, exifStart + ifd0Offset, isLittleEndian);
            int entryStart = exifStart + ifd0Offset + 2;

            // Orientation タグ (0x0112) を探す
            for (int i = 0; i < numEntries && entryStart + i * 12 + 12 <= exifStart + exifLength; i++)
            {
                int entryPos = entryStart + i * 12;
                int tag = ReadInt16(bytes, entryPos, isLittleEndian);

                if (tag == 0x0112) // Orientation タグ
                {
                    int type = ReadInt16(bytes, entryPos + 2, isLittleEndian);
                    if (type == 3) // SHORT type
                    {
                        return ReadInt16(bytes, entryPos + 8, isLittleEndian);
                    }
                }
            }

            return 1; // Orientationが見つからない場合
        }

        private static int ReadInt16(byte[] bytes, int offset, bool isLittleEndian)
        {
            if (offset + 1 >= bytes.Length) return 0;

            if (isLittleEndian)
            {
                return bytes[offset] | (bytes[offset + 1] << 8);
            }
            else
            {
                return (bytes[offset] << 8) | bytes[offset + 1];
            }
        }

        private static int ReadInt32(byte[] bytes, int offset, bool isLittleEndian)
        {
            if (offset + 3 >= bytes.Length) return 0;

            if (isLittleEndian)
            {
                return bytes[offset] | (bytes[offset + 1] << 8) |
                       (bytes[offset + 2] << 16) | (bytes[offset + 3] << 24);
            }
            else
            {
                return (bytes[offset] << 24) | (bytes[offset + 1] << 16) |
                       (bytes[offset + 2] << 8) | bytes[offset + 3];
            }
        }

        private static bool ShouldSwapDimensions(int orientation)
        {
            // Orientation値:
            // 1 = 0度 (正常)
            // 2 = 0度, 左右反転
            // 3 = 180度回転
            // 4 = 180度回転, 左右反転
            // 5 = 90度時計回り + 左右反転
            // 6 = 90度時計回り
            // 7 = 90度反時計回り + 左右反転
            // 8 = 90度反時計回り

            return orientation == 5 || orientation == 6 ||
                   orientation == 7 || orientation == 8;
        }
    }
}
