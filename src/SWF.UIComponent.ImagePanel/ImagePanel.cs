using NLog;
using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using SWF.Core.Resource;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.ImagePanel
{
    /// <summary>
    /// 画像パネルコントロール
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class ImagePanel
        : Control
    {
        private const int THUMBNAIL_PANEL_OFFSET = 16;
        private const int THUMBNAIL_OFFSET = 8;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly SolidBrush THUMBNAIL_FILTER_BRUSH
            = new(Color.FromArgb(128, 0, 0, 0));

        public event EventHandler<MouseEventArgs> ImageMouseClick;
        public event EventHandler<MouseEventArgs> ImageMouseDoubleClick;
        public event EventHandler DragStart;

        private readonly Image thumbnailPanelImage = ResourceFiles.ThumbnailPanelIcon.Value;
        private ImageSizeMode sizeMode = ImageSizeMode.FitOnlyBigImage;
        private ImageAlign imageAlign = ImageAlign.Center;
        private bool isShowThumbnailPanel = false;

        private SizeF imageScaleSize = SizeF.Empty;
        private CvImage image = CvImage.EMPTY;

        private int hMaximumScrollValue = 0;
        private int vMaximumScrollValue = 0;
        private int hScrollValue = 0;
        private int vScrollValue = 0;

        private bool isDrag = false;
        private bool isThumbnailMove = false;
        private Point moveFromPoint = Point.Empty;
        private bool isError = false;

        private readonly StringFormat stringFormat = new()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter,
        };

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new int TabIndex
        {
            get
            {
                return base.TabIndex;
            }
            private set
            {
                base.TabIndex = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FilePath { get; private set; } = string.Empty;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsImageMove { get; private set; } = false;

        public Size ImageSize
        {
            get
            {
                if (!this.HasImage)
                {
                    throw new NullReferenceException("イメージが存在しません。");
                }

                return this.image.Size;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ImageAlign ImageAlign
        {
            get
            {
                return this.imageAlign;
            }
            set
            {
                if (value != this.imageAlign)
                {
                    this.imageAlign = value;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsShowThumbnailPanel
        {
            get
            {
                return this.isShowThumbnailPanel;
            }
            set
            {
                if (value != this.isShowThumbnailPanel)
                {
                    this.isShowThumbnailPanel = value;
                }
            }
        }

        public bool IsMoving
        {
            get
            {
                return this.IsImageMove || this.isThumbnailMove;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasImage { get; private set; } = false;

        private int ThumbnailPanelSize
        {
            get
            {
                return this.thumbnailPanelImage.Width;
            }
        }

        private int ThumbnailSize
        {
            get
            {
                return this.thumbnailPanelImage.Width - THUMBNAIL_OFFSET;
            }
        }

        public ImagePanel()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);
            this.UpdateStyles();
        }

        public void SetImage(ImageSizeMode sizeMode, CvImage img, string filePath)
        {
            ArgumentNullException.ThrowIfNull(filePath, nameof(filePath));

            if (this.HasImage)
            {
                throw new InvalidOperationException("既にイメージが存在しています。");
            }

            this.FilePath = filePath;
            this.sizeMode = sizeMode;
            this.image = img;
            this.isError = false;

            this.HasImage = true;
        }

        public void SetScale(float scale)
        {
            const float ERROR_IMAGE_SCALE = 1.0f;
            if (!this.HasImage)
            {
                throw new NullReferenceException("イメージが存在しません。");
            }

            if (this.isError)
            {
                this.imageScaleSize = new SizeF(
                    this.image.Width * ERROR_IMAGE_SCALE,
                    this.image.Height * ERROR_IMAGE_SCALE);
            }
            else
            {
                this.imageScaleSize = new SizeF(
                    this.image.Width * scale,
                    this.image.Height * scale);
            }
        }

        public void SetError()
        {
            this.isError = true;
        }

        public void ClearImage()
        {
            this.HasImage = false;

            this.image.Dispose();

            this.imageScaleSize = SizeF.Empty;
            this.FilePath = string.Empty;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.ClearImage();
            }

            base.Dispose(disposing);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (TimeMeasuring.Run(false, "ImagePanel.OnPaint"))
            {
                e.Graphics.SmoothingMode = SmoothingMode.None;
                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

                if (this.isError)
                {
                    this.DrawErrorImage(e.Graphics);
                }
                else if (this.HasImage)
                {
                    this.DrawImage(e.Graphics);

                    if (!this.image.IsEmpty && this.isShowThumbnailPanel &&
                        (this.hMaximumScrollValue > 0 || this.vMaximumScrollValue > 0))
                    {
                        this.DrawThumbnailPanel(e.Graphics);
                    }
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.isDrag = false;
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.isShowThumbnailPanel &&
                    (this.hMaximumScrollValue > 0 || this.vMaximumScrollValue > 0))
                {
                    if (this.GetThumbnailViewRectangle().Contains(e.X, e.Y))
                    {
                        // サムネイル内の画像表示領域
                        this.isThumbnailMove = true;
                        this.moveFromPoint.X = e.X;
                        this.moveFromPoint.Y = e.Y;
                    }
                    else if (this.GetThumbnailRectangle().Contains(e.X, e.Y))
                    {
                        // サムネイル
                        var scale = this.GetThumbnailToScaleImageScale();
                        var thumbRect = this.GetThumbnailRectangle();
                        var srcRect = this.GetImageSrcRectangle();
                        var centerPoint = new PointF(srcRect.X + srcRect.Width / 2f, srcRect.Y + srcRect.Height / 2f);
                        if (this.SetHScrollValue(this.hScrollValue + (int)((e.X - thumbRect.X) * scale - centerPoint.X)) |
                            this.SetVScrollValue(this.vScrollValue + (int)((e.Y - thumbRect.Y) * scale - centerPoint.Y)))
                        {
                            this.Invalidate();
                        }

                        this.isThumbnailMove = true;
                        this.moveFromPoint.X = e.X;
                        this.moveFromPoint.Y = e.Y;
                    }
                    else if (this.GetThumbnailPanelRectangle().Contains(e.X, e.Y))
                    {
                        // サムネイル表示領域
                    }
                    else if (this.IsMousePointImage(e.X, e.Y))
                    {
                        // 画像
                        this.IsImageMove = true;
                        this.moveFromPoint.X = e.X;
                        this.moveFromPoint.Y = e.Y;
                        this.Cursor = Cursors.NoMove2D;
                    }
                }
                else if (this.IsMousePointImage(e.X, e.Y))
                {
                    this.isDrag = true;
                    this.moveFromPoint.X = e.X;
                    this.moveFromPoint.Y = e.Y;
                }
            }

            base.OnMouseDown(e);
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            this.SetDrawParameter();
            base.OnInvalidated(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.isThumbnailMove)
            {
                var scale = this.GetThumbnailToScaleImageScale();
                if (this.SetHScrollValue(this.hScrollValue + (int)((e.X - this.moveFromPoint.X) * scale)) |
                    this.SetVScrollValue(this.vScrollValue + (int)((e.Y - this.moveFromPoint.Y) * scale)))
                {
                    this.Invalidate();
                }

                this.moveFromPoint.X = e.X;
                this.moveFromPoint.Y = e.Y;
            }
            else if (this.IsImageMove)
            {
                if (this.SetHScrollValue(this.hScrollValue - (e.X - this.moveFromPoint.X)) |
                    this.SetVScrollValue(this.vScrollValue - (e.Y - this.moveFromPoint.Y)))
                {
                    this.Invalidate();
                }

                this.moveFromPoint.X = e.X;
                this.moveFromPoint.Y = e.Y;
            }
            else if (this.isDrag)
            {
                // ドラッグとしないマウスの移動範囲を取得します。
                var moveRect = new Rectangle(this.moveFromPoint.X - SystemInformation.DragSize.Width / 2,
                                             this.moveFromPoint.Y - SystemInformation.DragSize.Height / 2,
                                             SystemInformation.DragSize.Width,
                                             SystemInformation.DragSize.Height);

                if (!moveRect.Contains(e.X, e.Y))
                {
                    this.isDrag = true;
                    this.OnDragStart(EventArgs.Empty);
                }
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (this.Cursor != Cursors.Default)
            {
                this.Cursor = Cursors.Default;
            }

            if (this.IsImageMove || this.isThumbnailMove)
            {
                this.IsImageMove = false;
                this.isThumbnailMove = false;
                this.Invalidate();
            }

            this.isDrag = false;
            base.OnMouseUp(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (this.Cursor != Cursors.Default)
            {
                this.Cursor = Cursors.Default;
            }

            this.IsImageMove = false;
            this.isThumbnailMove = false;

            base.OnLostFocus(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (this.image.IsEmpty)
            {
                return;
            }

            if (this.IsMousePointImage(e.X, e.Y))
            {
                this.OnImageMouseClick(e);
            }

            base.OnMouseClick(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (this.image.IsEmpty)
            {
                return;
            }

            if (this.IsMousePointImage(e.X, e.Y))
            {
                this.OnImageMouseDoubleClick(e);
            }

            base.OnMouseDoubleClick(e);
        }

        private float GetThumbnailToScaleImageScale()
        {
            return Math.Min(
                this.imageScaleSize.Width / (float)this.ThumbnailSize,
                this.imageScaleSize.Height / (float)this.ThumbnailSize);
        }

        private float GetImageToThumbnailScale()
        {
            return Math.Min(
                this.ThumbnailSize / (float)this.image.Width,
                this.ThumbnailSize / (float)this.image.Height);
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
                this.hMaximumScrollValue = Math.Max(0, (int)this.imageScaleSize.Width - this.Width);
                this.vMaximumScrollValue = Math.Max(0, (int)this.imageScaleSize.Height - this.Height);
                this.hScrollValue = Math.Min(this.hScrollValue, this.hMaximumScrollValue);
                this.vScrollValue = Math.Min(this.vScrollValue, this.vMaximumScrollValue);
            }
            else
            {
                this.hMaximumScrollValue = 0;
                this.vMaximumScrollValue = 0;
                this.hScrollValue = 0;
                this.vScrollValue = 0;
            }
        }

        private bool IsMousePointImage(int x, int y)
        {
            if (this.HasImage)
            {
                var rect = this.GetImageDestRectangle();
                var imgX = x - rect.X;
                var imgY = y - rect.Y;
                if (imgX >= 0 && this.imageScaleSize.Width >= imgX && imgY >= 0 && this.imageScaleSize.Height >= imgY)
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
            float x;
            if (this.hMaximumScrollValue > 0)
            {
                x = 0f;
            }
            else
            {
                x = this.imageAlign switch
                {
                    ImageAlign.Left => 0f,
                    ImageAlign.LeftTop => 0f,
                    ImageAlign.LeftBottom => 0f,
                    ImageAlign.Right => this.Width - this.imageScaleSize.Width,
                    ImageAlign.RightTop => this.Width - this.imageScaleSize.Width,
                    ImageAlign.RightBottom => this.Width - this.imageScaleSize.Width,
                    _ => (this.Width - this.imageScaleSize.Width) / 2f,
                };
            }

            float y;
            if (this.vMaximumScrollValue > 0)
            {
                y = 0f;
            }
            else
            {
                y = this.imageAlign switch
                {
                    ImageAlign.Top => 0f,
                    ImageAlign.LeftTop => 0f,
                    ImageAlign.RightTop => 0f,
                    ImageAlign.Bottom => this.Height - this.imageScaleSize.Height,
                    ImageAlign.LeftBottom => this.Height - this.imageScaleSize.Height,
                    ImageAlign.RightBottom => this.Height - this.imageScaleSize.Height,
                    _ => (this.Height - this.imageScaleSize.Height) / 2f,
                };
            }

            var w = this.imageScaleSize.Width - this.hMaximumScrollValue;
            var h = this.imageScaleSize.Height - this.vMaximumScrollValue;

            return new RectangleF(x, y, w, h);
        }

        private RectangleF GetImageSrcRectangle()
        {
            var x = this.hScrollValue;
            var y = this.vScrollValue;
            var w = this.image.Width - this.hMaximumScrollValue;
            var h = this.image.Height - this.vMaximumScrollValue;

            return new RectangleF(x, y, w, h);
        }

        private RectangleF GetThumbnailPanelRectangle()
        {
            var x = this.Width - THUMBNAIL_PANEL_OFFSET - this.ThumbnailPanelSize;
            var y = this.Height - THUMBNAIL_PANEL_OFFSET - this.ThumbnailPanelSize;
            var w = this.ThumbnailPanelSize;
            var h = this.ThumbnailPanelSize;
            return new RectangleF(x, y, w, h);
        }

        private RectangleF GetThumbnailRectangle(RectangleF panelRect)
        {
            var scale = this.GetImageToThumbnailScale();
            var thumbSize = new SizeF(this.image.Width * scale, this.image.Height * scale);
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

        private RectangleF GetThumbnailViewRectangle(RectangleF thumbRect, RectangleF srcRect)
        {
            var scale = this.GetImageToThumbnailScale();
            var x = thumbRect.X + srcRect.X * scale;
            var y = thumbRect.Y + srcRect.Y * scale;
            var w = srcRect.Width * scale;
            var h = srcRect.Height * scale;

            return new RectangleF(x, y, w, h);
        }

        private RectangleF GetThumbnailViewDestRectangle(RectangleF srcRect)
        {
            var scale = this.GetImageToThumbnailScale();
            var x = srcRect.X * scale;
            var y = srcRect.Y * scale;
            var w = srcRect.Width * scale;
            var h = srcRect.Height * scale;

            return new RectangleF(x, y, w, h);
        }

        private RectangleF GetThumbnailViewRectangle()
        {
            return this.GetThumbnailViewRectangle(this.GetThumbnailRectangle(), this.GetImageSrcRectangle());
        }

        private void DrawErrorImage(Graphics g)
        {
            g.CompositingMode = CompositingMode.SourceOver;
            g.DrawString(
                $"Failed to load file",
                this.Font,
                Brushes.LightGray,
                new Rectangle(0, 0, this.Width, this.Height),
                this.stringFormat);
        }

        private void DrawImage(Graphics g)
        {
            using (TimeMeasuring.Run(false, "ImagePanel.DrawImage"))
            {
                try
                {
                    if (this.image.IsEmpty)
                    {
                        var destRect = this.GetImageDestRectangle();
                        this.image.DrawEmptyImage(g, Brushes.LightGray, destRect);

                        g.CompositingMode = CompositingMode.SourceOver;
                        g.DrawString(
                            FileUtil.GetFileName(this.FilePath),
                            this.Font,
                            Brushes.DarkGray,
                            destRect,
                            this.stringFormat);
                    }
                    else
                    {
                        if (this.sizeMode == ImageSizeMode.Original)
                        {
                            this.image.DrawSourceImage(g, this.GetImageDestRectangle(), this.GetImageSrcRectangle());
                        }
                        else
                        {
                            var destRect = this.GetImageDestRectangle();
                            this.image.DrawResizeImage(g, destRect);
                        }
                    }
                }
                catch (ImageUtilException ex)
                {
                    Logger.Error($"{ex}");
                    this.DrawErrorImage(g);
                }
                catch (OverflowException ex)
                {
                    Logger.Error($"{ex}");
                    this.DrawErrorImage(g);
                }
            }
        }

        private void DrawThumbnailPanel(Graphics g)
        {
            g.CompositingMode = CompositingMode.SourceOver;

            var panelRect = this.GetThumbnailPanelRectangle();
            g.DrawImage(this.thumbnailPanelImage, panelRect);

            var thumbRect = this.GetThumbnailRectangle(panelRect);
            using (var thumbnail = this.image.GetResizeImage(
                new Size((int)thumbRect.Size.Width, (int)thumbRect.Height)))
            {
                g.DrawImage(thumbnail, thumbRect);
                g.FillRectangle(THUMBNAIL_FILTER_BRUSH, thumbRect);

                var srcRect = this.GetImageSrcRectangle();
                var viewRect = this.GetThumbnailViewRectangle(thumbRect, srcRect);
                var viewDestRect = this.GetThumbnailViewDestRectangle(srcRect);
                g.DrawImage(thumbnail, viewRect, viewDestRect, GraphicsUnit.Pixel);
            }
        }

        private bool SetHScrollValue(int value)
        {
            int newValue;
            if (value > this.hMaximumScrollValue)
            {
                newValue = this.hMaximumScrollValue;
            }
            else if (value < 0)
            {
                newValue = 0;
            }
            else
            {
                newValue = value;
            }

            if (newValue != this.hScrollValue)
            {
                this.hScrollValue = newValue;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool SetVScrollValue(int value)
        {
            int newValue;
            if (value > this.vMaximumScrollValue)
            {
                newValue = this.vMaximumScrollValue;
            }
            else if (value < 0)
            {
                newValue = 0;
            }
            else
            {
                newValue = value;
            }

            if (newValue != this.vScrollValue)
            {
                this.vScrollValue = newValue;
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
