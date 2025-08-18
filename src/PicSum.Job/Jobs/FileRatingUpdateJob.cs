using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// ファイルの評価値を更新します。
    /// </summary>

    internal sealed class FileRatingUpdateJob
        : AbstractOneWayJob<FileRatingUpdateParameter>
    {
        protected override ValueTask Execute(FileRatingUpdateParameter param)
        {
            if (param.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(param));
            }

            using (var con = Instance<IFileInfoDB>.Value.ConnectWithTransaction())
            {
                var updateFileRating = new FileRatingUpdateLogic(this);
                var addFileMaster = new FileMasterAddLogic(this);
                var addDate = DateTime.Now;

                foreach (var filePath in param.FilePathList)
                {
                    if (!updateFileRating.Execute(con, filePath, param.RatingValue, addDate))
                    {
                        addFileMaster.Execute(con, filePath);
                        updateFileRating.Execute(con, filePath, param.RatingValue, addDate);
                    }
                }

                con.Commit();
            }

            return ValueTask.CompletedTask;
        }
    }
}
