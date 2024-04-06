using PicSum.Core.Base.Conf;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using PicSum.Task.Parameters;
using System;

namespace PicSum.Task.Logics
{
    /// <summary>
    /// フォルダ状態を取得します。
    /// </summary>
    internal sealed class GetDirectoryStateLogic
        : AbstractAsyncLogic
    {
        public GetDirectoryStateLogic(IAsyncTask task)
            : base(task)
        {

        }

        public DirectoryStateParameter Execute(string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            var sql = new ReadDirectoryStateByDirectorySql(directoryPath);
            var dto = DatabaseManager<FileInfoConnection>.ReadLine<DirectoryStateDto>(sql);
            if (dto != null)
            {
                var directoryState = new DirectoryStateParameter();
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
