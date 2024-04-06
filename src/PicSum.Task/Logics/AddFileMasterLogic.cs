using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System;
using System.Runtime.Versioning;

namespace PicSum.Task.Logics
{
    /// <summary>
    /// ファイルマスタに登録します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class AddFileMasterLogic
        : AbstractAsyncLogic
    {
        public AddFileMasterLogic(IAsyncTask task)
            : base(task)
        {

        }

        /// <summary>
        /// 処理を実行します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public void Execute(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var sql = new CreationFileSql(filePath);
            DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
