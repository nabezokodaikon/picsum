using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Xmp;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class MetadataExtractorUtil
    {
        private const int FILE_READ_BUFFER_SIZE = 128;

        public static DateTime GetTakenDate(string filePath)
        {
            ArgumentNullException.ThrowIfNull(filePath, nameof(filePath));

            using (TimeMeasuring.Run(false, "MetadataExtractorUtil.GetTakenDate"))
            {
                using (var fs = new FileStream(filePath,
                    FileMode.Open, FileAccess.Read, FileShare.Read, FILE_READ_BUFFER_SIZE, FileOptions.SequentialScan))
                {
                    var directories = ImageMetadataReader.ReadMetadata(fs);

                    var date = GetTakenDateByExifSubIfdDirectory(directories);
                    if (date != FileUtil.EMPTY_DATETIME)
                    {
                        return date;
                    }

                    date = GetTakenDateByXmpDirectory(directories);
                    if (date != FileUtil.EMPTY_DATETIME)
                    {
                        return date;
                    }

                    return FileUtil.EMPTY_DATETIME;
                }
            }
        }

        private static DateTime GetTakenDateByExifSubIfdDirectory(
            IReadOnlyList<MetadataExtractor.Directory> directories)
        {
            var dir = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            if (dir == null)
            {
                return FileUtil.EMPTY_DATETIME;
            }

            if (!dir.TryGetDateTime(ExifSubIfdDirectory.TagDateTimeOriginal, out DateTime dateTimeOriginal))
            {
                return FileUtil.EMPTY_DATETIME;
            }

            return dateTimeOriginal;
        }

        private static DateTime GetTakenDateByXmpDirectory(
            IReadOnlyList<MetadataExtractor.Directory> directories)
        {
            var dir = directories.OfType<XmpDirectory>().FirstOrDefault();
            if (dir == null)
            {
                return FileUtil.EMPTY_DATETIME;
            }

            if (!dir.GetXmpProperties().TryGetValue("xmp:CreateDate", out string? createDateString))
            {
                return FileUtil.EMPTY_DATETIME;
            }

            if (!DateTimeOffset.TryParse(createDateString, out DateTimeOffset createDateOffset))
            {
                return FileUtil.EMPTY_DATETIME;
            }

            return createDateOffset.DateTime;
        }
    }
}
