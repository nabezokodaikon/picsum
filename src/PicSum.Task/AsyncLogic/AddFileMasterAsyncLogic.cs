using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// ファイルマスタに登録します。
    /// </summary>
    internal sealed class AddFileMasterAsyncLogic
        : AbstractAsyncLogic
    {
        public AddFileMasterAsyncLogic(AbstractAsyncFacade facade)
            : base(facade)
        {

        }

        /// <summary>
        /// 処理を実行します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public void Execute(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var sql = new CreationFileSql(filePath);
            DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
