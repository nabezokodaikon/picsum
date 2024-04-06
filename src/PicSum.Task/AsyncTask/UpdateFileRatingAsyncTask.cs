using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Paramter;
using System;

namespace PicSum.Task.AsyncTask
{
    /// <summary>
    /// ファイルの評価値を更新します。
    /// </summary>
    public sealed class UpdateFileRatingAsyncTask
        : AbstractAsyncTask<UpdateFileRatingParameter>
    {
        protected override void Execute(UpdateFileRatingParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            using (var tran = DatabaseManager<FileInfoConnection>.BeginTransaction())
            {
                var updateFileRating = new UpdateFileRatingAsyncLogic(this);
                var addFileRating = new AddFileRatingAsyncLogic(this);
                var addFileMaster = new AddFileMasterAsyncLogic(this);
                var registrationDate = DateTime.Now;

                foreach (var filePath in param.FilePathList)
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
