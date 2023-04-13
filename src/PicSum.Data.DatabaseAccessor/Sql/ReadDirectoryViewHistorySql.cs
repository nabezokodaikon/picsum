using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// フォルダの表示履歴を取得します。
    /// </summary>
    public class ReadDirectoryViewHistorySql : SqlBase<DirectoryViewHistoryDto>
    {
        public ReadDirectoryViewHistorySql(int limit)
            : base()
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("limit", limit));
        }
    }
}
