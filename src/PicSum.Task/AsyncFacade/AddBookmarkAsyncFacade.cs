using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using System;

namespace PicSum.Task.AsyncFacade
{
    public sealed class AddBookmarkAsyncFacade
        : OneWayFacadeBase<SingleValueEntity<string>>
    {
        public override void Execute(SingleValueEntity<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var registrationDate = DateTime.Now;

            using (var tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                var deleteLogic = new DeleteBookmarkAsyncLogic(this);
                var addLogic = new AddBookmarkAsyncLogic(this);

                if (!deleteLogic.Execute(param.Value))
                {
                    if (!addLogic.Execute(param.Value, registrationDate))
                    {
                        var updateFileMaster = new UpdateFileMasterAsyncLogic(this);
                        if (!updateFileMaster.Execute(param.Value))
                        {
                            var addFileMasterLogic = new AddFileMasterAsyncLogic(this);
                            addFileMasterLogic.Execute(param.Value);
                            addLogic.Execute(param.Value, registrationDate);
                        }
                    }
                }

                tran.Commit();
            }
        }
    }
}
