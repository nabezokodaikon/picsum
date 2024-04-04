using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Paramter;
using System;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// ファイルにタグを追加します。
    /// </summary>
    public sealed class AddFileTagAsyncFacade
        : OneWayFacadeBase<UpdateFileTagParameter>
    {
        public override void Execute(UpdateFileTagParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            using (var tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                var updateTag = new UpdateFileTagAsyncLogic(this);
                var addTag = new AddFileTagAsyncLogic(this);
                var addFileMaster = new AddFileMasterAsyncLogic(this);
                var updateFileMaster = new UpdateFileMasterAsyncLogic(this);
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
