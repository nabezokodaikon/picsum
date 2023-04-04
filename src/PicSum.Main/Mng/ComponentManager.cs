using System;
using PicSum.Core.Data.FileAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Main.Conf;
using PicSum.Main.Properties;
using PicSum.Task.Entity;
using PicSum.Task.SyncFacade;
using PicSum.UIComponent.Contents.Conf;

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
            CommonConfig.ExportFolderPath = Settings.Default.ExportFolderPath;

            // ファイルリストコンテンツの設定をセットします。
            FileListContentsConfig.ThumbnailSize = Settings.Default.FileListContentsThumbnailSize;
            FileListContentsConfig.IsShowFileName = Settings.Default.FileListContentsIsShowFileName;
            FileListContentsConfig.IsShowFolder = Settings.Default.FileListContentsIsShowFolder;
            FileListContentsConfig.IsShowImageFile = Settings.Default.FileListContentsIsShowImageFile;
            FileListContentsConfig.IsShowOtherFile = Settings.Default.FileListContentsIsShowOthereFile;
            FileListContentsConfig.FavoriteFolderCount = Settings.Default.FileListContentsFavoriteFolderCount;

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
            Settings.Default.ExportFolderPath = CommonConfig.ExportFolderPath;

            // ファイルリストコンテンツの設定をセットします。
            Settings.Default.FileListContentsThumbnailSize = FileListContentsConfig.ThumbnailSize;
            Settings.Default.FileListContentsIsShowFileName = FileListContentsConfig.IsShowFileName;
            Settings.Default.FileListContentsIsShowFolder = FileListContentsConfig.IsShowFolder;
            Settings.Default.FileListContentsIsShowImageFile = FileListContentsConfig.IsShowImageFile;
            Settings.Default.FileListContentsIsShowOthereFile = FileListContentsConfig.IsShowOtherFile;
            Settings.Default.FileListContentsFavoriteFolderCount = FileListContentsConfig.FavoriteFolderCount;

            // ビューアコンテンツの設定をセットします。
            Settings.Default.ImageViewerContentsImageDisplayMode = ImageViewerContentsConfig.ImageDisplayMode;
            Settings.Default.ImageViewerContentsImageSizeMode = ImageViewerContentsConfig.ImageSizeMode;

            Settings.Default.Save();
        }
    }
}
