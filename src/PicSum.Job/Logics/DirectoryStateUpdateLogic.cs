using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Job.AsyncJob;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// フォルダ状態テーブルを更新します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class DirectoryStateUpdateLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public bool Execute(DirectoryStateParameter directoryState)
        {
            ArgumentNullException.ThrowIfNull(directoryState, nameof(directoryState));

            if (directoryState.DirectoryPath == null)
            {
                throw new ArgumentException("フォルダパスがNULLです。", nameof(directoryState));
            }

            if (directoryState.SelectedFilePath == null)
            {
                throw new ArgumentException("選択ファイルパスがNULLです。", nameof(directoryState));
            }

            if (directoryState.SortTypeID == SortTypeID.Default)
            {
                throw new ArgumentException("ソートIDがデフォルトです。", nameof(directoryState));
            }

            DirectoryStateUpdateSql sql;
            if (string.IsNullOrEmpty(directoryState.SelectedFilePath))
            {
                sql = new DirectoryStateUpdateSql(directoryState.DirectoryPath, (int)directoryState.SortTypeID, directoryState.IsAscending);
            }
            else
            {
                sql = new DirectoryStateUpdateSql(directoryState.DirectoryPath, (int)directoryState.SortTypeID, directoryState.IsAscending, directoryState.SelectedFilePath);
            }

            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
