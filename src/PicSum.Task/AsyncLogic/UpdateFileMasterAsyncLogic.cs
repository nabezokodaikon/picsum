using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSum.Task.AsyncLogic
{
    internal sealed class UpdateFileMasterAsyncLogic
        : AsyncLogicBase
    {
        public UpdateFileMasterAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        /// <summary>
        /// 処理を実行します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public bool Execute(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var sql = new UpdateFileSql(filePath);
            return  DatabaseManager<FileInfoConnection>.Update(sql);
        }
    }
}
