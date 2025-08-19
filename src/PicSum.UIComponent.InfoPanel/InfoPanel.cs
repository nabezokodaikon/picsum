using PicSum.Job.Common;
using PicSum.Job.Entities;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using SWF.Core.ResourceAccessor;
using SWF.UIComponent.Base;
using SWF.UIComponent.WideDropDown;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace PicSum.UIComponent.InfoPanel
{

    public sealed partial class InfoPanel
        : ToolPanel, ISender
    {
        private const int TAG_FLOW_LIST_DEFAULT_ITEM_HEIGHT = 32;
        private const float TAG_FLOW_LIST_DEFAULT_FONT_SIZE = 10f;

        private static readonly Rectangle THUMBNAIL_PICTURE_BOX_DEFAULT_BOUNDS
            = new(4, 0, 508, 256);
        private static readonly Rectangle FILE_INFO_LABEL_DEFAULT_BOUNDS
            = new(4, 256, 508, 180);
        private static readonly Rectangle RATING_BAR_DEFAULT_BOUNDS
            = new(4, 390, 508, 64);
        private static readonly Rectangle WIDE_COMBO_BOX_DEFAULT_BOUNDS
            = new(4, 446, 508, 32);
        private static readonly Rectangle TAG_FLOW_LIST_DEFAULT_BOUNDS
            = new(4, 478, 508, 231);
        private const int VERTICAL_DEFAULT_TOP_MARGIN = 28;

        public event EventHandler<SelectedTagEventArgs> SelectedTag;

        private bool disposed = false;

        private FileDeepInfoGetResult _fileInfoSource = FileDeepInfoGetResult.EMPTY;
        private Image _tagIcon = null;
        private readonly Dictionary<float, Bitmap> _tagIconCache = [];
        private string _contextMenuOperationTag = string.Empty;
        private readonly SolidBrush _foreColorBrush;
        private readonly StringFormat _stringFormat;
        private bool _isLoading = false;

        private string[] FilePathList
        {
            get
            {
                if (!this._fileInfoSource.IsEmpty && this._fileInfoSource.FilePathList != null)
                {
                    return this._fileInfoSource.FilePathList;
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
                if (this._fileInfoSource.IsEmpty)
                {
                    return FileDeepInfoEntity.EMPTY;
                }
                else if (this._fileInfoSource.IsError)
                {
                    return FileDeepInfoEntity.ERROR;
                }
                else if (this._fileInfoSource.FileInfo.IsEmpty)
                {
                    return FileDeepInfoEntity.EMPTY;
                }
                else if (this._fileInfoSource.FileInfo.IsError)
                {
                    return FileDeepInfoEntity.ERROR;
                }

                return this._fileInfoSource.FileInfo;
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
                if (!this._fileInfoSource.IsEmpty && this._fileInfoSource.TagInfoList != null)
                {
                    return this._fileInfoSource.TagInfoList;
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

            this._foreColorBrush = new SolidBrush(this.ForeColor);
            this._stringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
            };
        }

        public void SetControlsBounds(float scale)
        {
            this.SuspendLayout();

            this.thumbnailPictureBox.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left
                | AnchorStyles.Right;

            this.fileInfoLabel.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left
                | AnchorStyles.Right;

            this.ratingBar.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left
                | AnchorStyles.Right;

            this.wideComboBox.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left
                | AnchorStyles.Right;

            this.tagFlowList.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left
                | AnchorStyles.Right
                | AnchorStyles.Bottom;

            this.thumbnailPictureBox.SetBounds(
                THUMBNAIL_PICTURE_BOX_DEFAULT_BOUNDS.X,
                THUMBNAIL_PICTURE_BOX_DEFAULT_BOUNDS.Y,
                this.Width - THUMBNAIL_PICTURE_BOX_DEFAULT_BOUNDS.X * 2,
                (int)(THUMBNAIL_PICTURE_BOX_DEFAULT_BOUNDS.Height * scale));

            this.fileInfoLabel.SetBounds(
                this.thumbnailPictureBox.Left,
                this.thumbnailPictureBox.Top + this.thumbnailPictureBox.Bottom,
                this.thumbnailPictureBox.Width,
                (int)(FILE_INFO_LABEL_DEFAULT_BOUNDS.Height * scale));

            this.ratingBar.SetBounds(
                this.thumbnailPictureBox.Left,
                this.thumbnailPictureBox.Top + this.fileInfoLabel.Bottom,
                this.thumbnailPictureBox.Width,
                (int)(RATING_BAR_DEFAULT_BOUNDS.Height * scale));

            this.ratingBar.SetControlsBounds(scale);

            this.wideComboBox.SetBounds(
                this.thumbnailPictureBox.Left,
                this.thumbnailPictureBox.Top + this.ratingBar.Bottom,
                this.thumbnailPictureBox.Width,
                (int)(WIDE_COMBO_BOX_DEFAULT_BOUNDS.Height * scale));

            this.wideComboBox.SetControlsBounds(scale);

            this.tagFlowList.SetBounds(
                this.thumbnailPictureBox.Left,
                this.thumbnailPictureBox.Top + this.wideComboBox.Bottom,
                this.thumbnailPictureBox.Width,
                (int)(this.Height - (this.thumbnailPictureBox.Top + this.wideComboBox.Bottom) - 9 * scale));

            this.VerticalTopMargin = (int)(VERTICAL_DEFAULT_TOP_MARGIN * scale);

            this._tagIcon = this.GetTagIcon(scale);
            this.tagFlowList.ItemHeight = (int)(TAG_FLOW_LIST_DEFAULT_ITEM_HEIGHT * scale);

            this.ResumeLayout(false);
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
                    this.fileInfoLabel.Invalidate();
                    this.ratingBar.Invalidate();
                    this.thumbnailPictureBox.Invalidate();
                    return;
                }

                this._isLoading = true;

                var scale = WindowUtil.GetCurrentWindowScale(this);
                var param = new FileDeepInfoGetParameter
                {
                    FilePathList = filePathList,
                    ThumbnailSize = new Size(
                        ThumbnailUtil.THUMBNAIL_MAXIMUM_SIZE,
                        ThumbnailUtil.THUMBNAIL_MAXIMUM_SIZE)
                };

                Instance<JobCaller>.Value.FileDeepInfoLoadingJob.Value
                    .StartJob(this, param, _ =>
                    {
                        if (this.disposed)
                        {
                            return;
                        }

                        if (!this._isLoading)
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

                        this._isLoading = false;

                        this.GetFileInfoJob_Callback(_);
                    });
            }
            else
            {
                this.ClearInfo();
                this.fileInfoLabel.Invalidate();
                this.ratingBar.Invalidate();
                this.thumbnailPictureBox.Invalidate();
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
                foreach (var icon in this._tagIconCache.Values)
                {
                    icon.Dispose();
                }
                this._tagIconCache.Clear();

                this._foreColorBrush.Dispose();
                this._stringFormat.Dispose();
            }

            this.disposed = true;

            base.Dispose(disposing);
        }

        private Bitmap GetTagIcon(float scale)
        {
            if (this._tagIconCache.TryGetValue(scale, out var font))
            {
                return font;
            }

            var newTagIcon = new Bitmap(
                (int)(ResourceFiles.TagIcon.Value.Width * scale),
                (int)(ResourceFiles.TagIcon.Value.Height * scale));
            using (var g = Graphics.FromImage(newTagIcon))
            {
                g.DrawImage(ResourceFiles.TagIcon.Value, 0, 0, newTagIcon.Width, newTagIcon.Height);
            }

            this._tagIconCache.Add(scale, newTagIcon);

            return newTagIcon;
        }

        private void OnSelectedTag(SelectedTagEventArgs e)
        {
            this.SelectedTag?.Invoke(this, e);
        }

        private void ClearInfo()
        {
            if (!this.Thumbnail.IsEmpty
                && this.Thumbnail.ThumbnailImage != null)
            {
                this.Thumbnail.ThumbnailImage.Dispose();
            }

            this._fileInfoSource = FileDeepInfoGetResult.EMPTY;

            this.fileInfoLabel.FileName = string.Empty;
            this.fileInfoLabel.FileType = string.Empty;
            this.fileInfoLabel.FileSize = string.Empty;
            this.fileInfoLabel.CreateDate = string.Empty;
            this.fileInfoLabel.UpdateDate = string.Empty;
            this.fileInfoLabel.TakenDate = string.Empty;

            this.ratingBar.SetValue(0);
            this.tagFlowList.ItemCount = 0;

            this._contextMenuOperationTag = string.Empty;
            this.tagContextMenuStrip.Close();
        }

        private Font GetTagFont(FileTagInfoEntity tagInfo, float scale)
        {
            if (tagInfo.IsAll)
            {
                return Fonts.GetBoldFont(Fonts.Size.Medium, scale);
            }
            else
            {
                return Fonts.GetRegularFont(Fonts.Size.Medium, scale);
            }
        }

        private void AddTag(string tag)
        {
            if (this.FilePathList == null || this.TagList == null)
            {
                throw new NotSupportedException("ファイルの情報が存在しません。");
            }

            if (string.IsNullOrEmpty(tag))
            {
                throw new NotSupportedException("NULLまたは長さ0の文字列は、タグに登録できません。");
            }

            if (this.TagList.Any(t => t.Tag.Equals(tag, StringComparison.Ordinal) && t.IsAll))
            {
                throw new NotSupportedException("既に登録されているタグです。");
            }

            var param = new FileTagUpdateParameter
            {
                Tag = tag,
                FilePathList = this.FilePathList
            };
            Instance<JobCaller>.Value.EnqueueFileTagUpdateJob(this, param);

            var tagInfo = this.TagList.FirstOrDefault(
                t => t.Tag.Equals(tag, StringComparison.Ordinal),
                FileTagInfoEntity.EMPTY);
            if (!tagInfo.IsEmpty)
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
                this.TagList.Sort(static (x, y) => x.Tag.CompareTo(y.Tag));
                this.tagFlowList.ItemCount = this.TagList.Count;
            }
        }

        private void DeleteTag(string tag)
        {
            if (this.FilePathList == null || this.TagList == null)
            {
                throw new NotSupportedException("ファイルの情報が存在しません。");
            }

            if (string.IsNullOrEmpty(tag))
            {
                throw new NotSupportedException("タグがNULLまたは長さ0の文字列です。");
            }

            if (!this.TagList.Any(t => t.Tag.Equals(tag, StringComparison.Ordinal)))
            {
                throw new NotSupportedException("リストに存在しないタグを指定しました。");
            }

            var param = new FileTagUpdateParameter
            {
                Tag = tag,
                FilePathList = this.FilePathList
            };
            Instance<JobCaller>.Value.EnqueueFileTagDeleteJob(this, param);

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
            var scale = WindowUtil.GetCurrentWindowScale(this);
            var text = $"{this.FilePathList.Length} files selected";
            g.DrawString(
                text,
                Fonts.GetRegularFont(Fonts.Size.Large, scale),
                this._foreColorBrush,
                rect,
                this._stringFormat);
        }

        private void DrawErrorMessage(Graphics g, Rectangle rect)
        {
            var scale = WindowUtil.GetCurrentWindowScale(this);
            var text = $"Failed to load file";
            g.DrawString(
                text,
                Fonts.GetRegularFont(Fonts.Size.Large, scale),
                this._foreColorBrush,
                rect,
                this._stringFormat);
        }

        private void GetFileInfoJob_Callback(FileDeepInfoGetResult result)
        {
            this.ClearInfo();

            this._fileInfoSource = result;

            if (!this.FileInfo.IsEmpty
                && !this.FileInfo.IsError)
            {
                this.fileInfoLabel.FileName = this.FileInfo.FileName;
                this.fileInfoLabel.FileType = this.FileInfo.FileType;

                if (this.FileInfo.IsFile)
                {
                    this.fileInfoLabel.FileSize = FileUtil.ToSizeUnitString(this.FileInfo.FileSize);
                    if (this.FileInfo.ImageSize != ImageUtil.EMPTY_SIZE)
                    {
                        this.fileInfoLabel.FileSize
                            += $"  ({this.FileInfo.ImageSize.Width} x {this.FileInfo.ImageSize.Height})";
                    }
                }

                if (!this.FileInfo.CreateDate.IsEmpty())
                {
                    this.fileInfoLabel.CreateDate
                        = $"{this.FileInfo.CreateDate:yyyy/MM/dd HH:mm:ss}";
                }

                if (!this.FileInfo.UpdateDate.IsEmpty())
                {
                    this.fileInfoLabel.UpdateDate
                        = $"{this.FileInfo.UpdateDate:yyyy/MM/dd HH:mm:ss}";
                }

                if (!this.FileInfo.TakenDate.IsEmpty())
                {
                    this.fileInfoLabel.TakenDate
                        = $"{this.FileInfo.TakenDate:yyyy/MM/dd HH:mm:ss}";
                }

                this.ratingBar.SetValue(this.FileInfo.Rating);
            }

            if (this.TagList != null)
            {
                this.tagFlowList.ItemCount = this.TagList.Count;
            }

            this.fileInfoLabel.Invalidate();
            this.ratingBar.Invalidate();
            this.thumbnailPictureBox.Invalidate();
        }

        private void GetTagListJob_Callback(ListResult<string> result)
        {
            this.wideComboBox.SetItems([.. result]);
            this.wideComboBox.SelectItem();
        }

        private void ThumbnailPictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (this.FileInfo.IsError)
            {
                var rect = new Rectangle(0, 0, this.thumbnailPictureBox.Width, this.thumbnailPictureBox.Height);
                this.DrawErrorMessage(e.Graphics, rect);
            }
            else if (!this.Thumbnail.IsEmpty
                && !this.Thumbnail.ThumbnailImage.IsEmpry)
            {
                e.Graphics.SmoothingMode = SmoothingMode.None;
                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                e.Graphics.CompositingMode = CompositingMode.SourceOver;

                var size = Math.Min(this.thumbnailPictureBox.Width, this.thumbnailPictureBox.Height);
                var x = 0 + (this.thumbnailPictureBox.Width - size) / 2f;
                var y = 0 + (this.thumbnailPictureBox.Height - size) / 2f;
                var rect = new RectangleF(x, y, size, size);
                if (this.FileInfo.IsFile)
                {
                    ThumbnailUtil.DrawFileThumbnail(
                        this,
                        e.Graphics,
                        this.Thumbnail.ThumbnailImage,
                        rect,
                        new Size(this.Thumbnail.SourceWidth, this.Thumbnail.SourceHeight));
                }
                else
                {
                    ThumbnailUtil.DrawDirectoryThumbnail(
                        this,
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
        }

        private void TagFlowList_DrawItem(object sender, SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (this.TagList == null)
            {
                return;
            }

            if (this._tagIcon == null)
            {
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;

            if (e.IsSelected)
            {
                e.Graphics.FillRectangle(this.tagFlowList.SelectedItemBrush, e.ItemRectangle);
            }
            else if (e.IsMousePoint)
            {
                e.Graphics.FillRectangle(this.tagFlowList.MousePointItemBrush, e.ItemRectangle);
            }

            var item = this.TagList[e.ItemIndex];

            var scale = WindowUtil.GetCurrentWindowScale(this);
            var iconSizeMargin = 8 * scale;
            var iconSize = Math.Min(this._tagIcon.Width, e.ItemRectangle.Height) - iconSizeMargin * 2;

            var iconPoint = (this.tagFlowList.ItemHeight - iconSize) / 2f;

            var iconRect = new RectangleF(e.ItemRectangle.X + iconPoint,
                                          e.ItemRectangle.Y + iconPoint,
                                          iconSize,
                                          iconSize);

            e.Graphics.DrawImage(this._tagIcon, iconRect);

            var iconWidth = (int)(iconSize * 1.75);
            var itemFont = this.GetTagFont(item, scale);
            var itemText = item.Tag;
            var itemTextSize = TextRenderer.MeasureText(itemText, itemFont);
            var itemWidth = e.ItemRectangle.Width - iconWidth;
            var destText = itemText;
            var destTextSize = itemTextSize;
            while (destTextSize.Width > itemWidth)
            {
                destText = destText[..^1];
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
            if (this._fileInfoSource.IsEmpty)
            {
                return;
            }

            var param = new FileRatingUpdateParameter
            {
                FilePathList = this._fileInfoSource.FilePathList,
                RatingValue = this.ratingBar.Value
            };
            Instance<JobCaller>.Value.EnqueueFileRatingUpdateJob(this, param);
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
            this._contextMenuOperationTag = tagInfo.Tag;
        }

        private void TagDeleteMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this._contextMenuOperationTag))
            {
                return;
            }

            this.DeleteTag(this._contextMenuOperationTag);
        }

        private void TagToAllEntryMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this._contextMenuOperationTag))
            {
                return;
            }

            this.AddTag(this._contextMenuOperationTag);
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
            this.OnSelectedTag(new SelectedTagEventArgs(PageOpenMode.OverlapTab, tagInfo.Tag));
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
            this.OnSelectedTag(new SelectedTagEventArgs(PageOpenMode.AddTab, tagInfo.Tag));
        }

        private void WideComboBox_DropDownOpening(object sender, DropDownOpeningEventArgs e)
        {
            Instance<JobCaller>.Value.EnqueueTagsGetJob(this, _ =>
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
