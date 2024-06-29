using PicSum.Core.DatabaseAccessor;
using System;
using System.Runtime.Versioning;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// フォルダ表示履歴T作成
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class DirectoryViewHistoryCreationSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
INSERT INTO t_directory_view_history (
     file_id
    ,file_history_id
    ,view_date
)
SELECT mf.file_id
      ,( SELECT COUNT(1)
           FROM t_directory_view_history tfvh
          WHERE tfvh.file_id = mf.file_id
       )
      ,DATETIME('NOW', 'LOCALTIME')
  FROM m_file mf
 WHERE mf.file_path = :directory_path
";

        public DirectoryViewHistoryCreationSql(string directoryPath)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            base.ParameterList.Add(SqlParameterUtil.CreateParameter("directory_path", directoryPath));
        }
    }
}
