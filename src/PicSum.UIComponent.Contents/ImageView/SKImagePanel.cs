using NLog;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WinApi;

namespace PicSum.UIComponent.Contents.ImageView
{
    public sealed partial class SKImagePanel
        : SKGLControl
    {
        private const int THUMBNAIL_PANEL_OFFSET = 16;
        private const int THUMBNAIL_OFFSET = 8;
        private const int THUMBNAIL_PANEL_SIZE = 144;

        private static readonly Logger LOGGER = NLogManager.GetLogger();
        private static readonly Size DRAG_SIZE = GetDragSize();

        private static Size GetDragSize()
        {
            var size = SystemInformation.DragSize.Width * 16;
            return new Size(size, size);
        }

        public event EventHandler<MouseEventArgs> ImageMouseClick;
        public event EventHandler<MouseEventArgs> ImageMouseDoubleClick;
        public event EventHandler DragStart;

        private ImageSizeMode _sizeMode = ImageSizeMode.FitOnlyBigImage;
        private ImageAlign _align = ImageAlign.Center;
        private bool _isShowThumbnailPanel = false;

        private SizeF _imageScaleSize = SizeF.Empty;
        private SkiaImage _image = SkiaImage.EMPTY;
        private SKImage _thumbnail = null;

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

        private readonly SKPaint _imagePaint = new()
        {
            IsAntialias = true,
        };

        private readonly SKPaint _backgroundPaint = new()
        {
            Color = new(64, 68, 71),
            //BlendMode = SKBlendMode.Src,
        };

        private readonly SKPaint _inactiveThumbnailPaint = new()
        {

        };

        private readonly SKPaint _activeThumbnailPaint = new()
        {

        };

        private readonly SKPaint _thumbnaiPanelPaint = new()
        {
            Color = new(128, 128, 128, 128),
        };

        private readonly SKPaint _thumbnaiFilterPaint = new()
        {
            Color = new(0, 0, 0, 128),
        };

        private readonly SKPaint _activeThumbnaiStrokePaint = new()
        {
            Color = new(255, 0, 0, 255),
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
        };

        private readonly SKPaint _messagePaint = new()
        {
            Color = new(192, 192, 192),
        };

        private readonly SKPaint _loadingPaint = new()
        {
            Color = new(128, 128, 128),
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
                return THUMBNAIL_PANEL_SIZE * scale;
            }
        }

        public float ThumbnailSize
        {
            get
            {
                var scale = WindowUtil.GetCurrentWindowScale(this);
                return (THUMBNAIL_PANEL_SIZE - THUMBNAIL_OFFSET) * scale;
            }
        }

        public SKImagePanel()
        {
            this.VSync = true;
            this.DoubleBuffered = true;

            this.MouseDown += this.ImagePanel_MouseDown;
            this.MouseUp += this.ImagePanel_MouseUp;
            this.MouseClick += this.ImagePanel_MouseClick;
            this.MouseDoubleClick += this.ImagePanel_MouseDoubleClick;
            this.MouseMove += this.ImagePanel_MouseMove;
            this.Invalidated += this.ImagePanel_Invalidated;
            this.LostFocus += this.ImagePanel_LostFocus;
            this.PaintSurface += this.ImagePanel_PaintSurface;
        }

