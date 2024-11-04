namespace PicSum.UIComponent.Contents.Conf
{
    /// <summary>
    /// コンテンツ共通設定クラス
    /// </summary>
    public sealed class CommonConfig
    {
        public static readonly CommonConfig Instance = new();

        public string ExportDirectoryPath { get; set; }

        private CommonConfig()
        {

        }
    }
}
