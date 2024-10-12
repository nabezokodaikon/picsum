using PicSum.Core.DatabaseAccessor;
using System;
using System.Runtime.Versioning;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// ファイルM作成
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class FileCreationSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
INSERT INTO m_file (
     file_id
    ,file_path
)
SELECT mfi.file_id
      ,:file_path
  FROM m_file_id mfi
";

        public FileCreationSql(string filePath)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", filePath));
        }
    }
}
