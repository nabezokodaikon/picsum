using System.Data;
using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// 評価T作成
    /// </summary>
    public class CreationRatingSql : SqlBase
    {
        public CreationRatingSql(string filePath, int rating)
            : base()
        {
            base.ParameterList.AddRange(new IDbDataParameter[] { 
                SqlParameterUtil.CreateParameter("file_path", filePath), 
                SqlParameterUtil.CreateParameter("rating", rating) });
        }
    }
}
