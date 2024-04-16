using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Job.AsyncJob;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using PicSum.Job.Parameters;
using SWF.Common;
using System;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// フォルダ状態を取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class DirectoryStateGetLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
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
