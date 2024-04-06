using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.AsyncLogic;
using System;

namespace PicSum.Task.AsyncTask
{
    public sealed class DeleteBookmarkAsyncTask
        : AbstractAsyncTask<ListParameter<string>, EmptyResult>
    {
        protected override void Execute(ListParameter<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            using (var tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                var deleteLogic = new DeleteBookmarkAsyncLogic(this);

                foreach (var filePath in param)
                {
                    deleteLogic.Execute(filePath);
                }

                tran.Commit();
            }
        }
    }
}
