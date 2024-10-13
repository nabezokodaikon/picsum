using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// フォルダの表示回数を削除します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal class DirectoryViewCounterDeleteLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        /// <summary>
        /// 処理を実行します。
        /// </summary>
        /// <param name="directoryPath">フォルダパス</param>
        public void Execute(string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            var sql = new DirectoryViewCounterDeletionSql(directoryPath);
            DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
