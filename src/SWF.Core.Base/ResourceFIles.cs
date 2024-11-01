using SWF.Core.Base.Properties;

namespace SWF.Core.Base
{
    public static class ResourceFiles
    {
        public static readonly Lazy<Icon> ApplicationIcon = new(() =>
            Resources.ApplicationIcon);

        public static readonly Lazy<Bitmap> ActiveRatingIcon = new(() =>
            ResourceUtil.CreateBitmapFromByteArray(Resources.ActiveRatingIcon));

        public static readonly Lazy<Bitmap> BookmarkIcon = new(() =>
            ResourceUtil.CreateBitmapFromByteArray(Resources.BookmarkIcon));

        public static readonly Lazy<Bitmap> DragTabIcon = new(() =>
            ResourceUtil.CreateBitmapFromByteArray(Resources.DragTabIcon));

        public static readonly Lazy<Bitmap> DropArrowIcon = new(() =>
            ResourceUtil.CreateBitmapFromByteArray(Resources.DropArrowIcon));

        public static readonly Lazy<Bitmap> DropLeftIcon = new(() =>
            ResourceUtil.CreateBitmapFromByteArray(Resources.DropLeftIcon));

        public static readonly Lazy<Bitmap> DropMaximumIcon = new(() =>
            ResourceUtil.CreateBitmapFromByteArray(Resources.DropMaximumIcon));

        public static readonly Lazy<Bitmap> DropRightIcon = new(() =>
            ResourceUtil.CreateBitmapFromByteArray(Resources.DropRightIcon));

        public static readonly Lazy<Bitmap> EmptyIcon = new(() =>
            ResourceUtil.CreateBitmapFromByteArray(Resources.EmptyIcon));

        public static readonly Lazy<Bitmap> GoBackIcon = new(() =>
            ResourceUtil.CreateBitmapFromByteArray(Resources.GoBackIcon));

        public static readonly Lazy<Bitmap> GoNextIcon = new(() =>
            ResourceUtil.CreateBitmapFromByteArray(Resources.GoNextIcon));

        public static readonly Lazy<Bitmap> HomeIcon = new(() =>
            ResourceUtil.CreateBitmapFromByteArray(Resources.HomeIcon));

        public static readonly Lazy<Bitmap> InactiveRatingIcon = new(() =>
            ResourceUtil.CreateBitmapFromByteArray(Resources.InactiveRatingIcon));

        public static readonly Lazy<Bitmap> InfoIcon = new(() =>
                ResourceUtil.CreateBitmapFromByteArray(Resources.InfoIcon));

        public static readonly Lazy<Bitmap> RatingIcon = new(() =>
            ResourceUtil.CreateBitmapFromByteArray(Resources.RatingIcon));

        public static readonly Lazy<Bitmap> ReloadIcon = new(() =>
            ResourceUtil.CreateBitmapFromByteArray(Resources.ReloadIcon));

        public static readonly Lazy<Bitmap> SliderButtonIcon = new(() =>
            ResourceUtil.CreateBitmapFromByteArray(Resources.SliderButtonIcon));

        public static readonly Lazy<Bitmap> SmallArrowDownIcon = new(() =>
            ResourceUtil.CreateBitmapFromByteArray(Resources.SmallArrowDownIcon));

        public static readonly Lazy<Bitmap> SmallArrowLeftIcon = new(() =>
            ResourceUtil.CreateBitmapFromByteArray(Resources.SmallArrowLeftIcon));

        public static readonly Lazy<Bitmap> SmallArrowRightIcon = new(() =>
            ResourceUtil.CreateBitmapFromByteArray(Resources.SmallArrowRightIcon));

        public static readonly Lazy<Bitmap> SmallArrowUpIcon = new(() =>
            ResourceUtil.CreateBitmapFromByteArray(Resources.SmallArrowUpIcon));

        public static readonly Lazy<Bitmap> TagIcon = new(() =>
            ResourceUtil.CreateBitmapFromByteArray(Resources.TagIcon));

        public static readonly Lazy<Bitmap> ThumbnailPanelIcon = new(() =>
            ResourceUtil.CreateBitmapFromByteArray(Resources.ThumbnailPanelIcon));
    }
}
