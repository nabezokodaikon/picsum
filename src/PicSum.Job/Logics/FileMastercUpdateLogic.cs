using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Sql;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    [SupportedOSPlatform("windows")]
    internal sealed class FileMastercUpdateLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
        /// <summary>
        /// 処理を実行します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public bool Execute(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var sql = new FileUpdateSql(filePath);
            return Dao<FileInfoDB>.Instance.Update(sql);
        }
    }
}
