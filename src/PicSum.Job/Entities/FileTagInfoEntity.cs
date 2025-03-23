namespace PicSum.Job.Entities
{
    /// <summary>
    /// ファイルタグ情報エンティティ
    /// </summary>
    public struct FileTagInfoEntity
        : IEquatable<FileTagInfoEntity>
    {
        public static readonly FileTagInfoEntity EMPTY = new()
        {
            Tag = string.Empty,
            IsAll = false,
        };

        public string Tag { get; set; }
        public bool IsAll { get; set; }

        public readonly bool Equals(FileTagInfoEntity other)
        {
            if (this.Tag != other.Tag) { return false; }
            if (this.IsAll != other.IsAll) { return false; }

            return true;
        }
    }
}
