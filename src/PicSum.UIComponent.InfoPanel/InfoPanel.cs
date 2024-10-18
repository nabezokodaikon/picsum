using PicSum.Job.Entities;
using PicSum.Job.Jobs;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using PicSum.UIComponent.InfoPanel.Properties;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
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
        private static void DrawImageFileThumbnail(Graphics g, Image thumb, RectangleF rect)
        {
            ThumbnailUtil.DrawFileThumbnail(g, thumb, rect);
        }

        private static void DrawDirectoryThumbnail(Graphics g, Image thumb, RectangleF rect)
        {
            ThumbnailUtil.DrawDirectoryThumbnail(g, thumb, rect, FileIconCash.LargeDirectoryIcon);
        }

        public event EventHandler<SelectedTagEventArgs> SelectedTag;

        private TwoWayJob<FileDeepInfoGetJob, FileDeepInfoGetParameter, FileDeepInfoGetResult> getFileInfoJob = null;
        private OneWayJob<FileRatingUpdateJob, FileRatingUpdateParameter> updateFileRatingJob = null;
        private TwoWayJob<TagsGetJob, ListResult<string>> getTagListJob = null;
        private OneWayJob<FileTagAddJob, UpdateFileTagParameter> addFileTagJob = null;
        private OneWayJob<FileTagDeleteJob, UpdateFileTagParameter> deleteFileTagJob = null;

        private FileDeepInfoGetResult fileInfoSource = null;
        private Font allTagFont = null;
        private readonly Image tagIcon = Resources.TagIcon;
        private string contextMenuOperationTag = string.Empty;

        private TwoWayJob<FileDeepInfoGetJob, FileDeepInfoGetParameter, FileDeepInfoGetResult> GetFileInfoJob
        {
            get
            {
                if (this.getFileInfoJob == null)
                {
                    this.getFileInfoJob = new();
                    this.getFileInfoJob
                        .Callback(this.GetFileInfoJob_Callback);
                }

                return this.getFileInfoJob;
            }
        }

        private OneWayJob<FileRatingUpdateJob, FileRatingUpdateParameter> UpdateFileRatingJob
        {
            get
            {
                return this.updateFileRatingJob ??= new();
            }
        }

        private TwoWayJob<TagsGetJob, ListResult<string>> GetTagListJob
        {
            get
            {
                if (this.getTagListJob == null)
                {
                    this.getTagListJob = new();
                    this.getTagListJob
                        .Callback(this.GetTagListJob_Callback);
                }

                return this.getTagListJob;
            }
        }

        private OneWayJob<FileTagAddJob, UpdateFileTagParameter> AddFileTagJob
        {
            get
            {
                return this.addFileTagJob ??= new();
            }
        }

        private OneWayJob<FileTagDeleteJob, UpdateFileTagParameter> DeleteFileTagJob
        {
            get
            {
                return this.deleteFileTagJob ??= new();
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

        public InfoPanel()
        {
            this.InitializeComponent();

            if (!this.DesignMode)
            {
                this.CreateHandle();
            }
        }

        public void SetFileInfo(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this.SetFileInfo([filePath]);
        }

        public void SetFileInfo(IList<string> filePathList)
        {
            ArgumentNullException.ThrowIfNull(filePathList, nameof(filePathList));

            if (filePathList.Count > 0)
            {
                // コンピュータの場合は情報を表示しない。
                if (FileUtil.IsSystemRoot(filePathList.First()))
                {
                    this.ClearInfo();
                    return;
                }

                var param = new FileDeepInfoGetParameter
                {
                    FilePathList = filePathList,
                    ThumbnailSize = new Size(
                        ApplicationConstants.INFOPANEL_WIDTH,
                        ApplicationConstants.INFOPANEL_WIDTH)
                };

                this.GetFileInfoJob.StartJob(param);
            }
            else
            {
                this.ClearInfo();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.getFileInfoJob?.Dispose();
                this.getFileInfoJob = null;

                this.updateFileRatingJob?.Dispose();
                this.updateFileRatingJob = null;

                this.getTagListJob?.Dispose();
                this.getTagListJob = null;

                this.addFileTagJob?.Dispose();
                this.addFileTagJob = null;

                this.deleteFileTagJob?.Dispose();
                this.deleteFileTagJob = null;

                this.components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void OnSelectedTag(SelectedTagEventArgs e)
        {
            this.SelectedTag?.Invoke(this, e);
        }

        private void ClearInfo()
        {
            if (this.Thumbnail != null
                && this.Thumbnail.ThumbnailImage != null)
            {
                this.Thumbnail.ThumbnailImage.Dispose();
            }

            this.fileInfoSource = null;

            this.fileInfoLabel.FileName = string.Empty;
            this.fileInfoLabel.FileType = string.Empty;
            this.fileInfoLabel.FileSize = string.Empty;
            this.fileInfoLabel.Timestamp = string.Empty;
            this.ratingBar.SetValue(0);
            this.thumbnailPictureBox.Invalidate();
            this.tagFlowList.ItemCount = 0;

            this.contextMenuOperationTag = string.Empty;
            this.tagContextMenuStrip.Close();
        }

        private Font GetAllTagFont()
        {
            this.allTagFont ??=
                new Font(
                    this.Font.FontFamily,
                    this.Font.Size,
                    FontStyle.Bold,
                    this.Font.Unit,
                    this.Font.GdiCharSet);
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

            var param = new UpdateFileTagParameter
            {
                Tag = tag,
                FilePathList = this.FilePathList
            };
            this.AddFileTagJob.StartJob(param);

            var tagInfo = this.TagList.Find(t => t.Tag.Equals(tag, StringComparison.Ordinal));
            if (tagInfo != null)
            {
                tagInfo.IsAll = true;
                this.tagFlowList.Invalidate();
            }
            else
            {
                tagInfo = new FileTagInfoEntity
                {
                    Tag = tag,
                    IsAll = true
                };
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

            var param = new UpdateFileTagParameter
            {
                Tag = tag,
                FilePathList = this.FilePathList
            };
            this.DeleteFileTagJob.StartJob(param);

            var tagInfo = this.TagList.Find(t => t.Tag.Equals(tag, StringComparison.Ordinal));
            this.TagList.Remove(tagInfo);
            this.tagFlowList.ItemCount = this.TagList.Count;
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

        private void DrawSelectedFileCount(Graphics g, Rectangle rect)
        {
            using (var sb = new SolidBrush(this.ForeColor))
            using (var sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisCharacter;
                var text = $"{this.FilePathList.Count} files selected";
                g.DrawString(text, this.Font, sb, rect, sf);
            }
        }

        private void DrawErrorMessage(Graphics g, Rectangle rect)
        {
            using (var sb = new SolidBrush(this.ForeColor))
            using (var sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisCharacter;
                var text = $"Failed to load file";
                g.DrawString(text, this.Font, sb, rect, sf);
            }
        }

        private void GetFileInfoJob_Callback(FileDeepInfoGetResult result)
        {
            this.ClearInfo();

            this.fileInfoSource = result;

            if (this.FileInfo != null)
            {
                this.fileInfoLabel.FileName = this.FileInfo.FileName;
                this.fileInfoLabel.FileType = this.FileInfo.FileType;

                if (this.FileInfo.FileSize.HasValue)
                {
                    this.fileInfoLabel.FileSize = FileUtil.ToSizeUnitString(this.FileInfo.FileSize.Value);
                    if (this.FileInfo.ImageSize.HasValue)
                    {
                        this.fileInfoLabel.FileSize
                            += $" ({this.FileInfo.ImageSize.Value.Width} x {this.FileInfo.ImageSize.Value.Height})";
                    }
                }

                this.fileInfoLabel.Timestamp
                    = $"{this.FileInfo.UpdateDate:yyyy/MM/dd HH:mm:ss}";
                this.ratingBar.SetValue(this.FileInfo.Rating);
            }

            if (this.TagList != null)
            {
                this.tagFlowList.ItemCount = this.TagList.Count;
            }

            this.thumbnailPictureBox.Invalidate();
        }

        private void GetTagListJob_Callback(ListResult<string> result)
        {
            this.wideComboBox.SetItems(result);
            this.wideComboBox.SelectItem();
        }

        private void ThumbnailPictureBox_Paint(object sender, PaintEventArgs e)
        {

            if (this.Thumbnail != null)
            {
                var size = Math.Min(this.thumbnailPictureBox.Width, this.thumbnailPictureBox.Height);
                var x = 0 + (this.thumbnailPictureBox.Width - size) / 2f;
                var y = 0 + (this.thumbnailPictureBox.Height - size) / 2f;
                var rect = new RectangleF(x, y, size, size);
                if (this.FileInfo.IsFile)
                {
                    DrawImageFileThumbnail(e.Graphics, this.Thumbnail.ThumbnailImage, rect);
                }
                else
                {
                    DrawDirectoryThumbnail(e.Graphics, this.Thumbnail.ThumbnailImage, rect);
                }
            }
            else if (this.FileInfo != null && this.FileInfo.FileIcon != null)
            {
                const int margin = 32;
                var rect = new Rectangle(margin, margin, this.thumbnailPictureBox.Width - margin * 2, this.thumbnailPictureBox.Height - margin * 2);
                this.DrawFileIcon(e.Graphics, this.FileInfo.FileIcon, rect);
            }
            else if (this.FileInfo != null && this.FileInfo.FileIcon == null)
            {
                var rect = new Rectangle(0, 0, this.thumbnailPictureBox.Width, this.thumbnailPictureBox.Height);
                this.DrawErrorMessage(e.Graphics, rect);
            }
            else if (this.FilePathList != null)
            {
                var rect = new Rectangle(0, 0, this.thumbnailPictureBox.Width, this.thumbnailPictureBox.Height);
                this.DrawSelectedFileCount(e.Graphics, rect);
            }
        }

        private void TagFlowList_DrawItem(object sender, SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (this.TagList == null)
            {
                return;
            }

            if (e.IsSelected)
            {
                e.Graphics.FillRectangle(this.tagFlowList.SelectedItemBrush, e.ItemRectangle);
            }
            else if (e.IsMousePoint)
            {
                e.Graphics.FillRectangle(this.tagFlowList.MousePointItemBrush, e.ItemRectangle);
            }

            var item = this.TagList[e.ItemIndex];

            var iconSize = Math.Min(this.tagFlowList.ItemHeight, this.tagIcon.Width);

            var iconPoint = (this.tagFlowList.ItemHeight - iconSize) / 2f;

            var iconRect = new RectangleF(e.ItemRectangle.X + iconPoint,
                                          e.ItemRectangle.Y + iconPoint,
                                          iconSize,
                                          iconSize);

            e.Graphics.DrawImage(this.tagIcon, iconRect);

            var textRect = new RectangleF(e.ItemRectangle.X + this.tagFlowList.ItemHeight,
                                          e.ItemRectangle.Y,
                                          e.ItemRectangle.Width - this.tagFlowList.ItemHeight,
                                          e.ItemRectangle.Height);

            e.Graphics.DrawString(item.Tag,
                                  this.GetTagFont(item),
                                  this.tagFlowList.ItemTextBrush,
                                  textRect,
                                  this.tagFlowList.ItemTextFormat);
        }

        private void RatingBar_RatingButtonMouseClick(object sender, MouseEventArgs e)
        {
            if (this.fileInfoSource == null)
            {
                return;
            }

            var param = new FileRatingUpdateParameter
            {
                FilePathList = this.fileInfoSource.FilePathList,
                RatingValue = this.ratingBar.Value
            };
            this.UpdateFileRatingJob.StartJob(param);
        }

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
            var index = this.tagFlowList.IndexFromPoint(p.X, p.Y);
            if (index < 0)
            {
                return;
            }

            var tagInfo = this.TagList[index];
            this.OnSelectedTag(new SelectedTagEventArgs(PageOpenType.OverlapTab, tagInfo.Tag));
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
            this.OnSelectedTag(new SelectedTagEventArgs(PageOpenType.AddTab, tagInfo.Tag));
        }

        private void WideComboBox_DropDownOpening(object sender, DropDownOpeningEventArgs e)
        {
            this.GetTagListJob.StartJob();
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

    }
}
