using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// サムネイルT削除
    /// </summary>
    public class DeletionThumbnailByFileSql : SqlBase
    {
        private const string SQL_TEXT =
@"
DELETE FROM t_thumbnail
 WHERE file_path = :file_path
";

        public DeletionThumbnailByFileSql(string filePath)
            : base(SQL_TEXT)
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", filePath));
        }
    }
}