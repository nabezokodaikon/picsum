using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.Logics;
using System;

namespace PicSum.Task.Tasks
{
    public sealed class AddDirectoryViewHistoryTask
        : AbstractAsyncTask<ValueParameter<string>>
    {
        protected override void Execute(ValueParameter<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            using (var tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                var addDirectoryViewHistory = new AddDirectoryViewHistoryLogic(this);
                if (!addDirectoryViewHistory.Execute(param.Value))
                {
                    var updateFileMaster = new UpdateFileMastercLogic(this);
                    if (!updateFileMaster.Execute(param.Value))
                    {
                        var addFileMaster = new AddFileMasterLogic(this);
                        addFileMaster.Execute(param.Value);
                    }

                    var addDirectoryViewCounter = new AddDirectoryViewCounterLogic(this);
                    addDirectoryViewHistory.Execute(param.Value);
                    addDirectoryViewCounter.Execute(param.Value);
                }
                else
                {
                    var incrementDirectoryViewCounter = new IncrementDirectoryViewCounterLogic(this);
                    if (!incrementDirectoryViewCounter.Execute(param.Value))
                    {
                        var addDirectoryViewCounter = new AddDirectoryViewCounterLogic(this);
                        addDirectoryViewCounter.Execute(param.Value);
                    }
                }

                tran.Commit();
            }
        }
    }
}
