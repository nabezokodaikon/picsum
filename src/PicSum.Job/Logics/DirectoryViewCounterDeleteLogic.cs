using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// フォルダの表示回数を削除します。
    /// </summary>

    internal sealed class DirectoryViewCounterDeleteLogic(IJob job)
        : AbstractLogic(job)
    {
        /// <summary>
        /// 処理を実行します。
        /// </summary>
        /// <param name="directoryPath">フォルダパス</param>
        public async ValueTask Execute(IConnection con, string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            var sql = new DirectoryViewCounterDeletionSql(directoryPath);
            await con.Update(sql).WithConfig();
        }
    }
}
