using PicSum.Core.Base.Conf;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.Tasks;
using PicSum.Task.Entities;
using PicSum.Task.Paramters;
using PicSum.Task.Results;
using PicSum.UIComponent.InfoPanel.Properties;
using SWF.Common;
using SWF.UIComponent.WideDropDown;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.InfoPanel
{
    [SupportedOSPlatform("windows")]
    public sealed partial class InfoPanel
        : UserControl
    {
        #region イベント・デリゲート

        public event EventHandler<SelectedTagEventArgs> SelectedTag;

        #endregion

        #region インスタンス変数

        private TwoWayProcess<GetFileDeepInfoTask, GetFileDeepInfoParameter, GetFileDeepInfoResult> getFileInfoProcess = null;
        private OneWayProcess<UpdateFileRatingTask, UpdateFileRatingParameter> updateFileRatingProcess = null;
        private TwoWayProcess<GetTagListTask, ListEntity<string>> getTagListProcess = null;
        private OneWayProcess<AddFileTagTask, UpdateFileTagParameter> addFileTagProcess = null;
        private OneWayProcess<DeleteFileTagTask, UpdateFileTagParameter> deleteFileTagProcess = null;

        private GetFileDeepInfoResult fileInfoSource = null;
        private Font allTagFont = null;
        private Image tagIcon = Resources.TagIcon;
        private string contextMenuOperationTag = string.Empty;

        #endregion

        #region プライベートプロパティ

        private TwoWayProcess<GetFileDeepInfoTask, GetFileDeepInfoParameter, GetFileDeepInfoResult> GetFileInfoProcess
        {
            get
            {
                if (this.getFileInfoProcess == null)
                {
                    this.getFileInfoProcess = TaskManager.CreateTwoWayProcess<GetFileDeepInfoTask, GetFileDeepInfoParameter, GetFileDeepInfoResult>(this.components);
                    this.getFileInfoProcess.Callback += new AsyncTaskCallbackEventHandler<GetFileDeepInfoResult>(this.GetFileInfoProcess_Callback);
                }

                return this.getFileInfoProcess;
            }
        }

        private OneWayProcess<UpdateFileRatingTask, UpdateFileRatingParameter> UpdateFileRatingProcess
        {
            get
            {
                if (this.updateFileRatingProcess == null)
                {
                    this.updateFileRatingProcess = TaskManager.CreateOneWayProcess<UpdateFileRatingTask, UpdateFileRatingParameter>(this.components);
                }

                return this.updateFileRatingProcess;
            }
        }

        private TwoWayProcess<GetTagListTask, ListEntity<string>> GetTagListProcess
        {
            get
            {
                if (this.getTagListProcess == null)
                {
                    this.getTagListProcess = TaskManager.CreateTwoWayProcess<GetTagListTask, ListEntity<string>>(this.components);
                    this.getTagListProcess.Callback += new AsyncTaskCallbackEventHandler<ListEntity<string>>(this.GetTagListProcess_Callback);
                }

                return this.getTagListProcess;
            }
        }

        private OneWayProcess<AddFileTagTask, UpdateFileTagParameter> AddFileTagProcess
        {
            get
            {
                if (this.addFileTagProcess == null)
                {
                    this.addFileTagProcess = TaskManager.CreateOneWayProcess<AddFileTagTask, UpdateFileTagParameter>(this.components);
                }

                return this.addFileTagProcess;
            }
        }

        private OneWayProcess<DeleteFileTagTask, UpdateFileTagParameter> DeleteFileTagProcess
        {
            get
            {
                if (this.deleteFileTagProcess == null)
                {
                    this.deleteFileTagProcess = TaskManager.CreateOneWayProcess<DeleteFileTagTask, UpdateFileTagParameter>(this.components);
                }

                return this.deleteFileTagProcess;
            }
        }

        private IList<string> FilePathList
        {
            get
            {
                if (this.fileInfoSource != null && this.fileInfoSource.FilePathList != null)
                {
                    return this.fileInfoSource.FilePathList;
                }
                else
                {
                    return null;
                }
            }
        }

        private FileDeepInfoEntity FileInfo
        {
            get
            {
                if (this.fileInfoSource != null && this.fileInfoSource.FileInfo != null)
                {
                    return this.fileInfoSource.FileInfo;
                }
                else
                {
                    return null;
                }
            }
        }

        private ThumbnailImageResult Thumbnail
        {
            get
            {
                if (this.FileInfo != null && this.FileInfo.Thumbnail != null)
                {
                    return this.FileInfo.Thumbnail;
                }
                else
                {
                    return null;
                }
            }
        }

        private ListEntity<FileTagInfoEntity> TagList
        {
            get
            {
                if (this.fileInfoSource != null && this.fileInfoSource.TagInfoList != null)
                {
                    return this.fileInfoSource.TagInfoList;
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
            this.InitializeComponent();

            if (!this.DesignMode)
            {
                this.SubInitializeComponent();
            }
        }

        #endregion

        #region パブリックメソッド

        public void SetFileInfo(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            this.SetFileInfo(new List<string>() { filePath });
        }

        public void SetFileInfo(IList<string> filePathList)
        {
            if (filePathList == null)
            {
                throw new ArgumentNullException(nameof(filePathList));
            }

            if (filePathList.Count > 0)
            {
                // コンピュータ(空文字)の場合は、情報を表示しない。
                if (string.IsNullOrEmpty(filePathList.First()))
                {
                    this.ClearInfo();
                    return;
                }

                var param = new GetFileDeepInfoParameter();
                param.FilePathList = filePathList;
                param.ThumbnailSize = new Size(
                    ApplicationConst.INFOPANEL_THUMBANIL_SIZE,
                    ApplicationConst.INFOPANEL_THUMBANIL_SIZE);

                this.GetFileInfoProcess.Cancel();
                this.GetFileInfoProcess.Execute(this, param);
            }
            else
            {
                this.ClearInfo();
            }
        }

        #endregion

        #region プライベートメソッド

        private void OnSelectedTag(SelectedTagEventArgs e)
        {
            if (this.SelectedTag != null)
            {
                this.SelectedTag(this, e);
            }
        }

        private void SubInitializeComponent()
        {
            if (this.components == null)
            {
                this.components = new Container();
            }

            this.CreateHandle();
        }

        private void ClearInfo()
        {
            if (this.Thumbnail != null)
            {
                this.Thumbnail.ThumbnailImage.Dispose();
                this.Thumbnail.ThumbnailImage = null;
            }

            this.fileInfoSource = null;

            this.fileNameLabel.Text = string.Empty;
            this.fileTypeLabel.Text = string.Empty;
            this.fileSizeLabel.Text = string.Empty;
            this.fileUpdatedateLabel.Text = string.Empty;
            this.ratingBar.SetValue(0);
            this.thumbnailPictureBox.Invalidate();
            this.tagFlowList.ItemCount = 0;

            this.contextMenuOperationTag = string.Empty;
            this.tagContextMenuStrip.Close();
        }

        private Font GetAllTagFont()
        {
            if (this.allTagFont == null)
            {
                this.allTagFont = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Bold, this.Font.Unit, this.Font.GdiCharSet);
            }

            return this.allTagFont;
        }

        private Font GetTagFont(FileTagInfoEntity tagInfo)
        {
            if (tagInfo.IsAll)
            {
                return this.GetAllTagFont();
            }
            else
            {
                return this.Font;
            }
        }

        private void AddTag(string tag)
        {
            if (this.FilePathList == null || this.TagList == null)
            {
                throw new NullReferenceException("ファイルの情報が存在しません。");
            }

            if (string.IsNullOrEmpty(tag))
            {
                throw new Exception("NULLまたは長さ0の文字列は、タグに登録できません。");
            }

            if (this.TagList.Find(t => t.Tag.Equals(tag, StringComparison.Ordinal) && t.IsAll) != null)
            {
                throw new Exception("既に登録されているタグです。");
            }

            var param = new UpdateFileTagParameter();
            param.Tag = tag;
            param.FilePathList = this.FilePathList;
            this.AddFileTagProcess.Execute(this, param);

            var tagInfo = this.TagList.Find(t => t.Tag.Equals(tag, StringComparison.Ordinal));
            if (tagInfo != null)
            {
                tagInfo.IsAll = true;
                this.tagFlowList.Invalidate();
            }
            else
            {
                tagInfo = new FileTagInfoEntity();
                tagInfo.Tag = tag;
                tagInfo.IsAll = true;
                this.TagList.Add(tagInfo);
                this.TagList.Sort((x, y) => x.Tag.CompareTo(y.Tag));
                this.tagFlowList.ItemCount = this.TagList.Count;
            }
        }

        private void DeleteTag(string tag)
        {
            if (this.FilePathList == null || this.TagList == null)
            {
                throw new NullReferenceException("ファイルの情報が存在しません。");
            }

            if (string.IsNullOrEmpty(tag))
            {
                throw new Exception("タグがNULLまたは長さ0の文字列です。");
            }

            if (this.TagList.Find(t => t.Tag.Equals(tag, StringComparison.Ordinal)) == null)
            {
                throw new Exception("リストに存在しないタグを指定しました。");
            }

            var param = new UpdateFileTagParameter();
            param.Tag = tag;
            param.FilePathList = this.FilePathList;
            this.DeleteFileTagProcess.Execute(this, param);

            var tagInfo = this.TagList.Find(t => t.Tag.Equals(tag, StringComparison.Ordinal));
            this.TagList.Remove(tagInfo);
            this.tagFlowList.ItemCount = this.TagList.Count;
        }

        private void DrawImageFileThumbnail(Graphics g, Image thumb, Rectangle rect)
        {
            ThumbnailUtil.DrawFileThumbnail(g, thumb, rect);
        }

        private void DrawDirectoryThumbnail(Graphics g, Image thumb, Rectangle rect)
        {
            ThumbnailUtil.DrawDirectoryThumbnail(g, thumb, rect, FileIconCash.LargeDirectoryIcon);
        }

        private void DrawFileIcon(Graphics g, Image icon, Rectangle rect)
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

        private void DrawSelectedFileCount(Graphics g, int count, Rectangle rect)
        {
            using (var sb = new SolidBrush(this.ForeColor))
            using (var sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisCharacter;
                string text = string.Format("Select {0} file", this.FilePathList.Count);
                g.DrawString(text, this.Font, sb, rect, sf);
            }
        }

        #endregion

        #region プロセスイベント

        private void GetFileInfoProcess_Callback(object sender, GetFileDeepInfoResult e)
        {
            this.ClearInfo();

            this.fileInfoSource = e;

            if (this.FileInfo != null)
            {
                this.fileNameLabel.Text = this.FileInfo.FileName;
                this.fileTypeLabel.Text = this.FileInfo.FileType;

                if (this.FileInfo.FileSize.HasValue)
                {
                    this.fileSizeLabel.Text = FileUtil.ToSizeString(this.FileInfo.FileSize.Value);
                    if (this.FileInfo.ImageSize.HasValue)
                    {
                        this.fileSizeLabel.Text += string.Format(" ({0} x {1})", this.FileInfo.ImageSize.Value.Width, this.FileInfo.ImageSize.Value.Height);
                    }
                }

                if (this.FileInfo.UpdateDate.HasValue)
                {
                    this.fileUpdatedateLabel.Text = string.Format("{0:yyyy/MM/dd HH:mm:ss}", this.FileInfo.UpdateDate.Value);
                }

                this.ratingBar.SetValue(this.FileInfo.Rating);
            }

            if (this.TagList != null)
            {
                this.tagFlowList.ItemCount = this.TagList.Count;
            }

            this.thumbnailPictureBox.Invalidate();
        }

        private void GetTagListProcess_Callback(object sender, ListEntity<string> e)
        {
            this.wideComboBox.AddItems(e);
            this.wideComboBox.SelectItem();
        }

        #endregion

        #region サムネイルピクチャーボックスイベント

        private void ThumbnailPictureBox_Paint(object sender, PaintEventArgs e)
        {

            if (this.Thumbnail != null)
            {
                var size = Math.Min(this.thumbnailPictureBox.Width, this.thumbnailPictureBox.Height);
                var x = (int)(0 + (this.thumbnailPictureBox.Width - size) / 2d);
                var y = (int)(0 + (this.thumbnailPictureBox.Height - size) / 2d);
                var rect = new Rectangle(x, y, size, size);
                if (this.FileInfo.IsFile)
                {
                    this.DrawImageFileThumbnail(e.Graphics, this.Thumbnail.ThumbnailImage, rect);
                }
                else
                {
                    this.DrawDirectoryThumbnail(e.Graphics, this.Thumbnail.ThumbnailImage, rect);
                }
            }
            else if (this.FileInfo != null)
            {
                const int margin = 32;
                var rect = new Rectangle(margin, margin, this.thumbnailPictureBox.Width - margin * 2, this.thumbnailPictureBox.Height - margin * 2);
                this.DrawFileIcon(e.Graphics, this.FileInfo.FileIcon, rect);
            }
            else if (this.FilePathList != null)
            {
                var rect = new Rectangle(0, 0, this.thumbnailPictureBox.Width, this.thumbnailPictureBox.Height);
                this.DrawSelectedFileCount(e.Graphics, this.FilePathList.Count, rect);
            }
        }

        #endregion

        #region タグリストイベント

        private void TagFlowList_DrawItem(object sender, SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (this.TagList == null)
            {
                return;
            }

            if (e.IsSelected)
            {
                e.Graphics.FillRectangle(this.tagFlowList.SelectedItemBrush, e.ItemRectangle);
                e.Graphics.DrawRectangle(this.tagFlowList.SelectedItemPen, e.ItemRectangle);
            }
            else if (e.IsMousePoint)
            {
                e.Graphics.FillRectangle(this.tagFlowList.MousePointItemBrush, e.ItemRectangle);
            }

            var item = this.TagList[e.ItemIndex];

            var iconSize = Math.Min(this.tagFlowList.ItemHeight, this.tagIcon.Width);

            var iconPoint = (int)((this.tagFlowList.ItemHeight - iconSize) / 2);

            var iconRect = new Rectangle(e.ItemRectangle.X + iconPoint,
                                         e.ItemRectangle.Y + iconPoint,
                                         iconSize,
                                         iconSize);

            e.Graphics.DrawImage(this.tagIcon, iconRect);

            var textRect = new Rectangle(e.ItemRectangle.X + this.tagFlowList.ItemHeight,
                                         e.ItemRectangle.Y,
                                         e.ItemRectangle.Width - this.tagFlowList.ItemHeight,
                                         e.ItemRectangle.Height);

            e.Graphics.DrawString(item.Tag,
                                  this.GetTagFont(item),
                                  this.tagFlowList.ItemTextBrush,
                                  textRect,
                                  this.tagFlowList.ItemTextFormat);
        }

        #endregion

        #region 評価値バーイベント

        private void RatingBar_RatingButtonMouseClick(object sender, MouseEventArgs e)
        {
            if (this.fileInfoSource == null)
            {
                return;
            }

            var param = new UpdateFileRatingParameter();
            param.FilePathList = this.fileInfoSource.FilePathList;
            param.RatingValue = this.ratingBar.Value;
            this.UpdateFileRatingProcess.Execute(this, param);
        }

        #endregion

        #region タグコンテキストメニューイベント

        private void TagContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            var p = this.tagFlowList.PointToClient(Cursor.Position);
            var index = this.tagFlowList.IndexFromPoint(p.X, p.Y);
            if (index < 0)
            {
                e.Cancel = true;
                return;
            }

            var tagInfo = this.TagList[index];
            this.tagToAllEntryMenuItem.Visible = !tagInfo.IsAll;
            this.contextMenuOperationTag = tagInfo.Tag;
        }

        private void TagDeleteMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.contextMenuOperationTag))
            {
                return;
            }

            this.DeleteTag(this.contextMenuOperationTag);
        }

        private void TagToAllEntryMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.contextMenuOperationTag))
            {
                return;
            }

            this.AddTag(this.contextMenuOperationTag);
        }

        private void TagFlowList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var p = this.tagFlowList.PointToClient(Cursor.Position);
            int index = this.tagFlowList.IndexFromPoint(p.X, p.Y);
            if (index < 0)
            {
                return;
            }

            var tagInfo = this.TagList[index];
            this.OnSelectedTag(new SelectedTagEventArgs(ContentsOpenType.OverlapTab, tagInfo.Tag));
        }

        private void TagFlowList_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Middle)
            {
                return;
            }

            var p = this.tagFlowList.PointToClient(Cursor.Position);
            var index = this.tagFlowList.IndexFromPoint(p.X, p.Y);
            if (index < 0)
            {
                return;
            }

            var tagInfo = this.TagList[index];
            this.OnSelectedTag(new SelectedTagEventArgs(ContentsOpenType.AddTab, tagInfo.Tag));
        }

        #endregion

        #region ワイドコンボボックスイベント

        private void WideComboBox_DropDownOpening(object sender, DropDownOpeningEventArgs e)
        {
            this.GetTagListProcess.Execute(this);
        }

        private void WideComboBox_AddItem(object sender, AddItemEventArgs e)
        {
            if (this.FilePathList == null || this.TagList == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(e.Item))
            {
                return;
            }

            if (this.TagList.FirstOrDefault(t => t.Tag.Equals(e.Item, StringComparison.Ordinal) && t.IsAll) != null)
            {
                return;
            }

            this.AddTag(e.Item);
        }

        #endregion
    }
}
