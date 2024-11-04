namespace PicSum.UIComponent.Contents.Conf
{
    /// <summary>
    /// ファイルリストコンテンツ設定
    /// </summary>
    public sealed class FileListPageConfig
    {
        public static readonly FileListPageConfig Instance = new();

        public int ThumbnailSize { get; set; }
        public bool IsShowFileName { get; set; }
        public bool IsShowDirectory { get; set; }
        public bool IsShowImageFile { get; set; }
        public bool IsShowOtherFile { get; set; }
        public int FavoriteDirectoryCount { get; set; }

        private FileListPageConfig()
        {

        }
    }
}
