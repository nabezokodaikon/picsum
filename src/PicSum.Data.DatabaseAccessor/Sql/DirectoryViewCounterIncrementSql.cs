using PicSum.Core.Data.DatabaseAccessor;
using System;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    public sealed class DirectoryViewCounterIncrementSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
UPDATE t_directory_view_counter
    SET view_count = (SELECT tfvc.view_count + 1
                         FROM t_directory_view_counter tfvc
                        WHERE tfvc.file_id = (SELECT mf.file_id
                                                  FROM m_file mf
                                                 WHERE mf.file_path = :file_path
                                              )
                     )
 WHERE file_id = (SELECT mf.file_id
                    FROM m_file mf
                   WHERE mf.file_path = :file_path
                 )
";

        public DirectoryViewCounterIncrementSql(string directoryPath)
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
