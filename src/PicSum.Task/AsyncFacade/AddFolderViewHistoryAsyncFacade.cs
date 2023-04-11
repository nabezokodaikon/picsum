using System;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    public class AddFolderViewHistoryAsyncFacade
        : OneWayFacadeBase<SingleValueEntity<string>>
    {
        public override void Execute(SingleValueEntity<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            using (Transaction tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                var addFolderViewHistory = new AddFolderViewHistoryAsyncLogic(this);
                var addFileMaster = new AddFileMasterAsyncLogic(this);
                var addFolderViewCounter = new AddFolderViewCounterAsyncLogic(this);
                var incrementFolderViewCounter = new IncrementFolderViewCounterAsyncLogic(this);

                if (!addFolderViewHistory.Execute(param.Value))
                {
                    addFileMaster.Execute(param.Value);
                    addFolderViewHistory.Execute(param.Value);
                    addFolderViewCounter.Execute(param.Value) ;
                }

                if (!incrementFolderViewCounter.Execute(param.Value))
                {
                    addFolderViewCounter.Execute(param.Value);
                }

                tran.Commit();
            }
        }
    }
}
