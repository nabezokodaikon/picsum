using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    /// <summary>
    /// 次のコンテンツのパラメータを取得するエンティティ
    /// </summary>
    public sealed class NextDirectoryGetParameter
        : IJobParameter
    {
        public string CurrentParameter { get; set; } = string.Empty;
        public bool IsNext { get; set; }
    }
}
