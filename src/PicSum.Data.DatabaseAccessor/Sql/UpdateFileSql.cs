using PicSum.Core.Data.DatabaseAccessor;
using System;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    public sealed class UpdateFileSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
UPDATE m_file
   SET file_path = :file_path
WHERE file_path = :file_path
";

        public UpdateFileSql(string filePath)
            : base(SQL_TEXT)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", filePath));
        }
    }
}
