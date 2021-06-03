using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// マイコンピュータ、ドライブを除く、
    /// 表示回数の多いフォルダを取得します。
    /// </summary>
    public class ReadFavoriteFolderSql : SqlBase<SingleValueDto<string>>
    {
        public ReadFavoriteFolderSql() : base() { }
    }
}
