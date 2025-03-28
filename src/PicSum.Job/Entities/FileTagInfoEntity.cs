namespace PicSum.Job.Entities
{
    /// <summary>
    /// ファイルタグ情報エンティティ
    /// </summary>
    public sealed class FileTagInfoEntity
    {
        public static readonly FileTagInfoEntity EMPTY = new()
        {
            Tag = string.Empty,
            IsAll = false,
        };

        public string Tag { get; set; } = string.Empty;
        public bool IsAll { get; set; }
    }
}
