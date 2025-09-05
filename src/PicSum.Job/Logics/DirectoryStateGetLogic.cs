using PicSum.DatabaseAccessor.Dto;
using PicSum.DatabaseAccessor.Sql;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// フォルダ状態を取得します。
    /// </summary>

    internal sealed class DirectoryStateGetLogic(IJob job)
        : AbstractLogic(job)
    {
        public async ValueTask<DirectoryStateParameter> Execute(IConnection con, string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            var sql = new DirectoryStateReadSql(directoryPath);
            var dto = await con.ReadLine<DirectoryStateDto>(sql).False();
            if (dto != null)
            {
                var sortMode = (FileSortMode)dto.SortTypeId;
                if (sortMode == FileSortMode.TakenDate)
                {
                    var directoryState = new DirectoryStateParameter(
                        dto.DirectoryPath,
                        FileSortMode.FilePath,
                        true,
                        dto.SelectedFilePath);
                    return directoryState;
                }
                else
                {
                    var directoryState = new DirectoryStateParameter(
                        dto.DirectoryPath,
                        sortMode,
                        dto.IsAscending,
                        dto.SelectedFilePath);
                    return directoryState;
                }
            }
            else
            {
                return DirectoryStateParameter.EMPTY;
            }
        }
    }
}
