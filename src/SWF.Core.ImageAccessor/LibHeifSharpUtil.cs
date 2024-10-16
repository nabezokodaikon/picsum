using LibHeifSharp;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows")]
    internal static class LibHeifSharpUtil
    {
        public static Size GetImageSize(FileStream fs)
        {
            ArgumentNullException.ThrowIfNull(fs, nameof(fs));

            using (var context = new HeifContext(fs))
            using (var primaryImageHandle = context.GetPrimaryImageHandle())
            {
                return new Size(primaryImageHandle.Width, primaryImageHandle.Height);
            }
        }
    }
}
