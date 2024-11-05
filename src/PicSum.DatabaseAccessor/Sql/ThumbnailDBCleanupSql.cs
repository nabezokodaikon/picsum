using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    [SupportedOSPlatform("windows10.0.17763.0")]
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
