using PicSum.Core.Task.Base;
using System.Collections.Generic;

namespace PicSum.Task.Paramter
{
    /// <summary>
    /// ファイルの評価値を更新するパラメータエンティティ
    /// </summary>
    public sealed class UpdateFileRatingParameter
        : IEntity
    {
        public IList<string> FilePathList { get; set; }
        public int RatingValue { get; set; }
    }
}
