using LibHeifSharp;

namespace SWF.Core.ImageAccessor
{

    internal static class LibHeifSharpUtil
    {
        public static Size GetImageSize(Stream fs)
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

