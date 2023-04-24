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
    /// ファイルの評価値を更新します。
    /// </summary>
    public class UpdateFileRatingAsyncFacade
        : OneWayFacadeBase<UpdateFileRatingParameter>
    {
        public override void Execute(UpdateFileRatingParameter param)
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
                var registrationDate = DateTime.Now;

                foreach (string filePath in param.FilePathList)
                {
                    if (!updateFileRating.Execute(filePath, param.RatingValue, registrationDate))
                    {
                        if (!addFileRating.Execute(filePath, param.RatingValue, registrationDate))
                        {
                            addFileMaster.Execute(filePath);
                            addFileRating.Execute(filePath, param.RatingValue, registrationDate);
                        }
                    }
                }

                tran.Commit();
            }
        }
    }
}
