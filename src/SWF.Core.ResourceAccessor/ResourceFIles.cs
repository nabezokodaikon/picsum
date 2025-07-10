using SWF.Core.Base;
using SWF.Core.ResourceAccessor.Properties;
using System.Runtime.Versioning;

namespace SWF.Core.ResourceAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class ResourceFiles
    {
        public static readonly Lazy<Icon> AppIcon = new(() =>
            Resources.AppIcon,
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> ActiveRatingIcon = new(
            () => CreateBitmapFromByteArray(Resources.ActiveRatingIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> BookmarkIcon = new(
            () => CreateBitmapFromByteArray(Resources.BookmarkIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> DropArrowIcon = new(
            () => CreateBitmapFromByteArray(Resources.DropArrowIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> DropLeftIcon = new(
            () => CreateBitmapFromByteArray(Resources.DropLeftIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> DropRightIcon = new(
            () => CreateBitmapFromByteArray(Resources.DropRightIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> EmptyIcon = new(
            () => CreateBitmapFromByteArray(Resources.EmptyIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> GoBackIcon = new(
            () => CreateBitmapFromByteArray(Resources.GoBackIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> GoNextIcon = new(
            () => CreateBitmapFromByteArray(Resources.GoNextIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> HomeIcon = new(
            () => CreateBitmapFromByteArray(Resources.HomeIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> InactiveRatingIcon = new(
            () => CreateBitmapFromByteArray(Resources.InactiveRatingIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> InfoIcon = new(
            () => CreateBitmapFromByteArray(Resources.InfoIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> ReloadIcon = new(
            () => CreateBitmapFromByteArray(Resources.ReloadIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> SliderButtonIcon = new(
            () => CreateBitmapFromByteArray(Resources.SliderButtonIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> SmallArrowDownIcon = new(
            () => CreateBitmapFromByteArray(Resources.SmallArrowDownIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> SmallArrowLeftIcon = new(
            () => CreateBitmapFromByteArray(Resources.SmallArrowLeftIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> SmallArrowRightIcon = new(
            () => CreateBitmapFromByteArray(Resources.SmallArrowRightIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> TagIcon = new(
            () => CreateBitmapFromByteArray(Resources.TagIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> ThumbnailPanelIcon = new(
            () => CreateBitmapFromByteArray(Resources.ThumbnailPanelIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        private static Bitmap CreateBitmapFromByteArray(byte[] byteArray)
        {
            using (TimeMeasuring.Run(false, "ResourceFiles.CreateBitmapFromByteArray"))
            {
                using (var memoryStream = new MemoryStream(byteArray))
                {
                    return new Bitmap(memoryStream);
                }
            }
        }
    }
}
