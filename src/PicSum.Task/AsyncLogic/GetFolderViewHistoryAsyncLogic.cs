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
    public class GetFolderViewHistoryAsyncLogic : AsyncLogicBase
    {
        public GetFolderViewHistoryAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public IList<string> Execute()
        {
            var sql = new ReadFolderViewHistorySql(100);
            var dtoList = DatabaseManager<FileInfoConnection>.ReadList<FolderViewHistoryDto>(sql);

            var folderPathList = new List<string>();
            foreach (var dto in dtoList
                .Select(value => new { FolderPath = value.FolderPath, ViewDate = value.ViewDate })
                .OrderByDescending(value => value.ViewDate))
            {
                CheckCancel();
                if (FileUtil.CanAccess(dto.FolderPath))
                {
                    if (!folderPathList.Contains(dto.FolderPath))
                    {
                        folderPathList.Add(dto.FolderPath);
                    }                     
                }
            }

            return folderPathList;
        }
    }
}
