using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Data;
using ZLinq;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// フォルダの表示履歴を取得します。
    /// </summary>

    internal sealed class DirectoryViewHistoryGetLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public async ValueTask<string[]> Execute(IConnection con)
        {
            using (TimeMeasuring.Run(true, "DirectoryViewHistoryGetLogic.Execute"))
            {
                var sql = new DirectoryViewHistoryReadSql(100);
                var dtoList = await con.ReadList<DirectoryViewHistoryDto>(sql).WithConfig();
                return [.. dtoList
                    .AsValueEnumerable()
                    .Select(static dto => dto.DirectoryPath)];
            }
        }
    }
}
