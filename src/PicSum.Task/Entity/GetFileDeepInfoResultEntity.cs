using System.Collections.Generic;
using PicSum.Core.Task.Base;

namespace PicSum.Task.Entity
{
    /// <summary>
    /// ファイルの深い情報取得結果エンティティ
    /// </summary>
    public class GetFileDeepInfoResultEntity : IEntity
    {
        public IList<string> FilePathList;
        public FileDeepInfoEntity FileInfo;
        public ListEntity<FileTagInfoEntity> TagInfoList;
    }
}
