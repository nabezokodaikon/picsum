using PicSum.Core.DatabaseAccessor;
using System;
using System.Runtime.Versioning;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    [SupportedOSPlatform("windows")]
    public sealed class FileUpdateSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
UPDATE m_file
   SET file_path = :file_path
WHERE file_path = :file_path
";

        public FileUpdateSql(string filePath)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", filePath));
        }
    }
}