        public void SetImage(
            string filePath,
            ImageSizeMode sizeMode,
            SkiaImage img,
            SKImage thumbnail)
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
                this._image = SkiaImage.EMPTY;
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
                this._imagePaint.Dispose();
                this._backgroundPaint.Dispose();
                this._inactiveThumbnailPaint.Dispose();
                this._activeThumbnailPaint.Dispose();
                this._thumbnaiPanelPaint.Dispose();
                this._thumbnaiFilterPaint.Dispose();
                this._activeThumbnaiStrokePaint.Dispose();
                this._messagePaint.Dispose();
                this._loadingPaint.Dispose();
            }

            base.Dispose(disposing);
        }

        private void ImagePanel_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            using (Measuring.Time(true, "ImagePanel.ImagePanel_PaintSurface"))
            {
                e.Surface.Canvas.DrawRect(e.Info.Rect, this._backgroundPaint);

                if (this._isError)
                {
                    this.DrawErrorMessage(e.Surface.Canvas);
                }
                else
                {
                    if (this.HasImage)
                    {
                        this.DrawImage(e.Surface.Context as GRContext, e.Surface.Canvas);
                    }

                    if (this.CanDrawThumbnailPanel())
                    {
                        this.DrawThumbnailPanel(e.Surface.Canvas);
                        var _ = WinApiMembers.DwmFlush();
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
                        var centerPoint = new SKPoint(srcRect.Left + srcRect.Width / 2f, srcRect.Top + srcRect.Height / 2f);
                        if (this.SetHScrollValue(this._hScrollValue + (e.X - thumbRect.Left) * scale - centerPoint.X) |
                            this.SetVScrollValue(this._vScrollValue + (e.Y - thumbRect.Top) * scale - centerPoint.Y))
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
                var imgX = x - rect.Left;
                var imgY = y - rect.Top;
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

        private SKRect GetImageDestRectangle()
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

            return new SKRect(x, y, x + w, y + h);
        }

        private SKRect GetImageSrcRectangle()
        {
            var image = this._image;

            var x = this._hScrollValue;
            var y = this._vScrollValue;
            var w = image.Width - this._hMaximumScrollValue;
            var h = image.Height - this._vMaximumScrollValue;

            return new SKRect(x, y, x + w, y + h);
        }

        private SKRect GetThumbnailPanelRectangle()
        {
            var thumbnailPanelSize = this.ThumbnailPanelSize;

            var x = this.Width - THUMBNAIL_PANEL_OFFSET - thumbnailPanelSize;
            var y = this.Height - THUMBNAIL_PANEL_OFFSET - thumbnailPanelSize;
            var w = thumbnailPanelSize;
            var h = thumbnailPanelSize;
            return new SKRect(x, y, x + w, y + h);
        }

        private SKRect GetThumbnailRectangle(SKRect panelRect)
        {
            var image = this._image;
            var scale = this.GetImageToThumbnailScale();
            var thumbSize = new SizeF(image.Width * scale, image.Height * scale);
            var x = panelRect.Left + (panelRect.Width - thumbSize.Width) / 2f;
            var y = panelRect.Top + (panelRect.Height - thumbSize.Height) / 2f;
            var w = thumbSize.Width;
            var h = thumbSize.Height;
            return new SKRect(x, y, x + w, y + h);
        }

        private SKRect GetThumbnailRectangle()
        {
            return this.GetThumbnailRectangle(this.GetThumbnailPanelRectangle());
        }

        private SKRect GetThumbnailViewDestRectangle(SKRect thumbRect, SKRect srcRect)
        {
            var scale = this.GetImageToThumbnailScale();
            var x = thumbRect.Left + srcRect.Left * scale;
            var y = thumbRect.Top + srcRect.Top * scale;
            var w = srcRect.Width * scale;
            var h = srcRect.Height * scale;

            return new SKRect(x, y, x + w, y + h);
        }

        private SKRect GetThumbnailViewSrcRectangle(SKRect srcRect)
        {
            var scale = this.GetImageToThumbnailScale();
            var x = srcRect.Left * scale;
            var y = srcRect.Top * scale;
            var w = srcRect.Width * scale;
            var h = srcRect.Height * scale;

            return new SKRect(x, y, x + w, y + h);
        }

        private SKRect GetThumbnailViewDestRectangle()
        {
            return this.GetThumbnailViewDestRectangle(
                this.GetThumbnailRectangle(),
                this.GetImageSrcRectangle());
        }

        private void DrawErrorMessage(SKCanvas canvas)
        {
            const string TEXT = "Failed to load file";

            var font = SWF.UIComponent.SKFlowList.FontCacher.GetRegularSKFont(
                SWF.UIComponent.SKFlowList.FontCacher.Size.ExtraLarge,
                WindowUtil.GetCurrentWindowScale(this));
            var bounds = new SKRect(0, 0, this.Width, this.Height);
            var textWidth = font.MeasureText(TEXT, this._messagePaint);

            // 垂直・水平のオフセット計算
            font.GetFontMetrics(out var metrics);

            // 水平中央: 矩形の中心からテキスト幅の半分を引く
            var x = bounds.Left + (bounds.Width - textWidth) / 2;

            // 垂直中央: FontMetricsを使用してベースラインを計算
            var textHeightOffset = (metrics.Ascent + metrics.Descent) / 2;
            var y = bounds.MidY - textHeightOffset;

            // TextBlobを作成して描画
            using var textBlob = SKTextBlob.Create(TEXT, font);
            canvas.DrawText(textBlob, x, y, this._messagePaint);
        }

        private void DrawLoadingMessage(SKCanvas canvas, string text, SKRect bounds)
        {
            var font = SWF.UIComponent.SKFlowList.FontCacher.GetRegularSKFont(
                SWF.UIComponent.SKFlowList.FontCacher.Size.ExtraLarge,
                WindowUtil.GetCurrentWindowScale(this));

            var maxWidth = bounds.Width;
            var displayText = text;

            // 三点リーダーの処理
            var textWidth = font.MeasureText(text, this._messagePaint);
            if (textWidth > maxWidth)
            {
                var length = text.Length;
                while (length > 0 && font.MeasureText(string.Concat(text.AsSpan(0, length), "..."), this._loadingPaint) > maxWidth)
                {
                    length--;
                }
                displayText = string.Concat(text.AsSpan(0, length), "...");
                textWidth = font.MeasureText(displayText, this._messagePaint);
            }

            // 垂直・水平のオフセット計算
            font.GetFontMetrics(out var metrics);

            // 水平中央: 矩形の中心からテキスト幅の半分を引く
            var x = bounds.Left + (bounds.Width - textWidth) / 2;

            // 垂直中央: FontMetricsを使用してベースラインを計算
            var textHeightOffset = (metrics.Ascent + metrics.Descent) / 2;
            var y = bounds.MidY - textHeightOffset;

            // TextBlobを作成して描画
            using var textBlob = SKTextBlob.Create(displayText, font);
            canvas.DrawText(textBlob, x, y, this._messagePaint);
        }

        private void DrawImage(GRContext context, SKCanvas canvas)
        {
            using (Measuring.Time(false, "ImagePanel.DrawImage"))
            {
                try
                {
                    var image = this._image;

                    if (image.IsLoadingImage)
                    {
                        var destRect = this.GetImageDestRectangle();

                        image.DrawEmpty(
                            canvas, this._loadingPaint, destRect);

                        this.DrawLoadingMessage(
                            canvas, FileUtil.GetFileName(this.FilePath), destRect);
                    }
                    else if (image.IsThumbnailImage)
                    {
                        if (this._sizeMode == ImageSizeMode.Original)
                        {
                            var destRect = this.GetImageDestRectangle();
                            var srcRect = this.GetImageSrcRectangle();
                            image.DrawZoomThumbnail(context, canvas, this._imagePaint, destRect, srcRect);
                        }
                        else
                        {
                            var destRect = this.GetImageDestRectangle();
                            image.DrawResizeThumbnail(context, canvas, this._imagePaint, destRect);
                        }
                    }
                    else
                    {
                        if (this._sizeMode == ImageSizeMode.Original)
                        {
                            var destRect = this.GetImageDestRectangle();
                            var srcRect = this.GetImageSrcRectangle();
                            image.DrawZoomImage(context, canvas, this._imagePaint, destRect, srcRect);
                        }
                        else
                        {
                            var destRect = this.GetImageDestRectangle();
                            image.DrawResizeImage(context, canvas, this._imagePaint, destRect);
                        }
                    }
                }
                catch (Exception ex) when (
                    ex is ImageUtilException ||
                    ex is OverflowException)
                {
                    LOGGER.Error($"{ex}");
                    this.DrawErrorMessage(canvas);
                }
            }
        }

        private void DrawThumbnailPanel(SKCanvas canvas)
        {
            using (Measuring.Time(false, "ImagePanel.DrawThumbnailPanel"))
            {
                var panelRect = this.GetThumbnailPanelRectangle();
                canvas.DrawRect(
                    panelRect,
                    this._thumbnaiPanelPaint);

                var thumbRect = this.GetThumbnailRectangle(panelRect);
                canvas.DrawImage(
                    this._thumbnail,
                    thumbRect,
                    this._inactiveThumbnailPaint);
                canvas.DrawRect(
                    thumbRect,
                    this._thumbnaiFilterPaint);

                var srcRect = this.GetImageSrcRectangle();
                var viewDestRect = this.GetThumbnailViewDestRectangle(thumbRect, srcRect);
                var viewSrcRect = this.GetThumbnailViewSrcRectangle(srcRect);

                canvas.DrawRect(
                    viewDestRect,
                    this._activeThumbnaiStrokePaint);

                canvas.DrawImage(
                    this._thumbnail,
                    viewSrcRect,
                    viewDestRect,
                    this._activeThumbnailPaint);
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
