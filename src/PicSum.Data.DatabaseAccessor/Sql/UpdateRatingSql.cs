using PicSum.Core.Data.DatabaseAccessor;
using System.Data;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// 評価T更新
    /// </summary>
    public class UpdateRatingSql : SqlBase
    {
        public UpdateRatingSql(string filePath, int rating)
            : base()
        {
            base.ParameterList.AddRange(new IDbDataParameter[] 
                { SqlParameterUtil.CreateParameter("file_path", filePath), 
                  SqlParameterUtil.CreateParameter("rating", rating) });
        }
    }
}
