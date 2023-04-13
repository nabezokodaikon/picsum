using System;
using PicSum.Core.Task.AsyncTask;
using PicSum.Main.Conf;
using PicSum.Main.Properties;
using PicSum.Task.Entity;
using PicSum.Task.SyncFacade;
using PicSum.UIComponent.Contents.Conf;
using SWF.Common;

namespace PicSum.Main.Mng
{
    /// <summary>
    /// コンポーネント管理クラス
    /// </summary>
    class ComponentManager : IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ComponentManager()
        {
            // ファイルアイコンのキャッシュを初期化します。
            FileIconCash.Init();

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
            while (TaskManager.GetTaskList().Count > 0)
            {
                System.Windows.Forms.Application.DoEvents();
            }

            // 終了処理を行います。
            ClosingSyncFacade closingFacade = new ClosingSyncFacade();
            ClosingParameterEntity param = new ClosingParameterEntity();
            closingFacade.Execute(param);

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
