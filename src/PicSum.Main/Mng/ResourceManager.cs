using PicSum.Job.SyncJobs;
using PicSum.Main.Conf;
using PicSum.UIComponent.Contents.Conf;
using System;
using System.Runtime.Versioning;

namespace PicSum.Main.Mng
{
    /// <summary>
    /// コンポーネント管理クラス
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed partial class ResourceManager
        : IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ResourceManager()
        {
            BrowserConfig.Instance.WindowState = Config.Instance.WindowState;

            BrowserConfig.Instance.WindowLocaion
                = new(Config.Instance.WindowLocaionX, Config.Instance.WindowLocaionY);

            BrowserConfig.Instance.WindowSize
                = new(Config.Instance.WindowSizeWidth, Config.Instance.WindowSizeHeight);

            FileListPageConfig.Instance.ThumbnailSize = Config.Instance.ThumbnailSize;
            FileListPageConfig.Instance.IsShowFileName = Config.Instance.IsShowFileName;
            FileListPageConfig.Instance.IsShowDirectory = Config.Instance.IsShowDirectory;
            FileListPageConfig.Instance.IsShowImageFile = Config.Instance.IsShowImageFile;
            FileListPageConfig.Instance.IsShowOtherFile = Config.Instance.IsShowOtherFile;
            FileListPageConfig.Instance.FavoriteDirectoryCount = Config.Instance.FavoriteDirectoryCount;

            ImageViewerPageConfig.Instance.ImageDisplayMode = Config.Instance.ImageDisplayMode;
            ImageViewerPageConfig.Instance.ImageSizeMode = Config.Instance.ImageSizeMode;

            if (CommandLineArgs.IsCleanup())
            {
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

        /// <summary>
        /// リソースを解放します。
        /// </summary>
        public void Dispose()
        {
            var closingJob = new ClosingSyncJob();
            closingJob.Execute();

            Config.Instance.WindowState = BrowserConfig.Instance.WindowState;
            Config.Instance.WindowLocaionX = BrowserConfig.Instance.WindowLocaion.X;
            Config.Instance.WindowLocaionY = BrowserConfig.Instance.WindowLocaion.Y;
            Config.Instance.WindowSizeWidth = BrowserConfig.Instance.WindowSize.Width;
            Config.Instance.WindowSizeHeight = BrowserConfig.Instance.WindowSize.Height;
            Config.Instance.ThumbnailSize = FileListPageConfig.Instance.ThumbnailSize;
            Config.Instance.IsShowFileName = FileListPageConfig.Instance.IsShowFileName;
            Config.Instance.IsShowDirectory = FileListPageConfig.Instance.IsShowDirectory;
            Config.Instance.IsShowImageFile = FileListPageConfig.Instance.IsShowImageFile;
            Config.Instance.IsShowOtherFile = FileListPageConfig.Instance.IsShowOtherFile;
            Config.Instance.ImageDisplayMode = ImageViewerPageConfig.Instance.ImageDisplayMode;
            Config.Instance.ImageSizeMode = ImageViewerPageConfig.Instance.ImageSizeMode;

            Config.Instance.Save();

            GC.SuppressFinalize(this);
        }
    }
}
