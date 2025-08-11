using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// ファイルの評価値を更新します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class FileRatingUpdateJob
        : AbstractOneWayJob<FileRatingUpdateParameter>
    {
        protected async override ValueTask Execute(FileRatingUpdateParameter param)
        {
            if (param.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(param));
            }

            await using (var con = await Instance<IFileInfoDB>.Value.ConnectWithTransaction().WithConfig())
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
        }
    }
}
