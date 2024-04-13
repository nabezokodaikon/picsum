using PicSum.Core.Base.Conf;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using PicSum.Task.Parameters;
using System;
using System.Runtime.Versioning;

namespace PicSum.Task.Logics
{
    /// <summary>
    /// フォルダ状態を取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class DirectoryStateGetLogic(IAsyncTask task)
        : AbstractAsyncLogic(task)
    {
        public DirectoryStateParameter Execute(string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            var sql = new DirectoryStateReadSql(directoryPath);
            var dto = DatabaseManager<FileInfoConnection>.ReadLine<DirectoryStateDto>(sql);
            if (dto != null)
            {
                var directoryState = new DirectoryStateParameter
                {
                    DirectoryPath = dto.DirectoryPath,
                    SortTypeID = (SortTypeID)dto.SortTypeId,
                    IsAscending = dto.IsAscending,
                    SelectedFilePath = dto.SelectedFilePath
                };
                return directoryState;
            }
            else
            {
                return null;
            }
        }
    }
}
