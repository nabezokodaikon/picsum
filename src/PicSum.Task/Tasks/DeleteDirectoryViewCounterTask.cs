using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.Logics;
using PicSum.Task.Entities;
using System;

namespace PicSum.Task.Tasks
{
    public sealed class DeleteDirectoryViewCounterTask
        : OneWayTaskBase<ListEntity<string>>
    {
        public override void Execute(ListEntity<string> param)
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
