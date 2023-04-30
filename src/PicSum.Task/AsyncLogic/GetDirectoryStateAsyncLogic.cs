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
    internal class GetDirectoryStateAsyncLogic : AbstractAsyncLogic
    {
        public GetDirectoryStateAsyncLogic(AbstractAsyncFacade facade) : base(facade) { }

        public DirectoryStateEntity Execute(string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            ReadDirectoryStateByDirectorySql sql = new ReadDirectoryStateByDirectorySql(directoryPath);
            DirectoryStateDto dto = DatabaseManager<FileInfoConnection>.ReadLine<DirectoryStateDto>(sql);
            if (dto != null)
            {
                DirectoryStateEntity directoryState = new DirectoryStateEntity();
                directoryState.DirectoryPath = dto.DirectoryPath;
                directoryState.SortTypeID = (SortTypeID)dto.SortTypeId;
                directoryState.IsAscending = dto.IsAscending;
                directoryState.SelectedFilePath = dto.SelectedFilePath;
                return directoryState;
            }
            else
            {
                return null;
            }
        }
    }
}
