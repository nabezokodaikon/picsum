using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイルの評価値を登録します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class FileRatingAddLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public bool Execute(IDBConnection con, string filePath, int ratingValue, DateTime registrationDate)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (ratingValue < 0)
            {
                throw new ArgumentException("0未満は評価値として無効です。", nameof(ratingValue));
            }

            var sql = new RatingCreationSql(filePath, ratingValue, registrationDate);
            return con.Update(sql);
        }
    }
}
