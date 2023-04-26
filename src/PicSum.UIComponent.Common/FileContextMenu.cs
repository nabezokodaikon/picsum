using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SWF.Common;

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

        public event EventHandler<ExecuteFileListEventArgs> Export;
        public event EventHandler<ExecuteFileListEventArgs> PathCopy;
        public event EventHandler<ExecuteFileListEventArgs> NameCopy;
        public event EventHandler<ExecuteFileListEventArgs> AddKeep;
        public event EventHandler<ExecuteFileListEventArgs> RemoveFromList;

        #endregion

        #region インスタンス変数

        private IList<string> _filePathList = null;

        private bool _isFileActiveTabOpenMenuItemVisible = false;
        private bool _isDirectoryActiveTabOpenMenuItemVisible = false;

        // 画像ファイルメニュー項目
        private ToolStripMenuItem _fileActiveTabOpenMenuItem = new ToolStripMenuItem("Open");
        private ToolStripMenuItem _fileNewTabOpenMenuItem = new ToolStripMenuItem("Open in a new Tab");
        private ToolStripMenuItem _fileNewWindowOpenMenuItem = new ToolStripMenuItem("Open in a new Window");

        // ファイルメニュー項目
        private ToolStripMenuItem _fileOpen = new ToolStripMenuItem("Open in association with");
        private ToolStripMenuItem _saveDirectoryOpen = new ToolStripMenuItem("Open save folder");

        // フォルダメニュー項目
        private ToolStripMenuItem _directoryActiveTabOpenMenuItem = new ToolStripMenuItem("Open");
        private ToolStripMenuItem _directoryNewTabOpenMenuItem = new ToolStripMenuItem("Open in a new Tab");
        private ToolStripMenuItem _directoryNewWindowOpenMenuItem = new ToolStripMenuItem("Open in a new Window");
        private ToolStripMenuItem _explorerOpenMenuItem = new ToolStripMenuItem("Open in Explorer");

        private ToolStripMenuItem _exportMenuItem = new ToolStripMenuItem("Export");
        private ToolStripMenuItem _pathCopyMenuItem = new ToolStripMenuItem("Copy Path");
        private ToolStripMenuItem _nameCopyMenuItem = new ToolStripMenuItem("Copy Name");
        private ToolStripMenuItem _addKeepMenuItem = new ToolStripMenuItem("Keeps");
        private ToolStripMenuItem _removeFromListMenuItem = new ToolStripMenuItem("Remove from list");

        #endregion

        #region パブリックプロパティ

        public bool IsFileActiveTabOpenMenuItemVisible
        {
            get
            {
                return _isFileActiveTabOpenMenuItemVisible;
            }
            set
            {
                _isFileActiveTabOpenMenuItemVisible = value;
            }
        }

        public bool IsDirectoryActiveTabOpenMenuItemVisible
        {
            get
            {
                return _isDirectoryActiveTabOpenMenuItemVisible;
            }
            set
            {
                _isDirectoryActiveTabOpenMenuItemVisible = value;
            }
        }

        public bool IsAddKeepMenuItemVisible
        {
            get
            {
                return _addKeepMenuItem.Visible;
            }
            set
            {
                _addKeepMenuItem.Visible = value;
            }
        }

        public bool IsRemoveFromListMenuItemVisible
        {
            get
            {
                return _removeFromListMenuItem.Visible;
            }
            set
            {
                _removeFromListMenuItem.Visible = value;
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
                initializeComponent();
            }
        }

        #endregion

        #region パブリックメソッド

        public void SetFile(IList<string> filePathList)
        {
            if (filePathList == null)
            {
                throw new ArgumentNullException("filePathList");
            }

            if (filePathList.Count == 0)
            {
                throw new ArgumentException("0件のファイルリストはセットできません。", "filePathList");
            }

            if (filePathList.Count > 1)
            {
                setImageFileMenuItemVisible(false);
                setFileMenuItemVisible(false);
                setDirectoryMenuItemVisible(false);
                _exportMenuItem.Visible = filePathList.FirstOrDefault(file => !FileUtil.IsImageFile(file)) == null;
            }
            else
            {
                string filePath = filePathList.First();
                setImageFileMenuItemVisible(FileUtil.IsImageFile(filePath));
                setFileMenuItemVisible(FileUtil.IsFile(filePath));
                setDirectoryMenuItemVisible(!FileUtil.IsFile(filePath));
            }

            _filePathList = filePathList;
        }

        public void SetFile(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            SetFile(new List<string>() { filePath });
        }

        #endregion

        #region 継承メソッド

        protected virtual void OnFileActiveTabOpen(ExecuteFileEventArgs e)
        {
            if (FileActiveTabOpen != null)
            {
                FileActiveTabOpen(this, e);
            }
        }

        protected virtual void OnFileNewTabOpen(ExecuteFileEventArgs e)
        {
            if (FileNewTabOpen != null)
            {
                FileNewTabOpen(this, e);
            }
        }

        protected virtual void OnFileNewWindowOpen(ExecuteFileEventArgs e)
        {
            if (FileNewWindowOpen != null)
            {
                FileNewWindowOpen(this, e);
            }
        }

        protected virtual void OnFileOpen(ExecuteFileEventArgs e)
        {
            if (FileOpen != null)
            {
                FileOpen(this, e);
            }
        }

        protected virtual void OnSaveDirectoryOpen(ExecuteFileEventArgs e)
        {
            if (SaveDirectoryOpen != null)
            {
                SaveDirectoryOpen(this, e);
            }
        }

        protected virtual void OnDirectoryActiveTabOpen(ExecuteFileEventArgs e)
        {
            if (DirectoryActiveTabOpen != null)
            {
                DirectoryActiveTabOpen(this, e);
            }
        }

        protected virtual void OnDirectoryNewTabOpen(ExecuteFileEventArgs e)
        {
            if (DirectoryNewTabOpen != null)
            {
                DirectoryNewTabOpen(this, e);
            }
        }

        protected virtual void OnDirectoryNewWindowOpen(ExecuteFileEventArgs e)
        {
            if (DirectoryNewWindowOpen != null)
            {
                DirectoryNewWindowOpen(this, e);
            }
        }

        protected virtual void OnExplorerOpen(ExecuteFileEventArgs e)
        {
            if (ExplorerOpen != null)
            {
                ExplorerOpen(this, e);
            }
        }

        protected virtual void OnExport(ExecuteFileListEventArgs e)
        {
            if (Export != null)
            {
                Export(this, e);
            }
        }

        protected virtual void OnPathCopy(ExecuteFileListEventArgs e)
        {
            if (PathCopy != null)
            {
                PathCopy(this, e);
            }
        }

        protected virtual void OnNameCopy(ExecuteFileListEventArgs e)
        {
            if (NameCopy != null)
            {
                NameCopy(this, e);
            }
        }

        protected virtual void OnAddKeep(ExecuteFileListEventArgs e)
        {
            if (AddKeep != null)
            {
                AddKeep(this, e);
            }
        }

        protected virtual void OnRemoveFromList(ExecuteFileListEventArgs e)
        {
            if (RemoveFromList != null)
            {
                RemoveFromList(this, e);
            }
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            this.Items.AddRange(new ToolStripItem[] { _fileActiveTabOpenMenuItem,
                                                      _fileNewTabOpenMenuItem,
                                                      _fileNewWindowOpenMenuItem,
                                                      _fileOpen,
                                                      _saveDirectoryOpen,
                                                      _directoryActiveTabOpenMenuItem,
                                                      _directoryNewTabOpenMenuItem,
                                                      _directoryNewWindowOpenMenuItem,
                                                      _explorerOpenMenuItem,
                                                      _pathCopyMenuItem,
                                                      _nameCopyMenuItem,
                                                      _exportMenuItem,
                                                      _addKeepMenuItem,
                                                      _removeFromListMenuItem });

            _fileActiveTabOpenMenuItem.Click += new EventHandler(_fileActiveTabOpenMenuItem_Click);
            _fileNewTabOpenMenuItem.Click += new EventHandler(_fileNewTabOpenMenuItem_Click);
            _fileNewWindowOpenMenuItem.Click += new EventHandler(_fileNewWindowOpenMenuItem_Click);
            _fileOpen.Click += new EventHandler(_fileOpen_Click);
            _saveDirectoryOpen.Click += new EventHandler(_saveDirectoryOpen_Click);
            _directoryActiveTabOpenMenuItem.Click += new EventHandler(_directoryActiveTabOpenMenuItem_Click);
            _directoryNewTabOpenMenuItem.Click += new EventHandler(_directoryNewTabOpenMenuItem_Click);
            _directoryNewWindowOpenMenuItem.Click += new EventHandler(_directoryNewWindowOpenMenuItem_Click);
            _explorerOpenMenuItem.Click += new EventHandler(_explorerOpenMenuItem_Click);
            _pathCopyMenuItem.Click += new EventHandler(_pathCopyMenuItem_Click);
            _nameCopyMenuItem.Click += new EventHandler(_nameCopyMenuItem_Click);
            _exportMenuItem.Click += new EventHandler(_exportMenuItem_Click);
            _addKeepMenuItem.Click += new EventHandler(_addKeepMenuItem_Click);
            _removeFromListMenuItem.Click += new EventHandler(_removeFromListMenuItem_Click);
        }

        private void setImageFileMenuItemVisible(bool isVisible)
        {
            if (_isFileActiveTabOpenMenuItemVisible)
            {
                _fileActiveTabOpenMenuItem.Visible = isVisible;
            }
            else
            {
                _fileActiveTabOpenMenuItem.Visible = false;
            }

            _fileNewTabOpenMenuItem.Visible = isVisible;
            _fileNewWindowOpenMenuItem.Visible = isVisible;
            _exportMenuItem.Visible = isVisible;
        }

        private void setFileMenuItemVisible(bool isVisible)
        {
            _fileOpen.Visible = isVisible;
            _saveDirectoryOpen.Visible = isVisible;
        }

        private void setDirectoryMenuItemVisible(bool isVisible)
        {
            if (_isDirectoryActiveTabOpenMenuItemVisible)
            {
                _directoryActiveTabOpenMenuItem.Visible = isVisible;
            }
            else
            {
                _directoryActiveTabOpenMenuItem.Visible = false;
            }

            _directoryNewTabOpenMenuItem.Visible = isVisible;
            _directoryNewWindowOpenMenuItem.Visible = isVisible;
            _explorerOpenMenuItem.Visible = isVisible;
        }

        #endregion

        #region コンテキストメニューイベント

        private void _fileActiveTabOpenMenuItem_Click(object sender, EventArgs e)
        {
            OnFileActiveTabOpen(new ExecuteFileEventArgs(_filePathList.First()));
        }

        private void _fileNewTabOpenMenuItem_Click(object sender, EventArgs e)
        {
            OnFileNewTabOpen(new ExecuteFileEventArgs(_filePathList.First()));
        }

        private void _fileNewWindowOpenMenuItem_Click(object sender, EventArgs e)
        {
            OnFileNewWindowOpen(new ExecuteFileEventArgs(_filePathList.First()));
        }

        private void _fileOpen_Click(object sender, EventArgs e)
        {
            OnFileOpen(new ExecuteFileEventArgs(_filePathList.First()));
        }

        private void _saveDirectoryOpen_Click(object sender, EventArgs e)
        {
            OnSaveDirectoryOpen(new ExecuteFileEventArgs(_filePathList.First()));
        }

        private void _directoryActiveTabOpenMenuItem_Click(object sender, EventArgs e)
        {
            OnDirectoryActiveTabOpen(new ExecuteFileEventArgs(_filePathList.First()));
        }

        private void _directoryNewTabOpenMenuItem_Click(object sender, EventArgs e)
        {
            OnDirectoryNewTabOpen(new ExecuteFileEventArgs(_filePathList.First()));
        }

        private void _directoryNewWindowOpenMenuItem_Click(object sender, EventArgs e)
        {
            OnDirectoryNewWindowOpen(new ExecuteFileEventArgs(_filePathList.First()));
        }

        private void _explorerOpenMenuItem_Click(object sender, EventArgs e)
        {
            OnExplorerOpen(new ExecuteFileEventArgs(_filePathList.First()));
        }

        private void _pathCopyMenuItem_Click(object sender, EventArgs e)
        {
            OnPathCopy(new ExecuteFileListEventArgs(_filePathList));
        }

        private void _nameCopyMenuItem_Click(object sender, EventArgs e)
        {
            OnNameCopy(new ExecuteFileListEventArgs(_filePathList));
        }

        private void _exportMenuItem_Click(object sender, EventArgs e)
        {
            OnExport(new ExecuteFileListEventArgs(_filePathList));
        }

        private void _addKeepMenuItem_Click(object sender, EventArgs e)
        {
            OnAddKeep(new ExecuteFileListEventArgs(_filePathList));
        }

        private void _removeFromListMenuItem_Click(object sender, EventArgs e)
        {
            OnRemoveFromList(new ExecuteFileListEventArgs(_filePathList));
        }

        #endregion
    }
}
