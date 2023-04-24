﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using PicSum.Core.Base.Conf;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;
using PicSum.Task.Result;
using PicSum.UIComponent.InfoPanel.Properties;
using SWF.Common;
using SWF.UIComponent.WideDropDown;

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

        private TwoWayProcess<GetFileDeepInfoAsyncFacade, GetFileDeepInfoParameter, GetFileDeepInfoResult> _getFileInfoProcess = null;
        private OneWayProcess<UpdateFileRatingAsyncFacade, UpdateFileRatingParameter> _updateFileRatingProcess = null;
        private TwoWayProcess<GetTagListAsyncFacade, ListEntity<string>> _getTagListProcess = null;
        private OneWayProcess<AddFileTagAsyncFacade, UpdateFileTagParameter> _addFileTagProcess = null;
        private OneWayProcess<DeleteFileTagAsyncFacade, UpdateFileTagParameter> _deleteFileTagProcess = null;

        private GetFileDeepInfoResult _fileInfoSource = null;
        private Font _allTagFont = null;
        private Image _tagIcon = Resources.TagIcon;
        private string _contextMenuOperationTag = string.Empty;

        #endregion

        #region パブリックプロパティ

        #endregion

        #region 継承プロパティ

        #endregion

        #region プライベートプロパティ

        private TwoWayProcess<GetFileDeepInfoAsyncFacade, GetFileDeepInfoParameter, GetFileDeepInfoResult> getFileInfoProcess
        {
            get
            {
                if (_getFileInfoProcess == null)
                {
                    _getFileInfoProcess = TaskManager.CreateTwoWayProcess<GetFileDeepInfoAsyncFacade, GetFileDeepInfoParameter, GetFileDeepInfoResult>(components);
                    _getFileInfoProcess.Callback += new AsyncTaskCallbackEventHandler<GetFileDeepInfoResult>(getFileInfoProcess_Callback);
                }

                return _getFileInfoProcess;
            }
        }

        private OneWayProcess<UpdateFileRatingAsyncFacade, UpdateFileRatingParameter> updateFileRatingProcess
        {
            get
            {
                if (_updateFileRatingProcess == null)
                {
                    _updateFileRatingProcess = TaskManager.CreateOneWayProcess<UpdateFileRatingAsyncFacade, UpdateFileRatingParameter>(components);
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

        private OneWayProcess<AddFileTagAsyncFacade, UpdateFileTagParameter> addFileTagProcess
        {
            get
            {
                if (_addFileTagProcess == null)
                {
                    _addFileTagProcess = TaskManager.CreateOneWayProcess<AddFileTagAsyncFacade, UpdateFileTagParameter>(components);
                }

                return _addFileTagProcess;
            }
        }

        private OneWayProcess<DeleteFileTagAsyncFacade, UpdateFileTagParameter> deleteFileTagProcess
        {
            get
            {
                if (_deleteFileTagProcess == null)
                {
                    _deleteFileTagProcess = TaskManager.CreateOneWayProcess<DeleteFileTagAsyncFacade, UpdateFileTagParameter>(components);
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

                GetFileDeepInfoParameter param = new GetFileDeepInfoParameter();
                param.FilePathList = filePathList;
                param.ThumbnailSize = new Size(
                    ApplicationConst.INFOPANEL_THUMBANIL_SIZE, 
                    ApplicationConst.INFOPANEL_THUMBANIL_SIZE);

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

            UpdateFileTagParameter param = new UpdateFileTagParameter();
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

            UpdateFileTagParameter param = new UpdateFileTagParameter();
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

        private void drawDirectoryThumbnail(Graphics g, Image thumb, Rectangle rect)
        {
            ThumbnailUtil.DrawDirectoryThumbnail(g, thumb, rect, FileIconCash.LargeDirectoryIcon);
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
                    string text = string.Format("Select {0} file", filePathList.Count);
                    g.DrawString(text, this.Font, sb, rect, sf);
                }
            }
        }

        #endregion

        #region プロセスイベント

        private void getFileInfoProcess_Callback(object sender, GetFileDeepInfoResult e)
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
                    fileUpdatedateLabel.Text = string.Format("Update Date {0:yyyy/MM/dd HH:mm:ss}", fileInfo.UpdateDate.Value);
                }

                if (fileInfo.CreateDate.HasValue)
                {
                    fileCreateDateLabel.Text = string.Format("Creation Date {0:yyyy/MM/dd HH:mm:ss}", fileInfo.CreateDate.Value);

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
            this.wideComboBox.AddItems(e);
            this.wideComboBox.SelectItem();
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
                    drawDirectoryThumbnail(e.Graphics, thumbnail.ThumbnailImage, rect);
                }
            }
            else if (fileInfo != null)
            {
                const int margin = 32;
                Rectangle rect = new Rectangle(margin, margin, thumbnailPictureBox.Width - margin * 2, thumbnailPictureBox.Height - margin * 2);
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

            UpdateFileRatingParameter param = new UpdateFileRatingParameter();
            param.FilePathList = _fileInfoSource.FilePathList;
            param.RatingValue = ratingBar.Value;
            updateFileRatingProcess.Execute(this, param);
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

        #region ワイドコンボボックスイベント

        private void wideComboBox_DropDownOpening(object sender, DropDownOpeningEventArgs e)
        {
            this.getTagListProcess.Execute(this);
        }

        private void wideComboBox_AddItem(object sender, AddItemEventArgs e)
        {
            if (this.filePathList == null || this.tagList == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(e.Item))
            {
                return;
            }

            if (tagList.FirstOrDefault(t => t.Tag.Equals(e.Item, StringComparison.Ordinal) && t.IsAll) != null)
            {
                return;
            }

            this.addTag(e.Item);
        }

        #endregion
    }
}
