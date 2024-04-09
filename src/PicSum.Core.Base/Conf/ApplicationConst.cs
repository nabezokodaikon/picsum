namespace PicSum.Core.Base.Conf
{
    /// <summary>
    /// アプリケーション定数クラス
    /// </summary>
    public static class ApplicationConst
    {
        /// <summary>
        /// 番号付SQLパラメータの書式
        /// </summary>
        public const string NUMBERING_SQL_PARAMETER_FORMAT = "{0}_{1}";

        /// <summary>
        /// 情報パネルの幅
        /// </summary>
        public const int INFOPANEL_WIDTH = 240;
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
        RgistrationDate = 5,
    }

    /// <summary>
    /// コンテンツ表示種別
    /// </summary>
    public enum ContentsOpenType
    {
        Default = 0,
        OverlapTab = 1,
        AddTab = 2,
        InsertTab = 3,
        NewWindow = 4
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
        FitOnlyBigImage = 2
    }
}
