using System;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Base.Conf;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// フォルダ状態を取得します。
    /// </summary>
    internal class GetFolderStateAsyncLogic : AsyncLogicBase
    {
        public GetFolderStateAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public FolderStateEntity Execute(string folderPath)
        {
            if (folderPath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            ReadFolderStateByFolderSql sql = new ReadFolderStateByFolderSql(folderPath);
            FolderStateDto dto = DatabaseManager<FileInfoConnection>.ReadLine<FolderStateDto>(sql);
            if (dto != null)
            {
                FolderStateEntity folderState = new FolderStateEntity();
                folderState.FolderPath = dto.FolderPath;
                folderState.SortTypeID = (SortTypeID)dto.SortTypeId;
                folderState.IsAscending = dto.IsAscending;
                folderState.SelectedFilePath = dto.SelectedFilePath;
                return folderState;
            }
            else
            {
                return null;
            }
        }
    }
}
