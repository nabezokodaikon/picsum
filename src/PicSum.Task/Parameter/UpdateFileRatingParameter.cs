using PicSum.Core.Task.AsyncTaskV2;
using System.Collections.Generic;

namespace PicSum.Task.Paramter
{
    /// <summary>
    /// ファイルの評価値を更新するパラメータエンティティ
    /// </summary>
    public sealed class UpdateFileRatingParameter
        : ITaskParameter
    {
        public IList<string> FilePathList { get; set; }
        public int RatingValue { get; set; }
    }
}
