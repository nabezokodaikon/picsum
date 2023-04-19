using PicSum.Core.Data.DatabaseAccessor;
using System;
using System.Data;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// 評価T作成
    /// </summary>
    public class CreationRatingSql : SqlBase
    {
        public CreationRatingSql(string filePath, int rating, DateTime registration_date)
            : base()
        {
            base.ParameterList.AddRange(new IDbDataParameter[] {
                SqlParameterUtil.CreateParameter("file_path", filePath),
                SqlParameterUtil.CreateParameter("rating", rating),
                SqlParameterUtil.CreateParameter("registration_date", registration_date)
            });
        }
    }
}
