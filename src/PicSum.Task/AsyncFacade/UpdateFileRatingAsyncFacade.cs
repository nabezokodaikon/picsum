using System;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// ファイルの評価値を更新します。
    /// </summary>
    public class UpdateFileRatingAsyncFacade
        : OneWayFacadeBase<UpdateFileRatingParameterEntity>
    {
        public override void Execute(UpdateFileRatingParameterEntity param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            using (Transaction tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                UpdateFileRatingAsyncLogic updateFileRating = new UpdateFileRatingAsyncLogic(this);
                AddFileRatingAsyncLogic addFileRating = new AddFileRatingAsyncLogic(this);
                AddFileMasterAsyncLogic addFileMaster = new AddFileMasterAsyncLogic(this);

                foreach (string filePath in param.FilePathList)
                {
                    if (!updateFileRating.Execute(filePath, param.RatingValue))
                    {
                        if (!addFileRating.Execute(filePath, param.RatingValue))
                        {
                            addFileMaster.Execute(filePath);
                            addFileRating.Execute(filePath, param.RatingValue);
                        }
                    }
                }

                tran.Commit();
            }
        }
    }
}
