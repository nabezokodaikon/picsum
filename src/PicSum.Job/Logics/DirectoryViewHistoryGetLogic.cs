using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Data;
using System.Runtime.Versioning;
using ZLinq;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// フォルダの表示履歴を取得します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class DirectoryViewHistoryGetLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public string[] Execute(IConnection con)
        {
            var sql = new DirectoryViewHistoryReadSql(100);
            var dtoList = con.ReadList<DirectoryViewHistoryDto>(sql);
            return [.. dtoList
                .AsValueEnumerable()
                .Select(dto => dto.DirectoryPath)
                .Where(FileUtil.CanAccess)];
        }
    }
}
