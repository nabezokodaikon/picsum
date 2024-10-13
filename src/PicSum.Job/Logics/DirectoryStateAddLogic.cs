using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using PicSum.Job.Parameters;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// フォルダ状態テーブルに登録します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class DirectoryStateAddLogic(AbstractAsyncJob job)
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

            DirectoryStateCreationSql sql;
            if (string.IsNullOrEmpty(directoryState.SelectedFilePath))
            {
                sql = new DirectoryStateCreationSql(directoryState.DirectoryPath, (int)directoryState.SortTypeID, directoryState.IsAscending);
            }
            else
            {
                sql = new DirectoryStateCreationSql(directoryState.DirectoryPath, (int)directoryState.SortTypeID, directoryState.IsAscending, directoryState.SelectedFilePath);
            }

            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
