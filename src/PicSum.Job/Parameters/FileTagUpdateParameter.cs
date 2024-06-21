using PicSum.Core.Job.AsyncJob;

namespace PicSum.Job.Parameters
{
    /// <summary>
    /// タグを更新するパラメータエンティティ
    /// </summary>
    public sealed class UpdateFileTagParameter
        : IJobParameter
    {
        public IList<string>? FilePathList { get; set; }
        public string? Tag { get; set; }
    }
}
