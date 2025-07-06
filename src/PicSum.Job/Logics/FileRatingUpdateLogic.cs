using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイル指定評価T更新
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class FileRatingUpdateLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public bool Execute(
            IDatabaseConnection con, string filePath, int ratingValue, DateTime registrationDate)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (ratingValue < 0)
            {
                throw new ArgumentException("0未満は評価値として無効です。", nameof(ratingValue));
            }

            var sql = new RatingUpdateSql(filePath, ratingValue, registrationDate);
            return con.Update(sql);
        }
    }
}
