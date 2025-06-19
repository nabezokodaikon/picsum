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
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class DirectoryStateAddLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        public bool Execute(IDBConnection con, DirectoryStateParameter directoryState)
        {
            ArgumentNullException.ThrowIfNull(con, nameof(con));
            ArgumentNullException.ThrowIfNull(directoryState, nameof(directoryState));

            if (string.IsNullOrEmpty(directoryState.DirectoryPath))
            {
                throw new ArgumentException("フォルダパスがNULLです。", nameof(directoryState));
            }

            if (string.IsNullOrEmpty(directoryState.SelectedFilePath))
            {
                var sql = new DirectoryStateCreationSql(
                    directoryState.DirectoryPath,
                    (int)directoryState.SortTypeID,
                    directoryState.IsAscending);
                return con.Update(sql);
            }
            else
            {
                var sql = new DirectoryStateCreationSql(
                    directoryState.DirectoryPath,
                    (int)directoryState.SortTypeID,
                    directoryState.IsAscending,
                    directoryState.SelectedFilePath);
                return con.Update(sql);
            }
        }
    }
}
