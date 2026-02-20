using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using System.Drawing;

namespace PicSum.Job.Entities
{
    /// <summary>
    /// ファイルの浅い情報エンティティ
    /// </summary>

    public sealed class FileShallowInfoEntity
    {
        public static readonly FileShallowInfoEntity EMPTY = new()
        {
            FilePath = string.Empty,
            FileName = string.Empty,
            CreateDate = DateTimeExtensions.EMPTY,
            UpdateDate = DateTimeExtensions.EMPTY,
            TakenDate = DateTimeExtensions.EMPTY,
            RgistrationDate = DateTimeExtensions.EMPTY,
            ExtraLargeIcon = null,
            SmallIcon = null,
            JumboIcon = null,
            ThumbnailImage = null,
            ThumbnailWidth = 0,
            ThumbnailHeight = 0,
            SourceWidth = 0,
            SourceHeight = 0,
            IsFile = false,
            IsImageFile = false,
        };

        public bool IsEmpty
        {
            get
            {
                return this == EMPTY;
            }
        }

        public string FilePath { get; internal set; } = string.Empty;
        public string FileName { get; internal set; } = string.Empty;
        public DateTime CreateDate { get; internal set; } = DateTimeExtensions.EMPTY;
        public DateTime UpdateDate { get; internal set; } = DateTimeExtensions.EMPTY;
        public DateTime TakenDate { get; internal set; } = DateTimeExtensions.EMPTY;
        public DateTime RgistrationDate { get; internal set; } = DateTimeExtensions.EMPTY;
        public Image? ExtraLargeIcon { get; internal set; } = null;
        public IconImage? SmallIcon { get; internal set; } = null;
        public IconImage? JumboIcon { get; internal set; } = null;
        public OpenCVImage? ThumbnailImage { get; internal set; } = null;
        public int ThumbnailWidth { get; internal set; } = 0;
        public int ThumbnailHeight { get; internal set; } = 0;
        public int SourceWidth { get; internal set; } = 0;
        public int SourceHeight { get; internal set; } = 0;
        public bool IsFile { get; internal set; } = false;
        public bool IsImageFile { get; internal set; } = false;
    }
}
