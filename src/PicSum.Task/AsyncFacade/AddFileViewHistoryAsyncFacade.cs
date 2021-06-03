using System;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    public class AddFileViewHistoryAsyncFacade
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
                AddFileViewHistoryAsyncLogic addFileViewHistory = new AddFileViewHistoryAsyncLogic(this);
                AddFileMasterAsyncLogic addFileMaster = new AddFileMasterAsyncLogic(this);

                foreach (string filePath in param)
                {
                    if (!addFileViewHistory.Execute(filePath))
                    {
                        addFileMaster.Execute(filePath);
                        addFileViewHistory.Execute(filePath);
                    }
                }

                tran.Commit();
            }
        }
    }
}
