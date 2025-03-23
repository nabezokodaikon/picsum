using SWF.Core.ImageAccessor;
using System.Runtime.Versioning;

namespace PicSum.Job.Entities
{
    /// <summary>
    /// 画像ファイルエンティティ
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public struct ImageFileEntity
        : IEquatable<ImageFileEntity>
    {
        public string FilePath { get; internal set; }
        public CvImage Image { get; internal set; }
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

        public override readonly bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(ImageFileEntity))
            {
                return false;
            }

            return this.Equals((ImageFileEntity)obj);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(this.FilePath, this.Image, this.IsEmpty, this.IsError);
        }

        public static bool operator ==(ImageFileEntity left, ImageFileEntity right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ImageFileEntity left, ImageFileEntity right)
        {
            return !(left == right);
        }
    }
}
