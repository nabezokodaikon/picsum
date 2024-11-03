using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// フォルダの表示履歴を取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class DirectoryViewHistoryGetLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public IList<string> Execute()
        {
            var sql = new DirectoryViewHistoryReadSql(100);
            var dtoList = Dao<FileInfoDB>.Instance.ReadList<DirectoryViewHistoryDto>(sql);

            var directoryPathList = new List<string>();
            foreach (var dto in dtoList
                .Select(value => new { value.DirectoryPath, value.ViewDate })
                .OrderByDescending(value => value.ViewDate))
            {
                this.CheckCancel();
                if (FileUtil.CanAccess(dto.DirectoryPath))
                {
                    if (!directoryPathList.Contains(dto.DirectoryPath))
                    {
                        directoryPathList.Add(dto.DirectoryPath);
                    }
                }
            }

            return directoryPathList;
        }
    }
}
