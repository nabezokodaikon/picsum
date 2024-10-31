namespace SWF.Core.Base
{
    public static class ResourceFiles
    {
        public static readonly string EmptyFile = Path.Combine(ResourceUtil.RESOURCE_DIRECTORY, "EmptyFile");

        public static readonly Lazy<Bitmap> ActiveRatingIcon = new(() =>
            (Bitmap)Bitmap.FromFile(Path.Combine(ResourceUtil.RESOURCE_DIRECTORY, "ActiveRatingIcon.png")));

        public static readonly Lazy<Icon> ApplicationIcon = new(() =>
            new Icon(Path.Combine(ResourceUtil.RESOURCE_DIRECTORY, "application.ico")));

        public static readonly Lazy<Bitmap> BookmarkIcon = new(() =>
            (Bitmap)Bitmap.FromFile(Path.Combine(ResourceUtil.RESOURCE_DIRECTORY, "BookmarkIcon.png")));

        public static readonly Lazy<Bitmap> DragTab = new(() =>
            (Bitmap)Bitmap.FromFile(Path.Combine(ResourceUtil.RESOURCE_DIRECTORY, "DragTab.png")));

        public static readonly Lazy<Bitmap> DropArrow = new(() =>
            (Bitmap)Bitmap.FromFile(Path.Combine(ResourceUtil.RESOURCE_DIRECTORY, "DropArrow.png")));

        public static readonly Lazy<Bitmap> DropLeft = new(() =>
            (Bitmap)Bitmap.FromFile(Path.Combine(ResourceUtil.RESOURCE_DIRECTORY, "DropLeft.png")));

        public static readonly Lazy<Bitmap> DropMaximum = new(() =>
            (Bitmap)Bitmap.FromFile(Path.Combine(ResourceUtil.RESOURCE_DIRECTORY, "DropMaximum.png")));

        public static readonly Lazy<Bitmap> DropRight = new(() =>
            (Bitmap)Bitmap.FromFile(Path.Combine(ResourceUtil.RESOURCE_DIRECTORY, "DropRight.png")));

        public static readonly Lazy<Bitmap> GoBackIcon = new(() =>
            (Bitmap)Bitmap.FromFile(Path.Combine(ResourceUtil.RESOURCE_DIRECTORY, "GoBackIcon.png")));

        public static readonly Lazy<Bitmap> GoNextIcon = new(() =>
            (Bitmap)Bitmap.FromFile(Path.Combine(ResourceUtil.RESOURCE_DIRECTORY, "GoNextIcon.png")));

        public static readonly Lazy<Bitmap> HomeIcon = new(() =>
            (Bitmap)Bitmap.FromFile(Path.Combine(ResourceUtil.RESOURCE_DIRECTORY, "HomeIcon.png")));

        public static readonly Lazy<Bitmap> InactiveRatingIcon = new(() =>
            (Bitmap)Bitmap.FromFile(Path.Combine(ResourceUtil.RESOURCE_DIRECTORY, "InactiveRatingIcon.png")));

        public static readonly Lazy<Bitmap> InfoIcon = new(() =>
            (Bitmap)Bitmap.FromFile(Path.Combine(ResourceUtil.RESOURCE_DIRECTORY, "InfoIcon.png")));

        public static readonly Lazy<Bitmap> RatingIcon = new(() =>
            (Bitmap)Bitmap.FromFile(Path.Combine(ResourceUtil.RESOURCE_DIRECTORY, "RatingIcon.png")));

        public static readonly Lazy<Bitmap> ReloadIcon = new(() =>
            (Bitmap)Bitmap.FromFile(Path.Combine(ResourceUtil.RESOURCE_DIRECTORY, "ReloadIcon.png")));

        public static readonly Lazy<Bitmap> SliderButton = new(() =>
            (Bitmap)Bitmap.FromFile(Path.Combine(ResourceUtil.RESOURCE_DIRECTORY, "SliderButton.png")));

        public static readonly Lazy<Bitmap> SmallArrowDown = new(() =>
            (Bitmap)Bitmap.FromFile(Path.Combine(ResourceUtil.RESOURCE_DIRECTORY, "SmallArrowDown.png")));

        public static readonly Lazy<Bitmap> SmallArrowLeft = new(() =>
            (Bitmap)Bitmap.FromFile(Path.Combine(ResourceUtil.RESOURCE_DIRECTORY, "SmallArrowLeft.png")));

        public static readonly Lazy<Bitmap> SmallArrowRight = new(() =>
            (Bitmap)Bitmap.FromFile(Path.Combine(ResourceUtil.RESOURCE_DIRECTORY, "SmallArrowRight.png")));

        public static readonly Lazy<Bitmap> SmallArrowUp = new(() =>
            (Bitmap)Bitmap.FromFile(Path.Combine(ResourceUtil.RESOURCE_DIRECTORY, "SmallArrowUp.png")));

        public static readonly Lazy<Bitmap> TagIcon = new(() =>
            (Bitmap)Bitmap.FromFile(Path.Combine(ResourceUtil.RESOURCE_DIRECTORY, "TagIcon.png")));

        public static readonly Lazy<Bitmap> ThumbnailPanel = new(() =>
            (Bitmap)Bitmap.FromFile(Path.Combine(ResourceUtil.RESOURCE_DIRECTORY, "ThumbnailPanel.png")));
    }
}
