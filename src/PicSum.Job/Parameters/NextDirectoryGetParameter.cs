using PicSum.Job.Entities;
using SWF.Core.Job;

namespace PicSum.Job.Parameters
{
    /// <summary>
    /// 次のコンテンツのパラメータを取得するエンティティ
    /// </summary>
    public sealed class NextDirectoryGetParameter<T>
        : IJobParameter
    {
        public ValueEntity<T>? CurrentParameter { get; set; }
        public bool IsNext { get; set; }
    }
}
