namespace SWF.Core.Base
{
    public sealed class ApplicationConstants
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
        FitOnlyBigImage = 2
    }
}
