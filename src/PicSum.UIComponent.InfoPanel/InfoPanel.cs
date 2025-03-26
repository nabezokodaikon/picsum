using PicSum.Job.Common;
using PicSum.Job.Entities;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using SWF.UIComponent.Core;
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
        : ToolPanel, ISender
    {
        public event EventHandler<SelectedTagEventArgs> SelectedTag;

        private bool disposed = false;

        private FileDeepInfoGetResult fileInfoSource = FileDeepInfoGetResult.EMPTY;
        private Font allTagFont = null;
        private readonly Image tagIcon = ResourceFiles.TagIcon.Value;
        private string contextMenuOperationTag = string.Empty;
        private bool isLoading = false;
        private readonly SolidBrush foreColorBrush;
        private readonly StringFormat stringFormat;

        private string[] FilePathList
        {
            get
            {
                if (this.fileInfoSource != FileDeepInfoGetResult.EMPTY && this.fileInfoSource.FilePathList != null)
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
                if (this.fileInfoSource == FileDeepInfoGetResult.EMPTY)
                {
                    return FileDeepInfoEntity.EMPTY;
                }
                else if (this.fileInfoSource == FileDeepInfoGetResult.ERROR)
                {
                    return FileDeepInfoEntity.ERROR;
                }
                else if (this.fileInfoSource.FileInfo == FileDeepInfoEntity.EMPTY)
                {
                    return FileDeepInfoEntity.EMPTY;
                }
                else if (this.fileInfoSource.FileInfo == FileDeepInfoEntity.ERROR)
                {
                    return FileDeepInfoEntity.ERROR;
                }

                return this.fileInfoSource.FileInfo;
            }
        }

        private ThumbnailImageResult Thumbnail
        {
            get
            {
                if (this.FileInfo.Thumbnail.ThumbnailImage != null)
                {
                    return this.FileInfo.Thumbnail;
                }
                else
                {
                    return ThumbnailImageResult.EMPTY;
                }
            }
        }

        private ListEntity<FileTagInfoEntity> TagList
        {
            get
            {
                if (this.fileInfoSource != FileDeepInfoGetResult.EMPTY && this.fileInfoSource.TagInfoList != null)
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

            this.foreColorBrush = new SolidBrush(this.ForeColor);
            this.stringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
            };
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
                this.thumbnailPictureBox.Invalidate();
                this.thumbnailPictureBox.Update();
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
            if (this.Thumbnail != ThumbnailImageResult.EMPTY
                && this.Thumbnail.ThumbnailImage != null)
            {
                this.Thumbnail.ThumbnailImage.Dispose();
            }

            this.fileInfoSource = FileDeepInfoGetResult.EMPTY;

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

            if (this.TagList.Any(t => t.Tag.Equals(tag, StringComparison.Ordinal) && t.IsAll))
            {
                throw new Exception("既に登録されているタグです。");
            }

            var param = new FileTagUpdateParameter
            {
                Tag = tag,
                FilePathList = this.FilePathList
            };
            Instance<JobCaller>.Value.StartFileTagAddJob(this, param);

            var tagInfo = this.TagList.FirstOrDefault(
                t => t.Tag.Equals(tag, StringComparison.Ordinal),
                FileTagInfoEntity.EMPTY);
            if (tagInfo != FileTagInfoEntity.EMPTY)
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

            if (!this.TagList.Any(t => t.Tag.Equals(tag, StringComparison.Ordinal)))
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
            var text = $"{this.FilePathList.Length} files selected";
            g.DrawString(text, new Font(
                this.Font.FontFamily, (int)(this.Font.Size * 1.5)), this.foreColorBrush, rect, this.stringFormat);
        }

        private void DrawErrorMessage(Graphics g, Rectangle rect)
        {
            var text = $"Failed to load file";
            g.DrawString(text, new Font(
                this.Font.FontFamily, (int)(this.Font.Size * 1.5)), this.foreColorBrush, rect, this.stringFormat);
        }

        private void GetFileInfoJob_Callback(FileDeepInfoGetResult result)
        {
            this.ClearInfo();

            this.fileInfoSource = result;

            if (this.FileInfo != FileDeepInfoEntity.EMPTY
                && this.FileInfo != FileDeepInfoEntity.ERROR)
            {
                this.fileInfoLabel.FileName = this.FileInfo.FileName;
                this.fileInfoLabel.FileType = this.FileInfo.FileType;

                if (this.FileInfo.IsFile)
                {
                    this.fileInfoLabel.FileSize = FileUtil.ToSizeUnitString(this.FileInfo.FileSize);
                    if (this.FileInfo.ImageSize != ImageUtil.EMPTY_SIZE)
                    {
                        this.fileInfoLabel.FileSize
                            += $" ({this.FileInfo.ImageSize.Width} x {this.FileInfo.ImageSize.Height})";
                    }
                }

                if (this.FileInfo.UpdateDate != FileUtil.EMPTY_DATETIME)
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
            if (this.Thumbnail != ThumbnailImageResult.EMPTY)
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
            else if (this.FileInfo.FileIcon != null)
            {
                const int margin = 32;
                var rect = new Rectangle(margin, margin, this.thumbnailPictureBox.Width - margin * 2, this.thumbnailPictureBox.Height - margin * 2);
                this.DrawFileIcon(e.Graphics, this.FileInfo.FileIcon, rect);
            }
            else if (this.FilePathList != null)
            {
                var rect = new Rectangle(0, 0, this.thumbnailPictureBox.Width, this.thumbnailPictureBox.Height);
                this.DrawSelectedFileCount(e.Graphics, rect);
            }
            else if (this.FileInfo == FileDeepInfoEntity.ERROR)
            {
                var rect = new Rectangle(0, 0, this.thumbnailPictureBox.Width, this.thumbnailPictureBox.Height);
                this.DrawErrorMessage(e.Graphics, rect);
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
            if (this.fileInfoSource == FileDeepInfoGetResult.EMPTY)
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

            if (this.TagList.Any(t => t.Tag.Equals(e.Item, StringComparison.Ordinal) && t.IsAll))
            {
                return;
            }

            this.AddTag(e.Item);
        }
    }
}
