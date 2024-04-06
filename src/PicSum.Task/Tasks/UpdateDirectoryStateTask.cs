using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.Logics;
using PicSum.Task.Parameters;
using System;

namespace PicSum.Task.Tasks
{
    /// <summary>
    /// フォルダ状態更新タスク
    /// </summary>
    public sealed class UpdateDirectoryStateTask
        : AbstractAsyncTask<DirectoryStateParameter>
    {
        protected override void Execute(DirectoryStateParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            using (var tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                var updateDirectoryState = new UpdateDirectoryStateLogic(this);
                if (!updateDirectoryState.Execute(param))
                {
                    var addDirectoryStateLogic = new AddDirectoryStateLogic(this);
                    if (!addDirectoryStateLogic.Execute(param))
                    {
                        var addFileMasterLogic = new AddFileMasterLogic(this);
                        addFileMasterLogic.Execute(param.DirectoryPath);
                        addDirectoryStateLogic.Execute(param);
                    }
                }

                tran.Commit();
            }
        }
    }
}
