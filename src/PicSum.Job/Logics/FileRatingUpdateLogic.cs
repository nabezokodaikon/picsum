using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイル指定評価T更新
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class FileRatingUpdateLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public bool Execute(string filePath, int ratingValue, DateTime registrationDate)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (ratingValue < 0)
            {
                throw new ArgumentException("0未満は評価値として無効です。", nameof(ratingValue));
            }

            var sql = new RatingUpdateSql(filePath, ratingValue, registrationDate);
            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
