using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using PicSum.Core.Base.Conf;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.UIComponent.InfoPanel.Properties;
using SWF.Common;

namespace PicSum.UIComponent.InfoPanel
{
    public partial class InfoPanel : UserControl
    {
        #region 定数・列挙

        #endregion

        #region イベント・デリゲート

        public event EventHandler<SelectedTagEventArgs> SelectedTag;

        #endregion

        #region インスタンス変数

        private TwoWayProcess<GetFileDeepInfoAsyncFacade, GetFileDeepInfoParameterEntity, GetFileDeepInfoResultEntity> _getFileInfoProcess = null;
        private OneWayProcess<UpdateFileRatingAsyncFacade, UpdateFileRatingParameterEntity> _updateFileRatingProcess = null;
        private TwoWayProcess<GetTagListAsyncFacade, ListEntity<string>> _getTagListProcess = null;
        private OneWayProcess<AddFileTagAsyncFacade, UpdateFileTagParameterEntity> _addFileTagProcess = null;
        private OneWayProcess<DeleteFileTagAsyncFacade, UpdateFileTagParameterEntity> _deleteFileTagProcess = null;

        private GetFileDeepInfoResultEntity _fileInfoSource = null;
        private Font _allTagFont = null;
        private Image _tagIcon = Resources.TagIcon;
        private string _contextMenuOperationTag = string.Empty;

        #endregion

        #region パブリックプロパティ

        #endregion

        #region 継承プロパティ

        #endregion

        #region プライベートプロパティ

        private TwoWayProcess<GetFileDeepInfoAsyncFacade, GetFileDeepInfoParameterEntity, GetFileDeepInfoResultEntity> getFileInfoProcess
        {
            get
            {
                if (_getFileInfoProcess == null)
                {
                    _getFileInfoProcess = TaskManager.CreateTwoWayProcess<GetFileDeepInfoAsyncFacade, GetFileDeepInfoParameterEntity, GetFileDeepInfoResultEntity>(components);
                    _getFileInfoProcess.Callback += new AsyncTaskCallbackEventHandler<GetFileDeepInfoResultEntity>(getFileInfoProcess_Callback);
                }

                return _getFileInfoProcess;
            }
        }

        private OneWayProcess<UpdateFileRatingAsyncFacade, UpdateFileRatingParameterEntity> updateFileRatingProcess
        {
            get
            {
                if (_updateFileRatingProcess == null)
                {
                    _updateFileRatingProcess = TaskManager.CreateOneWayProcess<UpdateFileRatingAsyncFacade, UpdateFileRatingParameterEntity>(components);
                }

                return _updateFileRatingProcess;
            }
        }

        private TwoWayProcess<GetTagListAsyncFacade, ListEntity<string>> getTagListProcess
        {
            get
            {
                if (_getTagListProcess == null)
                {
                    _getTagListProcess = TaskManager.CreateTwoWayProcess<GetTagListAsyncFacade, ListEntity<string>>(components);
                    _getTagListProcess.Callback += new AsyncTaskCallbackEventHandler<ListEntity<string>>(getTagListProcess_Callback);
                }

                return _getTagListProcess;
            }
        }

        private OneWayProcess<AddFileTagAsyncFacade, UpdateFileTagParameterEntity> addFileTagProcess
        {
            get
            {
                if (_addFileTagProcess == null)
                {
                    _addFileTagProcess = TaskManager.CreateOneWayProcess<AddFileTagAsyncFacade, UpdateFileTagParameterEntity>(components);
                }

                return _addFileTagProcess;
            }
        }

        private OneWayProcess<DeleteFileTagAsyncFacade, UpdateFileTagParameterEntity> deleteFileTagProcess
        {
            get
            {
                if (_deleteFileTagProcess == null)
                {
                    _deleteFileTagProcess = TaskManager.CreateOneWayProcess<DeleteFileTagAsyncFacade, UpdateFileTagParameterEntity>(components);
                }

                return _deleteFileTagProcess;
            }
        }

        private IList<string> filePathList
        {
            get
            {
                if (_fileInfoSource != null && _fileInfoSource.FilePathList != null)
                {
                    return _fileInfoSource.FilePathList;
                }
                else
                {
                    return null;
                }
            }
        }

        private FileDeepInfoEntity fileInfo
        {
            get
            {
                if (_fileInfoSource != null && _fileInfoSource.FileInfo != null)
                {
                    return _fileInfoSource.FileInfo;
                }
                else
                {
                    return null;
                }
            }
        }

        private ThumbnailImageEntity thumbnail
        {
            get
            {
                if (fileInfo != null && fileInfo.Thumbnail != null)
                {
                    return fileInfo.Thumbnail;
                }
                else
                {
                    return null;
                }
            }
        }

