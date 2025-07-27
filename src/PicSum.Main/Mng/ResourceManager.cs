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
            BrowseConfig.INSTANCE.WindowState = Config.INSTANCE.WindowState;

            BrowseConfig.INSTANCE.WindowLocaion
                = new(Config.INSTANCE.WindowLocaionX, Config.INSTANCE.WindowLocaionY);

            BrowseConfig.INSTANCE.WindowSize
                = new(Config.INSTANCE.WindowSizeWidth, Config.INSTANCE.WindowSizeHeight);

            FileListPageConfig.INSTANCE.ThumbnailSize = Config.INSTANCE.ThumbnailSize;
            FileListPageConfig.INSTANCE.IsShowFileName = Config.INSTANCE.IsShowFileName;
            FileListPageConfig.INSTANCE.IsShowDirectory = Config.INSTANCE.IsShowDirectory;
            FileListPageConfig.INSTANCE.IsShowImageFile = Config.INSTANCE.IsShowImageFile;
            FileListPageConfig.INSTANCE.IsShowOtherFile = Config.INSTANCE.IsShowOtherFile;
            FileListPageConfig.INSTANCE.FavoriteDirectoryCount = Config.INSTANCE.FavoriteDirectoryCount;

            ImageViewPageConfig.INSTANCE.ImageDisplayMode = Config.INSTANCE.ImageDisplayMode;
            ImageViewPageConfig.INSTANCE.ImageSizeMode = Config.INSTANCE.ImageSizeMode;

            var configVersion = new Version(
                Config.INSTANCE.MajorVersion,
                Config.INSTANCE.MinorVersion,
                Config.INSTANCE.BuildVersion,
                Config.INSTANCE.RevisionVersion);

            LOGGER.Info($"コンフィグのバージョン '{configVersion}'");
            LOGGER.Info($"現在のバージョン '{AppInfo.CURRENT_VERSION}'");

            var updater = new Updater();
            updater.VersionUpTo_12_0_0_0(configVersion);
            updater.VersionUpTo_12_2_1_0(configVersion);
            updater.VersionUpTo_12_2_2_0(configVersion);
            updater.VersionUpTo_12_3_0_0(configVersion);

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

            Config.INSTANCE.WindowState = BrowseConfig.INSTANCE.WindowState;
            Config.INSTANCE.WindowLocaionX = BrowseConfig.INSTANCE.WindowLocaion.X;
            Config.INSTANCE.WindowLocaionY = BrowseConfig.INSTANCE.WindowLocaion.Y;
            Config.INSTANCE.WindowSizeWidth = BrowseConfig.INSTANCE.WindowSize.Width;
            Config.INSTANCE.WindowSizeHeight = BrowseConfig.INSTANCE.WindowSize.Height;
            Config.INSTANCE.ThumbnailSize = FileListPageConfig.INSTANCE.ThumbnailSize;
            Config.INSTANCE.IsShowFileName = FileListPageConfig.INSTANCE.IsShowFileName;
            Config.INSTANCE.IsShowDirectory = FileListPageConfig.INSTANCE.IsShowDirectory;
            Config.INSTANCE.IsShowImageFile = FileListPageConfig.INSTANCE.IsShowImageFile;
            Config.INSTANCE.IsShowOtherFile = FileListPageConfig.INSTANCE.IsShowOtherFile;
            Config.INSTANCE.ImageDisplayMode = ImageViewPageConfig.INSTANCE.ImageDisplayMode;
            Config.INSTANCE.ImageSizeMode = ImageViewPageConfig.INSTANCE.ImageSizeMode;

            Config.INSTANCE.Save();

            GC.SuppressFinalize(this);
        }
    }
}
