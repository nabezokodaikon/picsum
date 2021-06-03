using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SWF.UIComponent.FreeForm.Properties;
using WinApi;

namespace SWF.UIComponent.FreeForm
{
    public class ControlBox : Control
    {
        #region 定数・列挙

        public enum PointContentsEnum
        {
            Default = 0,
            MinimumButton = 1,
            MaximumButton = 2,
            CloseButton = 3
        }

        #endregion

        #region イベント・デリゲート

        public event EventHandler<MouseEventArgs> MinimumButtonClick;
        public event EventHandler<MouseEventArgs> MaximumButtonClick;
        public event EventHandler<MouseEventArgs> CloseButtonClick;

        #endregion

        #region インスタンス変数

        private readonly Bitmap _normalImage = Resources.NormalControlBox;
        private readonly Bitmap _maximumImage = Resources.MaximumControlBox;
        private readonly Bitmap _minimumButtonMousePointImage = Resources.MinimumButtonMousePoint;
        private readonly Bitmap _maximumButtonMousePointImage = Resources.MaximumButtonMousePoint;
        private readonly Bitmap _closeButtonMousePointImage = Resources.CloseButtonMousePoint;
        private readonly Rectangle _minimumButtonRectangle = new Rectangle(0, 0, 26, 18);
        private readonly Rectangle _maximumButtonRectangle = new Rectangle(28, 0, 26, 18);
        private readonly Rectangle _closeButtonRectangle = new Rectangle(56, 0, 26, 18);
       
        private Form _form = null;
        private FormWindowState _windowState = FormWindowState.Normal;
        private PointContentsEnum _pointContents = PointContentsEnum.Default;

        #endregion

        #region パブリックプロパティ

        #endregion

        #region 継承プロパティ

        #endregion

        #region プライベートプロパティ

        #endregion

        #region コンストラクタ

        public ControlBox()
        {
            if (!this.DesignMode)
            {
                initializeComponent();
            }
        }

        #endregion

        #region パブリックメソッド

        #endregion

        #region 継承メソッド

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_form != null)
            {
                if (_pointContents == PointContentsEnum.MinimumButton)
                {
                    Image img = _minimumButtonMousePointImage;
                    Rectangle rect = _minimumButtonRectangle;
                    e.Graphics.DrawImage(img, rect);
                }
                else if (_pointContents == PointContentsEnum.MaximumButton)
                {
                    Image img = _maximumButtonMousePointImage;
                    Rectangle rect = _maximumButtonRectangle;
                    e.Graphics.DrawImage(img, rect);
                }
                else if (_pointContents == PointContentsEnum.CloseButton)
                {
                    Image img = _closeButtonMousePointImage;
                    Rectangle rect = _closeButtonRectangle;
                    e.Graphics.DrawImage(img, rect);
                }

                e.Graphics.DrawImage(getImage(_form.WindowState), 0, 0, this.Width, this.Height);
            }

            //base.OnPaint(e);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            if (_form != null)
            {
                _form.Resize -= new EventHandler(_form_Resize);
            }

            _form = getForm();

            if (_form != null)
            {
                _form.Resize += new EventHandler(_form_Resize);
            }

            this.Invalidate();
            base.OnParentChanged(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _pointContents = PointContentsEnum.Default;
            this.Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            PointContentsEnum newContents = getPointContents(e.X, e.Y);
            if (newContents != _pointContents)
            {
                _pointContents = newContents;
                this.Invalidate();
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (_minimumButtonRectangle.Contains(e.X, e.Y))
            {
                OnMinimumButtonClick(e);
            }
            else if (_maximumButtonRectangle.Contains(e.X, e.Y))
            {
                OnMaximumButtonClick(e);
            }
            else if (_closeButtonRectangle.Contains(e.X, e.Y))
            {
                OnCloseButtonClick(e);
            }

            base.OnMouseClick(e);
        }

        protected virtual void OnMinimumButtonClick(MouseEventArgs e)
        {
            if (MinimumButtonClick != null)
            {
                MinimumButtonClick(this, e);
            }
        }

        protected virtual void OnMaximumButtonClick(MouseEventArgs e)
        {
            if (MaximumButtonClick != null)
            {
                MaximumButtonClick(this, e);
            }
        }

        protected virtual void OnCloseButtonClick(MouseEventArgs e)
        {
            if (CloseButtonClick != null)
            {
                CloseButtonClick(this, e);
            }
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            this.SetStyle(ControlStyles.DoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.SupportsTransparentBackColor, true);

            this.MinimumSize = _normalImage.Size;
            this.MaximumSize = _normalImage.Size;
            this.Size = _normalImage.Size;
        }

        private Form getForm()
        {
            Control ctl = this;
            while (ctl.Parent != null)
            {
                ctl = ctl.Parent;
            }

            if (ctl is Form)
            {
                return (Form)ctl;
            }
            else
            {
                return null;
            }
        }

        private Image getImage(FormWindowState state)
        {
            if (state == FormWindowState.Maximized)
            {
                return _maximumImage;
            }
            else if (state == FormWindowState.Minimized)
            {
                return _normalImage;
            }
            else
            {
                return _normalImage;
            }
        }

        private PointContentsEnum getPointContents(int x, int y)
        {
            if (_minimumButtonRectangle.Contains(x, y))
            {
                return PointContentsEnum.MinimumButton;
            }
            else if (_maximumButtonRectangle.Contains(x, y))
            {
                return PointContentsEnum.MaximumButton;
            }
            else if (_closeButtonRectangle.Contains(x, y))
            {
                return PointContentsEnum.CloseButton;
            }
            else
            {
                return PointContentsEnum.Default;
            }
        }

        #endregion

        #region フォームイベント

        private void _form_Resize(object sender, EventArgs e)
        {
            if (_windowState != _form.WindowState)
            {
                _windowState = _form.WindowState;
                this.Invalidate();
            }
        }

        #endregion
    }
}
