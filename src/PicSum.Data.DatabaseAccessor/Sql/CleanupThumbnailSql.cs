using PicSum.Core.Data.DatabaseAccessor;
using System.Data;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    public sealed class CleanupThumbnailSql
        : SqlBase
    {
        const string SQL_TEXT =
@"
DELETE FROM t_thumbnail
 WHERE file_path = :file_path;
";

        public CleanupThumbnailSql(string filePath)
            : base(SQL_TEXT)
        {
            base.ParameterList.AddRange(new IDbDataParameter[] {
                SqlParameterUtil.CreateParameter("file_path", filePath),
            });
        }
    }
}
