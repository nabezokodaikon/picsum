using System.Collections.Generic;
using System.Drawing;
using PicSum.Core.Task.Base;

namespace PicSum.Task.Paramter
{
    /// <summary>
    /// ファイルの深い情報取得パラメータエンティティ
    /// </summary>
    public class GetFileDeepInfoParameter : IEntity
    {
        public IList<string> FilePathList { get; set; }
        public Size ThumbnailSize { get; set; }
    }
}
