using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// ファイル指定評価T削除
    /// </summary>
    public class DeletionRatingByFileSql : SqlBase
    {
        public DeletionRatingByFileSql(string filePath)
            : base()
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("file_path", filePath));
        }
    }
}
