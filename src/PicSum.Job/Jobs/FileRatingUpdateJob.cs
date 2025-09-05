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
        protected override async ValueTask Execute(FileRatingUpdateParameter param)
        {
            if (param.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(param));
            }

            await using (var con = await Instance<IFileInfoDao>.Value.ConnectWithTransaction().False())
            {
                var updateFileRating = new FileRatingUpdateLogic(this);
                var addFileMaster = new FileMasterAddLogic(this);
                var addDate = DateTime.Now;

                foreach (var filePath in param.FilePathList)
                {
                    if (!await updateFileRating.Execute(con, filePath, param.RatingValue, addDate).False())
                    {
                        await addFileMaster.Execute(con, filePath).False();
                        await updateFileRating.Execute(con, filePath, param.RatingValue, addDate).False();
                    }
                }

                await con.Commit().False();
            }
        }
    }
}
