using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
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
        public DirectoryStateParameter Execute(IConnection con, string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            var sql = new DirectoryStateReadSql(directoryPath);
            var dto = con.ReadLine<DirectoryStateDto>(sql);
            if (dto != null)
            {
                var directoryState = new DirectoryStateParameter(
                    dto.DirectoryPath,
                    (SortTypeID)dto.SortTypeId,
                    dto.IsAscending,
                    dto.SelectedFilePath);
                return directoryState;
            }
            else
            {
                return DirectoryStateParameter.EMPTY;
            }
        }
    }
}
