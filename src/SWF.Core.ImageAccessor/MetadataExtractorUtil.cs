using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Xmp;
using SWF.Core.Base;

namespace SWF.Core.ImageAccessor
{

    public static class MetadataExtractorUtil
    {
        private const int FILE_READ_BUFFER_SIZE = 128;

        public static DateTime GetTakenDate(string filePath)
        {
            ArgumentNullException.ThrowIfNull(filePath, nameof(filePath));

            using (Measuring.Time(false, "MetadataExtractorUtil.GetTakenDate"))
            {
                using (var fs = new FileStream(filePath,
                    FileMode.Open, FileAccess.Read, FileShare.Read, FILE_READ_BUFFER_SIZE, FileOptions.SequentialScan))
                {
                    var directories = ImageMetadataReader.ReadMetadata(fs);

                    var date = GetTakenDateByExifSubIfdDirectory(directories);
                    if (!date.IsEmpty())
                    {
                        return date;
                    }

                    date = GetTakenDateByXmpDirectory(directories);
                    if (!date.IsEmpty())
                    {
                        return date;
                    }

                    return DateTimeExtensions.EMPTY;
                }
            }
        }

        private static DateTime GetTakenDateByExifSubIfdDirectory(
            IReadOnlyList<MetadataExtractor.Directory> directories)
        {
            var dir = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            if (dir == null)
            {
                return DateTimeExtensions.EMPTY;
            }

            if (!dir.TryGetDateTime(ExifSubIfdDirectory.TagDateTimeOriginal, out DateTime dateTimeOriginal))
            {
                return DateTimeExtensions.EMPTY;
            }

            return dateTimeOriginal;
        }

        private static DateTime GetTakenDateByXmpDirectory(
            IReadOnlyList<MetadataExtractor.Directory> directories)
        {
            var dir = directories.OfType<XmpDirectory>().FirstOrDefault();
            if (dir == null)
            {
                return DateTimeExtensions.EMPTY;
            }

            if (!dir.GetXmpProperties().TryGetValue("xmp:CreateDate", out string? createDateString))
            {
                return DateTimeExtensions.EMPTY;
            }

            if (!DateTimeOffset.TryParse(createDateString, out DateTimeOffset createDateOffset))
            {
                return DateTimeExtensions.EMPTY;
            }

            return createDateOffset.DateTime;
        }
    }
}
