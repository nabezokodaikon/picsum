using PicSum.Main.Conf;
using PicSum.Task.SyncTasks;
using PicSum.UIComponent.Contents.Conf;
using System;
using System.Runtime.Versioning;

namespace PicSum.Main.Mng
{
    /// <summary>
    /// コンポーネント管理クラス
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class ComponentManager
        : IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ComponentManager()
        {
            Config.Values.Load();

            BrowserConfig.WindowState = Config.Values.WindowState;
            BrowserConfig.WindowLocaion = Config.Values.WindowLocaion;
            BrowserConfig.WindowSize = Config.Values.WindowSize;

            CommonConfig.ExportDirectoryPath = Config.Values.ExportDirectoryPath;

            FileListPageConfig.ThumbnailSize = Config.Values.ThumbnailSize;
            FileListPageConfig.IsShowFileName = Config.Values.IsShowFileName;
            FileListPageConfig.IsShowDirectory = Config.Values.IsShowDirectory;
            FileListPageConfig.IsShowImageFile = Config.Values.IsShowImageFile;
            FileListPageConfig.IsShowOtherFile = Config.Values.IsShowOtherFile;
            FileListPageConfig.FavoriteDirectoryCount = Config.Values.FavoriteDirectoryCount;

            ImageViewerPageConfig.ImageDisplayMode = Config.Values.ImageDisplayMode;
            ImageViewerPageConfig.ImageSizeMode = Config.Values.ImageSizeMode;
        }

        /// <summary>
        /// リソースを解放します。
        /// </summary>
        public void Dispose()
        {
            var closingTask = new ClosingSyncTask();
            closingTask.Execute();

            Config.Values.WindowState = BrowserConfig.WindowState;
            Config.Values.WindowLocaion = BrowserConfig.WindowLocaion;
            Config.Values.WindowSize = BrowserConfig.WindowSize;

            Config.Values.ExportDirectoryPath = CommonConfig.ExportDirectoryPath;

            Config.Values.ThumbnailSize = FileListPageConfig.ThumbnailSize;
            Config.Values.IsShowFileName = FileListPageConfig.IsShowFileName;
            Config.Values.IsShowDirectory = FileListPageConfig.IsShowDirectory;
            Config.Values.IsShowImageFile = FileListPageConfig.IsShowImageFile;
            Config.Values.IsShowOtherFile = FileListPageConfig.IsShowOtherFile;

            Config.Values.ImageDisplayMode = ImageViewerPageConfig.ImageDisplayMode;
            Config.Values.ImageSizeMode = ImageViewerPageConfig.ImageSizeMode;

            Config.Values.Save();
        }
    }
}
