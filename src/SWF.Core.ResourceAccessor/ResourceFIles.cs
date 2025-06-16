using SWF.Core.Base;
using SWF.Core.ResourceAccessor.Properties;
using System.Runtime.Versioning;

namespace SWF.Core.ResourceAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class ResourceFiles
    {
        public static readonly FastLazy<Icon> AppIcon = new(() =>
            Resources.AppIcon);

        public static readonly FastLazy<Bitmap> ActiveRatingIcon = new(() =>
            CreateBitmapFromByteArray(Resources.ActiveRatingIcon));

        public static readonly FastLazy<Bitmap> BookmarkIcon = new(() =>
            CreateBitmapFromByteArray(Resources.BookmarkIcon));

        public static readonly FastLazy<Bitmap> DropArrowIcon = new(() =>
            CreateBitmapFromByteArray(Resources.DropArrowIcon));

        public static readonly FastLazy<Bitmap> DropLeftIcon = new(() =>
            CreateBitmapFromByteArray(Resources.DropLeftIcon));

        public static readonly FastLazy<Bitmap> DropRightIcon = new(() =>
            CreateBitmapFromByteArray(Resources.DropRightIcon));

        public static readonly FastLazy<Bitmap> EmptyIcon = new(() =>
            CreateBitmapFromByteArray(Resources.EmptyIcon));

        public static readonly FastLazy<Bitmap> GoBackIcon = new(() =>
            CreateBitmapFromByteArray(Resources.GoBackIcon));

        public static readonly FastLazy<Bitmap> GoNextIcon = new(() =>
            CreateBitmapFromByteArray(Resources.GoNextIcon));

        public static readonly FastLazy<Bitmap> HomeIcon = new(() =>
            CreateBitmapFromByteArray(Resources.HomeIcon));

        public static readonly FastLazy<Bitmap> InactiveRatingIcon = new(() =>
            CreateBitmapFromByteArray(Resources.InactiveRatingIcon));

        public static readonly FastLazy<Bitmap> InfoIcon = new(() =>
            CreateBitmapFromByteArray(Resources.InfoIcon));

        public static readonly FastLazy<Bitmap> ReloadIcon = new(() =>
            CreateBitmapFromByteArray(Resources.ReloadIcon));

        public static readonly FastLazy<Bitmap> SliderButtonIcon = new(() =>
            CreateBitmapFromByteArray(Resources.SliderButtonIcon));

        public static readonly FastLazy<Bitmap> SmallArrowDownIcon = new(() =>
            CreateBitmapFromByteArray(Resources.SmallArrowDownIcon));

        public static readonly FastLazy<Bitmap> SmallArrowLeftIcon = new(() =>
            CreateBitmapFromByteArray(Resources.SmallArrowLeftIcon));

        public static readonly FastLazy<Bitmap> SmallArrowRightIcon = new(() =>
            CreateBitmapFromByteArray(Resources.SmallArrowRightIcon));

        public static readonly FastLazy<Bitmap> TagIcon = new(() =>
            CreateBitmapFromByteArray(Resources.TagIcon));

        public static readonly FastLazy<Bitmap> ThumbnailPanelIcon = new(() =>
            CreateBitmapFromByteArray(Resources.ThumbnailPanelIcon));

        private static Bitmap CreateBitmapFromByteArray(byte[] byteArray)
        {
            using (var memoryStream = new MemoryStream(byteArray))
            {
                return new Bitmap(memoryStream);
            }
        }
    }
}
