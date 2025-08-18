using SWF.Core.DatabaseAccessor;

namespace PicSum.DatabaseAccessor.Sql
{
    /// <summary>
    /// タグT更新
    /// </summary>
    /// <remarks>タグの存在確認として使用します。</remarks>

    public sealed class TagUpdateSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
INSERT INTO t_tag (
     file_id
    ,tag
    ,registration_date
)
SELECT mf.file_id
      ,:tag
      ,:registration_date
  FROM m_file mf
 WHERE mf.file_path = :file_path
ON CONFLICT(file_id, tag) DO UPDATE SET
    tag = :tag
   ,registration_date = :registration_date
";

        public TagUpdateSql(string filePath, string tag, DateTime registrationDate)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentException.ThrowIfNullOrEmpty(tag, nameof(tag));
            base.Parameters = [
                SqlUtil.CreateParameter("file_path", filePath),
                SqlUtil.CreateParameter("tag", tag),
                SqlUtil.CreateParameter("registration_date", registrationDate)
            ];
        }
    }
}
