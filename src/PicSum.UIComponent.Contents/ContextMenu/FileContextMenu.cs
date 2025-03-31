using SWF.Core.Base;
using System;
using System.ComponentModel;
using System.Drawing;
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
        private readonly ToolStripMenuItem fileActiveTabOpenMenuItem = new("Open");
        private readonly ToolStripMenuItem fileNewTabOpenMenuItem = new("Open in new tab");
        private readonly ToolStripMenuItem fileNewWindowOpenMenuItem = new("Open in new window");
        private readonly ToolStripMenuItem fileBookmarkMenuItem = new("Bookmark");

        // ファイルメニュー項目
        private readonly ToolStripMenuItem selectApplicationMenuItem = new("Open in other application");
        private readonly ToolStripMenuItem saveDirectoryOpenMenuItem = new("Open save folder");

        // フォルダメニュー項目
        private readonly ToolStripMenuItem directoryActiveTabOpenMenuItem = new("Open");
        private readonly ToolStripMenuItem directoryNewTabOpenMenuItem = new("Open in new tab");
        private readonly ToolStripMenuItem directoryNewWindowOpenMenuItem = new("Open in new window");
        private readonly ToolStripMenuItem explorerOpenMenuItem = new("Open in explorer");

        private readonly ToolStripMenuItem pathCopyMenuItem = new("Copy path");
        private readonly ToolStripMenuItem nameCopyMenuItem = new("Copy name");
        private readonly ToolStripMenuItem removeFromListMenuItem = new("Remove from list");

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisibleFileActiveTabOpenMenuItem
        {
            get
            {
                return this.fileActiveTabOpenMenuItem.Visible;
            }
            set
            {
                this.fileActiveTabOpenMenuItem.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisibleFileNewTabOpenMenuItem
        {
            get
            {
                return this.fileNewTabOpenMenuItem.Visible;
            }
            set
            {
                this.fileNewTabOpenMenuItem.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisibleFileNewWindowOpenMenuItem
        {
            get
            {
                return this.fileNewWindowOpenMenuItem.Visible;
            }
            set
            {
                this.fileNewWindowOpenMenuItem.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisibleSelectApplicationMenuItem
        {
            get
            {
                return this.selectApplicationMenuItem.Visible;
            }
            set
            {
                this.selectApplicationMenuItem.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisibleSaveDirectoryOpenMenuItem
        {
            get
            {
                return this.saveDirectoryOpenMenuItem.Visible;
            }
            set
            {
                this.saveDirectoryOpenMenuItem.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisibleDirectoryActiveTabOpenMenuItem
        {
            get
            {
                return this.directoryActiveTabOpenMenuItem.Visible;
            }
            set
            {
                this.directoryActiveTabOpenMenuItem.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisibleDirectoryNewTabOpenMenuItem
        {
            get
            {
                return this.directoryNewTabOpenMenuItem.Visible;
            }
            set
            {
                this.directoryNewTabOpenMenuItem.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisibleDirectoryNewWindowOpenMenuItem
        {
            get
            {
                return this.directoryNewWindowOpenMenuItem.Visible;
            }
            set
            {
                this.directoryNewWindowOpenMenuItem.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisibleExplorerOpenMenuItem
        {
            get
            {
                return this.explorerOpenMenuItem.Visible;
            }
            set
            {
                this.explorerOpenMenuItem.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisiblePathCopyMenuItem
        {
            get
            {
                return this.pathCopyMenuItem.Visible;
            }
            set
            {
                this.pathCopyMenuItem.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VisibleNameCopyMenuItem
        {
            get
            {
                return this.nameCopyMenuItem.Visible;
            }
            set
            {
                this.nameCopyMenuItem.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        public FileContextMenu()
        {
            if (!this.DesignMode)
            {
                this.Font = new Font(this.Font.FontFamily, 10f);

                this.ShowImageMargin = false;

                this.Items.AddRange([this.fileActiveTabOpenMenuItem,
                    this.fileNewTabOpenMenuItem,
                    this.fileNewWindowOpenMenuItem,
                    this.selectApplicationMenuItem,
                    this.saveDirectoryOpenMenuItem,
                    this.directoryActiveTabOpenMenuItem,
                    this.directoryNewTabOpenMenuItem,
                    this.directoryNewWindowOpenMenuItem,
                    this.explorerOpenMenuItem,
                    this.pathCopyMenuItem,
                    this.nameCopyMenuItem,
                    this.fileBookmarkMenuItem,
                    this.removeFromListMenuItem
                                    ]);
                this.fileActiveTabOpenMenuItem.Click += new(this.FileActiveTabOpenMenuItem_Click);
                this.fileNewTabOpenMenuItem.Click += new(this.FileNewTabOpenMenuItem_Click);
                this.fileNewWindowOpenMenuItem.Click += new(this.FileNewWindowOpenMenuItem_Click);
                this.selectApplicationMenuItem.Click += new(this.SelectApplication_Click);
                this.saveDirectoryOpenMenuItem.Click += new(this.SaveDirectoryOpen_Click);
                this.directoryActiveTabOpenMenuItem.Click += new(this.DirectoryActiveTabOpenMenuItem_Click);
                this.directoryNewTabOpenMenuItem.Click += new(this.DirectoryNewTabOpenMenuItem_Click);
                this.directoryNewWindowOpenMenuItem.Click += new(this.DirectoryNewWindowOpenMenuItem_Click);
                this.explorerOpenMenuItem.Click += new(this.ExplorerOpenMenuItem_Click);
                this.pathCopyMenuItem.Click += new(this.PathCopyMenuItem_Click);
                this.nameCopyMenuItem.Click += new(this.NameCopyMenuItem_Click);
                this.fileBookmarkMenuItem.Click += new(this.FileBookmarkMenuItem_Click);
                this.removeFromListMenuItem.Click += new(this.RemoveFromListMenuItem_Click);
            }
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
                this.pathCopyMenuItem.Visible = true;
                this.nameCopyMenuItem.Visible = true;

                this.fileActiveTabOpenMenuItem.Visible = false;
                this.fileNewTabOpenMenuItem.Visible = false;
                this.fileNewWindowOpenMenuItem.Visible = false;
                this.selectApplicationMenuItem.Visible = false;
                this.saveDirectoryOpenMenuItem.Visible = false;

                this.directoryActiveTabOpenMenuItem.Visible = false;
                this.directoryNewTabOpenMenuItem.Visible = false;
                this.directoryNewWindowOpenMenuItem.Visible = false;
                this.explorerOpenMenuItem.Visible = false;

                this.fileBookmarkMenuItem.Visible = false;
            }
            else
            {
                var filePath = filePathList.First();

                this.pathCopyMenuItem.Visible = true;
                this.nameCopyMenuItem.Visible = true;

                var isFile = FileUtil.IsExistsFile(filePath);
                this.selectApplicationMenuItem.Visible = isFile;
                this.saveDirectoryOpenMenuItem.Visible = isFile;

                var isDirectory = FileUtil.IsExistsDirectory(filePath);
                this.directoryActiveTabOpenMenuItem.Visible = isDirectory;
                this.directoryNewTabOpenMenuItem.Visible = isDirectory;
                this.directoryNewWindowOpenMenuItem.Visible = isDirectory;
                this.explorerOpenMenuItem.Visible = isDirectory;

                var isImageFile = FileUtil.IsImageFile(filePath);
                this.fileActiveTabOpenMenuItem.Visible = isImageFile;
                this.fileNewTabOpenMenuItem.Visible = isImageFile;
                this.fileNewWindowOpenMenuItem.Visible = isImageFile;
                this.fileBookmarkMenuItem.Visible = isImageFile;
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
