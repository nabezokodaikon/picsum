using PicSum.Core.Task.Base;
using System.Collections.Generic;
using System.Drawing;

namespace PicSum.Task.Paramter
{
    /// <summary>
    /// ファイルの深い情報取得パラメータエンティティ
    /// </summary>
    public sealed class GetFileDeepInfoParameter
        : IEntity
    {
        public IList<string> FilePathList { get; set; }
        public Size ThumbnailSize { get; set; }
    }
}
