using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// ファイル指定評価T更新
    /// </summary>
    internal sealed class UpdateFileRatingAsyncLogic
        : AbstractAsyncLogic
    {
        public UpdateFileRatingAsyncLogic(IAsyncTask task)
            : base(task)
        {

        }

        public bool Execute(string filePath, int ratingValue, DateTime registrationDate)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (ratingValue < 0)
            {
                throw new ArgumentException("0未満は評価値として無効です。", nameof(ratingValue));
            }

            var sql = new UpdateRatingSql(filePath, ratingValue, registrationDate);
            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
