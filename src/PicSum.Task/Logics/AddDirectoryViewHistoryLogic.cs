using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System;
using System.Runtime.Versioning;

namespace PicSum.Task.Logics
{
    /// <summary>
    /// フォルダの表示履歴を追加します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class AddDirectoryViewHistoryLogic(IAsyncTask task)
        : AbstractAsyncLogic(task)
    {
        /// <summary>
        /// 処理を実行します。
        /// </summary>
        /// <param name="directoryPath">フォルダパス</param>
        /// <returns>表示履歴が追加されたらTrue、追加されなければFalseを返します。</returns>
        public bool Execute(string directoryPath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            var sql = new CreationDirectoryViewHistorySql(directoryPath);
            return DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
