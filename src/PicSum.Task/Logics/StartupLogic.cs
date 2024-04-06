using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.Paramters;
using System;

namespace PicSum.Task.Logics
{
    /// <summary>
    /// スタートアップ非同期ロジック
    /// </summary>
    internal sealed class StartupLogic
        : AbstractAsyncLogic
    {
        public StartupLogic(IAsyncTask task)
            : base(task)
        {

        }

        public void Execute(StartupPrameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            DatabaseManager<FileInfoConnection>.Connect(new FileInfoConnection(param.FileInfoDBFilePath));
            DatabaseManager<ThumbnailConnection>.Connect(new ThumbnailConnection(param.ThumbnailDBFilePath));
        }
    }
}
