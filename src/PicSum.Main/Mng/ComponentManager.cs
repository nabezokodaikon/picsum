using PicSum.Core.Task.AsyncTask;
using PicSum.Main.Conf;
using PicSum.Main.Properties;
using PicSum.Task.SyncFacade;
using PicSum.UIComponent.Contents.Conf;
using System;
using System.Windows.Forms;

namespace PicSum.Main.Mng
{
    /// <summary>
    /// コンポーネント管理クラス
    /// </summary>
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
            FileListContentsConfig.ThumbnailSize = Settings.Default.FileListContentsThumbnailSize;
            FileListContentsConfig.IsShowFileName = Settings.Default.FileListContentsIsShowFileName;
            FileListContentsConfig.IsShowDirectory = Settings.Default.FileListContentsIsShowDirectory;
            FileListContentsConfig.IsShowImageFile = Settings.Default.FileListContentsIsShowImageFile;
            FileListContentsConfig.IsShowOtherFile = Settings.Default.FileListContentsIsShowOthereFile;
            FileListContentsConfig.FavoriteDirectoryCount = Settings.Default.FileListContentsFavoriteDirectoryCount;

            // ビューアコンテンツの設定をセットします。
            ImageViewerContentsConfig.ImageDisplayMode = Settings.Default.ImageViewerContentsImageDisplayMode;
            ImageViewerContentsConfig.ImageSizeMode = Settings.Default.ImageViewerContentsImageSizeMode;
        }

        /// <summary>
        /// リソースを解放します。
        /// </summary>
        public void Dispose()
        {
            // 全ての非同期タスクが終了するまで待機します。
            while (TaskManager.TaskList.Count > 0)
            {
                Application.DoEvents();
            }

            // 終了処理を行います。
            var closingFacade = new ClosingSyncFacade();
            closingFacade.Execute();

            // ブラウザの設定を保存します。
            Settings.Default.BrowserWindowState = BrowserConfig.WindowState;
            Settings.Default.BrowserLocation = BrowserConfig.WindowLocaion;
            Settings.Default.BrowserSize = BrowserConfig.WindowSize;

            // コンテンツ共通の設定をセットします。
            Settings.Default.ExportDirectoryPath = CommonConfig.ExportDirectoryPath;

            // ファイルリストコンテンツの設定をセットします。
            Settings.Default.FileListContentsThumbnailSize = FileListContentsConfig.ThumbnailSize;
            Settings.Default.FileListContentsIsShowFileName = FileListContentsConfig.IsShowFileName;
            Settings.Default.FileListContentsIsShowDirectory = FileListContentsConfig.IsShowDirectory;
            Settings.Default.FileListContentsIsShowImageFile = FileListContentsConfig.IsShowImageFile;
            Settings.Default.FileListContentsIsShowOthereFile = FileListContentsConfig.IsShowOtherFile;

            // ビューアコンテンツの設定をセットします。
            Settings.Default.ImageViewerContentsImageDisplayMode = ImageViewerContentsConfig.ImageDisplayMode;
            Settings.Default.ImageViewerContentsImageSizeMode = ImageViewerContentsConfig.ImageSizeMode;

            Settings.Default.Save();
        }
    }
}
