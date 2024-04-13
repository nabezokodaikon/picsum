using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using SWF.Common;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;

namespace PicSum.Task.Logics
{
    /// <summary>
    /// フォルダの表示履歴を取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class DirectoryViewHistoryGetLogic(IAsyncTask task)
        : AbstractAsyncLogic(task)
    {
        public IList<string> Execute()
        {
            var sql = new ReadDirectoryViewHistorySql(100);
            var dtoList = DatabaseManager<FileInfoConnection>.ReadList<DirectoryViewHistoryDto>(sql);

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
