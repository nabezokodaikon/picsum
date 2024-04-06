using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Paramter;
using System;

namespace PicSum.Task.AsyncTask
{
    /// <summary>
    /// ファイルのタグを削除します。
    /// </summary>
    public class DeleteFileTagAsyncTask
        : AbstractAsyncTask<UpdateFileTagParameter, EmptyResult>
    {
        protected override void Execute(UpdateFileTagParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            using (var tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                var logic = new DeleteFileTagAsyncLogic(this);

                foreach (var filePath in param.FilePathList)
                {
                    logic.Execute(filePath, param.Tag);
                }

                tran.Commit();
            }
        }
    }
}
