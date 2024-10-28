using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    /// <summary>
    /// タグを更新するパラメータエンティティ
    /// </summary>
    public sealed class FileTagUpdateParameter
        : IJobParameter
    {
        public IList<string>? FilePathList { get; set; }
        public string? Tag { get; set; }
    }
}
