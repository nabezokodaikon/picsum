using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;
using System;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// 単一ファイル情報を読込みます。
    /// </summary>
    public sealed class ReadFileInfoSql
        : SqlBase<FileInfoDto>
    {
        private const string SQL_TEXT =
@"
SELECT mf.file_path
      ,COALESCE(tr.rating, 0) AS rating
  FROM m_file mf
       LEFT JOIN t_rating tr
         ON tr.file_id = mf.file_id
 WHERE mf.file_path = :file_path
";

        public ReadFileInfoSql(string filePath)
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
