using PicSum.Core.Data.DatabaseAccessor;
using System;
using System.Data;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// タグTを、ファイルパスとタグを指定して削除します。
    /// </summary>
    public sealed class DeletionTagByFileAndTagSql
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

        public DeletionTagByFileAndTagSql(string filePath, string tag)
            : base(SQL_TEXT)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            base.ParameterList.AddRange(new IDbDataParameter[]
                { SqlParameterUtil.CreateParameter("file_path", filePath),
                  SqlParameterUtil.CreateParameter("tag", tag) });
        }
    }
}
