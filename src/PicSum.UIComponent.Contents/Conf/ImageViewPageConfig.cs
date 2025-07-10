using SWF.Core.Base;

namespace PicSum.UIComponent.Contents.Conf
{
    /// <summary>
    /// ビューアコンテンツ設定エンティティ
    /// </summary>
    public sealed class ImageViewPageConfig
    {
        public static readonly ImageViewPageConfig INSTANCE = new();

        public ImageDisplayMode ImageDisplayMode { get; set; }
        public ImageSizeMode ImageSizeMode { get; set; }

        private ImageViewPageConfig()
        {

        }
    }
}
