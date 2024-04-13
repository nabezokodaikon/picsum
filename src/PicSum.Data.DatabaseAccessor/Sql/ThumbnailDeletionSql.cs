using PicSum.Core.Data.DatabaseAccessor;
using System;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// サムネイルT削除
    /// </summary>
    public sealed class ThumbnailDeletionSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
DELETE FROM t_thumbnail
 WHERE file_path = :file_path
";

        public ThumbnailDeletionSql(string filePath)
            : base(SQL_TEXT)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", filePath));
        }
    }
}