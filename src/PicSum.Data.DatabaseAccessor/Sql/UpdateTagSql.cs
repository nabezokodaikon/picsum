using System.Data;
using PicSum.Core.Data.DatabaseAccessor;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// タグT更新
    /// </summary>
    /// <remarks>タグの存在確認として使用します。</remarks>
    public class UpdateTagSql : SqlBase
    {
        public UpdateTagSql(string filePath, string tag)
            : base()
        {
            base.ParameterList.AddRange(new IDbDataParameter[] 
                { SqlParameterUtil.CreateParameter("file_path", filePath), 
                  SqlParameterUtil.CreateParameter("tag", tag) });
        }
    }
}
