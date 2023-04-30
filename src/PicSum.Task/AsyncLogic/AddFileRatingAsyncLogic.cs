using System;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// ファイルの評価値を登録します。
    /// </summary>
    internal class AddFileRatingAsyncLogic : AbstractAsyncLogic
    {
        public AddFileRatingAsyncLogic(AbstractAsyncFacade facade) : base(facade) { }

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

            CreationRatingSql sql = new CreationRatingSql(filePath, ratingValue, registrationDate);
            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
