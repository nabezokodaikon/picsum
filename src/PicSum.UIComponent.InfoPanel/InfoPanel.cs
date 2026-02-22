using PicSum.Job.Common;
using PicSum.Job.Entities;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using SWF.Core.ResourceAccessor;
using SWF.Core.StringAccessor;
using SWF.UIComponent.Base;
using SWF.UIComponent.SKFlowList;
using SWF.UIComponent.WideDropDown;
using System;
using System.ComponentModel;
using System.Drawing;
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

#pragma warning disable CA2213 // キャッシュを保持する変数。
        private SKImage _tagIcon = null;
#pragma warning restore CA2213

        private FileDeepInfoGetResult _fileInfoSource = FileDeepInfoGetResult.EMPTY;
        private string _contextMenuOperationTag = string.Empty;
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
        }

        public void SetControlsBounds(float scale)
        {
            this.SuspendLayout();

            this._thumbnailPictureBox.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left
                | AnchorStyles.Right;

            this._fileInfoLabel.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left
                | AnchorStyles.Right;

            this._ratingBar.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left
                | AnchorStyles.Right;

            this._wideComboBox.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left
                | AnchorStyles.Right;

            this._tagFlowList.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left
                | AnchorStyles.Right
                | AnchorStyles.Bottom;

            this._thumbnailPictureBox.SetBounds(
                THUMBNAIL_PICTURE_BOX_DEFAULT_BOUNDS.X,
                THUMBNAIL_PICTURE_BOX_DEFAULT_BOUNDS.Y,
                this.Width - THUMBNAIL_PICTURE_BOX_DEFAULT_BOUNDS.X * 2,
                (int)(THUMBNAIL_PICTURE_BOX_DEFAULT_BOUNDS.Height * scale));

            this._fileInfoLabel.SetBounds(
                this._thumbnailPictureBox.Left,
                this._thumbnailPictureBox.Top + this._thumbnailPictureBox.Bottom,
                this._thumbnailPictureBox.Width,
                (int)(FILE_INFO_LABEL_DEFAULT_BOUNDS.Height * scale));

            this._ratingBar.SetBounds(
                this._thumbnailPictureBox.Left,
                this._thumbnailPictureBox.Top + this._fileInfoLabel.Bottom,
                this._thumbnailPictureBox.Width,
                (int)(RATING_BAR_DEFAULT_BOUNDS.Height * scale));

            this._ratingBar.SetControlsBounds(scale);

            this._wideComboBox.SetBounds(
                this._thumbnailPictureBox.Left,
                this._thumbnailPictureBox.Top + this._ratingBar.Bottom,
                this._thumbnailPictureBox.Width,
                (int)(WIDE_COMBO_BOX_DEFAULT_BOUNDS.Height * scale));

            this._wideComboBox.SetControlsBounds(scale);

            this._tagFlowList.SetBounds(
                this._thumbnailPictureBox.Left,
                this._thumbnailPictureBox.Top + this._wideComboBox.Bottom,
                this._thumbnailPictureBox.Width,
                (int)(this.Height - (this._thumbnailPictureBox.Top + this._wideComboBox.Bottom) - 9 * scale));

            this.VerticalTopMargin = (int)(VERTICAL_DEFAULT_TOP_MARGIN * scale);

            this._tagIcon = InfoPanelResources.GetTagIcon(scale);
            this._tagFlowList.ItemHeight = (int)(TAG_FLOW_LIST_DEFAULT_ITEM_HEIGHT * scale);

            this.ResumeLayout(false);
        }

        public void SetFileInfo(string[] filePathList)
        {
            ArgumentNullException.ThrowIfNull(filePathList, nameof(filePathList));

            if (filePathList.Length > 0)
            {
                //// コンピュータの場合は情報を表示しない。
                //if (FileUtil.IsSystemRoot(filePathList.First()))
                //{
                //    this.ClearInfo();
                //    this.fileInfoLabel.Invalidate();
                //    this.ratingBar.Invalidate();
                //    this.thumbnailPictureBox.Invalidate();
                //    return;
                //}

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
                this._fileInfoLabel.Invalidate();
                this._ratingBar.Invalidate();
                this._thumbnailPictureBox.Invalidate();
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
                this._wideComboBox.Icon.Dispose();
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
            if (!this.Thumbnail.IsEmpty
                && this.Thumbnail.ThumbnailImage != null)
            {
                this.Thumbnail.ThumbnailImage.Dispose();
            }

            this._fileInfoSource = FileDeepInfoGetResult.EMPTY;

            this._fileInfoLabel.FileName = string.Empty;
            this._fileInfoLabel.FileType = string.Empty;
            this._fileInfoLabel.FileSize = string.Empty;
            this._fileInfoLabel.FilesAndDirectoriesCount = string.Empty;
            this._fileInfoLabel.CreateDate = string.Empty;
            this._fileInfoLabel.UpdateDate = string.Empty;
            this._fileInfoLabel.TakenDate = string.Empty;

            this._ratingBar.SetValue(0);
            this._tagFlowList.ItemCount = 0;

            this._contextMenuOperationTag = string.Empty;
            this._tagContextMenuStrip.Close();
        }

        private SKFont GetTagFont(FileTagInfoEntity tagInfo, float scale)
        {
            if (tagInfo.IsAll)
            {
                return FontCacher.GetBoldSKFont(FontCacher.Size.Medium, scale);
            }
            else
            {
                return FontCacher.GetRegularSKFont(FontCacher.Size.Medium, scale);
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
                this._tagFlowList.Invalidate();
            }
            else
            {
                tagInfo = new FileTagInfoEntity
                {
                    Tag = tag,
                    IsAll = true
                };
                this.TagList.Add(tagInfo);
                this.TagList.Sort(static (x, y) => NaturalStringComparer.WINDOWS.Compare(x.Tag, y.Tag));
                this._tagFlowList.ItemCount = this.TagList.Count;
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
            this._tagFlowList.ItemCount = this.TagList.Count;
        }

        private void GetFileInfoJob_Callback(FileDeepInfoGetResult result)
        {
            this.ClearInfo();

            this._fileInfoSource = result;

            if (!this.FileInfo.IsEmpty
                && !this.FileInfo.IsError)
            {
                this._fileInfoLabel.FileName = this.FileInfo.FileName;
                this._fileInfoLabel.FileType = this.FileInfo.FileType;

                if (this.FileInfo.IsFile)
                {
                    this._fileInfoLabel.FileSize = FileUtil.ToSizeUnitString(this.FileInfo.FileSize);
                    if (this.FileInfo.ImageSize != ImageUtil.EMPTY_SIZE)
                    {
                        this._fileInfoLabel.FileSize
                            += $"  ({this.FileInfo.ImageSize.Width} x {this.FileInfo.ImageSize.Height})";
                    }
                }
                else if (!FileUtil.IsSystemRoot(this.FileInfo.FilePath) && !this.FileInfo.FilesAndDirectoriesCount.IsEmpty)
                {
                    this._fileInfoLabel.FilesAndDirectoriesCount
                        += $"Files {this.FileInfo.FilesAndDirectoriesCount.FilesCount}, Folders {this.FileInfo.FilesAndDirectoriesCount.DirectoriesCount}";
                }

                if (!this.FileInfo.CreateDate.IsEmpty())
                {
                    this._fileInfoLabel.CreateDate
                        = $"{this.FileInfo.CreateDate:yyyy/MM/dd HH:mm:ss}";
                }

                if (!this.FileInfo.UpdateDate.IsEmpty())
                {
                    this._fileInfoLabel.UpdateDate
                        = $"{this.FileInfo.UpdateDate:yyyy/MM/dd HH:mm:ss}";
                }

                if (!this.FileInfo.TakenDate.IsEmpty())
                {
                    this._fileInfoLabel.TakenDate
                        = $"{this.FileInfo.TakenDate:yyyy/MM/dd HH:mm:ss}";
                }

                this._ratingBar.SetValue(this.FileInfo.Rating);
            }

            if (this.TagList != null)
            {
                this._tagFlowList.ItemCount = this.TagList.Count;
            }

            this._fileInfoLabel.Invalidate();
            this._ratingBar.Invalidate();
            this._thumbnailPictureBox.Invalidate();
        }

        private void GetTagListJob_Callback(ListResult<string> result)
        {
            this._wideComboBox.SetItems([.. result]);
            this._wideComboBox.SelectItem();
        }

        private void ThumbnailPictureBox_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.DrawRect(e.Info.Rect, InfoPanelResources.BACKGROUND_PAINT);

            if (this.FileInfo.IsError)
            {
                var rect = SKRect.Create(
                    0,
                    0,
                    this._thumbnailPictureBox.Width,
                    this._thumbnailPictureBox.Height);
                var text = $"Failed to load file";
                var font = FontCacher.GetRegularSKFont(
                    FontCacher.Size.Large,
                    WindowUtil.GetCurrentWindowScale(this));
                SkiaUtil.DrawText(e.Surface.Canvas, InfoPanelResources.IMAGE_PAINT, font, text, rect, SKTextAlign.Center, 2);
            }
            else if (!this.Thumbnail.IsEmpty
                && !this.Thumbnail.ThumbnailImage.IsEmpry)
            {
                var size = Math.Min(
                    this._thumbnailPictureBox.Width,
                    this._thumbnailPictureBox.Height);
                var x = (int)((this._thumbnailPictureBox.Width - size) / 2f);
                var y = (int)((this._thumbnailPictureBox.Height - size) / 2f);
                var rect = SKRectI.Create(x, y, size, size);
                if (this.FileInfo.IsFile)
                {
                    var thumbSize = new Size(
                        this.Thumbnail.SourceWidth,
                        this.Thumbnail.SourceHeight);
                    var scale = WindowUtil.GetCurrentWindowScale(this);
                    ThumbnailUtil.CacheFileThumbnail(
                        this.Thumbnail.ThumbnailImage,
                        rect,
                        thumbSize,
                        scale);
                    ThumbnailUtil.DrawFileThumbnail(
                        e.Surface.Canvas,
                        InfoPanelResources.IMAGE_PAINT,
                        this.Thumbnail.ThumbnailImage,
                        rect,
                        thumbSize,
                        WindowUtil.GetCurrentWindowScale(this));
                }
                else
                {
                    var thumbSize = new Size(
                        this.Thumbnail.SourceWidth,
                        this.Thumbnail.SourceHeight);
                    var scale = WindowUtil.GetCurrentWindowScale(this);
                    ThumbnailUtil.CacheFileThumbnail(
                        this.Thumbnail.ThumbnailImage,
                        rect,
                        thumbSize,
                        scale);
                    var icon = new IconImage((Bitmap)this.FileInfo.FileIcon);
                    ThumbnailUtil.DrawDirectoryThumbnail(
                        e.Surface.Canvas,
                        InfoPanelResources.IMAGE_PAINT,
                        this.Thumbnail.ThumbnailImage,
                        rect,
                        new Size(this.Thumbnail.SourceWidth, this.Thumbnail.SourceHeight),
                        icon,
                        WindowUtil.GetCurrentWindowScale(this));
                }
            }
            else if (this.FileInfo.FileIcon != null)
            {
                const int margin = 32;
                var size = Math.Min(
                    this._thumbnailPictureBox.Width - margin,
                    this._thumbnailPictureBox.Height - margin);
                var x = (int)((this._thumbnailPictureBox.Width - size) / 2f);
                var y = (int)((this._thumbnailPictureBox.Height - size) / 2f);
                var rect = SKRectI.Create(x, y, size, size);
                var icon = new IconImage((Bitmap)this.FileInfo.FileIcon);
                icon.Draw(e.Surface.Canvas, InfoPanelResources.IMAGE_PAINT, rect);
            }
            else if (this.FilePathList != null)
            {
                var text = $"{this.FilePathList.Length} files selected";
                var rect = SKRect.Create(
                    0,
                    0,
                    this._thumbnailPictureBox.Width,
                    this._thumbnailPictureBox.Height);
                var font = FontCacher.GetRegularSKFont(
                    FontCacher.Size.Large,
                    WindowUtil.GetCurrentWindowScale(this));
                SkiaUtil.DrawText(e.Surface.Canvas, InfoPanelResources.MESSAGE_PAINT, font, text, rect, SKTextAlign.Center, 2);
            }
        }

        private void TagFlowList_DrawItem(object sender, SKDrawItemEventArgs e)
        {
            if (this.TagList == null)
            {
                return;
            }

            if (this._tagIcon == null)
            {
                return;
            }

            if (e.IsMousePoint)
            {
                e.Canvas.DrawRect(e.ItemRectangle, SKFlowListResources.LIGHT_MOUSE_POINT_FILL_PAINT);
            }

            var item = this.TagList[e.ItemIndex];

            var scale = WindowUtil.GetCurrentWindowScale(this);
            var iconSizeMargin = 8 * scale;
            var iconSize = Math.Min(this._tagIcon.Width, e.ItemRectangle.Height) - iconSizeMargin * 2;

            var iconPoint = (this._tagFlowList.ItemHeight - iconSize) / 2f;

            var iconRect = SKRect.Create(e.ItemRectangle.Left + iconPoint,
                                          e.ItemRectangle.Top + iconPoint,
                                          iconSize,
                                          iconSize);

            e.Canvas.DrawImage(this._tagIcon, iconRect);

            var iconWidth = (int)(iconSize * 1.75);
            var itemFont = this.GetTagFont(item, scale);
            var itemText = item.Tag;

            var textRect = SKRect.Create(
                e.ItemRectangle.Left + iconWidth,
                e.ItemRectangle.Top,
                e.ItemRectangle.Width - iconWidth,
                e.ItemRectangle.Height);

            SkiaUtil.DrawText(
                e.Canvas,
                SKFlowListResources.LIGHT_TEXT_PAINT,
                itemFont,
                itemText,
                textRect,
                SKTextAlign.Left,
                1);
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
                RatingValue = this._ratingBar.Value
            };
            Instance<JobCaller>.Value.EnqueueFileRatingUpdateJob(this, param);
        }

        private void TagContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            var p = this._tagFlowList.PointToClient(Cursor.Position);
            var index = this._tagFlowList.IndexFromPoint(p.X, p.Y);
            if (index < 0)
            {
                e.Cancel = true;
                return;
            }

            var tagInfo = this.TagList[index];
            this._tagToAllEntryMenuItem.Visible = !tagInfo.IsAll;
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
            var p = this._tagFlowList.PointToClient(Cursor.Position);
            var index = this._tagFlowList.IndexFromPoint(p.X, p.Y);
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

            var p = this._tagFlowList.PointToClient(Cursor.Position);
            var index = this._tagFlowList.IndexFromPoint(p.X, p.Y);
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
