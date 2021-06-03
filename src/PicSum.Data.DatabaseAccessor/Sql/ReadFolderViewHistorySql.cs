using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// フォルダの表示履歴を取得します。
    /// </summary>
    public class ReadFolderViewHistorySql : SqlBase<SingleValueDto<string>>
    {
        public ReadFolderViewHistorySql(int limit)
            : base()
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("limit", limit));
        }
    }
}
