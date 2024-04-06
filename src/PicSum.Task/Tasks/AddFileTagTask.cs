using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.Logics;
using PicSum.Task.Paramters;
using System;

namespace PicSum.Task.Tasks
{
    /// <summary>
    /// ファイルにタグを追加します。
    /// </summary>
    public sealed class AddFileTagTask
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
                var updateTag = new UpdateFileTagLogic(this);
                var addTag = new AddFileTagLogic(this);
                var addFileMaster = new AddFileMasterLogic(this);
                var updateFileMaster = new UpdateFileMastercLogic(this);
                var registrationDate = DateTime.Now;

                foreach (var filePath in param.FilePathList)
                {
                    if (!updateTag.Execute(filePath, param.Tag, registrationDate))
                    {
                        if (!addTag.Execute(filePath, param.Tag, registrationDate))
                        {
                            if (!updateFileMaster.Execute(filePath))
                            {
                                addFileMaster.Execute(filePath);
                            }

                            addTag.Execute(filePath, param.Tag, registrationDate);
                        }
                    }
                }

                tran.Commit();
            }
        }
    }
}
