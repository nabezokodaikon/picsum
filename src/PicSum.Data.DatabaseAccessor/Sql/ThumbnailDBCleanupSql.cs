using PicSum.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    [SupportedOSPlatform("windows")]
    public sealed class ThumbnailDBCleanupSql
        : SqlBase
    {
        const string SQL_TEXT =
@"
DELETE FROM t_thumbnail
 WHERE file_path = :file_path;
";

        public ThumbnailDBCleanupSql(string filePath)
            : base(SQL_TEXT)
        {
            base.ParameterList.AddRange([
                SqlParameterUtil.CreateParameter("file_path", filePath),
            ]);
        }
    }
}
