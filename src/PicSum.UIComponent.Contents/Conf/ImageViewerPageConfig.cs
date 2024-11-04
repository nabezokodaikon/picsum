using SWF.Core.Base;

namespace PicSum.UIComponent.Contents.Conf
{
    /// <summary>
    /// ビューアコンテンツ設定エンティティ
    /// </summary>
    public sealed class ImageViewerPageConfig
    {
        public static readonly ImageViewerPageConfig Instance = new();

        public ImageDisplayMode ImageDisplayMode { get; set; }
        public ImageSizeMode ImageSizeMode { get; set; }

        private ImageViewerPageConfig()
        {

        }
    }
}
