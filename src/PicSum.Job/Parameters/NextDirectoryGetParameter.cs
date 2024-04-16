using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Entities;

namespace PicSum.Job.Paramters
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
