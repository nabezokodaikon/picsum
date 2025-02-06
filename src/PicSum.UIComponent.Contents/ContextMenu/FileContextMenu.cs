using SWF.Core.FileAccessor;
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
        public event EventHandler<ExecuteFileEventArgs> FileOpen;
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
        public event EventHandler<ExecuteFileListEventArgs> Clip;

        public event EventHandler<ExecuteFileListEventArgs> ConvertToAvif;
        public event EventHandler<ExecuteFileListEventArgs> ConvertToBitmap;
        public event EventHandler<ExecuteFileListEventArgs> ConvertToHeif;
        public event EventHandler<ExecuteFileListEventArgs> ConvertToIcon;
        public event EventHandler<ExecuteFileListEventArgs> ConvertToJpeg;
        public event EventHandler<ExecuteFileListEventArgs> ConvertToPng;
        public event EventHandler<ExecuteFileListEventArgs> ConvertToSvg;
        public event EventHandler<ExecuteFileListEventArgs> ConvertToWebp;


        private string[] filePathList = null;

        // 画像ファイルメニュー項目
        private readonly ToolStripMenuItem fileActiveTabOpenMenuItem = new("Open");
        private readonly ToolStripMenuItem fileNewTabOpenMenuItem = new("Open in new tab");
        private readonly ToolStripMenuItem fileNewWindowOpenMenuItem = new("Open in new window");
        private readonly ToolStripMenuItem fileBookmarkMenuItem = new("Make a bookmark");

        // ファイルメニュー項目
        private readonly ToolStripMenuItem fileOpenMenuItem = new("Open in association");
        private readonly ToolStripMenuItem selectApplicationMenuItem = new("Select and open the application");
        private readonly ToolStripMenuItem saveDirectoryOpenMenuItem = new("Open save folder");

        // フォルダメニュー項目
        private readonly ToolStripMenuItem directoryActiveTabOpenMenuItem = new("Open");
        private readonly ToolStripMenuItem directoryNewTabOpenMenuItem = new("Open in new tab");
        private readonly ToolStripMenuItem directoryNewWindowOpenMenuItem = new("Open in new window");
        private readonly ToolStripMenuItem explorerOpenMenuItem = new("Open in explorer");

        private readonly ToolStripMenuItem pathCopyMenuItem = new("Copy the file path");
        private readonly ToolStripMenuItem nameCopyMenuItem = new("Copy the file name");
        private readonly ToolStripMenuItem removeFromListMenuItem = new("Remove from list");
        private readonly ToolStripMenuItem clipMenuItem = new("Add to clip");
        private readonly ToolStripMenuItem convertMenuItem = new("Convert and export");

        private readonly ToolStripMenuItem convertToAvifMenuItem = new("To Avif");
        private readonly ToolStripMenuItem convertToBitmapMenuItem = new("To Bitmap");
        private readonly ToolStripMenuItem convertToHeifMenuItem = new("To Heif");
        private readonly ToolStripMenuItem convertToIconMenuItem = new("To Icon");
        private readonly ToolStripMenuItem convertToJpegMenuItem = new("To Jpeg");
        private readonly ToolStripMenuItem convertToPngMenuItem = new("To Png");
        private readonly ToolStripMenuItem convertToSvgMenuItem = new("To Svg");
        private readonly ToolStripMenuItem convertToWebpMenuItem = new("To Webp");

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
        public bool VisibleConvertMenuItem
        {
            get
            {
                return this.convertMenuItem.Visible;
            }
            set
            {
                this.convertMenuItem.Visible = value;
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
                this.Font = new Font(this.Font.FontFamily, 10f);

                this.ShowImageMargin = false;
                ((ToolStripDropDownMenu)this.convertMenuItem.DropDown).ShowImageMargin = false;

                this.convertToHeifMenuItem.Visible = false;
                this.convertToIconMenuItem.Visible = false;

                this.convertMenuItem.DropDownItems.AddRange([
                    this.convertToAvifMenuItem,
                    this.convertToBitmapMenuItem,
                    this.convertToHeifMenuItem,
                    this.convertToIconMenuItem,
                    this.convertToJpegMenuItem,
                    this.convertToPngMenuItem,
                    this.convertToSvgMenuItem,
                    this.convertToWebpMenuItem
                    ]);

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
                    //this.convertMenuItem,
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
                this.fileBookmarkMenuItem.Click += new(this.FileBookmarkMenuItem_Click);
                this.removeFromListMenuItem.Click += new(this.RemoveFromListMenuItem_Click);
                this.clipMenuItem.Click += new(this.ClipMenuItem_Click);
                this.convertToAvifMenuItem.Click += new(this.ConvertToAvifMenuItem_Click);
                this.convertToBitmapMenuItem.Click += new(this.ConvertToBitmapMenuItem_Click);
                this.convertToHeifMenuItem.Click += new(this.ConvertToHeifMenuItem_Click);
                this.convertToIconMenuItem.Click += new(this.ConvertToIconMenuItem_Click);
                this.convertToJpegMenuItem.Click += new(this.ConvertToJpegMenuItem_Click);
                this.convertToPngMenuItem.Click += new(this.ConvertToPngMenuItem_Click);
                this.convertToSvgMenuItem.Click += new(this.ConvertToSvgMenuItem_Click);
                this.convertToWebpMenuItem.Click += new(this.ConvertToWebpMenuItem_Click);
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
                    .Count() == filePathList.Length;
                this.convertMenuItem.Visible = isAllImageFiles;
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
                this.convertMenuItem.Visible = isImageFile;
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

        private void OnConvertToAvif(ExecuteFileListEventArgs e)
        {
            this.ConvertToAvif?.Invoke(this, e);
        }

        private void OnConvertToBitmap(ExecuteFileListEventArgs e)
        {
            this.ConvertToBitmap?.Invoke(this, e);
        }

        private void OnConvertToHeif(ExecuteFileListEventArgs e)
        {
            this.ConvertToHeif?.Invoke(this, e);
        }

        private void OnConvertToIcon(ExecuteFileListEventArgs e)
        {
            this.ConvertToIcon?.Invoke(this, e);
        }

        private void OnConvertToJpeg(ExecuteFileListEventArgs e)
        {
            this.ConvertToJpeg?.Invoke(this, e);
        }

        private void OnConvertToPng(ExecuteFileListEventArgs e)
        {
            this.ConvertToPng?.Invoke(this, e);
        }

        private void OnConvertToSvg(ExecuteFileListEventArgs e)
        {
            this.ConvertToSvg?.Invoke(this, e);
        }

        private void OnConvertToWebp(ExecuteFileListEventArgs e)
        {
            this.ConvertToWebp?.Invoke(this, e);
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

        private void ConvertToAvifMenuItem_Click(object sender, EventArgs e)
        {
            this.OnConvertToAvif(new ExecuteFileListEventArgs(this.filePathList));
        }

        private void ConvertToBitmapMenuItem_Click(object sender, EventArgs e)
        {
            this.OnConvertToBitmap(new ExecuteFileListEventArgs(this.filePathList));
        }

        private void ConvertToHeifMenuItem_Click(object sender, EventArgs e)
        {
            this.OnConvertToHeif(new ExecuteFileListEventArgs(this.filePathList));
        }

        private void ConvertToIconMenuItem_Click(object sender, EventArgs e)
        {
            this.OnConvertToIcon(new ExecuteFileListEventArgs(this.filePathList));
        }

        private void ConvertToJpegMenuItem_Click(object sender, EventArgs e)
        {
            this.OnConvertToJpeg(new ExecuteFileListEventArgs(this.filePathList));
        }

        private void ConvertToPngMenuItem_Click(object sender, EventArgs e)
        {
            this.OnConvertToPng(new ExecuteFileListEventArgs(this.filePathList));
        }

        private void ConvertToSvgMenuItem_Click(object sender, EventArgs e)
        {
            this.OnConvertToSvg(new ExecuteFileListEventArgs(this.filePathList));
        }

        private void ConvertToWebpMenuItem_Click(object sender, EventArgs e)
        {
            this.OnConvertToWebp(new ExecuteFileListEventArgs(this.filePathList));
        }
    }
}
