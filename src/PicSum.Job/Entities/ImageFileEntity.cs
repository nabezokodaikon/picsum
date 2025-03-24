using SWF.Core.ImageAccessor;
using System.Runtime.Versioning;

namespace PicSum.Job.Entities
{
    /// <summary>
    /// 画像ファイルエンティティ
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ImageFileEntity
    {
        public static readonly ImageFileEntity EMPTY = new()
        {
            FilePath = string.Empty,
            Image = CvImage.EMPTY,
            IsEmpty = false,
            IsError = false,
        };

        public string FilePath { get; internal set; } = string.Empty;
        public CvImage Image { get; internal set; } = CvImage.EMPTY;
        public bool IsEmpty { get; internal set; }
        public bool IsError { get; internal set; }
    }
}
