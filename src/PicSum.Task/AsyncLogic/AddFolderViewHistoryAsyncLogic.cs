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
    internal class AddFolderViewHistoryAsyncLogic : AsyncLogicBase
    {
        public AddFolderViewHistoryAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        /// <summary>
        /// 処理を実行します。
        /// </summary>
        /// <param name="folderPath">フォルダパス</param>
        /// <returns>表示履歴が追加されたらTrue、追加されなければFalseを返します。</returns>
        public bool Execute(string folderPath)
        {
            if (folderPath == null)
            {
                throw new ArgumentNullException("folderPath");
            }

            CreationFolderViewHistorySql sql = new CreationFolderViewHistorySql(folderPath);

            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
