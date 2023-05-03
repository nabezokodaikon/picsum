using PicSum.Core.Data.DatabaseAccessor;
using System;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    public sealed class DeletionDirectoryViewCounterByFileSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
DELETE FROM t_directory_view_counter
 WHERE file_id = (SELECT mf.file_id
                    FROM m_file mf
                   WHERE mf.file_path = :file_path
                 )
";

        public DeletionDirectoryViewCounterByFileSql(string directoryPath)
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