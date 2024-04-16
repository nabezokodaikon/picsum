using PicSum.Core.Job.AsyncJob;
using System.Collections.Generic;

namespace PicSum.Job.Paramters
{
    /// <summary>
    /// ファイルの評価値を更新するパラメータエンティティ
    /// </summary>
    public sealed class FileRatingUpdateParameter
        : IJobParameter
    {
        public IList<string>? FilePathList { get; set; }
        public int RatingValue { get; set; }
    }
}
