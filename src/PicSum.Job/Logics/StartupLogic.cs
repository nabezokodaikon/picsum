using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Job.AsyncJob;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Job.Paramters;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// スタートアップ非同期ロジック
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class StartupLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {
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
