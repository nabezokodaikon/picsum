using SWF.Core.ImageAccessor;

namespace PicSum.Job.Entities
{
    /// <summary>
    /// 画像ファイルエンティティ
    /// </summary>
    public struct ImageFileEntity
        : IEquatable<ImageFileEntity>
    {
        public string FilePath { get; internal set; }
        public CvImage? Image { get; internal set; }
        public bool IsEmpty { get; internal set; }
        public bool IsError { get; internal set; }

        public readonly bool Equals(ImageFileEntity other)
        {
            if (this.FilePath != other.FilePath) { return false; }
            if (this.Image != other.Image) { return false; }
            if (this.IsEmpty != other.IsEmpty) { return false; }
            if (this.IsError != other.IsError) { return false; }

            return true;
        }
    }
}
