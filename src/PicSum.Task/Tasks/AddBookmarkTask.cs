using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.Logics;
using PicSum.Tasks.Logics;
using System;

namespace PicSum.Task.Tasks
{
    public sealed class AddBookmarkTask
        : AbstractAsyncTask<ValueParameter<string>>
    {
        protected override void Execute(ValueParameter<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var registrationDate = DateTime.Now;

            using (var tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                var deleteLogic = new DeleteBookmarkLogic(this);
                var addLogic = new AddBookmarkLogic(this);

                if (!deleteLogic.Execute(param.Value))
                {
                    if (!addLogic.Execute(param.Value, registrationDate))
                    {
                        var updateFileMaster = new UpdateFileMastercLogic(this);
                        if (!updateFileMaster.Execute(param.Value))
                        {
                            var addFileMasterLogic = new AddFileMasterLogic(this);
                            addFileMasterLogic.Execute(param.Value);
                        }

                        addLogic.Execute(param.Value, registrationDate);
                    }
                }
                else
                {
                    addLogic.Execute(param.Value, registrationDate);
                }

                tran.Commit();
            }
        }
    }
}
