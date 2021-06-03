using System.Collections.Generic;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Data.FileAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using SWF.Common;

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
            ReadFolderViewHistorySql sql = new ReadFolderViewHistorySql(100);
            IList<SingleValueDto<string>> dtoList = DatabaseManager<FileInfoConnection>.ReadList<SingleValueDto<string>>(sql);

            List<string> folderPathList = new List<string>();
            foreach (SingleValueDto<string> dto in dtoList)
            {
                CheckCancel();
                if (FileUtil.CanAccess(dto.Value))
                {
                    folderPathList.Add(dto.Value);
                }
            }

            return folderPathList;
        }
    }
}
