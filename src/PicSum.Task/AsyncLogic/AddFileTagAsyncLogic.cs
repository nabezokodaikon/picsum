using System;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// タグ追加ロジック
    /// </summary>
    internal class AddFileTagAsyncLogic:AbstractAsyncLogic
    {
        public AddFileTagAsyncLogic(AbstractAsyncFacade facade) : base(facade) { }

        public bool Execute(string filePath, string tag, DateTime registrationDate)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            if (tag == null)
            {
                throw new ArgumentNullException("tag");
            }

            CreationTagSql sql = new CreationTagSql(filePath, tag, registrationDate);
            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
