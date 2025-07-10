using NLog;
using PicSum.Job.SyncJobs;
using PicSum.Main.Conf;
using PicSum.UIComponent.Contents.Conf;
using SWF.Core.Base;
using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace PicSum.Main.Mng
{
    /// <summary>
    /// コンポーネント管理クラス
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed partial class ResourceManager
        : IAsyncDisposable
    {
        private static readonly Logger LOGGER = Log.GetLogger();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ResourceManager()
        {
            BrowseConfig.Instance.WindowState = Config.Instance.WindowState;

            BrowseConfig.Instance.WindowLocaion
                = new(Config.Instance.WindowLocaionX, Config.Instance.WindowLocaionY);

            BrowseConfig.Instance.WindowSize
                = new(Config.Instance.WindowSizeWidth, Config.Instance.WindowSizeHeight);

            FileListPageConfig.Instance.ThumbnailSize = Config.Instance.ThumbnailSize;
            FileListPageConfig.Instance.IsShowFileName = Config.Instance.IsShowFileName;
            FileListPageConfig.Instance.IsShowDirectory = Config.Instance.IsShowDirectory;
            FileListPageConfig.Instance.IsShowImageFile = Config.Instance.IsShowImageFile;
            FileListPageConfig.Instance.IsShowOtherFile = Config.Instance.IsShowOtherFile;
            FileListPageConfig.Instance.FavoriteDirectoryCount = Config.Instance.FavoriteDirectoryCount;

            ImageViewPageConfig.Instance.ImageDisplayMode = Config.Instance.ImageDisplayMode;
            ImageViewPageConfig.Instance.ImageSizeMode = Config.Instance.ImageSizeMode;

            LOGGER.Info($"現在のバージョン '{Config.Instance.GetCurrentVersion()}'");

            if (CommandLineArgs.IsCleanup())
            {
                LOGGER.Info("コマンドライン引数に '--cleanup' が指定されました。");

                var thumbnailDBCleanupJob = new ThumbnailDBCleanupSyncJob();
                thumbnailDBCleanupJob.Execute();

                var startupJob = new StartupSyncJob();
                startupJob.Execute();

                var fileInfoDBCleanupJob = new FileInfoDBCleanupSyncJob();
                fileInfoDBCleanupJob.Execute();
            }
            else if (AppInfo.IsUpdated(
                Config.Instance.MajorVersion,
                Config.Instance.MinorVersion,
                Config.Instance.BuildVersion,
                Config.Instance.RevisionVersion))
            {
                LOGGER.Info($"バージョン '{Config.Instance.GetOldVersion()}' から起動されました。");

                var thumbnailDBCleanupJob = new ThumbnailDBCleanupSyncJob();
                thumbnailDBCleanupJob.Execute();

                var startupJob = new StartupSyncJob();
                startupJob.Execute();
            }
            else
            {
                var startupJob = new StartupSyncJob();
                startupJob.Execute();
            }
        }

        public async ValueTask DisposeAsync()
        {
            var closingJob = new ClosingSyncJob();
            await closingJob.Execute();

            Config.Instance.WindowState = BrowseConfig.Instance.WindowState;
            Config.Instance.WindowLocaionX = BrowseConfig.Instance.WindowLocaion.X;
            Config.Instance.WindowLocaionY = BrowseConfig.Instance.WindowLocaion.Y;
            Config.Instance.WindowSizeWidth = BrowseConfig.Instance.WindowSize.Width;
            Config.Instance.WindowSizeHeight = BrowseConfig.Instance.WindowSize.Height;
            Config.Instance.ThumbnailSize = FileListPageConfig.Instance.ThumbnailSize;
            Config.Instance.IsShowFileName = FileListPageConfig.Instance.IsShowFileName;
            Config.Instance.IsShowDirectory = FileListPageConfig.Instance.IsShowDirectory;
            Config.Instance.IsShowImageFile = FileListPageConfig.Instance.IsShowImageFile;
            Config.Instance.IsShowOtherFile = FileListPageConfig.Instance.IsShowOtherFile;
            Config.Instance.ImageDisplayMode = ImageViewPageConfig.Instance.ImageDisplayMode;
            Config.Instance.ImageSizeMode = ImageViewPageConfig.Instance.ImageSizeMode;

            Config.Instance.Save();

            GC.SuppressFinalize(this);
        }
    }
}
