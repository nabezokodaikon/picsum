using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// タグ指定タグT削除
    /// </summary>
    public class DeletionTagByTagSql : SqlBase
    {
        public DeletionTagByTagSql(string tag)
            : base()
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("tag", tag));
        }
    }
}