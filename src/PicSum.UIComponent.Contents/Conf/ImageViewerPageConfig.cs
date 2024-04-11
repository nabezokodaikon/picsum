using PicSum.Core.Base.Conf;

namespace PicSum.UIComponent.Contents.Conf
{
    /// <summary>
    /// ビューアコンテンツ設定エンティティ
    /// </summary>
    public sealed class ImageViewerPageConfig
    {
        public static ImageDisplayMode ImageDisplayMode { get; set; }
        public static ImageSizeMode ImageSizeMode { get; set; }
    }
}