        private ListEntity<FileTagInfoEntity> tagList
        {
            get
            {
                if (_fileInfoSource != null && _fileInfoSource.TagInfoList != null)
                {
                    return _fileInfoSource.TagInfoList;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region コンストラクタ

        public InfoPanel()
        {
            InitializeComponent();

            if (!this.DesignMode)
            {
                initializeComponent();
            }
        }

        #endregion

        #region パブリックメソッド

        public void SetFileInfo(string filePath)
        {
            SetFileInfo(new List<string>() { filePath });
        }

        public void SetFileInfo(IList<string> filePathList)
        {
            if (filePathList == null)
            {
                throw new ArgumentNullException("filePathList");
            }

            if (filePathList.Count > 0)
            {
                // コンピュータ(空文字)の場合は、情報を表示しない。
                if (string.IsNullOrEmpty(filePathList.First()))
                {
                    clearInfo();
                    return;
                }

                GetFileDeepInfoParameterEntity param = new GetFileDeepInfoParameterEntity();
                param.FilePathList = filePathList;
                const int OFFSET = 16;
                int size = Math.Min(thumbnailPictureBox.Width - OFFSET, thumbnailPictureBox.Height - OFFSET);
                param.ThumbnailSize = new Size(size, size);

                getFileInfoProcess.Cancel();
                getFileInfoProcess.Execute(this, param);
            }
            else
            {
                clearInfo();
            }
        }

        #endregion

        #region 継承メソッド

        protected virtual void OnSelectedTag(SelectedTagEventArgs e)
        {
            if (SelectedTag != null)
            {
                SelectedTag(this, e);
            }
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            if (components == null)
            {
                components = new Container();
            }

            tagFlowList.ItemHeight = 24;
            this.CreateHandle();
        }

        private void clearInfo()
        {
            if (thumbnail != null)
            {
                thumbnail.ThumbnailImage.Dispose();
                thumbnail.ThumbnailImage = null;
            }

            _fileInfoSource = null;

            fileNameLabel.Text = string.Empty;
            fileTypeLabel.Text = string.Empty;
            fileSizeLabel.Text = string.Empty;
            fileUpdatedateLabel.Text = string.Empty;
            fileCreateDateLabel.Text = string.Empty;
            ratingBar.SetValue(0);
            thumbnailPictureBox.Invalidate();
            tagFlowList.ItemCount = 0;

            _contextMenuOperationTag = string.Empty;
            tagContextMenuStrip.Close();
        }

        private Font getAllTagFont()
        {
            if (_allTagFont == null)
            {
                _allTagFont = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Bold, this.Font.Unit, this.Font.GdiCharSet);
            }

            return _allTagFont;
        }

        private Font getTagFont(FileTagInfoEntity tagInfo)
        {
            if (tagInfo.IsAll)
            {
                return getAllTagFont();
            }
            else
            {
                return this.Font;
            }
        }

        private void addTag(string tag)
        {
            if (filePathList == null || tagList == null)
            {
                throw new NullReferenceException("ファイルの情報が存在しません。");
            }

            if (string.IsNullOrEmpty(tag))
            {
                throw new Exception("NULLまたは長さ0の文字列は、タグに登録できません。");
            }

            if (tagList.Find(t => t.Tag.Equals(tag, StringComparison.Ordinal) && t.IsAll) != null)
            {
                throw new Exception("既に登録されているタグです。");
            }

            UpdateFileTagParameterEntity param = new UpdateFileTagParameterEntity();
            param.Tag = tag;
            param.FilePathList = filePathList;
            addFileTagProcess.Execute(this, param);

            FileTagInfoEntity tagInfo = tagList.Find(t => t.Tag.Equals(tag, StringComparison.Ordinal));
            if (tagInfo != null)
            {
                tagInfo.IsAll = true;
                tagFlowList.Invalidate();
            }
            else
            {
                tagInfo = new FileTagInfoEntity();
                tagInfo.Tag = tag;
                tagInfo.IsAll = true;
                tagList.Add(tagInfo);
                tagList.Sort((x, y) => x.Tag.CompareTo(y.Tag));
                tagFlowList.ItemCount = tagList.Count;
            }
        }

        private void deleteTag(string tag)
        {
            if (filePathList == null || tagList == null)
            {
                throw new NullReferenceException("ファイルの情報が存在しません。");
            }

            if (string.IsNullOrEmpty(tag))
            {
                throw new Exception("タグがNULLまたは長さ0の文字列です。");
            }

            if (tagList.Find(t => t.Tag.Equals(tag, StringComparison.Ordinal)) == null)
            {
                throw new Exception("リストに存在しないタグを指定しました。");
            }

            UpdateFileTagParameterEntity param = new UpdateFileTagParameterEntity();
            param.Tag = tag;
            param.FilePathList = filePathList;
            deleteFileTagProcess.Execute(this, param);

            FileTagInfoEntity tagInfo = tagList.Find(t => t.Tag.Equals(tag, StringComparison.Ordinal));
            tagList.Remove(tagInfo);
            tagFlowList.ItemCount = tagList.Count;
        }

        private void drawImageFileThumbnail(Graphics g, Image thumb, Rectangle rect)
        {
            ThumbnailUtil.DrawFileThumbnail(g, thumb, rect);
        }

        private void drawFolderThumbnail(Graphics g, Image thumb, Rectangle rect)
        {
            ThumbnailUtil.DrawFolderThumbnail(g, thumb, rect, FileIconCash.LargeFolderIcon);
        }

        private void drawFileIcon(Graphics g, Image icon, Rectangle rect)
        {
            var iconWidth = (float)icon.Width;
            var iconHeight = (float)icon.Height;

            if (Math.Max(iconWidth, iconHeight) <= Math.Min(rect.Width, rect.Height))
            {
                var w = iconWidth;
                var h = iconHeight;
                var x = rect.X + (rect.Width - w) / 2f;
                var y = rect.Y + (rect.Height - h) / 2f;
                g.DrawImage(icon, new RectangleF(x, y, w, h));
            }
            else
            {
                var scale = Math.Min(rect.Width / iconWidth, rect.Height / iconHeight);
                var w = iconWidth * scale;
                var h = iconWidth * scale;
                var x = rect.X + (rect.Width - w) / 2f;
                var y = rect.Y + (rect.Height - h) / 2f;
                g.DrawImage(icon, new RectangleF(x, y, w, h));
            }
        }

        private void drawSelectedFileCount(Graphics g, int count, Rectangle rect)
        {
            using (SolidBrush sb = new SolidBrush(this.ForeColor))
            {
                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Trimming = StringTrimming.EllipsisCharacter;
                    string text = string.Format("{0}ファイル選択", filePathList.Count);
                    g.DrawString(text, this.Font, sb, rect, sf);
                }
            }
        }

        #endregion

        #region プロセスイベント

        private void getFileInfoProcess_Callback(object sender, GetFileDeepInfoResultEntity e)
        {
            clearInfo();

            _fileInfoSource = e;

            if (fileInfo != null)
            {
                fileNameLabel.Text = fileInfo.FileName;
                fileTypeLabel.Text = fileInfo.FileType;

                if (fileInfo.FileSize.HasValue)
                {
                    fileSizeLabel.Text = FileUtil.ToSizeString(fileInfo.FileSize.Value);
                    if (fileInfo.ImageSize.HasValue)
                    {
                        fileSizeLabel.Text += string.Format(" ({0} x {1})", fileInfo.ImageSize.Value.Width, fileInfo.ImageSize.Value.Height);
                    }
                }

                if (fileInfo.UpdateDate.HasValue)
                {
                    fileUpdatedateLabel.Text = string.Format("更新日時 {0:yyyy/MM/dd}", fileInfo.UpdateDate.Value);
                }

                if (fileInfo.CreateDate.HasValue)
                {
                    fileCreateDateLabel.Text = string.Format("作成日時 {0:yyyy/MM/dd}", fileInfo.CreateDate.Value);

                }

                ratingBar.SetValue(fileInfo.Rating);
            }

            if (tagList != null)
            {
                tagFlowList.ItemCount = tagList.Count;
            }

            thumbnailPictureBox.Invalidate();
        }

        private void getTagListProcess_Callback(object sender, ListEntity<string> e)
        {
            tagComboBox.Items.Clear();
            tagComboBox.Items.AddRange(e.ToArray());
        }

        #endregion

        #region サムネイルピクチャーボックスイベント

        private void thumbnailPictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (thumbnail != null)
            {
                int size = Math.Min(thumbnailPictureBox.Width, thumbnailPictureBox.Height);
                int x = (int)(0 + (thumbnailPictureBox.Width - size) / 2d);
                int y = (int)(0 + (thumbnailPictureBox.Height - size) / 2d);
                Rectangle rect = new Rectangle(x, y, size, size);
                if (fileInfo.IsFile)
                {
                    drawImageFileThumbnail(e.Graphics, thumbnail.ThumbnailImage, rect);
                }
                else
                {
                    drawFolderThumbnail(e.Graphics, thumbnail.ThumbnailImage, rect);
                }
            }
            else if (fileInfo != null)
            {
                Rectangle rect = new Rectangle(0, 0, thumbnailPictureBox.Width, thumbnailPictureBox.Height);
                drawFileIcon(e.Graphics, fileInfo.FileIcon, rect);
            }
            else if (filePathList != null)
            {
                Rectangle rect = new Rectangle(0, 0, thumbnailPictureBox.Width, thumbnailPictureBox.Height);
                drawSelectedFileCount(e.Graphics, filePathList.Count, rect);
            }
        }

