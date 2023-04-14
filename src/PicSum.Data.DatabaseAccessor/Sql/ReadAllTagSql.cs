using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// 全てのタグを読込みます。
    /// </summary>
    public class ReadAllTagSql : SqlBase<TagInfoDto>
    {
        public ReadAllTagSql() : base() { }
    }
}
