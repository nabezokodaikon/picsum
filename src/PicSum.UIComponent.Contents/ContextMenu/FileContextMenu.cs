using SWF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.ContextMenu
{
    /// <summary>
    /// ファイルコンテキストメニュー
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class FileContextMenu
        : ContextMenuStrip
    {
        #region イベント・デリゲート

        public event EventHandler<ExecuteFileEventArgs> FileActiveTabOpen;
        public event EventHandler<ExecuteFileEventArgs> FileNewTabOpen;
        public event EventHandler<ExecuteFileEventArgs> FileNewWindowOpen;
        public event EventHandler<ExecuteFileEventArgs> FileOpen;
        public event EventHandler<ExecuteFileEventArgs> SaveDirectoryOpen;
        public event EventHandler<ExecuteFileEventArgs> DirectoryActiveTabOpen;
        public event EventHandler<ExecuteFileEventArgs> DirectoryNewTabOpen;
        public event EventHandler<ExecuteFileEventArgs> DirectoryNewWindowOpen;
        public event EventHandler<ExecuteFileEventArgs> ExplorerOpen;
        public event EventHandler<ExecuteFileEventArgs> Bookmark;

        public event EventHandler<ExecuteFileListEventArgs> Export;
        public event EventHandler<ExecuteFileListEventArgs> PathCopy;
        public event EventHandler<ExecuteFileListEventArgs> NameCopy;
        public event EventHandler<ExecuteFileListEventArgs> RemoveFromList;

        #endregion

        #region インスタンス変数

        private IList<string> filePathList = null;

        private bool visibleFileActiveTabOpenMenuItem = false;
        private bool visibleDirectoryActiveTabOpenMenuItem = false;

        // 画像ファイルメニュー項目
        private readonly ToolStripMenuItem fileActiveTabOpenMenuItem = new("Open");
        private readonly ToolStripMenuItem fileNewTabOpenMenuItem = new("Open New Tab");
        private readonly ToolStripMenuItem fileNewWindowOpenMenuItem = new("Open New Window");
        private readonly ToolStripMenuItem fileBookmarkMenuItem = new("Bookmark");

        // ファイルメニュー項目
        private readonly ToolStripMenuItem fileOpen = new("Open In Association");
        private readonly ToolStripMenuItem saveDirectoryOpen = new("Open Save Folder");

        // フォルダメニュー項目
        private readonly ToolStripMenuItem directoryActiveTabOpenMenuItem = new("Open");
        private readonly ToolStripMenuItem directoryNewTabOpenMenuItem = new("Open New Tab");
        private readonly ToolStripMenuItem directoryNewWindowOpenMenuItem = new("Open New Window");
        private readonly ToolStripMenuItem explorerOpenMenuItem = new("Open In Explorer");

        private readonly ToolStripMenuItem exportMenuItem = new("Export");
        private readonly ToolStripMenuItem pathCopyMenuItem = new("Copy Path");
        private readonly ToolStripMenuItem nameCopyMenuItem = new("Copy Name");
        private readonly ToolStripMenuItem removeFromListMenuItem = new("Remove");

        #endregion

        #region パブリックプロパティ

        public bool VisibleFileActiveTabOpenMenuItem
        {
            get
            {
                return this.visibleFileActiveTabOpenMenuItem;
            }
            set
            {
                this.visibleFileActiveTabOpenMenuItem = value;
            }
        }

        public bool VisibleDirectoryActiveTabOpenMenuItem
        {
            get
            {
                return this.visibleDirectoryActiveTabOpenMenuItem;
            }
            set
            {
                this.visibleDirectoryActiveTabOpenMenuItem = value;
            }
        }

        public bool VisibleRemoveFromListMenuItem
        {
            get
            {
                return this.removeFromListMenuItem.Visible;
            }
            set
            {
                this.removeFromListMenuItem.Visible = value;
            }
        }

        public bool VisibleBookmarkMenuItem
        {
            get
            {
                return this.fileBookmarkMenuItem.Visible;
            }
            set
            {
                this.fileBookmarkMenuItem.Visible = value;
            }
        }

        public bool VisibleExportMenuItem
        {
            get
            {
                return this.exportMenuItem.Visible;
            }
            set
            {
                this.exportMenuItem.Visible = value;
            }
        }

        #endregion

        #region コンストラクタ

        public FileContextMenu()
        {
            if (!this.DesignMode)
            {
                this.InitializeComponent();
            }
        }

        #endregion

        #region パブリックメソッド

        public void SetFile(IList<string> filePathList)
        {
            ArgumentNullException.ThrowIfNull(filePathList, nameof(filePathList));

            if (filePathList.Count == 0)
            {
                throw new ArgumentException("0件のファイルリストはセットできません。", nameof(filePathList));
            }

            if (filePathList.Count > 1)
            {
                this.SetDirectoryMenuItemVisible(false);
                this.SetFileMenuItemVisible(false);
                this.SetImageFilesMenuItemVisible(
                    filePathList.Where(file => FileUtil.IsImageFile(file)).Count() == filePathList.Count);
            }
            else
            {
                var filePath = filePathList.First();
                this.SetDirectoryMenuItemVisible(!FileUtil.IsFile(filePath));
                this.SetFileMenuItemVisible(FileUtil.IsFile(filePath));
                this.SetImageFileMenuItemVisible(FileUtil.IsImageFile(filePath));
            }

            this.filePathList = filePathList;
        }

        public void SetFile(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this.SetFile(new List<string>() { filePath });
        }

        #endregion

        #region プライベートメソッド

        private void InitializeComponent()
        {
            this.ShowImageMargin = false;

            this.Items.AddRange(new ToolStripItem[] { this.fileActiveTabOpenMenuItem,
                                                      this.fileNewTabOpenMenuItem,
                                                      this.fileNewWindowOpenMenuItem,
                                                      this.fileOpen,
                                                      this.saveDirectoryOpen,
                                                      this.directoryActiveTabOpenMenuItem,
                                                      this.directoryNewTabOpenMenuItem,
                                                      this.directoryNewWindowOpenMenuItem,
                                                      this.explorerOpenMenuItem,
                                                      this.pathCopyMenuItem,
                                                      this.nameCopyMenuItem,
                                                      this.exportMenuItem,
                                                      this.fileBookmarkMenuItem,
                                                      this.removeFromListMenuItem
                                                    });
            this.fileActiveTabOpenMenuItem.Click += new(this.FileActiveTabOpenMenuItem_Click);
            this.fileNewTabOpenMenuItem.Click += new(this.FileNewTabOpenMenuItem_Click);
            this.fileNewWindowOpenMenuItem.Click += new(this.FileNewWindowOpenMenuItem_Click);
            this.fileOpen.Click += new(this.FileOpen_Click);
            this.saveDirectoryOpen.Click += new(this.SaveDirectoryOpen_Click);
            this.directoryActiveTabOpenMenuItem.Click += new(this.DirectoryActiveTabOpenMenuItem_Click);
            this.directoryNewTabOpenMenuItem.Click += new(this.DirectoryNewTabOpenMenuItem_Click);
            this.directoryNewWindowOpenMenuItem.Click += new(this.DirectoryNewWindowOpenMenuItem_Click);
            this.explorerOpenMenuItem.Click += new(this.ExplorerOpenMenuItem_Click);
            this.pathCopyMenuItem.Click += new(this.PathCopyMenuItem_Click);
            this.nameCopyMenuItem.Click += new(this.NameCopyMenuItem_Click);
            this.exportMenuItem.Click += new(this.ExportMenuItem_Click);
            this.fileBookmarkMenuItem.Click += new(this.FileBookmarkMenuItem_Click);
            this.removeFromListMenuItem.Click += new(this.RemoveFromListMenuItem_Click);
        }

        private void SetImageFileMenuItemVisible(bool isVisible)
        {
            if (this.visibleFileActiveTabOpenMenuItem)
            {
                this.fileActiveTabOpenMenuItem.Visible = isVisible;
            }
            else
            {
                this.fileActiveTabOpenMenuItem.Visible = false;
            }

            this.fileNewTabOpenMenuItem.Visible = isVisible;
            this.fileNewWindowOpenMenuItem.Visible = isVisible;
            this.exportMenuItem.Visible = isVisible;
            this.fileBookmarkMenuItem.Visible = isVisible;
            this.fileOpen.Visible = isVisible;
        }

        private void SetImageFilesMenuItemVisible(bool isVisible)
        {
            this.fileActiveTabOpenMenuItem.Visible = false;
            this.fileNewTabOpenMenuItem.Visible = false;
            this.fileNewWindowOpenMenuItem.Visible = false;
            this.exportMenuItem.Visible = isVisible;
            this.fileBookmarkMenuItem.Visible = false;
            this.fileOpen.Visible = false;
        }

        private void SetFileMenuItemVisible(bool isVisible)
        {
            this.saveDirectoryOpen.Visible = isVisible;
        }

        private void SetDirectoryMenuItemVisible(bool isVisible)
        {
            if (this.visibleDirectoryActiveTabOpenMenuItem)
            {
                this.directoryActiveTabOpenMenuItem.Visible = isVisible;
            }
            else
            {
                this.directoryActiveTabOpenMenuItem.Visible = false;
            }

            this.directoryNewTabOpenMenuItem.Visible = isVisible;
            this.directoryNewWindowOpenMenuItem.Visible = isVisible;
            this.explorerOpenMenuItem.Visible = isVisible;
            this.exportMenuItem.Visible = isVisible;
        }

        #endregion

        #region コンテキストメニューイベント

        private void OnFileActiveTabOpen(ExecuteFileEventArgs e)
        {
            this.FileActiveTabOpen?.Invoke(this, e);
        }

        private void OnFileNewTabOpen(ExecuteFileEventArgs e)
        {
            this.FileNewTabOpen?.Invoke(this, e);
        }

        private void OnFileNewWindowOpen(ExecuteFileEventArgs e)
        {
            this.FileNewWindowOpen?.Invoke(this, e);
        }

        private void OnFileOpen(ExecuteFileEventArgs e)
        {
            this.FileOpen?.Invoke(this, e);
        }

        private void OnSaveDirectoryOpen(ExecuteFileEventArgs e)
        {
            this.SaveDirectoryOpen?.Invoke(this, e);
        }

        private void OnDirectoryActiveTabOpen(ExecuteFileEventArgs e)
        {
            this.DirectoryActiveTabOpen?.Invoke(this, e);
        }

        private void OnDirectoryNewTabOpen(ExecuteFileEventArgs e)
        {
            this.DirectoryNewTabOpen?.Invoke(this, e);
        }

        private void OnDirectoryNewWindowOpen(ExecuteFileEventArgs e)
        {
            this.DirectoryNewWindowOpen?.Invoke(this, e);
        }

        private void OnExplorerOpen(ExecuteFileEventArgs e)
        {
            this.ExplorerOpen?.Invoke(this, e);
        }

        private void OnExport(ExecuteFileListEventArgs e)
        {
            this.Export?.Invoke(this, e);
        }

        private void OnPathCopy(ExecuteFileListEventArgs e)
        {
            this.PathCopy?.Invoke(this, e);
        }

        private void OnNameCopy(ExecuteFileListEventArgs e)
        {
            this.NameCopy?.Invoke(this, e);
        }

        private void OnBookmark(ExecuteFileEventArgs e)
        {
            this.Bookmark?.Invoke(this, e);
        }

        private void OnRemoveFromList(ExecuteFileListEventArgs e)
        {
            this.RemoveFromList?.Invoke(this, e);
        }

        private void FileActiveTabOpenMenuItem_Click(object sender, EventArgs e)
        {
            this.OnFileActiveTabOpen(new ExecuteFileEventArgs(this.filePathList.First()));
        }

        private void FileNewTabOpenMenuItem_Click(object sender, EventArgs e)
        {
            this.OnFileNewTabOpen(new ExecuteFileEventArgs(this.filePathList.First()));
        }

        private void FileNewWindowOpenMenuItem_Click(object sender, EventArgs e)
        {
            this.OnFileNewWindowOpen(new ExecuteFileEventArgs(this.filePathList.First()));
        }

        private void FileOpen_Click(object sender, EventArgs e)
        {
            this.OnFileOpen(new ExecuteFileEventArgs(this.filePathList.First()));
        }

        private void SaveDirectoryOpen_Click(object sender, EventArgs e)
        {
            this.OnSaveDirectoryOpen(new ExecuteFileEventArgs(this.filePathList.First()));
        }

        private void DirectoryActiveTabOpenMenuItem_Click(object sender, EventArgs e)
        {
            this.OnDirectoryActiveTabOpen(new ExecuteFileEventArgs(this.filePathList.First()));
        }

        private void DirectoryNewTabOpenMenuItem_Click(object sender, EventArgs e)
        {
            this.OnDirectoryNewTabOpen(new ExecuteFileEventArgs(this.filePathList.First()));
        }

        private void DirectoryNewWindowOpenMenuItem_Click(object sender, EventArgs e)
        {
            this.OnDirectoryNewWindowOpen(new ExecuteFileEventArgs(this.filePathList.First()));
        }

        private void ExplorerOpenMenuItem_Click(object sender, EventArgs e)
        {
            this.OnExplorerOpen(new ExecuteFileEventArgs(this.filePathList.First()));
        }

        private void PathCopyMenuItem_Click(object sender, EventArgs e)
        {
            this.OnPathCopy(new ExecuteFileListEventArgs(this.filePathList));
        }

        private void NameCopyMenuItem_Click(object sender, EventArgs e)
        {
            this.OnNameCopy(new ExecuteFileListEventArgs(this.filePathList));
        }

        private void ExportMenuItem_Click(object sender, EventArgs e)
        {
            this.OnExport(new ExecuteFileListEventArgs(this.filePathList));
        }

        private void FileBookmarkMenuItem_Click(object sender, EventArgs e)
        {
            this.OnBookmark(new ExecuteFileEventArgs(this.filePathList.First()));
        }

        private void RemoveFromListMenuItem_Click(object sender, EventArgs e)
        {
            this.OnRemoveFromList(new ExecuteFileListEventArgs(this.filePathList));
        }

        #endregion
    }
}
