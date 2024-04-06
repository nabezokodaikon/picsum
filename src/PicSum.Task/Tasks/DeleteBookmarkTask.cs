using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.Logics;
using System;

namespace PicSum.Task.Tasks
{
    public sealed class DeleteBookmarkTask
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
                var deleteLogic = new DeleteBookmarkLogic(this);

                foreach (var filePath in param)
                {
                    deleteLogic.Execute(filePath);
                }

                tran.Commit();
            }
        }
    }
}
