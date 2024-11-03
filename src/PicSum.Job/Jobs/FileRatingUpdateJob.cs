using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// ファイルの評価値を更新します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class FileRatingUpdateJob
        : AbstractOneWayJob<FileRatingUpdateParameter>
    {
        protected override void Execute(FileRatingUpdateParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            if (param.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(param));
            }

            using (var tran = Dao<FileInfoDB>.Instance.BeginTransaction())
            {
                var updateFileRating = new FileRatingUpdateLogic(this);
                var addFileRating = new FileRatingAddLogic(this);
                var addFileMaster = new FileMasterAddLogic(this);
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
