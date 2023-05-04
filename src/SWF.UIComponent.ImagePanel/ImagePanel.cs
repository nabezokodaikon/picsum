﻿using SWF.UIComponent.ImagePanel.Properties;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SWF.UIComponent.ImagePanel
{
    /// <summary>
    /// 画像表示位置
    /// </summary>
    public enum ImageAlign
    {
        Center = 0,
        Left = 1,
        LeftTop = 2,
        Top = 3,
        RightTop = 4,
        Right = 5,
        RightBottom = 6,
        Bottom = 7,
        LeftBottom = 8
    }

    /// <summary>
    /// 画像パネルコントロール
    /// </summary>
    public sealed class ImagePanel
        : Control
    {
        #region 定数・列挙

        private const int THUMBNAIL_PANEL_OFFSET = 16;
        private const int THUMBNAIL_OFFSET = 8;

        #endregion

        #region イベント・デリゲート

        public event EventHandler<MouseEventArgs> ImageMouseClick;
        public event EventHandler<MouseEventArgs> ImageMouseDoubleClick;
        public event EventHandler DragStart;

        #endregion

        #region インスタンス変数

        private Image thumbnailPanelImage = Resources.ThumbnailPanel;
        private ImageAlign imageAlign = ImageAlign.Center;
        private bool isShowThumbnailPanel = false;

        private Image image = null;
        private Image thumbnail = null;

        private int hMaximumScrollValue = 0;
        private int vMaximumScrollValue = 0;
        private int hScrollValue = 0;
        private int vScrollValue = 0;

        private bool isDrag = false;
        private bool isImageMove = false;
        private bool isThumbnailMove = false;
        private Point moveFromPoint = Point.Empty;

        private SolidBrush thumbnailFilterBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));

        #endregion

        #region パブリックプロパティ

        public int ThumbnailSize
        {
            get
            {
                return this.thumbnailPanelImage.Width - THUMBNAIL_OFFSET;
            }
        }

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
                    this.Invalidate();
                }
            }
        }

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
                    this.Invalidate();
                }
            }
        }

        public bool IsMoving
        {
            get
            {
                return this.isImageMove || this.isThumbnailMove;
            }
        }

        public bool HasImage
        {
            get
            {
                return this.image != null;
            }
        }

        #endregion

        #region 継承プロパティ

        #endregion

        #region プライベートプロパティ

        private int ThumbnailPanelSize
        {
            get
            {
                return this.thumbnailPanelImage.Width;
            }
        }

        #endregion

        #region コンストラクタ

        public ImagePanel()
        {
            this.InitializeComponent();
        }

        #endregion

        #region パブリックメソッド

        public void SetImage(Image img, Image thumb)
        {
            if (img == null)
            {
                throw new ArgumentNullException("img");
            }

            if (thumb == null)
            {
                throw new ArgumentNullException("thumb");
            }

            if (this.image != null)
            {
                throw new Exception("既にイメージが存在しています。");
            }

            this.image = img;
            this.thumbnail = thumb;
        }

        public void ClearImage()
        {
            if (this.image == null)
            {
                throw new Exception("イメージが存在していません。");
            }

            this.thumbnail.Dispose();
            this.thumbnail = null;
            this.image.Dispose();
            this.image = null;
        }

        public bool IsImagePoint(int x, int y)
        {
            return this.GetImageDestRectangle().Contains(x, y);
        }

        #endregion

        #region 継承メソッド

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            this.setDrawParameter();

            base.OnInvalidated(e);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.image != null)
            {
                e.Graphics.SmoothingMode = this.GetSmoothingMode();
                e.Graphics.InterpolationMode = this.GetInterpolationMode();
                e.Graphics.CompositingQuality = this.GetCompositingQuality();

                this.DrawImage(e.Graphics);

                if (this.isShowThumbnailPanel &&
                    (this.hMaximumScrollValue > 0 || this.vMaximumScrollValue > 0))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
                    e.Graphics.InterpolationMode = InterpolationMode.Low;
                    e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;

                    this.DrawThumbnailPanel(e.Graphics);
                }
            }

            base.OnPaint(e);
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
                        var scale = this.image.Width / (float)this.thumbnail.Width;
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
                        this.isImageMove = true;
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

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.isThumbnailMove)
            {
                float scale = this.image.Width / (float)this.thumbnail.Width;
                if (this.SetHScrollValue(this.hScrollValue + (int)((e.X - this.moveFromPoint.X) * scale)) |
                    this.SetVScrollValue(this.vScrollValue + (int)((e.Y - this.moveFromPoint.Y) * scale)))
                {
                    this.Invalidate();
                }

                this.moveFromPoint.X = e.X;
                this.moveFromPoint.Y = e.Y;
            }
            else if (this.isImageMove)
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
                    this.OnDragStart(new EventArgs());
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

            if (this.isImageMove || this.isThumbnailMove)
            {
                this.isImageMove = false;
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

            this.isImageMove = false;
            this.isThumbnailMove = false;

            base.OnLostFocus(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (this.IsMousePointImage(e.X, e.Y))
            {
                this.OnImageMouseClick(e);
            }

            base.OnMouseClick(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (this.IsMousePointImage(e.X, e.Y))
            {
                this.OnImageMouseDoubleClick(e);
            }

            base.OnMouseDoubleClick(e);
        }

        #endregion

        #region プライベートメソッド

        private void InitializeComponent()
        {
            this.SetStyle(ControlStyles.DoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.SupportsTransparentBackColor, true);
        }

        private void OnImageMouseClick(MouseEventArgs e)
        {
            if (this.ImageMouseClick != null)
            {
                this.ImageMouseClick(this, e);
            }
        }

        private void OnImageMouseDoubleClick(MouseEventArgs e)
        {
            if (this.ImageMouseDoubleClick != null)
            {
                this.ImageMouseDoubleClick(this, e);
            }
        }

        private void OnDragStart(EventArgs e)
        {
            if (this.DragStart != null)
            {
                this.DragStart(this, e);
            }
        }

        private void setDrawParameter()
        {
            if (this.image != null)
            {
                this.hMaximumScrollValue = Math.Max(0, this.image.Width - this.Width);
                this.vMaximumScrollValue = Math.Max(0, this.image.Height - this.Height);
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
            if (this.image != null)
            {
                var rect = this.GetImageDestRectangle();
                var imgX = x - (int)rect.X;
                var imgY = y - (int)rect.Y;
                if (imgX >= 0 && this.image.Width >= imgX && imgY >= 0 && this.image.Height >= imgY)
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
                switch (this.imageAlign)
                {
                    case ImageAlign.Left:
                        x = 0f;
                        break;
                    case ImageAlign.LeftTop:
                        x = 0f;
                        break;
                    case ImageAlign.LeftBottom:
                        x = 0f;
                        break;
                    case ImageAlign.Right:
                        x = this.Width - this.image.Width;
                        break;
                    case ImageAlign.RightTop:
                        x = this.Width - this.image.Width;
                        break;
                    case ImageAlign.RightBottom:
                        x = this.Width - this.image.Width;
                        break;
                    default:
                        x = (this.Width - this.image.Width) / 2f;
                        break;
                }
            }

            float y;
            if (this.vMaximumScrollValue > 0)
            {
                y = 0f;
            }
            else
            {
                switch (this.imageAlign)
                {
                    case ImageAlign.Top:
                        y = 0f;
                        break;
                    case ImageAlign.LeftTop:
                        y = 0f;
                        break;
                    case ImageAlign.RightTop:
                        y = 0f;
                        break;
                    case ImageAlign.Bottom:
                        y = this.Height - this.image.Height;
                        break;
                    case ImageAlign.LeftBottom:
                        y = this.Height - this.image.Height;
                        break;
                    case ImageAlign.RightBottom:
                        y = this.Height - this.image.Height;
                        break;
                    default:
                        y = (this.Height - this.image.Height) / 2f;
                        break;
                }
            }

            var w = this.image.Width - this.hMaximumScrollValue;
            var h = this.image.Height - this.vMaximumScrollValue;

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

        private Rectangle GetThumbnailPanelRectangle()
        {
            var x = this.Width - THUMBNAIL_PANEL_OFFSET - this.ThumbnailPanelSize;
            var y = this.Height - THUMBNAIL_PANEL_OFFSET - this.ThumbnailPanelSize;
            var w = this.ThumbnailPanelSize;
            var h = this.ThumbnailPanelSize;
            return new Rectangle(x, y, w, h);
        }

        private RectangleF GetThumbnailRectangle(Rectangle panelRect)
        {
            var x = panelRect.X + (panelRect.Width - this.thumbnail.Width) / 2f;
            var y = panelRect.Y + (panelRect.Height - this.thumbnail.Height) / 2f;
            var w = this.thumbnail.Width;
            var h = this.thumbnail.Height;
            return new RectangleF(x, y, w, h);
        }

        private RectangleF GetThumbnailRectangle()
        {
            return this.GetThumbnailRectangle(this.GetThumbnailPanelRectangle());
        }

        private RectangleF GetThumbnailViewRectangle(RectangleF thumbRect, RectangleF srcRect)
        {
            var scale = this.thumbnail.Width / (float)this.image.Width;
            var x = thumbRect.X + srcRect.X * scale;
            var y = thumbRect.Y + srcRect.Y * scale;
            var w = srcRect.Width * scale;
            var h = srcRect.Height * scale;

            return new RectangleF(x, y, w, h);
        }

        private RectangleF GetThumbnailViewDestRectangle(RectangleF thumbRect, RectangleF srcRect)
        {
            var scale = this.thumbnail.Width / (float)this.image.Width;
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

        private void DrawImage(Graphics g)
        {
            g.DrawImage(this.image, this.GetImageDestRectangle(), this.GetImageSrcRectangle(), GraphicsUnit.Pixel);
        }

        private void DrawThumbnailPanel(Graphics g)
        {
            var panelRect = this.GetThumbnailPanelRectangle();
            g.DrawImage(this.thumbnailPanelImage, panelRect);

            var thumbRect = this.GetThumbnailRectangle(panelRect);
            g.DrawImage(this.thumbnail, thumbRect);
            g.FillRectangle(this.thumbnailFilterBrush, thumbRect);

            var srcRect = this.GetImageSrcRectangle();
            var viewRect = this.GetThumbnailViewRectangle(thumbRect, srcRect);
            var viewDestRect = this.GetThumbnailViewDestRectangle(thumbRect, srcRect);
            //g.FillRectangle(_thumbnailViewBrush, viewRect);
            g.DrawImage(this.thumbnail, viewRect, viewDestRect, GraphicsUnit.Pixel);
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

        private SmoothingMode GetSmoothingMode()
        {
            if (this.isImageMove || this.isThumbnailMove)
            {
                return SmoothingMode.HighSpeed;
            }
            else
            {
                return SmoothingMode.HighQuality;
            }
        }

        private InterpolationMode GetInterpolationMode()
        {
            if (this.isImageMove || this.isThumbnailMove)
            {
                return InterpolationMode.Low;
            }
            else
            {
                return InterpolationMode.HighQualityBicubic;
            }
        }

        private CompositingQuality GetCompositingQuality()
        {
            if (this.isImageMove || this.isThumbnailMove)
            {
                return CompositingQuality.HighSpeed;
            }
            else
            {
                return CompositingQuality.HighQuality;
            }
        }

        #endregion
    }
}
