using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// サムネイルT削除
    /// </summary>
    public class DeletionThumbnailByFileSql : SqlBase
    {
        public DeletionThumbnailByFileSql(string filePath)
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", filePath));
        }
    }
}