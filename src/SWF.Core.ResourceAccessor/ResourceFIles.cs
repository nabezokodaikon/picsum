using SWF.Core.Base;
using SWF.Core.ResourceAccessor.Properties;

namespace SWF.Core.ResourceAccessor
{

    public static class ResourceFiles
    {
        public static readonly Lazy<Icon> AppIcon = new(static () =>
            Resources.AppIcon,
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> ActiveRatingIcon = new(
            static () => CreateBitmapFromByteArray(Resources.ActiveRatingIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> BookmarkIcon = new(
            static () => CreateBitmapFromByteArray(Resources.BookmarkIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> DropArrowIcon = new(
            static () => CreateBitmapFromByteArray(Resources.DropArrowIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> EmptyIcon = new(
            static () => CreateBitmapFromByteArray(Resources.EmptyIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> GoBackIcon = new(
            static () => CreateBitmapFromByteArray(Resources.GoBackIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> GoNextIcon = new(
            static () => CreateBitmapFromByteArray(Resources.GoNextIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> HomeIcon = new(
            static () => CreateBitmapFromByteArray(Resources.HomeIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> InactiveRatingIcon = new(
            static () => CreateBitmapFromByteArray(Resources.InactiveRatingIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> InfoIcon = new(
            static () => CreateBitmapFromByteArray(Resources.InfoIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> ReloadIcon = new(
            static () => CreateBitmapFromByteArray(Resources.ReloadIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> SliderButtonIcon = new(
            static () => CreateBitmapFromByteArray(Resources.SliderButtonIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> SmallArrowDownIcon = new(
            static () => CreateBitmapFromByteArray(Resources.SmallArrowDownIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> SmallArrowLeftIcon = new(
            static () => CreateBitmapFromByteArray(Resources.SmallArrowLeftIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> SmallArrowRightIcon = new(
            static () => CreateBitmapFromByteArray(Resources.SmallArrowRightIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> TagIcon = new(
            static () => CreateBitmapFromByteArray(Resources.TagIcon),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static readonly Lazy<Bitmap> ThumbnailPanelIcon = new(
            static () => CreateBitmapFromByteArray(Resources.ThumbnailPanelIcon),
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
