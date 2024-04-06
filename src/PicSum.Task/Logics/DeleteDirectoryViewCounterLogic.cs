using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System;

namespace PicSum.Task.Logics
{
    /// <summary>
    /// フォルダの表示回数を削除します。
    /// </summary>
    internal class DeleteDirectoryViewCounterLogic
        : AbstractAsyncLogic
    {
        public DeleteDirectoryViewCounterLogic(AbstractAsyncTask task)
            : base(task)
        {

        }

        /// <summary>
        /// 処理を実行します。
        /// </summary>
        /// <param name="directoryPath">フォルダパス</param>
        public void Execute(string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            var sql = new DeletionDirectoryViewCounterByFileSql(directoryPath);
            DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
