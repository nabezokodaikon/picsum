using System;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// ファイルのタグを削除します。
    /// </summary>
    public class DeleteFileTagAsyncFacade
        : OneWayFacadeBase<UpdateFileTagParameter>
    {
        public override void Execute(UpdateFileTagParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            using (Transaction tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                DeleteFileTagAsyncLogic logic = new DeleteFileTagAsyncLogic(this);
                
                foreach (string filePath in param.FilePathList)
                {
                    logic.Execute(filePath, param.Tag);
                }

                tran.Commit();
            }
        }
    }
}
