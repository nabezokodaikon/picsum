using SWF.Core.Base;

namespace PicSum.UIComponent.Contents.Conf
{
    /// <summary>
    /// ビューアコンテンツ設定エンティティ
    /// </summary>
    public sealed class ImageViewPageConfig
    {
        public static readonly ImageViewPageConfig INSTANCE = new();

        public ImageDisplayMode DisplayMode { get; set; }
        public ImageSizeMode SizeMode { get; set; }

        private ImageViewPageConfig()
        {

        }
    }
}
