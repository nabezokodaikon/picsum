using SWF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PicSum.UIComponent.Common
{
    /// <summary>
    /// ファイルコンテキストメニュー
    /// </summary>
    public class FileContextMenu : ContextMenuStrip
    {
        #region 定数・列挙

        #endregion

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
        public event EventHandler<ExecuteFileListEventArgs> AddKeep;
        public event EventHandler<ExecuteFileListEventArgs> RemoveFromList;

        #endregion

        #region インスタンス変数

        private IList<string> filePathList = null;

        private bool isFileActiveTabOpenMenuItemVisible = false;
        private bool isDirectoryActiveTabOpenMenuItemVisible = false;

        // 画像ファイルメニュー項目
        private readonly ToolStripMenuItem fileActiveTabOpenMenuItem = new ToolStripMenuItem("Open");
        private readonly ToolStripMenuItem fileNewTabOpenMenuItem = new ToolStripMenuItem("Open in a new Tab");
        private readonly ToolStripMenuItem fileNewWindowOpenMenuItem = new ToolStripMenuItem("Open in a new Window");
        private readonly ToolStripMenuItem fileBookmarkMenuItem = new ToolStripMenuItem("Bookmark");

        // ファイルメニュー項目
        private readonly ToolStripMenuItem fileOpen = new ToolStripMenuItem("Open in association with");
        private readonly ToolStripMenuItem saveDirectoryOpen = new ToolStripMenuItem("Open save folder");

        // フォルダメニュー項目
        private readonly ToolStripMenuItem directoryActiveTabOpenMenuItem = new ToolStripMenuItem("Open");
        private readonly ToolStripMenuItem directoryNewTabOpenMenuItem = new ToolStripMenuItem("Open in a new Tab");
        private readonly ToolStripMenuItem directoryNewWindowOpenMenuItem = new ToolStripMenuItem("Open in a new Window");
        private readonly ToolStripMenuItem explorerOpenMenuItem = new ToolStripMenuItem("Open in Explorer");

        private readonly ToolStripMenuItem exportMenuItem = new ToolStripMenuItem("Export");
        private readonly ToolStripMenuItem pathCopyMenuItem = new ToolStripMenuItem("Copy Path");
        private readonly ToolStripMenuItem nameCopyMenuItem = new ToolStripMenuItem("Copy Name");
        private readonly ToolStripMenuItem addKeepMenuItem = new ToolStripMenuItem("Keep");
        private readonly ToolStripMenuItem removeFromListMenuItem = new ToolStripMenuItem("Remove from list");

        #endregion

        #region パブリックプロパティ

        public bool IsFileActiveTabOpenMenuItemVisible
        {
            get
            {
                return this.isFileActiveTabOpenMenuItemVisible;
            }
            set
            {
                this.isFileActiveTabOpenMenuItemVisible = value;
            }
        }

        public bool IsDirectoryActiveTabOpenMenuItemVisible
        {
            get
            {
                return this.isDirectoryActiveTabOpenMenuItemVisible;
            }
            set
            {
                this.isDirectoryActiveTabOpenMenuItemVisible = value;
            }
        }

        public bool IsAddKeepMenuItemVisible
        {
            get
            {
                return this.addKeepMenuItem.Visible;
            }
            set
            {
                this.addKeepMenuItem.Visible = value;
            }
        }

        public bool IsRemoveFromListMenuItemVisible
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

        public bool IsBookmarkMenuItem
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

        #endregion

        #region 継承プロパティ

        #endregion

        #region プライベートプロパティ

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
            if (filePathList == null)
            {
                throw new ArgumentNullException(nameof(filePathList));
            }

            if (filePathList.Count == 0)
            {
                throw new ArgumentException("0件のファイルリストはセットできません。", nameof(filePathList));
            }

            if (filePathList.Count > 1)
            {
                this.SetImageFileMenuItemVisible(false);
                this.SetFileMenuItemVisible(false);
                this.SetDirectoryMenuItemVisible(false);
                this.exportMenuItem.Visible = filePathList.FirstOrDefault(file => !FileUtil.IsImageFile(file)) == null;
            }
            else
            {
                var filePath = filePathList.First();
                this.SetImageFileMenuItemVisible(FileUtil.IsImageFile(filePath));
                this.SetFileMenuItemVisible(FileUtil.IsFile(filePath));
                this.SetDirectoryMenuItemVisible(!FileUtil.IsFile(filePath));
            }

            this.filePathList = filePathList;
        }

        public void SetFile(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            this.SetFile(new List<string>() { filePath });
        }

        #endregion

        #region 継承メソッド

        protected virtual void OnFileActiveTabOpen(ExecuteFileEventArgs e)
        {
            if (this.FileActiveTabOpen != null)
            {
                this.FileActiveTabOpen(this, e);
            }
        }

        protected virtual void OnFileNewTabOpen(ExecuteFileEventArgs e)
        {
            if (this.FileNewTabOpen != null)
            {
                this.FileNewTabOpen(this, e);
            }
        }

        protected virtual void OnFileNewWindowOpen(ExecuteFileEventArgs e)
        {
            if (this.FileNewWindowOpen != null)
            {
                this.FileNewWindowOpen(this, e);
            }
        }

        protected virtual void OnFileOpen(ExecuteFileEventArgs e)
        {
            if (this.FileOpen != null)
            {
                this.FileOpen(this, e);
            }
        }

        protected virtual void OnSaveDirectoryOpen(ExecuteFileEventArgs e)
        {
            if (this.SaveDirectoryOpen != null)
            {
                this.SaveDirectoryOpen(this, e);
            }
        }

        protected virtual void OnDirectoryActiveTabOpen(ExecuteFileEventArgs e)
        {
            if (this.DirectoryActiveTabOpen != null)
            {
                this.DirectoryActiveTabOpen(this, e);
            }
        }

        protected virtual void OnDirectoryNewTabOpen(ExecuteFileEventArgs e)
        {
            if (this.DirectoryNewTabOpen != null)
            {
                this.DirectoryNewTabOpen(this, e);
            }
        }

        protected virtual void OnDirectoryNewWindowOpen(ExecuteFileEventArgs e)
        {
            if (this.DirectoryNewWindowOpen != null)
            {
                this.DirectoryNewWindowOpen(this, e);
            }
        }

        protected virtual void OnExplorerOpen(ExecuteFileEventArgs e)
        {
            if (this.ExplorerOpen != null)
            {
                this.ExplorerOpen(this, e);
            }
        }

        protected virtual void OnExport(ExecuteFileListEventArgs e)
        {
            if (this.Export != null)
            {
                this.Export(this, e);
            }
        }

        protected virtual void OnPathCopy(ExecuteFileListEventArgs e)
        {
            if (this.PathCopy != null)
            {
                this.PathCopy(this, e);
            }
        }

        protected virtual void OnNameCopy(ExecuteFileListEventArgs e)
        {
            if (this.NameCopy != null)
            {
                this.NameCopy(this, e);
            }
        }

        protected virtual void OnBookmark(ExecuteFileEventArgs e)
        {
            if (this.Bookmark != null)
            {
                this.Bookmark(this, e);
            }
        }

        protected virtual void OnAddKeep(ExecuteFileListEventArgs e)
        {
            if (this.AddKeep != null)
            {
                this.AddKeep(this, e);
            }
        }

        protected virtual void OnRemoveFromList(ExecuteFileListEventArgs e)
        {
            if (this.RemoveFromList != null)
            {
                this.RemoveFromList(this, e);
            }
        }

        #endregion

        #region プライベートメソッド

        private void InitializeComponent()
        {
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
                                                      this.addKeepMenuItem,
                                                      this.removeFromListMenuItem });

            this.fileActiveTabOpenMenuItem.Click += new EventHandler(this.FileActiveTabOpenMenuItem_Click);
            this.fileNewTabOpenMenuItem.Click += new EventHandler(this.FileNewTabOpenMenuItem_Click);
            this.fileNewWindowOpenMenuItem.Click += new EventHandler(this.FileNewWindowOpenMenuItem_Click);
            this.fileOpen.Click += new EventHandler(this.FileOpen_Click);
            this.saveDirectoryOpen.Click += new EventHandler(this.SaveDirectoryOpen_Click);
            this.directoryActiveTabOpenMenuItem.Click += new EventHandler(this.DirectoryActiveTabOpenMenuItem_Click);
            this.directoryNewTabOpenMenuItem.Click += new EventHandler(this.DirectoryNewTabOpenMenuItem_Click);
            this.directoryNewWindowOpenMenuItem.Click += new EventHandler(this.DirectoryNewWindowOpenMenuItem_Click);
            this.explorerOpenMenuItem.Click += new EventHandler(this.ExplorerOpenMenuItem_Click);
            this.pathCopyMenuItem.Click += new EventHandler(this.PathCopyMenuItem_Click);
            this.nameCopyMenuItem.Click += new EventHandler(this.NameCopyMenuItem_Click);
            this.exportMenuItem.Click += new EventHandler(this.ExportMenuItem_Click);
            this.fileBookmarkMenuItem.Click += new EventHandler(this.FileBookmarkMenuItem_Click);
            this.addKeepMenuItem.Click += new EventHandler(this.AddKeepMenuItem_Click);
            this.removeFromListMenuItem.Click += new EventHandler(this.RemoveFromListMenuItem_Click);
        }

        private void SetImageFileMenuItemVisible(bool isVisible)
        {
            if (this.isFileActiveTabOpenMenuItemVisible)
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
        }

        private void SetFileMenuItemVisible(bool isVisible)
        {
            this.fileOpen.Visible = isVisible;
            this.saveDirectoryOpen.Visible = isVisible;
        }

        private void SetDirectoryMenuItemVisible(bool isVisible)
        {
            if (this.isDirectoryActiveTabOpenMenuItemVisible)
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
        }

        #endregion

        #region コンテキストメニューイベント

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

        private void AddKeepMenuItem_Click(object sender, EventArgs e)
        {
            this.OnAddKeep(new ExecuteFileListEventArgs(this.filePathList));
        }

        private void RemoveFromListMenuItem_Click(object sender, EventArgs e)
        {
            this.OnRemoveFromList(new ExecuteFileListEventArgs(this.filePathList));
        }

        #endregion
    }
}
