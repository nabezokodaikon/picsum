using PicSum.Core.Data.DatabaseAccessor;
using System;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    public sealed class CreationDirectoryViewCounterSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
INSERT INTO t_directory_view_counter (
       file_id
      ,view_count
)
SELECT mf.file_id
      ,1
  FROM m_file mf
 WHERE mf.file_path = :file_path
";

        public CreationDirectoryViewCounterSql(string directoryPath)
            : base(SQL_TEXT)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", directoryPath));
        }
    }
}
