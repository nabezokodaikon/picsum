namespace PicSum.Job.Entities
{
    /// <summary>
    /// サムネイルバッファエンティティ
    /// </summary>
    public struct ThumbnailCacheEntity
        : IEquatable<ThumbnailCacheEntity>
    {
        public static readonly ThumbnailCacheEntity EMPTY = new()
        {
            FilePath = string.Empty,
            ThumbnailWidth = 0,
            ThumbnailHeight = 0,
            SourceWidth = 0,
            SourceHeight = 0,
            FileUpdatedate = DateTime.MinValue,
            ThumbnailBuffer = null,
        };

        public string FilePath { get; internal set; }
        public int ThumbnailWidth { get; internal set; }
        public int ThumbnailHeight { get; internal set; }
        public int SourceWidth { get; internal set; }
        public int SourceHeight { get; internal set; }
        public DateTime FileUpdatedate { get; internal set; }
        public byte[]? ThumbnailBuffer { get; internal set; }

        public readonly bool Equals(ThumbnailCacheEntity other)
        {
            if (this.FilePath != other.FilePath) { return false; }
            if (this.ThumbnailWidth != other.ThumbnailWidth) { return false; }
            if (this.ThumbnailHeight != other.ThumbnailHeight) { return false; }
            if (this.SourceWidth != other.SourceWidth) { return false; }
            if (this.SourceHeight != other.SourceHeight) { return false; }
            if (this.FileUpdatedate != other.FileUpdatedate) { return false; }
            if (this.ThumbnailBuffer != other.ThumbnailBuffer) { return false; }

            return true;
        }

        public override readonly bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(ThumbnailCacheEntity))
            {
                return false;
            }

            return this.Equals((ThumbnailCacheEntity)obj);
        }

        public static bool operator ==(ThumbnailCacheEntity left, ThumbnailCacheEntity right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ThumbnailCacheEntity left, ThumbnailCacheEntity right)
        {
            return !(left == right);
        }

        public override readonly int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(this.FilePath);
            hash.Add(this.ThumbnailWidth);
            hash.Add(this.ThumbnailHeight);
            hash.Add(this.SourceWidth);
            hash.Add(this.SourceHeight);
            hash.Add(this.FileUpdatedate);
            hash.Add(this.ThumbnailBuffer);
            return hash.ToHashCode();
        }
    }
}
