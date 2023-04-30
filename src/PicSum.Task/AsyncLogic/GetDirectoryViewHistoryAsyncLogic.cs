using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using SWF.Common;
using System.Collections.Generic;
using System.Linq;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// フォルダの表示履歴を取得します。
    /// </summary>
    public class GetDirectoryViewHistoryAsyncLogic : AbstractAsyncLogic
    {
        public GetDirectoryViewHistoryAsyncLogic(AbstractAsyncFacade facade) : base(facade) { }

        public IList<string> Execute()
        {
            var sql = new ReadDirectoryViewHistorySql(100);
            var dtoList = DatabaseManager<FileInfoConnection>.ReadList<DirectoryViewHistoryDto>(sql);

            var directoryPathList = new List<string>();
            foreach (var dto in dtoList
                .Select(value => new { DirectoryPath = value.DirectoryPath, ViewDate = value.ViewDate })
                .OrderByDescending(value => value.ViewDate))
            {
                CheckCancel();
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
