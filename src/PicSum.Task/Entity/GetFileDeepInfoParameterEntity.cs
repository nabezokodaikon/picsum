using System.Collections.Generic;
using System.Drawing;
using PicSum.Core.Task.Base;

namespace PicSum.Task.Entity
{
    /// <summary>
    /// ファイルの深い情報取得パラメータエンティティ
    /// </summary>
    public class GetFileDeepInfoParameterEntity : IEntity
    {
        public IList<string> FilePathList { get; set; }
        public Size ThumbnailSize { get; set; }
    }
}
