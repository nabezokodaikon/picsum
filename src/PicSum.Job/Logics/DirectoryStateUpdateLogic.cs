using PicSum.DatabaseAccessor.Sql;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// フォルダ状態テーブルを更新します。
    /// </summary>

    internal sealed class DirectoryStateUpdateLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public async ValueTask<bool> Execute(IConnection con, DirectoryStateParameter directoryState)
        {
            ArgumentNullException.ThrowIfNull(directoryState, nameof(directoryState));

            if (string.IsNullOrEmpty(directoryState.DirectoryPath))
            {
                throw new ArgumentException("フォルダパスがNULLです。", nameof(directoryState));
            }

            if (string.IsNullOrEmpty(directoryState.SelectedFilePath))
            {
                throw new ArgumentException("選択ファイルパスがNULLです。", nameof(directoryState));
            }

            if (directoryState.SortMode == FileSortMode.Default)
            {
                throw new ArgumentException("ソートIDがデフォルトです。", nameof(directoryState));
            }

            if (string.IsNullOrEmpty(directoryState.SelectedFilePath))
            {
                var sql = new DirectoryStateUpdateSql(
                    directoryState.DirectoryPath,
                    (int)directoryState.SortMode,
                    directoryState.IsAscending);
                return await con.Update(sql).WithConfig();
            }
            else
            {
                var sql = new DirectoryStateUpdateSql(
                    directoryState.DirectoryPath,
                    (int)directoryState.SortMode,
                    directoryState.IsAscending,
                    directoryState.SelectedFilePath);
                return await con.Update(sql).WithConfig();
            }
        }
    }
}
