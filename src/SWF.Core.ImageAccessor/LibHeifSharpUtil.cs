using LibHeifSharp;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows")]
    internal static class LibHeifSharpUtil
    {
        public static Size GetImageSize(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            using (var context = new HeifContext(filePath))
            using (var primaryImageHandle = context.GetPrimaryImageHandle())
            {
                return new Size(primaryImageHandle.Width, primaryImageHandle.Height);
            }
        }
    }
}
