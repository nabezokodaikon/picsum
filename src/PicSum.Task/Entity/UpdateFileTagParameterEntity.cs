using System.Collections.Generic;
using PicSum.Core.Task.Base;

namespace PicSum.Task.Entity
{
    /// <summary>
    /// タグを更新するパラメータエンティティ
    /// </summary>
    public class UpdateFileTagParameterEntity : IEntity
    {
        public IList<string> FilePathList { get; set; }
        public string Tag { get; set; }
    }
}
