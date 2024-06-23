using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Entities;

namespace PicSum.Job.Parameters
{
    /// <summary>
    /// 次のコンテンツのパラメータを取得するエンティティ
    /// </summary>
    public struct NextDirectoryGetParameter<T>
        : IJobParameter
    {
        public ValueEntity<T>? CurrentParameter { get; set; }
        public bool IsNext { get; set; }
    }
}
