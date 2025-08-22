using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイル指定評価T更新
    /// </summary>

    internal sealed class FileRatingUpdateLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public async ValueTask<bool> Execute(
            IConnection con, string filePath, int ratingValue, DateTime addDate)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (ratingValue < 0)
            {
                throw new ArgumentException("0未満は評価値として無効です。", nameof(ratingValue));
            }

            var sql = new RatingUpdateSql(filePath, ratingValue, addDate);
            return await con.Update(sql).WithConfig();
        }
    }
}
