using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// ファイルマスタに登録します。
    /// </summary>

    internal sealed class FileMasterAddLogic(IJob job)
        : AbstractLogic(job)
    {
        /// <summary>
        /// 処理を実行します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public async ValueTask Execute(IConnection con, string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var sql = new FileCreationSql(filePath);
            await con.Update(sql).False();
        }
    }
}
