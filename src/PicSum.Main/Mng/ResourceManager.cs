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
    [SupportedOSPlatform("windows")]
    internal sealed partial class ResourceManager
        : IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ResourceManager()
        {
            Config.Instance.Load();

            BrowserConfig.Instance.WindowState = Config.Instance.WindowState;
            BrowserConfig.Instance.WindowLocaion = Config.Instance.WindowLocaion;
            BrowserConfig.Instance.WindowSize = Config.Instance.WindowSize;

            CommonConfig.Instance.ExportDirectoryPath = Config.Instance.ExportDirectoryPath;

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
            Config.Instance.WindowLocaion = BrowserConfig.Instance.WindowLocaion;
            Config.Instance.WindowSize = BrowserConfig.Instance.WindowSize;

            Config.Instance.ExportDirectoryPath = CommonConfig.Instance.ExportDirectoryPath;

            Config.Instance.ThumbnailSize = FileListPageConfig.Instance.ThumbnailSize;
            Config.Instance.IsShowFileName = FileListPageConfig.Instance.IsShowFileName;
            Config.Instance.IsShowDirectory = FileListPageConfig.Instance.IsShowDirectory;
            Config.Instance.IsShowImageFile = FileListPageConfig.Instance.IsShowImageFile;
            Config.Instance.IsShowOtherFile = FileListPageConfig.Instance.IsShowOtherFile;

            Config.Instance.ImageDisplayMode = ImageViewerPageConfig.Instance.ImageDisplayMode;
            Config.Instance.ImageSizeMode = ImageViewerPageConfig.Instance.ImageSizeMode;

            Config.Instance.Save();
        }
    }
}
