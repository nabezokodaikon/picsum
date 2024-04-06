using PicSum.Core.Task.AsyncTaskV2;
using System.Collections.Generic;

namespace PicSum.Task.Paramter
{
    /// <summary>
    /// タグを更新するパラメータエンティティ
    /// </summary>
    public sealed class UpdateFileTagParameter
        : ITaskParameter
    {
        public IList<string> FilePathList { get; set; }
        public string Tag { get; set; }
    }
}
