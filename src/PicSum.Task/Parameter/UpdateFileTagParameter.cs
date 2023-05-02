using PicSum.Core.Task.Base;
using System.Collections.Generic;

namespace PicSum.Task.Paramter
{
    /// <summary>
    /// タグを更新するパラメータエンティティ
    /// </summary>
    public sealed class UpdateFileTagParameter
        : IEntity
    {
        public IList<string> FilePathList { get; set; }
        public string Tag { get; set; }
    }
}
