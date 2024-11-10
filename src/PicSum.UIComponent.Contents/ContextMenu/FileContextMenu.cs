using SWF.Core.FileAccessor;
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
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class FileContextMenu
        : ContextMenuStrip
    {

        public event EventHandler<ExecuteFileEventArgs> FileActiveTabOpen;
        public event EventHandler<ExecuteFileEventArgs> FileNewTabOpen;
        public event EventHandler<ExecuteFileEventArgs> FileNewWindowOpen;
        public event EventHandler<ExecuteFileEventArgs> FileOpen;
        public event EventHandler<ExecuteFileEventArgs> SelectApplication;
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
        public event EventHandler<ExecuteFileListEventArgs> Clip;

        private IList<string> filePathList = null;

        // 画像ファイルメニュー項目
        private readonly ToolStripMenuItem fileActiveTabOpenMenuItem = new("Open");
        private readonly ToolStripMenuItem fileNewTabOpenMenuItem = new("Open New Tab");
        private readonly ToolStripMenuItem fileNewWindowOpenMenuItem = new("Open New Window");
        private readonly ToolStripMenuItem fileBookmarkMenuItem = new("Bookmark");

        // ファイルメニュー項目
        private readonly ToolStripMenuItem fileOpenMenuItem = new("Open In Association");
        private readonly ToolStripMenuItem selectApplicationMenuItem = new("Select and open the application");
        private readonly ToolStripMenuItem saveDirectoryOpenMenuItem = new("Open Save Folder");

        // フォルダメニュー項目
        private readonly ToolStripMenuItem directoryActiveTabOpenMenuItem = new("Open");
        private readonly ToolStripMenuItem directoryNewTabOpenMenuItem = new("Open New Tab");
        private readonly ToolStripMenuItem directoryNewWindowOpenMenuItem = new("Open New Window");
        private readonly ToolStripMenuItem explorerOpenMenuItem = new("Open In Explorer");

        private readonly ToolStripMenuItem exportMenuItem = new("Export");
        private readonly ToolStripMenuItem pathCopyMenuItem = new("Copy Path");
        private readonly ToolStripMenuItem nameCopyMenuItem = new("Copy Name");
        private readonly ToolStripMenuItem removeFromListMenuItem = new("Remove From List");
        private readonly ToolStripMenuItem clipMenuItem = new("Clip");

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

        public bool VisibleFileOpenMenuItem
        {
            get
            {
                return this.fileOpenMenuItem.Visible;
            }
            set
            {
                this.fileOpenMenuItem.Visible = value;
            }
        }

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

        public bool VisibleClipMenuItem
        {
            get
            {
                return this.clipMenuItem.Visible;
            }
            set
            {
                this.clipMenuItem.Visible = value;
            }
        }

        public FileContextMenu()
        {
            if (!this.DesignMode)
            {
                this.ShowImageMargin = false;

                this.Items.AddRange([this.fileActiveTabOpenMenuItem,
                    this.fileNewTabOpenMenuItem,
                    this.fileNewWindowOpenMenuItem,
                    this.fileOpenMenuItem,
                    this.selectApplicationMenuItem,
                    this.saveDirectoryOpenMenuItem,
                    this.directoryActiveTabOpenMenuItem,
                    this.directoryNewTabOpenMenuItem,
                    this.directoryNewWindowOpenMenuItem,
                    this.explorerOpenMenuItem,
                    this.pathCopyMenuItem,
                    this.nameCopyMenuItem,
                    this.exportMenuItem,
                    this.fileBookmarkMenuItem,
                    this.clipMenuItem,
                    this.removeFromListMenuItem
                                    ]);
                this.fileActiveTabOpenMenuItem.Click += new(this.FileActiveTabOpenMenuItem_Click);
                this.fileNewTabOpenMenuItem.Click += new(this.FileNewTabOpenMenuItem_Click);
                this.fileNewWindowOpenMenuItem.Click += new(this.FileNewWindowOpenMenuItem_Click);
                this.fileOpenMenuItem.Click += new(this.FileOpen_Click);
                this.selectApplicationMenuItem.Click += new(this.SelectApplication_Click);
                this.saveDirectoryOpenMenuItem.Click += new(this.SaveDirectoryOpen_Click);
                this.directoryActiveTabOpenMenuItem.Click += new(this.DirectoryActiveTabOpenMenuItem_Click);
                this.directoryNewTabOpenMenuItem.Click += new(this.DirectoryNewTabOpenMenuItem_Click);
                this.directoryNewWindowOpenMenuItem.Click += new(this.DirectoryNewWindowOpenMenuItem_Click);
                this.explorerOpenMenuItem.Click += new(this.ExplorerOpenMenuItem_Click);
                this.pathCopyMenuItem.Click += new(this.PathCopyMenuItem_Click);
                this.nameCopyMenuItem.Click += new(this.NameCopyMenuItem_Click);
                this.exportMenuItem.Click += new(this.ExportMenuItem_Click);
                this.fileBookmarkMenuItem.Click += new(this.FileBookmarkMenuItem_Click);
                this.removeFromListMenuItem.Click += new(this.RemoveFromListMenuItem_Click);
                this.clipMenuItem.Click += new(this.ClipMenuItem_Click);
            }
        }

        public void SetFile(IList<string> filePathList)
        {
            ArgumentNullException.ThrowIfNull(filePathList, nameof(filePathList));

            if (filePathList.Count == 0)
            {
                throw new ArgumentException("0件のファイルリストはセットできません。", nameof(filePathList));
            }

            if (filePathList.Count > 1)
            {
                this.pathCopyMenuItem.Visible = true;
                this.nameCopyMenuItem.Visible = true;

                this.fileActiveTabOpenMenuItem.Visible = false;
                this.fileNewTabOpenMenuItem.Visible = false;
                this.fileNewWindowOpenMenuItem.Visible = false;
                this.fileOpenMenuItem.Visible = false;
                this.selectApplicationMenuItem.Visible = false;
                this.saveDirectoryOpenMenuItem.Visible = false;

                this.directoryActiveTabOpenMenuItem.Visible = false;
                this.directoryNewTabOpenMenuItem.Visible = false;
                this.directoryNewWindowOpenMenuItem.Visible = false;
                this.explorerOpenMenuItem.Visible = false;

                this.fileBookmarkMenuItem.Visible = false;

                var isAllImageFiles = filePathList
                    .Where(_ => FileUtil.IsImageFile(_))
                    .Count() == filePathList.Count;
                this.exportMenuItem.Visible = isAllImageFiles;
            }
            else
            {
                var filePath = filePathList.First();

                this.pathCopyMenuItem.Visible = true;
                this.nameCopyMenuItem.Visible = true;
                this.clipMenuItem.Visible = true;

                var isFile = FileUtil.IsFile(filePath);
                this.fileOpenMenuItem.Visible = isFile;
                this.selectApplicationMenuItem.Visible = isFile;
                this.saveDirectoryOpenMenuItem.Visible = isFile;

                var isDirectory = FileUtil.IsDirectory(filePath);
                this.directoryActiveTabOpenMenuItem.Visible = isDirectory;
                this.directoryNewTabOpenMenuItem.Visible = isDirectory;
                this.directoryNewWindowOpenMenuItem.Visible = isDirectory;
                this.explorerOpenMenuItem.Visible = isDirectory;

                var isImageFile = FileUtil.IsImageFile(filePath);
                this.fileActiveTabOpenMenuItem.Visible = isImageFile;
                this.fileNewTabOpenMenuItem.Visible = isImageFile;
                this.fileNewWindowOpenMenuItem.Visible = isImageFile;
                this.fileBookmarkMenuItem.Visible = isImageFile;
                this.exportMenuItem.Visible = isImageFile;
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

        private void OnFileOpen(ExecuteFileEventArgs e)
        {
            this.FileOpen?.Invoke(this, e);
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

        private void OnClip(ExecuteFileListEventArgs e)
        {
            this.Clip?.Invoke(this, e);
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

        private void ClipMenuItem_Click(object sender, EventArgs e)
        {
            this.OnClip(new ExecuteFileListEventArgs(this.filePathList));
        }
    }
}
