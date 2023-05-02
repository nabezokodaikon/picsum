using PicSum.Core.Base.Conf;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using PicSum.Task.Entity;
using System;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// フォルダ状態を取得します。
    /// </summary>
    internal sealed class GetDirectoryStateAsyncLogic
        : AbstractAsyncLogic
    {
        public GetDirectoryStateAsyncLogic(AbstractAsyncFacade facade)
            : base(facade)
        {

        }

        public DirectoryStateEntity Execute(string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            var sql = new ReadDirectoryStateByDirectorySql(directoryPath);
            var dto = DatabaseManager<FileInfoConnection>.ReadLine<DirectoryStateDto>(sql);
            if (dto != null)
            {
                var directoryState = new DirectoryStateEntity();
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
