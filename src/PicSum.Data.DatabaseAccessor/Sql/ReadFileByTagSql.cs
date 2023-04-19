using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// タグを指定してファイルを読込みます。
    /// </summary>
    public class ReadFileByTagSql : SqlBase<FileByTagDto>
    {
        public ReadFileByTagSql(string tag)
            : base()
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("tag", tag));
        }
    }
}