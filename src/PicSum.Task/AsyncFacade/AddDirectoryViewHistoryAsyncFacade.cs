using System;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    public class AddDirectoryViewHistoryAsyncFacade
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
                var addDirectoryViewHistory = new AddDirectoryViewHistoryAsyncLogic(this);
                var addFileMaster = new AddFileMasterAsyncLogic(this);
                var addDirectoryViewCounter = new AddDirectoryViewCounterAsyncLogic(this);
                var incrementDirectoryViewCounter = new IncrementDirectoryViewCounterAsyncLogic(this);

                if (!addDirectoryViewHistory.Execute(param.Value))
                {
                    addFileMaster.Execute(param.Value);
                    addDirectoryViewHistory.Execute(param.Value);
                    addDirectoryViewCounter.Execute(param.Value) ;
                }

                if (!incrementDirectoryViewCounter.Execute(param.Value))
                {
                    addDirectoryViewCounter.Execute(param.Value);
                }

                tran.Commit();
            }
        }
    }
}
