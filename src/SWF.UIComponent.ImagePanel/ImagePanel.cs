using SWF.UIComponent.ImagePanel.Properties;
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
    public class ImagePanel : Control
    {
        #region 定数・列挙

        private const int ThumbnailPanelOffset = 16;
        private const int ThumbnailOffset = 8;

        #endregion

        #region イベント・デリゲート

        public event EventHandler<MouseEventArgs> ImageMouseClick;
        public event EventHandler<MouseEventArgs> ImageMouseDoubleClick;
        public event EventHandler DragStart;

        #endregion

        #region インスタンス変数

        private Image _thumbnailPanelImage = Resources.ThumbnailPanel;
        private ImageAlign _imageAlign = ImageAlign.Center;
        private bool _isShowThumbnailPanel = false;

        private Image _image = null;
        private Image _thumbnail = null;

        private int _hMaximumScrollValue = 0;
        private int _vMaximumScrollValue = 0;
        private int _hScrollValue = 0;
        private int _vScrollValue = 0;

        private bool _isDrag = false;
        private bool _isImageMove = false;
        private bool _isThumbnailMove = false;
        private Point _moveFromPoint = Point.Empty;

        private SolidBrush _thumbnailFilterBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));

        #endregion

        #region パブリックプロパティ

        public int ThumbnailSize
        {
            get
            {
                return _thumbnailPanelImage.Width - ThumbnailOffset;
            }
        }

        public ImageAlign ImageAlign
        {
            get
            {
                return _imageAlign;
            }
            set
            {
                if (value != _imageAlign)
                {
                    _imageAlign = value;
                    this.Invalidate();
                }
            }
        }

        public bool IsShowThumbnailPanel
        {
            get
            {
                return _isShowThumbnailPanel;
            }
            set
            {
                if (value != _isShowThumbnailPanel)
                {
                    _isShowThumbnailPanel = value;
                    this.Invalidate();
                }
            }
        }

        public bool IsMoving
        {
            get
            {
                return _isImageMove || _isThumbnailMove;
            }
        }

        public bool HasImage
        {
            get
            {
                return _image != null;
            }
        }

        #endregion

        #region 継承プロパティ

        #endregion

        #region プライベートプロパティ

        private int thumbnailPanelSize
        {
            get
            {
                return _thumbnailPanelImage.Width;
            }
        }

        #endregion

        #region コンストラクタ

        public ImagePanel()
        {
            initializeComponent();
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

            if (_image != null)
            {
                throw new Exception("既にイメージが存在しています。");
            }

            _image = img;
            _thumbnail = thumb;
        }

        public void ClearImage()
        {
            if (_image == null)
            {
                throw new Exception("イメージが存在していません。");
            }

            _thumbnail.Dispose();
            _thumbnail = null;
            _image.Dispose();
            _image = null;
        }

        public bool IsImagePoint(int x, int y)
        {
            return getImageDestRectangle().Contains(x, y);
        }

        #endregion

        #region 継承メソッド

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            setDrawParameter();

            base.OnInvalidated(e);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_image != null)
            {
                e.Graphics.SmoothingMode = getSmoothingMode();
                e.Graphics.InterpolationMode = getInterpolationMode();
                e.Graphics.CompositingQuality = getCompositingQuality();

                drawImage(e.Graphics);

                if (_isShowThumbnailPanel &&
                    (_hMaximumScrollValue > 0 || _vMaximumScrollValue > 0))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
                    e.Graphics.InterpolationMode = InterpolationMode.Low;
                    e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;

                    drawThumbnailPanel(e.Graphics);
                }
            }

            base.OnPaint(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isDrag = false;
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_isShowThumbnailPanel &&
                    (_hMaximumScrollValue > 0 || _vMaximumScrollValue > 0))
                {
                    if (getThumbnailViewRectangle().Contains(e.X, e.Y))
                    {
                        // サムネイル内の画像表示領域
                        _isThumbnailMove = true;
                        _moveFromPoint.X = e.X;
                        _moveFromPoint.Y = e.Y;
                    }
                    else if (getThumbnailRectangle().Contains(e.X, e.Y))
                    {
                        // サムネイル
                        float scale = _image.Width / (float)_thumbnail.Width;
                        RectangleF thumbRect = getThumbnailRectangle();
                        RectangleF srcRect = getImageSrcRectangle();
                        PointF centerPoint = new PointF(srcRect.X + srcRect.Width / 2f, srcRect.Y + srcRect.Height / 2f);
                        if (setHScrollValue(_hScrollValue + (int)((e.X - thumbRect.X) * scale - centerPoint.X)) |
                            setVScrollValue(_vScrollValue + (int)((e.Y - thumbRect.Y) * scale - centerPoint.Y)))
                        {
                            this.Invalidate();
                        }

                        _isThumbnailMove = true;
                        _moveFromPoint.X = e.X;
                        _moveFromPoint.Y = e.Y;
                    }
                    else if (getThumbnailPanelRectangle().Contains(e.X, e.Y))
                    {
                        // サムネイル表示領域
                    }
                    else if (isMousePointImage(e.X, e.Y))
                    {
                        // 画像
                        _isImageMove = true;
                        _moveFromPoint.X = e.X;
                        _moveFromPoint.Y = e.Y;
                        this.Cursor = Cursors.NoMove2D;
                    }
                }
                else if (isMousePointImage(e.X, e.Y))
                {
                    _isDrag = true;
                    _moveFromPoint.X = e.X;
                    _moveFromPoint.Y = e.Y;
                }
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_isThumbnailMove)
            {
                float scale = _image.Width / (float)_thumbnail.Width;
                if (setHScrollValue(_hScrollValue + (int)((e.X - _moveFromPoint.X) * scale)) |
                    setVScrollValue(_vScrollValue + (int)((e.Y - _moveFromPoint.Y) * scale)))
                {
                    this.Invalidate();
                }

                _moveFromPoint.X = e.X;
                _moveFromPoint.Y = e.Y;
            }
            else if (_isImageMove)
            {
                if (setHScrollValue(_hScrollValue - (e.X - _moveFromPoint.X)) |
                    setVScrollValue(_vScrollValue - (e.Y - _moveFromPoint.Y)))
                {
                    this.Invalidate();
                }

                _moveFromPoint.X = e.X;
                _moveFromPoint.Y = e.Y;
            }
            else if (_isDrag)
            {
                // ドラッグとしないマウスの移動範囲を取得します。
                Rectangle moveRect = new Rectangle(_moveFromPoint.X - SystemInformation.DragSize.Width / 2,
                                                   _moveFromPoint.Y - SystemInformation.DragSize.Height / 2,
                                                   SystemInformation.DragSize.Width,
                                                   SystemInformation.DragSize.Height);

                if (!moveRect.Contains(e.X, e.Y))
                {
                    _isDrag = true;
                    OnDragStart(new EventArgs());
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

            if (_isImageMove || _isThumbnailMove)
            {
                _isImageMove = false;
                _isThumbnailMove = false;
                this.Invalidate();
            }

            _isDrag = false;
            base.OnMouseUp(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (this.Cursor != Cursors.Default)
            {
                this.Cursor = Cursors.Default;
            }

            _isImageMove = false;
            _isThumbnailMove = false;

            base.OnLostFocus(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (isMousePointImage(e.X, e.Y))
            {
                OnImageMouseClick(e);
            }

            base.OnMouseClick(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (isMousePointImage(e.X, e.Y))
            {
                OnImageMouseDoubleClick(e);
            }

            base.OnMouseDoubleClick(e);
        }

        protected virtual void OnImageMouseClick(MouseEventArgs e)
        {
            if (ImageMouseClick != null)
            {
                ImageMouseClick(this, e);
            }
        }

        protected virtual void OnImageMouseDoubleClick(MouseEventArgs e)
        {
            if (ImageMouseDoubleClick != null)
            {
                ImageMouseDoubleClick(this, e);
            }
        }

        protected virtual void OnDragStart(EventArgs e)
        {
            if (DragStart != null)
            {
                DragStart(this, e);
            }
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            this.SetStyle(ControlStyles.DoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.SupportsTransparentBackColor, true);
        }

        private void setDrawParameter()
        {
            if (_image != null)
            {
                _hMaximumScrollValue = Math.Max(0, _image.Width - this.Width);
                _vMaximumScrollValue = Math.Max(0, _image.Height - this.Height);
                _hScrollValue = Math.Min(_hScrollValue, _hMaximumScrollValue);
                _vScrollValue = Math.Min(_vScrollValue, _vMaximumScrollValue);
            }
            else
            {
                _hMaximumScrollValue = 0;
                _vMaximumScrollValue = 0;
                _hScrollValue = 0;
                _vScrollValue = 0;
            }
        }

        private bool isMousePointImage(int x, int y)
        {
            if (_image != null)
            {
                RectangleF rect = getImageDestRectangle();
                int imgX = x - (int)rect.X;
                int imgY = y - (int)rect.Y;
                if (imgX >= 0 && _image.Width >= imgX && imgY >= 0 && _image.Height >= imgY)
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

        private RectangleF getImageDestRectangle()
        {
            float x;
            if (_hMaximumScrollValue > 0)
            {
                x = 0f;
            }
            else
            {
                switch (_imageAlign)
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
                        x = this.Width - _image.Width;
                        break;
                    case ImageAlign.RightTop:
                        x = this.Width - _image.Width;
                        break;
                    case ImageAlign.RightBottom:
                        x = this.Width - _image.Width;
                        break;
                    default:
                        x = (this.Width - _image.Width) / 2f;
                        break;
                }
            }

            float y;
            if (_vMaximumScrollValue > 0)
            {
                y = 0f;
            }
            else
            {
                switch (_imageAlign)
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
                        y = this.Height - _image.Height;
                        break;
                    case ImageAlign.LeftBottom:
                        y = this.Height - _image.Height;
                        break;
                    case ImageAlign.RightBottom:
                        y = this.Height - _image.Height;
                        break;
                    default:
                        y = (this.Height - _image.Height) / 2f;
                        break;
                }
            }

            float w = _image.Width - _hMaximumScrollValue;
            float h = _image.Height - _vMaximumScrollValue;

            return new RectangleF(x, y, w, h);
        }

        private RectangleF getImageSrcRectangle()
        {
            float x = _hScrollValue;
            float y = _vScrollValue;
            float w = _image.Width - _hMaximumScrollValue;
            float h = _image.Height - _vMaximumScrollValue;

            return new RectangleF(x, y, w, h);
        }

        private Rectangle getThumbnailPanelRectangle()
        {
            int x = this.Width - ThumbnailPanelOffset - thumbnailPanelSize;
            int y = this.Height - ThumbnailPanelOffset - thumbnailPanelSize;
            int w = thumbnailPanelSize;
            int h = thumbnailPanelSize;
            return new Rectangle(x, y, w, h);
        }

        private RectangleF getThumbnailRectangle(Rectangle panelRect)
        {
            float x = panelRect.X + (panelRect.Width - _thumbnail.Width) / 2f;
            float y = panelRect.Y + (panelRect.Height - _thumbnail.Height) / 2f;
            float w = _thumbnail.Width;
            float h = _thumbnail.Height;
            return new RectangleF(x, y, w, h);
        }

        private RectangleF getThumbnailRectangle()
        {
            return getThumbnailRectangle(getThumbnailPanelRectangle());
        }

        private RectangleF getThumbnailViewRectangle(RectangleF thumbRect, RectangleF srcRect)
        {
            float scale = _thumbnail.Width / (float)_image.Width;
            float x = thumbRect.X + srcRect.X * scale;
            float y = thumbRect.Y + srcRect.Y * scale;
            float w = srcRect.Width * scale;
            float h = srcRect.Height * scale;

            return new RectangleF(x, y, w, h);
        }

        private RectangleF getThumbnailViewDestRectangle(RectangleF thumbRect, RectangleF srcRect)
        {
            float scale = _thumbnail.Width / (float)_image.Width;
            float x = srcRect.X * scale;
            float y = srcRect.Y * scale;
            float w = srcRect.Width * scale;
            float h = srcRect.Height * scale;

            return new RectangleF(x, y, w, h);
        }

        private RectangleF getThumbnailViewRectangle()
        {
            return getThumbnailViewRectangle(getThumbnailRectangle(), getImageSrcRectangle());
        }

        private void drawImage(Graphics g)
        {
            g.DrawImage(_image, getImageDestRectangle(), getImageSrcRectangle(), GraphicsUnit.Pixel);
        }

        private void drawThumbnailPanel(Graphics g)
        {
            Rectangle panelRect = getThumbnailPanelRectangle();
            g.DrawImage(_thumbnailPanelImage, panelRect);

            RectangleF thumbRect = getThumbnailRectangle(panelRect);
            g.DrawImage(_thumbnail, thumbRect);
            g.FillRectangle(_thumbnailFilterBrush, thumbRect);

            RectangleF srcRect = getImageSrcRectangle();
            RectangleF viewRect = getThumbnailViewRectangle(thumbRect, srcRect);
            RectangleF viewDestRect = getThumbnailViewDestRectangle(thumbRect, srcRect);
            //g.FillRectangle(_thumbnailViewBrush, viewRect);
            g.DrawImage(_thumbnail, viewRect, viewDestRect, GraphicsUnit.Pixel);
        }

        private bool setHScrollValue(int value)
        {
            int newValue;
            if (value > _hMaximumScrollValue)
            {
                newValue = _hMaximumScrollValue;
            }
            else if (value < 0)
            {
                newValue = 0;
            }
            else
            {
                newValue = value;
            }

            if (newValue != _hScrollValue)
            {
                _hScrollValue = newValue;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool setVScrollValue(int value)
        {
            int newValue;
            if (value > _vMaximumScrollValue)
            {
                newValue = _vMaximumScrollValue;
            }
            else if (value < 0)
            {
                newValue = 0;
            }
            else
            {
                newValue = value;
            }

            if (newValue != _vScrollValue)
            {
                _vScrollValue = newValue;
                return true;
            }
            else
            {
                return false;
            }
        }

        private SmoothingMode getSmoothingMode()
        {
            if (_isImageMove || _isThumbnailMove)
            {
                return SmoothingMode.HighSpeed;
            }
            else
            {
                return SmoothingMode.HighQuality;
            }
        }

        private InterpolationMode getInterpolationMode()
        {
            if (_isImageMove || _isThumbnailMove)
            {
                return InterpolationMode.Low;
            }
            else
            {
                return InterpolationMode.HighQualityBicubic;
            }
        }

        private CompositingQuality getCompositingQuality()
        {
            if (_isImageMove || _isThumbnailMove)
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
