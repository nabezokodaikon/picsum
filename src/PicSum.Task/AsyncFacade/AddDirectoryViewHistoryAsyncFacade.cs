using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using System;

namespace PicSum.Task.AsyncFacade
{
    public sealed class AddDirectoryViewHistoryAsyncFacade
        : OneWayFacadeBase<SingleValueEntity<string>>
    {
        public override void Execute(SingleValueEntity<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            using (var tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                var addDirectoryViewHistory = new AddDirectoryViewHistoryAsyncLogic(this);
                if (!addDirectoryViewHistory.Execute(param.Value))
                {
                    var updateFileMaster = new UpdateFileMasterAsyncLogic(this);
                    if (!updateFileMaster.Execute(param.Value))
                    {
                        var addFileMaster = new AddFileMasterAsyncLogic(this);
                        var addDirectoryViewCounter = new AddDirectoryViewCounterAsyncLogic(this);
                        addFileMaster.Execute(param.Value);
                        addDirectoryViewHistory.Execute(param.Value);
                        addDirectoryViewCounter.Execute(param.Value);
                    }
                }
                else
                {
                    var incrementDirectoryViewCounter = new IncrementDirectoryViewCounterAsyncLogic(this);
                    if (!incrementDirectoryViewCounter.Execute(param.Value))
                    {
                        var addDirectoryViewCounter = new AddDirectoryViewCounterAsyncLogic(this);
                        addDirectoryViewCounter.Execute(param.Value);
                    }
                }

                tran.Commit();
            }
        }
    }
}