        #endregion

        #region タグリストイベント

        private void tagFlowList_DrawItem(object sender, SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (tagList == null)
            {
                return;
            }

            if (e.IsSelected)
            {
                e.Graphics.FillRectangle(tagFlowList.SelectedItemBrush, e.ItemRectangle);
            }
            else if (e.IsMousePoint)
            {
                e.Graphics.FillRectangle(tagFlowList.MousePointItemBrush, e.ItemRectangle);
            }

            FileTagInfoEntity item = tagList[e.ItemIndex];

            int iconSize = Math.Min(tagFlowList.ItemHeight, _tagIcon.Width);

            int iconPoint = (int)((tagFlowList.ItemHeight - iconSize) / 2);

            Rectangle iconRect = new Rectangle(e.ItemRectangle.X + iconPoint,
                                               e.ItemRectangle.Y + iconPoint,
                                               iconSize,
                                               iconSize);

            e.Graphics.DrawImage(_tagIcon, iconRect);

            Rectangle textRect = new Rectangle(e.ItemRectangle.X + tagFlowList.ItemHeight,
                                               e.ItemRectangle.Y,
                                               e.ItemRectangle.Width - tagFlowList.ItemHeight,
                                               e.ItemRectangle.Height);

            e.Graphics.DrawString(item.Tag,
                                  getTagFont(item),
                                  tagFlowList.ItemTextBrush,
                                  textRect,
                                  tagFlowList.ItemTextFormat);
        }

