using PicSum.Core.Task.AsyncTaskV2;
using System.Collections.Generic;
using System.Drawing;

namespace PicSum.Task.Paramter
{
    /// <summary>
    /// ファイルの深い情報取得パラメータエンティティ
    /// </summary>
    public sealed class GetFileDeepInfoParameter
        : ITaskParameter
    {
        public IList<string> FilePathList { get; set; }
        public Size ThumbnailSize { get; set; }
    }
}
