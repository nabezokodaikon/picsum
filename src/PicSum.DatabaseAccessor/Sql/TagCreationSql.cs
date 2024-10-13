using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    /// <summary>
    /// タグT作成
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class TagCreationSql
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
";

        public TagCreationSql(string filePath, string tag, DateTime registrationDate)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentException.ThrowIfNullOrEmpty(tag, nameof(tag));

            base.ParameterList.AddRange(
                [SqlParameterUtil.CreateParameter("file_path", filePath),
                    SqlParameterUtil.CreateParameter("tag", tag),
                    SqlParameterUtil.CreateParameter("registration_date", registrationDate)
                ]);
        }
    }
}
