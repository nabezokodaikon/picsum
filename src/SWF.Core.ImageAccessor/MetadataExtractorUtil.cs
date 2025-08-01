using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class MetadataExtractorUtil
    {
        private const int FILE_READ_BUFFER_SIZE = 128;

        public static DateTime GetPhotographDate(string filePath)
        {
            ArgumentNullException.ThrowIfNull(filePath, nameof(filePath));

            using (TimeMeasuring.Run(true, "MetadataExtractorUtil.GetPhotographDate"))
            {
                using (var fs = new FileStream(filePath,
                    FileMode.Open, FileAccess.Read, FileShare.Read, FILE_READ_BUFFER_SIZE, FileOptions.SequentialScan))
                {
                    var directories = ImageMetadataReader.ReadMetadata(filePath);
                    var exifSubIfdDirectory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                    if (exifSubIfdDirectory == null)
                    {
                        return FileUtil.EMPTY_DATETIME;
                    }

                    if (!exifSubIfdDirectory.TryGetDateTime(ExifSubIfdDirectory.TagDateTimeOriginal, out var dateTimeOriginal))
                    {
                        return FileUtil.EMPTY_DATETIME;
                    }

                    return dateTimeOriginal;
                }
            }

        }
    }
}