        #endregion

        #region 評価値バーイベント

        private void ratingBar_RatingButtonMouseClick(object sender, MouseEventArgs e)
        {
            if (_fileInfoSource == null)
            {
                return;
            }

            UpdateFileRatingParameterEntity param = new UpdateFileRatingParameterEntity();
            param.FilePathList = _fileInfoSource.FilePathList;
            param.RatingValue = ratingBar.Value;
            updateFileRatingProcess.Execute(this, param);
        }

        #endregion

        #region タグコンボボックスイベント

        private void tagComboBox_DropDown(object sender, EventArgs e)
        {
            getTagListProcess.Execute(this);
        }

        private void tagComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;

                if (filePathList == null || tagList == null)
                {
                    return;
                }

                string tag = tagComboBox.Text;

                if (string.IsNullOrEmpty(tag))
                {
                    return;
                }

                if (tagList.FirstOrDefault(t => t.Tag.Equals(tag, StringComparison.Ordinal) && t.IsAll) != null)
                {
                    return;
                }

                addTag(tag);
            }
        }

        #endregion

        #region タグ追加ボタンイベント

        private void addTagButton_Click(object sender, EventArgs e)
        {
            if (filePathList == null || tagList == null)
            {
                return;
            }

            string tag = tagComboBox.Text;

            if (string.IsNullOrEmpty(tag))
            {
                return;
            }

            if (tagList.FirstOrDefault(t => t.Tag.Equals(tag, StringComparison.Ordinal) && t.IsAll) != null)
            {
                return;
            }

            addTag(tag);
        }

        #endregion

        #region タグコンテキストメニューイベント

        private void tagContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            Point p = tagFlowList.PointToClient(Cursor.Position);
            int index = tagFlowList.IndexFromPoint(p.X, p.Y);
            if (index < 0)
            {
                e.Cancel = true;
                return;
            }

            FileTagInfoEntity tagInfo = tagList[index];
            tagToAllEntryMenuItem.Visible = !tagInfo.IsAll;
            _contextMenuOperationTag = tagInfo.Tag;
        }

        private void tagDeleteMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_contextMenuOperationTag))
            {
                return;
            }

            deleteTag(_contextMenuOperationTag);
        }

        private void tagToAllEntryMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_contextMenuOperationTag))
            {
                return;
            }

            addTag(_contextMenuOperationTag);
        }

        private void tagFlowList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var p = tagFlowList.PointToClient(Cursor.Position);
            int index = tagFlowList.IndexFromPoint(p.X, p.Y);
            if (index < 0)
            {
                return;
            }

            var tagInfo = tagList[index];
            OnSelectedTag(new SelectedTagEventArgs(ContentsOpenType.OverlapTab, tagInfo.Tag));
        }

        private void tagFlowList_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Middle)
            {
                return;
            }

            var p = tagFlowList.PointToClient(Cursor.Position);
            int index = tagFlowList.IndexFromPoint(p.X, p.Y);
            if (index < 0)
            {
                return;
            }

            var tagInfo = tagList[index];
            OnSelectedTag(new SelectedTagEventArgs(ContentsOpenType.AddTab, tagInfo.Tag));
        }

        #endregion
    }
}
