using NLog;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.ResourceAccessor;
using SWF.UIComponent.Base;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.ImageView
{
    /// <summary>
    /// 画像パネルコントロール
    /// </summary>

    public sealed partial class ImagePanel
        : BasePaintingControl
    {
        private const int THUMBNAIL_PANEL_OFFSET = 16;
        private const int THUMBNAIL_OFFSET = 8;

        private static readonly Logger LOGGER = NLogManager.GetLogger();

        private static readonly SolidBrush THUMBNAIL_FILTER_BRUSH
            = new(Color.FromArgb(128, 0, 0, 0));
        private static readonly Pen THUMBNAIL_VIEW_BORDER_PEN
            = new(Color.FromArgb(250, 0, 0));

        private static readonly Size DRAG_SIZE = GetDragSize();

        private static Size GetDragSize()
        {
            var size = SystemInformation.DragSize.Width * 16;
            return new Size(size, size);
        }

        public event EventHandler<MouseEventArgs> ImageMouseClick;
        public event EventHandler<MouseEventArgs> ImageMouseDoubleClick;
        public event EventHandler DragStart;

#pragma warning disable CA2213 // リソースを保持する変数。
        private readonly Image _thumbnailPanelImage = ResourceFiles.ThumbnailPanelIcon.Value;
#pragma warning restore CA2213

        private ImageSizeMode _sizeMode = ImageSizeMode.FitOnlyBigImage;
        private ImageAlign _align = ImageAlign.Center;
        private bool _isShowThumbnailPanel = false;

        private SizeF _imageScaleSize = SizeF.Empty;
        private CvImage _image = CvImage.EMPTY;
        private Bitmap _thumbnail = null;

        private float _hMaximumScrollValue = 0f;
        private float _vMaximumScrollValue = 0f;
        private float _hScrollValue = 0f;
        private float _vScrollValue = 0;

        private bool _isThumbnailMove = false;
        private Point _moveFromPoint = Point.Empty;
        private bool _isError = false;

        private Rectangle _dragJudgementRectangle = new();

        private readonly StringFormat _stringFormat = new()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter,
        };

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FilePath { get; private set; } = string.Empty;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsImageMove { get; private set; } = false;

        public SizeF ImageSize
        {
            get
            {
                if (!this.HasImage)
                {
                    throw new InvalidOperationException("イメージが存在しません。");
                }

                return this._image.Size;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ImageAlign Align
        {
            get
            {
                return this._align;
            }
            set
            {
                if (value != this._align)
                {
                    this._align = value;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsShowThumbnailPanel
        {
            get
            {
                return this._isShowThumbnailPanel;
            }
            set
            {
                if (value != this._isShowThumbnailPanel)
                {
                    this._isShowThumbnailPanel = value;
                }
            }
        }

        public bool IsMoving
        {
            get
            {
                return this.IsImageMove || this._isThumbnailMove;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasImage { get; private set; } = false;

        private float ThumbnailPanelSize
        {
            get
            {
                var scale = WindowUtil.GetCurrentWindowScale(this);
                return this._thumbnailPanelImage.Width * scale;
            }
        }

        public float ThumbnailSize
        {
            get
            {
                var scale = WindowUtil.GetCurrentWindowScale(this);
                return (this._thumbnailPanelImage.Width - THUMBNAIL_OFFSET) * scale;
            }
        }

        public ImagePanel()
        {
            this.MouseDown += this.ImagePanel_MouseDown;
            this.MouseUp += this.ImagePanel_MouseUp;
            this.MouseClick += this.ImagePanel_MouseClick;
            this.MouseDoubleClick += this.ImagePanel_MouseDoubleClick;
            this.MouseMove += this.ImagePanel_MouseMove;
            this.Invalidated += this.ImagePanel_Invalidated;
            this.LostFocus += this.ImagePanel_LostFocus;
            this.Paint += this.ImagePanel_Paint;
        }

        public void SetImage(
            string filePath,
            ImageSizeMode sizeMode,
            CvImage img,
            Bitmap thumbnail)
        {
            ArgumentNullException.ThrowIfNull(filePath, nameof(filePath));

            if (this.HasImage)
            {
                throw new InvalidOperationException("既にイメージが存在しています。");
            }

            this.FilePath = filePath;
            this._sizeMode = sizeMode;
            this._image = img;
            this._thumbnail = thumbnail;
            this._isError = false;

            this.HasImage = true;
        }

        public void SetScale(float scale)
        {
            const float ERROR_IMAGE_SCALE = 1.0f;
            if (!this.HasImage)
            {
                throw new InvalidOperationException("イメージが存在しません。");
            }

            if (this._isError)
            {
                this._imageScaleSize = new SizeF(
                    this._image.Width * ERROR_IMAGE_SCALE,
                    this._image.Height * ERROR_IMAGE_SCALE);
            }
            else
            {
                this._imageScaleSize = new SizeF(
                    this._image.Width * scale,
                    this._image.Height * scale);
            }
        }

        public void SetError()
        {
            this._isError = true;
        }

        public void ClearImage()
        {
            this.HasImage = false;

            if (!this._image.IsEmpry)
            {
                this._image.Dispose();
                this._image = CvImage.EMPTY;
            }

            this._thumbnail?.Dispose();
            this._thumbnail = null;
            this._imageScaleSize = SizeF.Empty;
            this.FilePath = string.Empty;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.ClearImage();

                this._stringFormat.Dispose();
            }

            base.Dispose(disposing);
        }

        private void ImagePanel_Paint(object sender, PaintEventArgs e)
        {
            using (TimeMeasuring.Run(false, "ImagePanel.ImagePanel_Paint"))
            {
                e.Graphics.SmoothingMode = SmoothingMode.None;
                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                e.Graphics.CompositingMode = CompositingMode.SourceOver;

                if (this._isError)
                {
                    this.DrawErrorImage(e.Graphics);
                }
                else
                {
                    if (this.HasImage)
                    {
                        this.DrawImage(e.Graphics);
                    }

                    if (this.CanDrawThumbnailPanel())
                    {
                        this.DrawThumbnailPanel(e.Graphics);
                    }
                }
            }
        }

        private void ImagePanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this._isShowThumbnailPanel &&
                    (this._hMaximumScrollValue > 0 || this._vMaximumScrollValue > 0))
                {
                    if (this.GetThumbnailViewDestRectangle().Contains(e.X, e.Y))
                    {
                        // サムネイル内の画像表示領域
                        this._isThumbnailMove = true;
                        this._moveFromPoint.X = e.X;
                        this._moveFromPoint.Y = e.Y;
                    }
                    else if (this.GetThumbnailRectangle().Contains(e.X, e.Y))
                    {
                        // サムネイル
                        var scale = this.GetThumbnailToScaleImageScale();
                        var thumbRect = this.GetThumbnailRectangle();
                        var srcRect = this.GetImageSrcRectangle();
                        var centerPoint = new PointF(srcRect.X + srcRect.Width / 2f, srcRect.Y + srcRect.Height / 2f);
                        if (this.SetHScrollValue(this._hScrollValue + (e.X - thumbRect.X) * scale - centerPoint.X) |
                            this.SetVScrollValue(this._vScrollValue + (e.Y - thumbRect.Y) * scale - centerPoint.Y))
                        {
                            this.Invalidate();
                        }

                        this._isThumbnailMove = true;
                        this._moveFromPoint.X = e.X;
                        this._moveFromPoint.Y = e.Y;
                    }
                    else if (this.GetThumbnailPanelRectangle().Contains(e.X, e.Y))
                    {
                        // サムネイル表示領域
                    }
                    else if (this.IsMousePointImage(e.X, e.Y))
                    {
                        // 画像
                        this.IsImageMove = true;
                        this._moveFromPoint.X = e.X;
                        this._moveFromPoint.Y = e.Y;
                    }
                }
                else if (this.IsMousePointImage(e.X, e.Y))
                {
                    var dragSize = DRAG_SIZE;
                    this._dragJudgementRectangle = new Rectangle(
                        e.X - dragSize.Width / 2,
                        e.Y - dragSize.Height / 2,
                        dragSize.Width,
                        dragSize.Height);
                }
            }
        }

        private void ImagePanel_Invalidated(object sender, InvalidateEventArgs e)
        {
            this.SetDrawParameter();
        }

        private void ImagePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (this._isThumbnailMove)
            {
                var scale = this.GetThumbnailToScaleImageScale();
                if (this.SetHScrollValue(this._hScrollValue + (e.X - this._moveFromPoint.X) * scale) |
                    this.SetVScrollValue(this._vScrollValue + (e.Y - this._moveFromPoint.Y) * scale))
                {
                    this.Invalidate();
                }

                this._moveFromPoint.X = e.X;
                this._moveFromPoint.Y = e.Y;
            }
            else if (this.IsImageMove)
            {
                if (this.SetHScrollValue(this._hScrollValue - (e.X - this._moveFromPoint.X)) |
                    this.SetVScrollValue(this._vScrollValue - (e.Y - this._moveFromPoint.Y)))
                {
                    this.Invalidate();
                }

                this._moveFromPoint.X = e.X;
                this._moveFromPoint.Y = e.Y;
            }
            else if (!this._dragJudgementRectangle.Contains(e.X, e.Y))
            {
                this.OnDragStart(EventArgs.Empty);
            }
        }

        private void ImagePanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.IsImageMove || this._isThumbnailMove)
            {
                this.IsImageMove = false;
                this._isThumbnailMove = false;
                this.Invalidate();
            }
        }

        private void ImagePanel_LostFocus(object sender, EventArgs e)
        {
            this.IsImageMove = false;
            this._isThumbnailMove = false;
        }

        private void ImagePanel_MouseClick(object sender, MouseEventArgs e)
        {
            this.Focus();

            if (this._image.IsLoadingImage)
            {
                return;
            }

            if (this.IsMousePointImage(e.X, e.Y))
            {
                this.OnImageMouseClick(e);
            }
        }

        private void ImagePanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this._image.IsLoadingImage)
            {
                return;
            }

            if (this.IsMousePointImage(e.X, e.Y))
            {
                this.OnImageMouseDoubleClick(e);
            }
        }

        private bool CanDrawThumbnailPanel()
        {
            if (this._thumbnail != null
                && !this._image.IsLoadingImage
                && this._isShowThumbnailPanel
                && (this._hMaximumScrollValue > 0 || this._vMaximumScrollValue > 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private float GetThumbnailToScaleImageScale()
        {
            return Math.Max(
                this._imageScaleSize.Width / this.ThumbnailSize,
                this._imageScaleSize.Height / this.ThumbnailSize);
        }

        private float GetImageToThumbnailScale()
        {
            var thumbnailSize = this.ThumbnailSize;
            var image = this._image;

            return Math.Min(
                thumbnailSize / image.Width,
                thumbnailSize / image.Height);
        }

        private void OnImageMouseClick(MouseEventArgs e)
        {
            this.ImageMouseClick?.Invoke(this, e);
        }

        private void OnImageMouseDoubleClick(MouseEventArgs e)
        {
            this.ImageMouseDoubleClick?.Invoke(this, e);
        }

        private void OnDragStart(EventArgs e)
        {
            this.DragStart?.Invoke(this, e);
        }

        private void SetDrawParameter()
        {
            if (this.HasImage)
            {
                this._hMaximumScrollValue = Math.Max(0, this._imageScaleSize.Width - this.Width);
                this._vMaximumScrollValue = Math.Max(0, this._imageScaleSize.Height - this.Height);
                this._hScrollValue = Math.Min(this._hScrollValue, this._hMaximumScrollValue);
                this._vScrollValue = Math.Min(this._vScrollValue, this._vMaximumScrollValue);
            }
            else
            {
                this._hMaximumScrollValue = 0;
                this._vMaximumScrollValue = 0;
                this._hScrollValue = 0;
                this._vScrollValue = 0;
            }
        }

        private bool IsMousePointImage(float x, float y)
        {
            if (this.HasImage)
            {
                var rect = this.GetImageDestRectangle();
                var imgX = x - rect.X;
                var imgY = y - rect.Y;
                if (imgX >= 0 && this._imageScaleSize.Width >= imgX && imgY >= 0 && this._imageScaleSize.Height >= imgY)
                {
                    //Bitmap bmp = (Bitmap)_image;
                    //Color cr = bmp.GetPixel(imgX, imgY);
                    //return cr.A > 0;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private RectangleF GetImageDestRectangle()
        {
            var scaleWidth = this._imageScaleSize.Width;
            var scaleHeight = this._imageScaleSize.Height;

            float x;
            if (this._hMaximumScrollValue > 0)
            {
                x = 0f;
            }
            else
            {
                var width = this.Width;
                x = this._align switch
                {
                    ImageAlign.Left => 0f,
                    ImageAlign.Right => width - scaleWidth,
                    _ => (width - scaleWidth) / 2f,
                };
            }

            float y;
            if (this._vMaximumScrollValue > 0)
            {
                y = 0f;
            }
            else
            {
                y = (this.Height - scaleHeight) / 2f;
            }

            var w = (float)Math.Ceiling(scaleWidth - this._hMaximumScrollValue);
            var h = scaleHeight - this._vMaximumScrollValue;

            return new RectangleF(x, y, w, h);
        }

        private RectangleF GetImageSrcRectangle()
        {
            var image = this._image;

            var x = this._hScrollValue;
            var y = this._vScrollValue;
            var w = image.Width - this._hMaximumScrollValue;
            var h = image.Height - this._vMaximumScrollValue;

            return new RectangleF(x, y, w, h);
        }

        private RectangleF GetThumbnailPanelRectangle()
        {
            var thumbnailPanelSize = this.ThumbnailPanelSize;

            var x = this.Width - THUMBNAIL_PANEL_OFFSET - thumbnailPanelSize;
            var y = this.Height - THUMBNAIL_PANEL_OFFSET - thumbnailPanelSize;
            var w = thumbnailPanelSize;
            var h = thumbnailPanelSize;
            return new RectangleF(x, y, w, h);
        }

        private RectangleF GetThumbnailRectangle(RectangleF panelRect)
        {
            var image = this._image;
            var scale = this.GetImageToThumbnailScale();
            var thumbSize = new SizeF(image.Width * scale, image.Height * scale);
            var x = panelRect.X + (panelRect.Width - thumbSize.Width) / 2f;
            var y = panelRect.Y + (panelRect.Height - thumbSize.Height) / 2f;
            var w = thumbSize.Width;
            var h = thumbSize.Height;
            return new RectangleF(x, y, w, h);
        }

        private RectangleF GetThumbnailRectangle()
        {
            return this.GetThumbnailRectangle(this.GetThumbnailPanelRectangle());
        }

        private RectangleF GetThumbnailViewDestRectangle(RectangleF thumbRect, RectangleF srcRect)
        {
            var scale = this.GetImageToThumbnailScale();
            var x = thumbRect.X + srcRect.X * scale;
            var y = thumbRect.Y + srcRect.Y * scale;
            var w = srcRect.Width * scale;
            var h = srcRect.Height * scale;

            return new RectangleF(x, y, w, h);
        }

        private RectangleF GetThumbnailViewSrcRectangle(RectangleF srcRect)
        {
            var scale = this.GetImageToThumbnailScale();
            var x = srcRect.X * scale;
            var y = srcRect.Y * scale;
            var w = srcRect.Width * scale;
            var h = srcRect.Height * scale;

            return new RectangleF(x, y, w, h);
        }

        private RectangleF GetThumbnailViewDestRectangle()
        {
            return this.GetThumbnailViewDestRectangle(this.GetThumbnailRectangle(), this.GetImageSrcRectangle());
        }

        private void DrawErrorImage(Graphics g)
        {
            g.DrawString(
                $"Failed to load file",
                Fonts.GetRegularFont(Fonts.Size.ExtraLarge, WindowUtil.GetCurrentWindowScale(this)),
                Brushes.LightGray,
                new RectangleF(0, 0, this.Width, this.Height),
                this._stringFormat);
        }

        private void DrawImage(Graphics g)
        {
            using (TimeMeasuring.Run(false, "ImagePanel.DrawImage"))
            {
                try
                {
                    var image = this._image;

                    if (image.IsLoadingImage)
                    {
                        var destRect = this.GetImageDestRectangle();
                        image.DrawEmptyImage(g, Brushes.LightGray, destRect);

                        g.DrawString(
                            FileUtil.GetFileName(this.FilePath),
                            Fonts.GetRegularFont(Fonts.Size.ExtraLarge, WindowUtil.GetCurrentWindowScale(this)),
                            Brushes.DarkGray,
                            destRect,
                            this._stringFormat);
                    }
                    else if (image.IsThumbnailImage)
                    {
                        if (this._sizeMode == ImageSizeMode.Original)
                        {
                            var destRect = this.GetImageDestRectangle();
                            var srcRect = this.GetImageSrcRectangle();
                            image.DrawZoomThumbnailImage(g, destRect, srcRect);
                        }
                        else
                        {
                            var destRect = this.GetImageDestRectangle();
                            image.DrawResizeThumbnailImage(g, destRect);
                        }
                    }
                    else
                    {
                        if (this._sizeMode == ImageSizeMode.Original)
                        {
                            var destRect = this.GetImageDestRectangle();
                            var srcRect = this.GetImageSrcRectangle();
                            image.DrawZoomImage(g, destRect, srcRect);
                        }
                        else
                        {
                            var destRect = this.GetImageDestRectangle();
                            image.DrawResizeImage(g, destRect);
                        }
                    }
                }
                catch (Exception ex) when (
                    ex is ImageUtilException ||
                    ex is OverflowException)
                {
                    LOGGER.Error($"{ex}");
                    this.DrawErrorImage(g);
                }
            }
        }

        private void DrawThumbnailPanel(Graphics g)
        {
            using (TimeMeasuring.Run(false, "ImagePanel.DrawThumbnailPanel"))
            {
                var panelRect = this.GetThumbnailPanelRectangle();
                g.DrawImage(this._thumbnailPanelImage, panelRect);

                var thumbRect = this.GetThumbnailRectangle(panelRect);
                g.DrawImage(this._thumbnail, thumbRect);
                g.FillRectangle(THUMBNAIL_FILTER_BRUSH, thumbRect);

                var srcRect = this.GetImageSrcRectangle();
                var viewDestRect = this.GetThumbnailViewDestRectangle(thumbRect, srcRect);
                var viewSrcRect = this.GetThumbnailViewSrcRectangle(srcRect);
                g.DrawImage(this._thumbnail, viewDestRect, viewSrcRect, GraphicsUnit.Pixel);
                g.DrawRectangle(THUMBNAIL_VIEW_BORDER_PEN, viewDestRect);
            }
        }

        private bool SetHScrollValue(float value)
        {
            float newValue;
            if (value > this._hMaximumScrollValue)
            {
                newValue = this._hMaximumScrollValue;
            }
            else if (value < 0)
            {
                newValue = 0;
            }
            else
            {
                newValue = value;
            }

            if (newValue != this._hScrollValue)
            {
                this._hScrollValue = newValue;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool SetVScrollValue(float value)
        {
            float newValue;
            if (value > this._vMaximumScrollValue)
            {
                newValue = this._vMaximumScrollValue;
            }
            else if (value < 0)
            {
                newValue = 0;
            }
            else
            {
                newValue = value;
            }

            if (newValue != this._vScrollValue)
            {
                this._vScrollValue = newValue;
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
