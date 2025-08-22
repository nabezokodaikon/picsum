using PicSum.DatabaseAccessor.Sql;
using SWF.Core.Base;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイルマスタに登録します。
    /// </summary>

    internal sealed class FileMasterAddLogic(IAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        /// <summary>
        /// 処理を実行します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public async ValueTask Execute(IDatabaseConnection con, string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var sql = new FileCreationSql(filePath);
            await con.Update(sql).WithConfig();
        }
    }
}
