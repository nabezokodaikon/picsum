using System;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// フォルダの表示履歴を追加します。
    /// </summary>
    internal class AddDirectoryViewHistoryAsyncLogic : AsyncLogicBase
    {
        public AddDirectoryViewHistoryAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        /// <summary>
        /// 処理を実行します。
        /// </summary>
        /// <param name="directoryPath">フォルダパス</param>
        /// <returns>表示履歴が追加されたらTrue、追加されなければFalseを返します。</returns>
        public bool Execute(string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException("directoryPath");
            }

            CreationDirectoryViewHistorySql sql = new CreationDirectoryViewHistorySql(directoryPath);

            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
