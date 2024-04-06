using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.Logics;
using PicSum.Task.Paramters;
using System;

namespace PicSum.Task.Tasks
{
    /// <summary>
    /// ファイルのタグを削除します。
    /// </summary>
    public class DeleteFileTagTask
        : AbstractAsyncTask<UpdateFileTagParameter>
    {
        protected override void Execute(UpdateFileTagParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            using (var tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                var logic = new DeleteFileTagLogic(this);

                foreach (var filePath in param.FilePathList)
                {
                    logic.Execute(filePath, param.Tag);
                }

                tran.Commit();
            }
        }
    }
}
