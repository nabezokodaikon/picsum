using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System;
using System.Runtime.Versioning;

namespace PicSum.Task.Logics
{
    /// <summary>
    /// ファイルの評価値を登録します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class AddFileRatingLogic(IAsyncTask task)
        : AbstractAsyncLogic(task)
    {
        public bool Execute(string filePath, int ratingValue, DateTime registrationDate)
        {
            ArgumentNullException.ThrowIfNull(filePath, nameof(filePath));

            if (ratingValue < 0)
            {
                throw new ArgumentException("0未満は評価値として無効です。", nameof(ratingValue));
            }

            var sql = new CreationRatingSql(filePath, ratingValue, registrationDate);
            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
