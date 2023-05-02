using PicSum.Core.Task.Base;

namespace PicSum.Task.Paramter
{
    /// <summary>
    /// スタートアップパラメータエンティティ
    /// </summary>
    public sealed class StartupPrameter
        : IEntity
    {
        public string FileInfoDBFilePath { get; set; }
        public string ThumbnailDBFilePath { get; set; }
    }
}
