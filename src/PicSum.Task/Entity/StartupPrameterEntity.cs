using PicSum.Core.Task.Base;

namespace PicSum.Task.Entity
{
    /// <summary>
    /// スタートアップパラメータエンティティ
    /// </summary>
    public class StartupPrameterEntity : IEntity
    {
        public bool IsWriteLog { get; set; }
        public string FileInfoDBFilePath { get; set; }
        public string ThumbnailDBFilePath { get; set; }
        public string SqlDirectoryPath { get; set; }
    }
}
