using SWF.Core.DatabaseAccessor;
using System.Runtime.Versioning;

namespace PicSum.DatabaseAccessor.Sql
{
    /// <summary>
    /// タグTを、ファイルパスとタグを指定して削除します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class TagDeletionSql
        : SqlBase
    {
        private const string SQL_TEXT =
@"
DELETE FROM t_tag
 WHERE file_id = (SELECT mf.file_id
                    FROM m_file mf
                   WHERE mf.file_path = :file_path
                 )
   AND tag = :tag
";

        public TagDeletionSql(string filePath, string tag)
            : base(SQL_TEXT)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
            ArgumentException.ThrowIfNullOrEmpty(tag, nameof(tag));

            base.ParameterList.AddRange(
                [SqlUtil.CreateParameter("file_path", filePath),
                    SqlUtil.CreateParameter("tag", tag)]);
        }
    }
}
