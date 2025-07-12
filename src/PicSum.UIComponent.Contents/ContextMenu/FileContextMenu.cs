using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.ResourceAccessor;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.ContextMenu
{
    /// <summary>
    /// ファイルコンテキストメニュー
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class FileContextMenu
        : ContextMenuStrip
    {
        public event EventHandler<ExecuteFileEventArgs> FileActiveTabOpen;
        public event EventHandler<ExecuteFileEventArgs> FileNewTabOpen;
        public event EventHandler<ExecuteFileEventArgs> FileNewWindowOpen;
        public event EventHandler<ExecuteFileEventArgs> SelectApplication;
        public event EventHandler<ExecuteFileEventArgs> SaveDirectoryOpen;
        public event EventHandler<ExecuteFileEventArgs> DirectoryActiveTabOpen;
        public event EventHandler<ExecuteFileEventArgs> DirectoryNewTabOpen;
        public event EventHandler<ExecuteFileEventArgs> DirectoryNewWindowOpen;
        public event EventHandler<ExecuteFileEventArgs> ExplorerOpen;
        public event EventHandler<ExecuteFileEventArgs> Bookmark;

        public event EventHandler<ExecuteFileListEventArgs> PathCopy;
        public event EventHandler<ExecuteFileListEventArgs> NameCopy;
        public event EventHandler<ExecuteFileListEventArgs> RemoveFromList;

        private string[] filePathList = null;

        // 画像ファイルメニュー項目
        private readonly ToolStripMenuItem _fileActiveTabOpenMenuItem = new("Open");
        private readonly ToolStripMenuItem _fileNewTabOpenMenuItem = new("Open in new tab");
        private readonly ToolStripMenuItem _fileNewWindowOpenMenuItem = new("Open in new window");
        private readonly ToolStripMenuItem _fileBookmarkMenuItem = new("Bookmark");

        // ファイルメニュー項目
        private readonly ToolStripMenuItem _selectApplicationMenuItem = new("Open in other application");
        private readonly ToolStripMenuItem _saveDirectoryOpenMenuItem = new("Open save folder");

        // フォルダメニュー項目
        private readonly ToolStripMenuItem _directoryActiveTabOpenMenuItem = new("Open");
        private readonly ToolStripMenuItem _directoryNewTabOpenMenuItem = new("Open in new tab");
        private readonly ToolStripMenuItem _directoryNewWindowOpenMenuItem = new("Open in new window");
        private readonly ToolStripMenuItem _explorerOpenMenuItem = new("Open in explorer");

        private readonly ToolStripMenuItem _pathCopyMenuItem = new("Copy path");
        private readonly ToolStripMenuItem _nameCopyMenuItem = new("Copy name");
        private readonly ToolStripMenuItem _removeFromListMenuItem = new("Remove from list");

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisibleFileActiveTabOpenMenuItem
        {
            get
            {
                return this._fileActiveTabOpenMenuItem.Visible;
            }
            set
            {
                this._fileActiveTabOpenMenuItem.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisibleFileNewTabOpenMenuItem
        {
            get
            {
                return this._fileNewTabOpenMenuItem.Visible;
            }
            set
            {
                this._fileNewTabOpenMenuItem.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisibleFileNewWindowOpenMenuItem
        {
            get
            {
                return this._fileNewWindowOpenMenuItem.Visible;
            }
            set
            {
                this._fileNewWindowOpenMenuItem.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisibleBookmarkMenuItem
        {
            get
            {
                return this._fileBookmarkMenuItem.Visible;
            }
            set
            {
                this._fileBookmarkMenuItem.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisibleSelectApplicationMenuItem
        {
            get
            {
                return this._selectApplicationMenuItem.Visible;
            }
            set
            {
                this._selectApplicationMenuItem.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisibleSaveDirectoryOpenMenuItem
        {
            get
            {
                return this._saveDirectoryOpenMenuItem.Visible;
            }
            set
            {
                this._saveDirectoryOpenMenuItem.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisibleDirectoryActiveTabOpenMenuItem
        {
            get
            {
                return this._directoryActiveTabOpenMenuItem.Visible;
            }
            set
            {
                this._directoryActiveTabOpenMenuItem.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisibleDirectoryNewTabOpenMenuItem
        {
            get
            {
                return this._directoryNewTabOpenMenuItem.Visible;
            }
            set
            {
                this._directoryNewTabOpenMenuItem.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisibleDirectoryNewWindowOpenMenuItem
        {
            get
            {
                return this._directoryNewWindowOpenMenuItem.Visible;
            }
            set
            {
                this._directoryNewWindowOpenMenuItem.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisibleExplorerOpenMenuItem
        {
            get
            {
                return this._explorerOpenMenuItem.Visible;
            }
            set
            {
                this._explorerOpenMenuItem.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisiblePathCopyMenuItem
        {
            get
            {
                return this._pathCopyMenuItem.Visible;
            }
            set
            {
                this._pathCopyMenuItem.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisibleNameCopyMenuItem
        {
            get
            {
                return this._nameCopyMenuItem.Visible;
            }
            set
            {
                this._nameCopyMenuItem.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisibleRemoveFromListMenuItem
        {
            get
            {
                return this._removeFromListMenuItem.Visible;
            }
            set
            {
                this._removeFromListMenuItem.Visible = value;
            }
        }

        public FileContextMenu()
        {
            this.Font = Fonts.UI_FONT_14_REGULAR;

            this.ShowImageMargin = false;

            this.Items.AddRange([
                this._fileActiveTabOpenMenuItem,
                this._fileNewTabOpenMenuItem,
                this._fileNewWindowOpenMenuItem,
                this._selectApplicationMenuItem,
                this._saveDirectoryOpenMenuItem,
                this._directoryActiveTabOpenMenuItem,
                this._directoryNewTabOpenMenuItem,
                this._directoryNewWindowOpenMenuItem,
                this._explorerOpenMenuItem,
                this._pathCopyMenuItem,
                this._nameCopyMenuItem,
                this._fileBookmarkMenuItem,
                this._removeFromListMenuItem
            ]);

            this._fileActiveTabOpenMenuItem.Click += new(this.FileActiveTabOpenMenuItem_Click);
            this._fileNewTabOpenMenuItem.Click += new(this.FileNewTabOpenMenuItem_Click);
            this._fileNewWindowOpenMenuItem.Click += new(this.FileNewWindowOpenMenuItem_Click);
            this._selectApplicationMenuItem.Click += new(this.SelectApplication_Click);
            this._saveDirectoryOpenMenuItem.Click += new(this.SaveDirectoryOpen_Click);
            this._directoryActiveTabOpenMenuItem.Click += new(this.DirectoryActiveTabOpenMenuItem_Click);
            this._directoryNewTabOpenMenuItem.Click += new(this.DirectoryNewTabOpenMenuItem_Click);
            this._directoryNewWindowOpenMenuItem.Click += new(this.DirectoryNewWindowOpenMenuItem_Click);
            this._explorerOpenMenuItem.Click += new(this.ExplorerOpenMenuItem_Click);
            this._pathCopyMenuItem.Click += new(this.PathCopyMenuItem_Click);
            this._nameCopyMenuItem.Click += new(this.NameCopyMenuItem_Click);
            this._fileBookmarkMenuItem.Click += new(this.FileBookmarkMenuItem_Click);
            this._removeFromListMenuItem.Click += new(this.RemoveFromListMenuItem_Click);
        }

        public void SetFile(string[] filePathList)
        {
            ArgumentNullException.ThrowIfNull(filePathList, nameof(filePathList));

            if (filePathList.Length == 0)
            {
                throw new ArgumentException("0件のファイルリストはセットできません。", nameof(filePathList));
            }

            if (filePathList.Length > 1)
            {
                this._pathCopyMenuItem.Visible = true;
                this._nameCopyMenuItem.Visible = true;

                this._fileActiveTabOpenMenuItem.Visible = false;
                this._fileNewTabOpenMenuItem.Visible = false;
                this._fileNewWindowOpenMenuItem.Visible = false;
                this._selectApplicationMenuItem.Visible = false;
                this._saveDirectoryOpenMenuItem.Visible = false;

                this._directoryActiveTabOpenMenuItem.Visible = false;
                this._directoryNewTabOpenMenuItem.Visible = false;
                this._directoryNewWindowOpenMenuItem.Visible = false;
                this._explorerOpenMenuItem.Visible = false;

                this._fileBookmarkMenuItem.Visible = false;
            }
            else
            {
                var filePath = filePathList.First();

                this._pathCopyMenuItem.Visible = true;
                this._nameCopyMenuItem.Visible = true;

                var isFile = FileUtil.IsExistsFile(filePath);
                this._selectApplicationMenuItem.Visible = isFile;
                this._saveDirectoryOpenMenuItem.Visible = isFile;

                var isDirectory = FileUtil.IsExistsDirectory(filePath);
                this._directoryActiveTabOpenMenuItem.Visible = isDirectory;
                this._directoryNewTabOpenMenuItem.Visible = isDirectory;
                this._directoryNewWindowOpenMenuItem.Visible = isDirectory;
                this._explorerOpenMenuItem.Visible = isDirectory;

                var isImageFile = ImageUtil.IsImageFile(filePath);
                this._fileActiveTabOpenMenuItem.Visible = isImageFile;
                this._fileNewTabOpenMenuItem.Visible = isImageFile;
                this._fileNewWindowOpenMenuItem.Visible = isImageFile;
                this._fileBookmarkMenuItem.Visible = isImageFile;
            }

            this.filePathList = filePathList;
        }

        public void SetFile(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this.SetFile([filePath]);
        }

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

        private void OnSelectApplicationOpen(ExecuteFileEventArgs e)
        {
            this.SelectApplication?.Invoke(this, e);
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

        private void SelectApplication_Click(object sender, EventArgs e)
        {
            this.OnSelectApplicationOpen(new ExecuteFileEventArgs(this.filePathList.First()));
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

        private void FileBookmarkMenuItem_Click(object sender, EventArgs e)
        {
            this.OnBookmark(new ExecuteFileEventArgs(this.filePathList.First()));
        }

        private void RemoveFromListMenuItem_Click(object sender, EventArgs e)
        {
            this.OnRemoveFromList(new ExecuteFileListEventArgs(this.filePathList));
        }
    }
}
