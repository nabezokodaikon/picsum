using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ThumbnailCountByFileSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
SELECT COUNT()
  FROM t_thumbnail tt
 WHERE tt.file_path = :file_path
";

        public ThumbnailCountByFileSql(string filePath)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", filePath));
        }
    }
}
