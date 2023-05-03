using PicSum.UIComponent.Contents.Properties;
using SWF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.ContextMenu
{
    /// <summary>
    /// ファイルコンテキストメニュー
    /// </summary>
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
        private readonly ToolStripMenuItem fileActiveTabOpenMenuItem = new ToolStripMenuItem("Open");
        private readonly ToolStripMenuItem fileNewTabOpenMenuItem = new ToolStripMenuItem("Open New Tab");
        private readonly ToolStripMenuItem fileNewWindowOpenMenuItem = new ToolStripMenuItem("Open New Window");
        private readonly ToolStripMenuItem fileBookmarkMenuItem = new ToolStripMenuItem("Bookmark");

        // ファイルメニュー項目
        private readonly ToolStripMenuItem fileOpen = new ToolStripMenuItem("Open In Association");
        private readonly ToolStripMenuItem saveDirectoryOpen = new ToolStripMenuItem("Open Save Folder");

        // フォルダメニュー項目
        private readonly ToolStripMenuItem directoryActiveTabOpenMenuItem = new ToolStripMenuItem("Open");
        private readonly ToolStripMenuItem directoryNewTabOpenMenuItem = new ToolStripMenuItem("Open New Tab");
        private readonly ToolStripMenuItem directoryNewWindowOpenMenuItem = new ToolStripMenuItem("Open New Window");
        private readonly ToolStripMenuItem explorerOpenMenuItem = new ToolStripMenuItem("Open In Explorer");

        private readonly ToolStripMenuItem exportMenuItem = new ToolStripMenuItem("Export");
        private readonly ToolStripMenuItem pathCopyMenuItem = new ToolStripMenuItem("Copy Path");
        private readonly ToolStripMenuItem nameCopyMenuItem = new ToolStripMenuItem("Copy Name");
        private readonly ToolStripMenuItem removeFromListMenuItem = new ToolStripMenuItem("Remove");

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
                this.SetDirectoryMenuItemVisible(false);
                this.SetFileMenuItemVisible(false);
                var isImageFile = filePathList.FirstOrDefault(file => !FileUtil.IsImageFile(file)) == null;
                this.SetImageFileMenuItemVisible(isImageFile);
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
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            this.SetFile(new List<string>() { filePath });
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
                                                      this.removeFromListMenuItem
                                                    });

            this.fileOpen.Image = Resources.AssociationOpenIcon;
            this.fileActiveTabOpenMenuItem.Image = Resources.ImageFileOpenIcon;
            this.fileNewTabOpenMenuItem.Image = Resources.TabPlusIcon;
            this.fileNewWindowOpenMenuItem.Image = Resources.WindowOpenIcon;
            this.directoryActiveTabOpenMenuItem.Image = Resources.DirectoryOpenIcon;
            this.directoryNewTabOpenMenuItem.Image = Resources.TabPlusIcon;
            this.directoryNewWindowOpenMenuItem.Image = Resources.WindowOpenIcon;
            this.pathCopyMenuItem.Image = Resources.ClipboardIcon;
            this.nameCopyMenuItem.Image = Resources.ClipboardIcon;
            this.saveDirectoryOpen.Image = Resources.DirectoryOpenIcon;
            this.explorerOpenMenuItem.Image = Resources.DirectoryOpenIcon;
            this.exportMenuItem.Image = Resources.ExportIcon;
            this.fileBookmarkMenuItem.Image = Resources.BookmarkIcon;
            this.removeFromListMenuItem.Image = Resources.RemoveIcon;

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
            this.removeFromListMenuItem.Click += new EventHandler(this.RemoveFromListMenuItem_Click);
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
        }

        private void SetFileMenuItemVisible(bool isVisible)
        {
            this.fileOpen.Visible = isVisible;
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
            if (this.FileActiveTabOpen != null)
            {
                this.FileActiveTabOpen(this, e);
            }
        }

        private void OnFileNewTabOpen(ExecuteFileEventArgs e)
        {
            if (this.FileNewTabOpen != null)
            {
                this.FileNewTabOpen(this, e);
            }
        }

        private void OnFileNewWindowOpen(ExecuteFileEventArgs e)
        {
            if (this.FileNewWindowOpen != null)
            {
                this.FileNewWindowOpen(this, e);
            }
        }

        private void OnFileOpen(ExecuteFileEventArgs e)
        {
            if (this.FileOpen != null)
            {
                this.FileOpen(this, e);
            }
        }

        private void OnSaveDirectoryOpen(ExecuteFileEventArgs e)
        {
            if (this.SaveDirectoryOpen != null)
            {
                this.SaveDirectoryOpen(this, e);
            }
        }

        private void OnDirectoryActiveTabOpen(ExecuteFileEventArgs e)
        {
            if (this.DirectoryActiveTabOpen != null)
            {
                this.DirectoryActiveTabOpen(this, e);
            }
        }

        private void OnDirectoryNewTabOpen(ExecuteFileEventArgs e)
        {
            if (this.DirectoryNewTabOpen != null)
            {
                this.DirectoryNewTabOpen(this, e);
            }
        }

        private void OnDirectoryNewWindowOpen(ExecuteFileEventArgs e)
        {
            if (this.DirectoryNewWindowOpen != null)
            {
                this.DirectoryNewWindowOpen(this, e);
            }
        }

        private void OnExplorerOpen(ExecuteFileEventArgs e)
        {
            if (this.ExplorerOpen != null)
            {
                this.ExplorerOpen(this, e);
            }
        }

        private void OnExport(ExecuteFileListEventArgs e)
        {
            if (this.Export != null)
            {
                this.Export(this, e);
            }
        }

        private void OnPathCopy(ExecuteFileListEventArgs e)
        {
            if (this.PathCopy != null)
            {
                this.PathCopy(this, e);
            }
        }

        private void OnNameCopy(ExecuteFileListEventArgs e)
        {
            if (this.NameCopy != null)
            {
                this.NameCopy(this, e);
            }
        }

        private void OnBookmark(ExecuteFileEventArgs e)
        {
            if (this.Bookmark != null)
            {
                this.Bookmark(this, e);
            }
        }

        private void OnRemoveFromList(ExecuteFileListEventArgs e)
        {
            if (this.RemoveFromList != null)
            {
                this.RemoveFromList(this, e);
            }
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
