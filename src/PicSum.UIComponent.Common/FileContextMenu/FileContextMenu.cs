using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PicSum.Core.Data.FileAccessor;
using SWF.Common;

namespace PicSum.UIComponent.Common.FileContextMenu
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
        public event EventHandler<ExecuteFileEventArgs> SaveFolderOpen;
        public event EventHandler<ExecuteFileEventArgs> FolderActiveTabOpen;
        public event EventHandler<ExecuteFileEventArgs> FolderNewTabOpen;
        public event EventHandler<ExecuteFileEventArgs> FolderNewWindowOpen;
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
        private bool _isFolderActiveTabOpenMenuItemVisible = false;

        // 画像ファイルメニュー項目
        private ToolStripMenuItem _fileActiveTabOpenMenuItem = new ToolStripMenuItem("開く");
        private ToolStripMenuItem _fileNewTabOpenMenuItem = new ToolStripMenuItem("新しいタブで開く");
        private ToolStripMenuItem _fileNewWindowOpenMenuItem = new ToolStripMenuItem("新しいウィンドウで開く");

        // ファイルメニュー項目
        private ToolStripMenuItem _fileOpen = new ToolStripMenuItem("関連付けて開く");
        private ToolStripMenuItem _saveFolderOpen = new ToolStripMenuItem("保存フォルダを開く");

        // フォルダメニュー項目
        private ToolStripMenuItem _folderActiveTabOpenMenuItem = new ToolStripMenuItem("開く");
        private ToolStripMenuItem _folderNewTabOpenMenuItem = new ToolStripMenuItem("新しいタブで開く");
        private ToolStripMenuItem _folderNewWindowOpenMenuItem = new ToolStripMenuItem("新しいウィンドウで開く");
        private ToolStripMenuItem _explorerOpenMenuItem = new ToolStripMenuItem("エクスプローラで開く");

        private ToolStripMenuItem _exportMenuItem = new ToolStripMenuItem("エクスポート");
        private ToolStripMenuItem _pathCopyMenuItem = new ToolStripMenuItem("パスをコピー");
        private ToolStripMenuItem _nameCopyMenuItem = new ToolStripMenuItem("名前をコピー");
        private ToolStripMenuItem _addKeepMenuItem = new ToolStripMenuItem("キープ");
        private ToolStripMenuItem _removeFromListMenuItem = new ToolStripMenuItem("一覧から削除");

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

        public bool IsFolderActiveTabOpenMenuItemVisible
        {
            get
            {
                return _isFolderActiveTabOpenMenuItemVisible;
            }
            set
            {
                _isFolderActiveTabOpenMenuItemVisible = value;
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
                setFolderMenuItemVisible(false);
                _exportMenuItem.Visible = filePathList.FirstOrDefault(file => !ImageUtil.ImageFileExtensionList.Contains(FileUtil.GetExtension(file))) == null;
            }
            else
            {
                string filePath = filePathList.First();
                setImageFileMenuItemVisible(ImageUtil.ImageFileExtensionList.Contains(FileUtil.GetExtension(filePath)));
                setFileMenuItemVisible(FileUtil.IsFile(filePath));
                setFolderMenuItemVisible(!FileUtil.IsFile(filePath));
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

        protected virtual void OnSaveFolderOpen(ExecuteFileEventArgs e)
        {
            if (SaveFolderOpen != null)
            {
                SaveFolderOpen(this, e);
            }
        }

        protected virtual void OnFolderActiveTabOpen(ExecuteFileEventArgs e)
        {
            if (FolderActiveTabOpen != null)
            {
                FolderActiveTabOpen(this, e);
            }
        }

        protected virtual void OnFolderNewTabOpen(ExecuteFileEventArgs e)
        {
            if (FolderNewTabOpen != null)
            {
                FolderNewTabOpen(this, e);
            }
        }

        protected virtual void OnFolderNewWindowOpen(ExecuteFileEventArgs e)
        {
            if (FolderNewWindowOpen != null)
            {
                FolderNewWindowOpen(this, e);
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
                                                      _saveFolderOpen,
                                                      _folderActiveTabOpenMenuItem, 
                                                      _folderNewTabOpenMenuItem, 
                                                      _folderNewWindowOpenMenuItem, 
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
            _saveFolderOpen.Click += new EventHandler(_saveFolderOpen_Click);
            _folderActiveTabOpenMenuItem.Click += new EventHandler(_folderActiveTabOpenMenuItem_Click);
            _folderNewTabOpenMenuItem.Click += new EventHandler(_folderNewTabOpenMenuItem_Click);
            _folderNewWindowOpenMenuItem.Click += new EventHandler(_folderNewWindowOpenMenuItem_Click);
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
            _saveFolderOpen.Visible = isVisible;
        }

        private void setFolderMenuItemVisible(bool isVisible)
        {
            if (_isFolderActiveTabOpenMenuItemVisible)
            {
                _folderActiveTabOpenMenuItem.Visible = isVisible;
            }
            else
            {
                _folderActiveTabOpenMenuItem.Visible = false;
            }

            _folderNewTabOpenMenuItem.Visible = isVisible;
            _folderNewWindowOpenMenuItem.Visible = isVisible;
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

        private void _saveFolderOpen_Click(object sender, EventArgs e)
        {
            OnSaveFolderOpen(new ExecuteFileEventArgs(_filePathList.First()));
        }

        private void _folderActiveTabOpenMenuItem_Click(object sender, EventArgs e)
        {
            OnFolderActiveTabOpen(new ExecuteFileEventArgs(_filePathList.First()));
        }

        private void _folderNewTabOpenMenuItem_Click(object sender, EventArgs e)
        {
            OnFolderNewTabOpen(new ExecuteFileEventArgs(_filePathList.First()));
        }

        private void _folderNewWindowOpenMenuItem_Click(object sender, EventArgs e)
        {
            OnFolderNewWindowOpen(new ExecuteFileEventArgs(_filePathList.First()));
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
