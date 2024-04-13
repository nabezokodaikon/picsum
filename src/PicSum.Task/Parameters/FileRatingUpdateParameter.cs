using PicSum.Core.Task.AsyncTaskV2;
using System.Collections.Generic;

namespace PicSum.Task.Paramters
{
    /// <summary>
    /// ファイルの評価値を更新するパラメータエンティティ
    /// </summary>
    public sealed class FileRatingUpdateParameter
        : ITaskParameter
    {
        public IList<string> FilePathList { get; set; }
        public int RatingValue { get; set; }
    }
}
