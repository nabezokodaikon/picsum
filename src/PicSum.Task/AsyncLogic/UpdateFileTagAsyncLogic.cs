using System;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// タグを更新します。
    /// </summary>
    /// <remarks>タグの存在確認として使用します。</remarks>
    internal class UpdateFileTagAsyncLogic : AsyncLogicBase
    {
        public UpdateFileTagAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public bool Execute(string filePath, string tag)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            if (tag == null)
            {
                throw new ArgumentNullException("tag");
            }

            UpdateTagSql sql = new UpdateTagSql(filePath, tag);
            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
