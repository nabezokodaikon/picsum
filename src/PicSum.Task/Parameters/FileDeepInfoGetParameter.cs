using PicSum.Core.Task.AsyncTaskV2;
using System.Collections.Generic;
using System.Drawing;

namespace PicSum.Task.Paramters
{
    /// <summary>
    /// ファイルの深い情報取得パラメータエンティティ
    /// </summary>
    public sealed class FileDeepInfoGetParameter
        : ITaskParameter
    {
        public IList<string> FilePathList { get; set; }
        public Size ThumbnailSize { get; set; }
    }
}
