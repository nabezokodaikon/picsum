using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    /// <summary>
    /// スタートアップパラメータエンティティ
    /// </summary>
    public struct StartupPrameter
        : IJobParameter
    {
        public string? FileInfoDBFilePath { get; set; }
        public string? ThumbnailDBFilePath { get; set; }
    }
}
