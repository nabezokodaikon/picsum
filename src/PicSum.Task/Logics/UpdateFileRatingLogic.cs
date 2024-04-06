using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System;
using System.Runtime.Versioning;

namespace PicSum.Task.Logics
{
    /// <summary>
    /// ファイル指定評価T更新
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class UpdateFileRatingLogic
        : AbstractAsyncLogic
    {
        public UpdateFileRatingLogic(IAsyncTask task)
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
