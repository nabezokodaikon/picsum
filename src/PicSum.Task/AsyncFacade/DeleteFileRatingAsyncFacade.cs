using System;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// ファイルの評価値を削除します。
    /// </summary>
    public class DeleteFileRatingAsyncFacade
        : OneWayFacadeBase<ListEntity<string>>
    {
        public override void Execute(ListEntity<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            using (Transaction tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                DeleteFileRatingAsyncLogic logic = new DeleteFileRatingAsyncLogic(this);

                foreach (string filePath in param)
                {
                    logic.Execute(filePath);
                }

                tran.Commit();
            }
        }
    }
}
