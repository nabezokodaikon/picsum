using System;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// ファイル指定評価T更新
    /// </summary>
    internal class UpdateFileRatingAsyncLogic : AsyncLogicBase
    {
        public UpdateFileRatingAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public bool Execute(string filePath, int ratingValue, DateTime registrationDate)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            if (ratingValue < 0)
            {
                throw new ArgumentException("0未満は評価値として無効です。", "ratingValue");
            }

            UpdateRatingSql sql = new UpdateRatingSql(filePath, ratingValue, registrationDate);
            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
