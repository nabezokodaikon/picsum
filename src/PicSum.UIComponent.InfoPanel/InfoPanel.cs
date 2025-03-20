using PicSum.Job.Common;
using PicSum.Job.Entities;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using SWF.UIComponent.WideDropDown;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.InfoPanel
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class InfoPanel
        : UserControl, ISender
    {
        public event EventHandler<SelectedTagEventArgs> SelectedTag;

        private bool disposed = false;

        private FileDeepInfoGetResult fileInfoSource = null;
        private Font allTagFont = null;
        private readonly Image tagIcon = ResourceFiles.TagIcon.Value;
        private string contextMenuOperationTag = string.Empty;
        private bool isLoading = false;

        private string[] FilePathList
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

        public void SetFileInfo(string[] filePathList)
        {
            ArgumentNullException.ThrowIfNull(filePathList, nameof(filePathList));

            if (filePathList.Length > 0)
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
                        AppConstants.INFOPANEL_WIDTH,
                        AppConstants.INFOPANEL_WIDTH)
                };

                this.isLoading = true;

                Instance<JobCaller>.Value.FileDeepInfoLoadingJob.Value
                    .StartJob(this, param, _ =>
                    {
                        if (this.disposed)
                        {
                            return;
                        }

                        if (!this.isLoading)
                        {
                            return;
                        }

                        this.GetFileInfoJob_Callback(_);
                    });

                Instance<JobCaller>.Value.FileDeepInfoGetJob.Value
                    .StartJob(this, param, _ =>
                    {
                        if (this.disposed)
                        {
                            return;
                        }

                        this.isLoading = false;

                        this.GetFileInfoJob_Callback(_);
                    });
            }
            else
            {
                this.ClearInfo();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.components?.Dispose();
            }

            this.disposed = true;

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
            this.tagFlowList.ItemCount = 0;

            this.contextMenuOperationTag = string.Empty;
            this.tagContextMenuStrip.Close();
        }

        private Font GetAllTagFont()
        {
            this.allTagFont ??=
                new Font(
                    this.tagFlowList.Font.FontFamily,
                    this.tagFlowList.Font.Size,
                    FontStyle.Bold,
                    this.tagFlowList.Font.Unit,
                    this.tagFlowList.Font.GdiCharSet);
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
                return this.tagFlowList.Font;
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

            var param = new FileTagUpdateParameter
            {
                Tag = tag,
                FilePathList = this.FilePathList
            };
            Instance<JobCaller>.Value.StartFileTagAddJob(this, param);

            var tagInfo = this.TagList.Find(t => t.Tag.Equals(tag, StringComparison.Ordinal));
            if (tagInfo != null)
            {
                tagInfo.IsAll = true;
                this.tagFlowList.Invalidate();
                this.tagFlowList.Update();
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

            var param = new FileTagUpdateParameter
            {
                Tag = tag,
                FilePathList = this.FilePathList
            };
            Instance<JobCaller>.Value.StartFileTagDeleteJob(this, param);

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
                var text = $"{this.FilePathList.Length} files selected";
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

                if (this.FileInfo.UpdateDate == FileUtil.EMPTY_DATETIME)
                {
                    this.fileInfoLabel.Timestamp = string.Empty;
                }
                else
                {
                    this.fileInfoLabel.Timestamp
                        = $"{this.FileInfo.UpdateDate:yyyy/MM/dd HH:mm:ss}";
                }

                this.ratingBar.SetValue(this.FileInfo.Rating);
            }

            if (this.TagList != null)
            {
                this.tagFlowList.ItemCount = this.TagList.Count;
            }

            this.thumbnailPictureBox.Invalidate();
            this.thumbnailPictureBox.Update();
        }

        private void GetTagListJob_Callback(ListResult<string> result)
        {
            this.wideComboBox.SetItems([.. result]);
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
                    ThumbnailUtil.AdjustDrawFileThumbnail(
                        e.Graphics,
                        this.Thumbnail.ThumbnailImage,
                        rect,
                        new Size(this.Thumbnail.SourceWidth, this.Thumbnail.SourceHeight));
                }
                else
                {
                    ThumbnailUtil.AdjustDrawDirectoryThumbnail(
                        e.Graphics,
                        this.Thumbnail.ThumbnailImage,
                        rect,
                        new Size(this.Thumbnail.SourceWidth, this.Thumbnail.SourceHeight),
                        Instance<IFileIconCacher>.Value.JumboDirectoryIcon);
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

            var iconWidth = this.tagIcon.Width + 8;
            var itemFont = this.GetTagFont(item);
            var itemText = item.Tag;
            var itemTextSize = TextRenderer.MeasureText(itemText, itemFont);
            var itemWidth = e.ItemRectangle.Width - iconWidth;
            var destText = itemText;
            var destTextSize = itemTextSize;
            while (destTextSize.Width > itemWidth)
            {
                destText = destText.Substring(0, destText.Length - 1);
                destTextSize = TextRenderer.MeasureText($"{destText}...", itemFont);
            }
            destText = itemText == destText ? itemText : $"{destText}...";

            var textRect = new Rectangle(e.ItemRectangle.X + iconWidth,
                                         e.ItemRectangle.Y + (int)((e.ItemRectangle.Height - destTextSize.Height) / 2f),
                                         e.ItemRectangle.Width - iconWidth,
                                         e.ItemRectangle.Height);

            TextRenderer.DrawText(
                e.Graphics,
                destText,
                itemFont,
                textRect.Location,
                this.tagFlowList.ItemTextBrush.Color,
                TextFormatFlags.Top);
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
            Instance<JobCaller>.Value.StartFileRatingUpdateJob(this, param);
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
            Instance<JobCaller>.Value.TagsGetJob.Value
                .StartJob(this, _ =>
                {
                    if (this.disposed)
                    {
                        return;
                    }

                    this.GetTagListJob_Callback(_);
                });
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
