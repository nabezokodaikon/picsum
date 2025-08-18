using PicSum.DatabaseAccessor.Dto;
using SWF.Core.DatabaseAccessor;

namespace PicSum.DatabaseAccessor.Sql
{
    /// <summary>
    /// 単一ファイル情報を読込みます。
    /// </summary>

    public sealed class FileInfoReadSql
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

        public FileInfoReadSql(string filePath)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            base.Parameters = [
                SqlUtil.CreateParameter("file_path", filePath)
            ];
        }
    }
}
