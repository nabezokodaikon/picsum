using PicSum.Core.Task.AsyncTaskV2;

namespace PicSum.Task.Paramter
{
    /// <summary>
    /// スタートアップパラメータエンティティ
    /// </summary>
    public sealed class StartupPrameter
        : ITaskParameter
    {
        public string FileInfoDBFilePath { get; set; }
        public string ThumbnailDBFilePath { get; set; }
    }
}
