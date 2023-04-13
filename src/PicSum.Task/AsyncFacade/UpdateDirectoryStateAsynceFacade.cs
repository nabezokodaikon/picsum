using System;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// フォルダ状態更新ファサード
    /// </summary>
    public class UpdateDirectoryStateAsynceFacade : OneWayFacadeBase<DirectoryStateEntity>
    {
        public override void Execute(DirectoryStateEntity param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            using (Transaction tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                UpdateDirectoryStateAsyncLogic updateDirectoryState = new UpdateDirectoryStateAsyncLogic(this);
                if (!updateDirectoryState.Execute(param))
                {
                    AddDirectoryStateAsyncLogic addDirectoryStateLogic = new AddDirectoryStateAsyncLogic(this);
                    if (!addDirectoryStateLogic.Execute(param))
                    {
                        AddFileMasterAsyncLogic addFileMasterLogic = new AddFileMasterAsyncLogic(this);
                        addFileMasterLogic.Execute(param.DirectoryPath);
                        addDirectoryStateLogic.Execute(param);
                    }
                }

                tran.Commit();
            }
        }
    }
}
