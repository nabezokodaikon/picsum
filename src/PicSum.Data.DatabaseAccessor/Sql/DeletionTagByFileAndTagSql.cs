using System.Data;
using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// タグTを、ファイルパスとタグを指定して削除します。
    /// </summary>
    public class DeletionTagByFileAndTagSql : SqlBase
    {
        public DeletionTagByFileAndTagSql(string filePath, string tag)
            : base()
        {
            base.ParameterList.AddRange(new IDbDataParameter[] 
                { SqlParameterUtil.CreateParameter("file_path", filePath), 
                  SqlParameterUtil.CreateParameter("tag", tag) });
        }
    }
}
