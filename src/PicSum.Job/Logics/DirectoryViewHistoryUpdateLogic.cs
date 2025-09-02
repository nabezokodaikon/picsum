using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// フォルダの表示履歴を追加します。
    /// </summary>

    internal sealed class DirectoryViewHistoryUpdateLogic(IJob job)
        : AbstractLogic(job)
    {
        /// <summary>
        /// 処理を実行します。
        /// </summary>
        /// <param name="directoryPath">フォルダパス</param>
        /// <returns>表示履歴が追加されたらTrue、追加されなければFalseを返します。</returns>
        public bool Execute(IConnection con, string directoryPath, long ticks)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            var sql = new DirectoryViewHistoryUpdateSql(directoryPath, ticks);
            return con.Update(sql);
        }
    }
}
