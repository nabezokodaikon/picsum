using System.Collections.Generic;
using PicSum.Core.Task.Base;

namespace PicSum.Task.Paramter
{
    /// <summary>
    /// ファイルの評価値を更新するパラメータエンティティ
    /// </summary>
    public class UpdateFileRatingParameter : IEntity
    {
        public IList<string> FilePathList { get; set; }
        public int RatingValue { get; set; }
    }
}
