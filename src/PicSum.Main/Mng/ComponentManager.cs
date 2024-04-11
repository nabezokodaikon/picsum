using PicSum.Main.Conf;
using PicSum.Main.Properties;
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
            // ブラウザの設定をセットします。
            BrowserConfig.WindowState = Settings.Default.BrowserWindowState;
            BrowserConfig.WindowLocaion = Settings.Default.BrowserLocation;
            BrowserConfig.WindowSize = Settings.Default.BrowserSize;

            // コンテンツ共通の設定をセットします。
            CommonConfig.ExportDirectoryPath = Settings.Default.ExportDirectoryPath;

            // ファイルリストコンテンツの設定をセットします。
            FileListPageConfig.ThumbnailSize = Settings.Default.FileListPageThumbnailSize;
            FileListPageConfig.IsShowFileName = Settings.Default.FileListPageIsShowFileName;
            FileListPageConfig.IsShowDirectory = Settings.Default.FileListPageIsShowDirectory;
            FileListPageConfig.IsShowImageFile = Settings.Default.FileListPageIsShowImageFile;
            FileListPageConfig.IsShowOtherFile = Settings.Default.FileListPageIsShowOthereFile;
            FileListPageConfig.FavoriteDirectoryCount = Settings.Default.FileListPageFavoriteDirectoryCount;

            // ビューアコンテンツの設定をセットします。
            ImageViewerPageConfig.ImageDisplayMode = Settings.Default.ImageViewerPageImageDisplayMode;
            ImageViewerPageConfig.ImageSizeMode = Settings.Default.ImageViewerPageImageSizeMode;
        }

        /// <summary>
        /// リソースを解放します。
        /// </summary>
        public void Dispose()
        {
            // 終了処理を行います。
            var closingTask = new ClosingSyncTask();
            closingTask.Execute();

            // ブラウザの設定を保存します。
            Settings.Default.BrowserWindowState = BrowserConfig.WindowState;
            Settings.Default.BrowserLocation = BrowserConfig.WindowLocaion;
            Settings.Default.BrowserSize = BrowserConfig.WindowSize;

            // コンテンツ共通の設定をセットします。
            Settings.Default.ExportDirectoryPath = CommonConfig.ExportDirectoryPath;

            // ファイルリストコンテンツの設定をセットします。
            Settings.Default.FileListPageThumbnailSize = FileListPageConfig.ThumbnailSize;
            Settings.Default.FileListPageIsShowFileName = FileListPageConfig.IsShowFileName;
            Settings.Default.FileListPageIsShowDirectory = FileListPageConfig.IsShowDirectory;
            Settings.Default.FileListPageIsShowImageFile = FileListPageConfig.IsShowImageFile;
            Settings.Default.FileListPageIsShowOthereFile = FileListPageConfig.IsShowOtherFile;

            // ビューアコンテンツの設定をセットします。
            Settings.Default.ImageViewerPageImageDisplayMode = ImageViewerPageConfig.ImageDisplayMode;
            Settings.Default.ImageViewerPageImageSizeMode = ImageViewerPageConfig.ImageSizeMode;

            Settings.Default.Save();
        }
    }
}
