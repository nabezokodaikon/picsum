using PicSum.Core.Job.AsyncJob;

namespace PicSum.Job.Parameters
{
    /// <summary>
    /// スタートアップパラメータエンティティ
    /// </summary>
    public sealed class StartupPrameter
        : IJobParameter
    {
        public string? FileInfoDBFilePath { get; set; }
        public string? ThumbnailDBFilePath { get; set; }
    }
}
