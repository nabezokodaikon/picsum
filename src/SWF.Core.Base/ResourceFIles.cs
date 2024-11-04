using SWF.Core.Base.Properties;

namespace SWF.Core.Base
{
    public static class ResourceFiles
    {
        public static readonly Lazy<Icon> ApplicationIcon = new(() =>
            Resources.ApplicationIcon);

        public static readonly Lazy<Bitmap> ActiveRatingIcon = new(() =>
            CreateBitmapFromByteArray(Resources.ActiveRatingIcon));

        public static readonly Lazy<Bitmap> BookmarkIcon = new(() =>
            CreateBitmapFromByteArray(Resources.BookmarkIcon));

        public static readonly Lazy<Bitmap> DragTabIcon = new(() =>
            CreateBitmapFromByteArray(Resources.DragTabIcon));

        public static readonly Lazy<Bitmap> DropArrowIcon = new(() =>
            CreateBitmapFromByteArray(Resources.DropArrowIcon));

        public static readonly Lazy<Bitmap> DropLeftIcon = new(() =>
            CreateBitmapFromByteArray(Resources.DropLeftIcon));

        public static readonly Lazy<Bitmap> DropMaximumIcon = new(() =>
            CreateBitmapFromByteArray(Resources.DropMaximumIcon));

        public static readonly Lazy<Bitmap> DropRightIcon = new(() =>
            CreateBitmapFromByteArray(Resources.DropRightIcon));

        public static readonly Lazy<Bitmap> ExtraLargeEmptyIcon = new(() =>
            CreateBitmapFromByteArray(Resources.ExtraLargeEmptyIcon));

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

        public static readonly Lazy<Bitmap> JumboEmptyIcon = new(() =>
            CreateBitmapFromByteArray(Resources.JumboEmptyIcon));

        public static readonly Lazy<Bitmap> RatingIcon = new(() =>
            CreateBitmapFromByteArray(Resources.RatingIcon));

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

        public static readonly Lazy<Bitmap> SmallArrowUpIcon = new(() =>
            CreateBitmapFromByteArray(Resources.SmallArrowUpIcon));

        public static readonly Lazy<Bitmap> SmallEmptyIcon = new(() =>
            CreateBitmapFromByteArray(Resources.SmallEmptyIcon));

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
