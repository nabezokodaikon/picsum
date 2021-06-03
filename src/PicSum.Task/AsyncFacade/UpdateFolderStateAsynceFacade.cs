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
    public class UpdateFolderStateAsynceFacade : OneWayFacadeBase<FolderStateEntity>
    {
        public override void Execute(FolderStateEntity param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            using (Transaction tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                UpdateFolderStateAsyncLogic updateFolderState = new UpdateFolderStateAsyncLogic(this);
                if (!updateFolderState.Execute(param))
                {
                    AddFolderStateAsyncLogic addFolderStateLogic = new AddFolderStateAsyncLogic(this);
                    if (!addFolderStateLogic.Execute(param))
                    {
                        AddFileMasterAsyncLogic addFileMasterLogic = new AddFileMasterAsyncLogic(this);
                        addFileMasterLogic.Execute(param.FolderPath);
                        addFolderStateLogic.Execute(param);
                    }
                }

                tran.Commit();
            }
        }
    }
}
