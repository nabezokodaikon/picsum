using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Parameter;
using System;

namespace PicSum.Task.AsyncTask
{
    /// <summary>
    /// フォルダ状態更新タスク
    /// </summary>
    public sealed class UpdateDirectoryStateAsynceTask
        : AbstractAsyncTask<DirectoryStateParameter, EmptyResult>
    {
        protected override void Execute(DirectoryStateParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            using (var tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                var updateDirectoryState = new UpdateDirectoryStateAsyncLogic(this);
                if (!updateDirectoryState.Execute(param))
                {
                    var addDirectoryStateLogic = new AddDirectoryStateAsyncLogic(this);
                    if (!addDirectoryStateLogic.Execute(param))
                    {
                        var addFileMasterLogic = new AddFileMasterAsyncLogic(this);
                        addFileMasterLogic.Execute(param.DirectoryPath);
                        addDirectoryStateLogic.Execute(param);
                    }
                }

                tran.Commit();
            }
        }
    }
}
