using SWF.Core.Resource.Properties;
using System.Runtime.Versioning;

namespace SWF.Core.Resource
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class ResourceFiles
    {
        public static readonly Lazy<Bitmap> ApplicationIcon = new(() =>
            CreateBitmapFromByteArray(Resources.ApplicationIcon));

        public static readonly Lazy<Bitmap> ActiveRatingIcon = new(() =>
            CreateBitmapFromByteArray(Resources.ActiveRatingIcon));

        public static readonly Lazy<Bitmap> BookmarkIcon = new(() =>
            CreateBitmapFromByteArray(Resources.BookmarkIcon));

        public static readonly Lazy<Bitmap> DropArrowIcon = new(() =>
            CreateBitmapFromByteArray(Resources.DropArrowIcon));

        public static readonly Lazy<Bitmap> DropLeftIcon = new(() =>
            CreateBitmapFromByteArray(Resources.DropLeftIcon));

        public static readonly Lazy<Bitmap> DropRightIcon = new(() =>
            CreateBitmapFromByteArray(Resources.DropRightIcon));

        public static readonly Lazy<Bitmap> EmptyIcon = new(() =>
            CreateBitmapFromByteArray(Resources.EmptyIcon));

        public static readonly Lazy<Bitmap> GoBackIcon = new(() =>
            CreateBitmapFromByteArray(Resources.GoBackIcon));

        public static readonly Lazy<Bitmap> GoNextIcon = new(() =>
            CreateBitmapFromByteArray(Resources.GoNextIcon));

        public static readonly Lazy<Bitmap> HomeIcon = new(() =>
            CreateBitmapFromByteArray(Resources.HomeIcon));

        public static readonly Lazy<Bitmap> InactiveRatingIcon = new(() =>
            CreateBitmapFromByteArray(Resources.InactiveRatingIcon));

        public static readonly Lazy<Bitmap> InfoIcon = new(() =>
            CreateBitmapFromByteArray(Resources.InfoIcon));

        public static readonly Lazy<Bitmap> ReloadIcon = new(() =>
            CreateBitmapFromByteArray(Resources.ReloadIcon));

        public static readonly Lazy<Bitmap> SliderButtonIcon = new(() =>
            CreateBitmapFromByteArray(Resources.SliderButtonIcon));

        public static readonly Lazy<Bitmap> SmallArrowDownIcon = new(() =>
            CreateBitmapFromByteArray(Resources.SmallArrowDownIcon));

        public static readonly Lazy<Bitmap> SmallArrowLeftIcon = new(() =>
            CreateBitmapFromByteArray(Resources.SmallArrowLeftIcon));

        public static readonly Lazy<Bitmap> SmallArrowRightIcon = new(() =>
            CreateBitmapFromByteArray(Resources.SmallArrowRightIcon));

        public static readonly Lazy<Bitmap> TagIcon = new(() =>
            CreateBitmapFromByteArray(Resources.TagIcon));

        public static readonly Lazy<Bitmap> ThumbnailPanelIcon = new(() =>
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
