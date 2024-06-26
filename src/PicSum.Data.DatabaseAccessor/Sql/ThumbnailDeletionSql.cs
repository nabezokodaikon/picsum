using PicSum.Core.DatabaseAccessor;
using System;
using System.Runtime.Versioning;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// サムネイルT削除
    /// </summary>
    [SupportedOSPlatform("windows")]
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
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", filePath));
        }
    }
}
