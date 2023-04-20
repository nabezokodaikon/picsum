using System.Data;
using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// タグTを、ファイルパスとタグを指定して削除します。
    /// </summary>
    public class DeletionTagByFileAndTagSql : SqlBase
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
            base.ParameterList.AddRange(new IDbDataParameter[] 
                { SqlParameterUtil.CreateParameter("file_path", filePath), 
                  SqlParameterUtil.CreateParameter("tag", tag) });
        }
    }
}
