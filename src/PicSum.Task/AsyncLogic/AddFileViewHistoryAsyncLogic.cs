using System;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// ファイルの表示履歴を追加します。
    /// </summary>
    internal class AddFileViewHistoryAsyncLogic : AsyncLogicBase
    {
        public AddFileViewHistoryAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        /// <summary>
        /// 処理を実行します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>表示履歴が追加されたらTrue、追加されなければFalseを返します。</returns>
        public bool Execute(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            CreationFileViewHistorySql sql = new CreationFileViewHistorySql(filePath);

            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
