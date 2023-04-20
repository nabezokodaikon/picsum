using PicSum.Core.Task.Base;

namespace PicSum.Task.Entity
{
    /// <summary>
    /// スタートアップパラメータエンティティ
    /// </summary>
    public class StartupPrameterEntity : IEntity
    {
        public string FileInfoDBFilePath { get; set; }
        public string ThumbnailDBFilePath { get; set; }
    }
}
