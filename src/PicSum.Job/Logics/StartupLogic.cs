using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Parameters;
using SWF.Core.DatabaseAccessor;
using SWF.Core.Job;
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
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            DatabaseManager<FileInfoConnection>.Connect(new FileInfoConnection(param.FileInfoDBFilePath));
            DatabaseManager<ThumbnailConnection>.Connect(new ThumbnailConnection(param.ThumbnailDBFilePath));
        }
    }
}
