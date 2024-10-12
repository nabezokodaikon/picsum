using PicSum.Core.DatabaseAccessor;
using PicSum.Core.Job.AsyncJob;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Job.Parameters;
using System.Runtime.Versioning;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
