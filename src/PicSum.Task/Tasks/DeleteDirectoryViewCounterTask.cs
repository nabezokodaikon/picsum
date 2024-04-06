using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.Logics;
using System;
using System.Runtime.Versioning;

namespace PicSum.Task.Tasks
{
    [SupportedOSPlatform("windows")]
    public sealed class DeleteDirectoryViewCounterTask
        : AbstractAsyncTask<ListParameter<string>>
    {
        protected override void Execute(ListParameter<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            using (var tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                var logic = new DeleteDirectoryViewCounterLogic(this);

                foreach (var dir in param)
                {
                    logic.Execute(dir);
                }

                tran.Commit();
            }
        }
    }
}
