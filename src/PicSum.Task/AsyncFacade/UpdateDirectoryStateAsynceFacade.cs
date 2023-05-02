using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using System;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// フォルダ状態更新ファサード
    /// </summary>
    public sealed class UpdateDirectoryStateAsynceFacade
        : OneWayFacadeBase<DirectoryStateEntity>
    {
        public override void Execute(DirectoryStateEntity param)
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
