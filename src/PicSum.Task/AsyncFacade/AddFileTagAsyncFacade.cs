using System;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// ファイルにタグを追加します。
    /// </summary>
    public class AddFileTagAsyncFacade
        : OneWayFacadeBase<UpdateFileTagParameterEntity>
    {
        public override void Execute(UpdateFileTagParameterEntity param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            using (Transaction tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                UpdateFileTagAsyncLogic updateTag = new UpdateFileTagAsyncLogic(this);
                AddFileTagAsyncLogic addTag = new AddFileTagAsyncLogic(this);
                AddFileMasterAsyncLogic addFileMaster = new AddFileMasterAsyncLogic(this);

                foreach (string filePath in param.FilePathList)
                {
                    if (!updateTag.Execute(filePath, param.Tag))
                    {
                        if (!addTag.Execute(filePath, param.Tag))
                        {
                            addFileMaster.Execute(filePath);
                            addTag.Execute(filePath, param.Tag);
                        }
                    }
                }

                tran.Commit();
            }
        }
    }
}
