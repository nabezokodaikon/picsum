using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.Job;
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
        public string[] Execute()
        {
            var sql = new DirectoryViewHistoryReadSql(100);
            var dtoList = Instance<IFileInfoDB>.Value.ReadList<DirectoryViewHistoryDto>(sql);
            return [.. dtoList
                .AsValueEnumerable()
                .Select(dto => dto.DirectoryPath)
                .Where(FileUtil.CanAccess)];
        }
    }
}
