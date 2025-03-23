using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// フォルダ状態を取得します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class DirectoryStateGetLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public DirectoryStateParameter Execute(string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            var sql = new DirectoryStateReadSql(directoryPath);
            var dto = Instance<IFileInfoDB>.Value.ReadLine<DirectoryStateDto>(sql);
            if (dto != default(DirectoryStateDto))
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
                return DirectoryStateParameter.EMPTY;
            }
        }
    }
}
