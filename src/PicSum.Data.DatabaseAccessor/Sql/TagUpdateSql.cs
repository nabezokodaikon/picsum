using PicSum.Core.DatabaseAccessor;
using System;
using System.Runtime.Versioning;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// タグT更新
    /// </summary>
    /// <remarks>タグの存在確認として使用します。</remarks>
    [SupportedOSPlatform("windows")]
    public sealed class TagUpdateSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
UPDATE t_tag
   SET tag = :tag
      ,registration_date = :registration_date
 WHERE file_id = (SELECT mf.file_id
                    FROM m_file mf
                   WHERE mf.file_path = :file_path
                 )
   AND tag = :tag
";

        public TagUpdateSql(string filePath, string tag, DateTime registrationDate)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentException.ThrowIfNullOrEmpty(tag, nameof(tag));
            base.ParameterList.AddRange(
            [
                SqlParameterUtil.CreateParameter("file_path", filePath),
                SqlParameterUtil.CreateParameter("tag", tag),
                SqlParameterUtil.CreateParameter("registration_date", registrationDate)
            ]);
        }
    }
}
