using PicSum.Core.Task.Base;

namespace PicSum.Task.Paramter
{
    /// <summary>
    /// スタートアップパラメータエンティティ
    /// </summary>
    public class StartupPrameter : IEntity
    {
        public string FileInfoDBFilePath { get; set; }
        public string ThumbnailDBFilePath { get; set; }
    }
}
