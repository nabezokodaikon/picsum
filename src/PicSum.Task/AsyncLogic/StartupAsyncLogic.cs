using System;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// スタートアップ非同期ロジック
    /// </summary>
    internal class StartupAsyncLogic : AbstractAsyncLogic
    {
        public StartupAsyncLogic(AbstractAsyncFacade facade) : base(facade) { }

        public void Execute(StartupPrameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            DatabaseManager<FileInfoConnection>.Connect(new FileInfoConnection(param.FileInfoDBFilePath));
            DatabaseManager<ThumbnailConnection>.Connect(new ThumbnailConnection(param.ThumbnailDBFilePath));
        }
    }
}
