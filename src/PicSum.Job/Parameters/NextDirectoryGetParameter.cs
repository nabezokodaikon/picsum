using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    /// <summary>
    /// 次のコンテンツのパラメータを取得するエンティティ
    /// </summary>
    public struct NextDirectoryGetParameter
        : IJobParameter
    {
        public string CurrentParameter { get; set; }
        public bool IsNext { get; set; }
    }
}
