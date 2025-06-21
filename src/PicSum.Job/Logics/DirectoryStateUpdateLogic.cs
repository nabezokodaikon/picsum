using PicSum.DatabaseAccessor.Sql;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// フォルダ状態テーブルを更新します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class DirectoryStateUpdateLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public bool Execute(IConnection con, DirectoryStateParameter directoryState)
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

            if (directoryState.SortTypeID == SortTypeID.Default)
            {
                throw new ArgumentException("ソートIDがデフォルトです。", nameof(directoryState));
            }

            if (string.IsNullOrEmpty(directoryState.SelectedFilePath))
            {
                var sql = new DirectoryStateUpdateSql(
                    directoryState.DirectoryPath,
                    (int)directoryState.SortTypeID,
                    directoryState.IsAscending);
                return con.Update(sql);
            }
            else
            {
                var sql = new DirectoryStateUpdateSql(
                    directoryState.DirectoryPath,
                    (int)directoryState.SortTypeID,
                    directoryState.IsAscending,
                    directoryState.SelectedFilePath);
                return con.Update(sql);
            }
        }
    }
}
