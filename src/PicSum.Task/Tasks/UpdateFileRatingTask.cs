using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.Logics;
using PicSum.Task.Paramters;
using System;
using System.Runtime.Versioning;

namespace PicSum.Task.Tasks
{
    /// <summary>
    /// ファイルの評価値を更新します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class UpdateFileRatingTask
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
                var updateFileRating = new UpdateFileRatingLogic(this);
                var addFileRating = new AddFileRatingLogic(this);
                var addFileMaster = new AddFileMasterLogic(this);
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
