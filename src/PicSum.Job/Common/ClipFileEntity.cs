namespace PicSum.Job.Common
{
    public struct ClipFileEntity
    {
        public string FilePath { get; private set; }
        public DateTime Timestamp { get; private set; }

        public ClipFileEntity(string filePath)
        {
            ArgumentNullException.ThrowIfNull(filePath, nameof(filePath));

            this.FilePath = filePath;
            this.Timestamp = DateTime.Now;
        }

        public readonly bool Equals(ClipFileEntity other)
        {
            if (other.FilePath != this.FilePath)
            {
                return false;
            }

            return true;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(this.FilePath);
        }

        public override readonly bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(ClipFileEntity))
            {
                return false;
            }

            return this.Equals((ClipFileEntity)obj);
        }
    }
}
