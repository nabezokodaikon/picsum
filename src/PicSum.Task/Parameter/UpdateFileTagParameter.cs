using System.Collections.Generic;
using PicSum.Core.Task.Base;

namespace PicSum.Task.Paramter
{
    /// <summary>
    /// タグを更新するパラメータエンティティ
    /// </summary>
    public class UpdateFileTagParameter : IEntity
    {
        public IList<string> FilePathList { get; set; }
        public string Tag { get; set; }
    }
}
