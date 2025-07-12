namespace SWF.Core.Base
{
    public static class AppConstants
    {
        public const string UI_THREAD_NAME = "Main";
        public const string MUTEX_NAME = "11d9bca9";
        public const string PIPE_NAME = "be752c43";
        public const float DEFAULT_ZOOM_VALUE = 1f;

        public static readonly Font UI_FONT_09 = new("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Pixel);
        public static readonly Font UI_FONT_10 = new("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Pixel);
        public static readonly Font UI_FONT_12 = new("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Pixel);
        public static readonly Font UI_FONT_14_REGULAR = new("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
        public static readonly Font UI_FONT_14_BOLD = new("Yu Gothic UI", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
        public static readonly Font UI_FONT_18 = new("Yu Gothic UI", 18F, FontStyle.Regular, GraphicsUnit.Pixel);
        public static readonly Font UI_FONT_22 = new("Yu Gothic UI", 22F, FontStyle.Regular, GraphicsUnit.Pixel);
    }

    /// <summary>
    /// コンテンツ表示種別
    /// </summary>
    public enum PageOpenType
    {
        Default = 0,
        OverlapTab = 1,
        AddHome = 2,
        AddTab = 3,
        InsertTab = 4,
        NewWindow = 5
    }

    /// <summary>
    /// 画像表示モード
    /// </summary>
    public enum ImageDisplayMode
    {
        Single = 0,
        LeftFacing = 1,
        RightFacing = 2
    }

    /// <summary>
    /// 画像サイズモード
    /// </summary>
    public enum ImageSizeMode
    {
        Original = 0,
        FitAllImage = 1,
        FitOnlyBigImage = 2,
    }

    /// <summary>
    /// 画像表示位置
    /// </summary>
    public enum ImageAlign
    {
        Center = 0,
        Left = 1,
        Right = 2,
    }

    /// <summary>
    /// ソート種別ID
    /// </summary>
    public enum SortTypeID
    {
        Default = 0,
        FileName = 1,
        FilePath = 2,
        UpdateDate = 3,
        RegistrationDate = 5,
    }
}
