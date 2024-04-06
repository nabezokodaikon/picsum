using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System;

namespace PicSum.Task.Logics
{
    internal sealed class UpdateFileMastercLogic
        : AbstractAsyncLogic
    {
        public UpdateFileMastercLogic(IAsyncTask task)
            : base(task)
        {

        }

        /// <summary>
        /// 処理を実行します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public bool Execute(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var sql = new UpdateFileSql(filePath);
            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
