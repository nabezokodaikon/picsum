using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using WinApi;

namespace SWF.UIComponent.FreeForm
{
    public class StandardForm : Form
    {
        #region 定数・列挙

        public enum PointContentsEnum
        {
            Default = 0,
            Left = 1,
            Top = 2,
            Right = 3,
            Bottom = 4,
            LeftTop = 5,
            RightTop = 6,
            LeftBottom = 7,
            RightBottom = 8,
        }

        #endregion

        #region イベント・デリゲート

        #endregion

        #region インスタンス変数

        private StandardFrame _standardFrame = new StandardFrame();

        // マウス位置の内容
        private PointContentsEnum _mousePointContents = PointContentsEnum.Default;

        // マウスのクリック時の座標
        private Point _pointFromMouseDown;

        // マウスクリック時のフォームの領域
        private Rectangle _formRectangleFromMouseDown;

        // マウス移動時の処理
        private Action<Point> _mouseMoveAction = (p) => { };

        #endregion

        #region パブリックプロパティ

        #endregion

        #region 継承プロパティ

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style = cp.Style | WinApiMembers.WS_SYSMENU;
                return cp;
            }
        }

        #endregion

        #region プライベートプロパティ

        #endregion

        #region コンストラクタ

        public StandardForm()
        {
            initializeComponent();
        }

        #endregion

        #region パブリックメソッド

        public void MouseDownProcess()
        {
            if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left && this.WindowState == FormWindowState.Normal)
            {
                Point screenPoint = Cursor.Position;
                Point clientPoint = this.PointToClient(screenPoint);
                PointContentsEnum pointContents = getPointContents(clientPoint.X, clientPoint.Y);
                this.Cursor = getCursor(pointContents);
                _formRectangleFromMouseDown = this.Bounds;
                _mouseMoveAction = getMouseMoveAction(pointContents);
                _pointFromMouseDown = screenPoint;
            }
        }

        public void MouseMoveProcess()
        {
            if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left)
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    Point screenPoint = Cursor.Position;
                    _mouseMoveAction(screenPoint);
                    this.Update();
                }
            }
            else
            {
                Point screenPoint = Cursor.Position;
                Point clientPoint = this.PointToClient(screenPoint);
                PointContentsEnum pointContents = getPointContents(clientPoint.X, clientPoint.Y);

                if (this.WindowState == FormWindowState.Normal)
                {
                    this.Cursor = getCursor(pointContents);
                }

                if (pointContents != _mousePointContents)
                {
                    _mousePointContents = pointContents;
                    this.Invalidate();
                }
            }
        }

        public void MouseLeftDoubleClickProcess()
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        #endregion

        #region 継承メソッド

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            this.Region = _standardFrame.GetRegion(this.Width, this.Height);

            base.OnInvalidated(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.None;
            e.Graphics.InterpolationMode = InterpolationMode.Low;

            drawBackground(e.Graphics);

            //base.OnPaint(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            PointContentsEnum pointContents = PointContentsEnum.Default;
            this.Cursor = getCursor(pointContents);

            if (pointContents != _mousePointContents)
            {
                _mouseMoveAction = getMouseMoveAction(pointContents);
                _mousePointContents = pointContents;
                this.Invalidate();
            }

            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            MouseDownProcess();

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            MouseMoveProcess();

            base.OnMouseMove(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MouseLeftDoubleClickProcess();
            }

            base.OnMouseDoubleClick(e);
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            Rectangle rect = Screen.GetWorkingArea(this);
            this.MaximizedBounds = new Rectangle(0, 0, rect.Size.Width, rect.Size.Height);

            base.OnLocationChanged(e);
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            this.FormBorderStyle = FormBorderStyle.None;

            this.SetStyle(ControlStyles.DoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.ResizeRedraw, true);
        }

        private PointContentsEnum getPointContents(int x, int y)
        {
            int frameSize = _standardFrame.FrameSize * 2;

            if (x < frameSize && y < frameSize)
            {
                // 左上
                return PointContentsEnum.LeftTop;
            }
            else if (this.Width - frameSize < x && y < frameSize)
            {
                // 右上
                return PointContentsEnum.RightTop;
            }
            else if (x < frameSize && this.Height - frameSize < y)
            {
                // 左下
                return PointContentsEnum.LeftBottom;
            }
            else if (this.Width - frameSize < x && this.Height - frameSize < y)
            {
                // 右下
                return PointContentsEnum.RightBottom;
            }
            else if (x < frameSize)
            {
                // 左
                return PointContentsEnum.Left;
            }
            else if (this.Width - frameSize < x)
            {
                // 右
                return PointContentsEnum.Right;
            }
            else if (y < frameSize)
            {
                // 上
                return PointContentsEnum.Top;
            }
            else if (this.Height - frameSize < y)
            {
                // 下
                return PointContentsEnum.Bottom;
            }
            else
            {
                return PointContentsEnum.Default;
            }
        }

        private Cursor getCursor(PointContentsEnum pointContents)
        {
            switch (pointContents)
            {
                case PointContentsEnum.Default:
                    return Cursors.Default;
                case PointContentsEnum.Left:
                    return Cursors.SizeWE;
                case PointContentsEnum.Top:
                    return Cursors.SizeNS;
                case PointContentsEnum.Right:
                    return Cursors.SizeWE;
                case PointContentsEnum.Bottom:
                    return Cursors.SizeNS;
                case PointContentsEnum.LeftTop:
                    return Cursors.SizeNWSE;
                case PointContentsEnum.RightTop:
                    return Cursors.SizeNESW;
                case PointContentsEnum.LeftBottom:
                    return Cursors.SizeNESW;
                case PointContentsEnum.RightBottom:
                    return Cursors.SizeNWSE;
                default:
                    throw new Exception();
            }
        }

        private Action<Point> getMouseMoveAction(PointContentsEnum pointContents)
        {
            switch (pointContents)
            {
                case PointContentsEnum.Default:
                    return new Action<Point>(moveAction);
                case PointContentsEnum.Left:
                    return new Action<Point>(sizeChageActionFromLeft);
                case PointContentsEnum.Top:
                    return new Action<Point>(sizeChageActionFromTop);
                case PointContentsEnum.Right:
                    return new Action<Point>(sizeChageActionFromRight);
                case PointContentsEnum.Bottom:
                    return new Action<Point>(sizeChageActionFromBottom);
                case PointContentsEnum.LeftTop:
                    return new Action<Point>(sizeChageActionFromLeftTop);
                case PointContentsEnum.RightTop:
                    return new Action<Point>(sizeChageActionFromRightTop);
                case PointContentsEnum.LeftBottom:
                    return new Action<Point>(sizeChageActionFromLeftBottom);
                case PointContentsEnum.RightBottom:
                    return new Action<Point>(sizeChageActionFromRightBottom);
                default:
                    throw new Exception();
            }
        }

        #region 描画処理

        private void drawBackground(Graphics g)
        {
            g.DrawImage(_standardFrame.LeftTopImage, _standardFrame.GetLeftTopRectangle());
            g.DrawImage(_standardFrame.RightTopImage, _standardFrame.GetRightTopRectangle(this.Width));
            g.DrawImage(_standardFrame.RightBottomImage, _standardFrame.GetRightBottomRectangle(this.Width, this.Height));
            g.DrawImage(_standardFrame.LeftBottomImage, _standardFrame.GetLeftBottomRectangle(this.Width, this.Height));

            Rectangle left = _standardFrame.GetLeftRectangle(this.Height);
            Rectangle top = _standardFrame.GetTopRectangle(this.Width);
            Rectangle right = _standardFrame.GetRightRectangle(this.Width, this.Height);
            Rectangle bottom = _standardFrame.GetBottomRectangle(this.Width, this.Height);

            for (int i = 0; i < _standardFrame.FrameSize; i++)
            {
                SolidBrush b = _standardFrame.FrameBrushList[i];
                g.FillRectangles(b, new Rectangle[] { left, top, right, bottom });
                left.Offset(1, 0);
                top.Offset(0, 1);
                right.Offset(-1, 0);
                bottom.Offset(0, -1);
            }

            g.FillRectangle(_standardFrame.FrameBrushList.Last(), _standardFrame.GetContentsRectangle(this.Width, this.Height));
        }

        #endregion

        #region マウス移動操作

        private void defaultAction(Point p) { }

        private void moveAction(Point p)
        {
            int moveWidth = p.X - _pointFromMouseDown.X;
            int moveHeight = p.Y - _pointFromMouseDown.Y;
            Rectangle rect = _formRectangleFromMouseDown;
            this.Left = rect.Left + moveWidth;
            this.Top = rect.Top + moveHeight;
        }

        private void sizeChageActionFromLeft(Point p)
        {
            int offsetWidth = p.X - _pointFromMouseDown.X;
            Rectangle rect = _formRectangleFromMouseDown;
            Size size = new Size(rect.Width - offsetWidth, rect.Height);
            if (size.Width > this.MinimumSize.Width)
            {
                this.SetBounds(rect.X + offsetWidth, rect.Y, size.Width, size.Height);
            }
        }

        private void sizeChageActionFromTop(Point p)
        {
            int offsetHeight = p.Y - _pointFromMouseDown.Y;
            Rectangle rect = _formRectangleFromMouseDown;
            Size size = new Size(rect.Width, rect.Height - offsetHeight);
            if (size.Height > this.MinimumSize.Height)
            {
                this.SetBounds(rect.X, rect.Y + offsetHeight, size.Width, size.Height);
            }
        }

        private void sizeChageActionFromRight(Point p)
        {
            int offsetWidth = p.X - _pointFromMouseDown.X;
            Rectangle rect = _formRectangleFromMouseDown;
            Size size = new Size(rect.Width + offsetWidth, rect.Height);
            if (size.Width > this.MinimumSize.Width)
            {
                this.SetBounds(rect.X, rect.Y, size.Width, size.Height);
            }
        }

        private void sizeChageActionFromBottom(Point p)
        {
            int offsetHeight = p.Y - _pointFromMouseDown.Y;
            Rectangle rect = _formRectangleFromMouseDown;
            Size size = new Size(rect.Width, rect.Height + offsetHeight);
            if (size.Height > this.MinimumSize.Height)
            {
                this.SetBounds(rect.X, rect.Y, size.Width, size.Height);
            }
        }

        private void sizeChageActionFromLeftTop(Point p)
        {
            int offsetWidth = p.X - _pointFromMouseDown.X;
            int offsetHeight = p.Y - _pointFromMouseDown.Y;

            Rectangle rect = _formRectangleFromMouseDown;

            Size size = new Size(Math.Max(rect.Width - offsetWidth, this.MinimumSize.Width),
                                 Math.Max(rect.Height - offsetHeight, this.MinimumSize.Height));

            Point location = Point.Empty;

            if (size.Width > this.MinimumSize.Width)
            {
                location.X = rect.X + offsetWidth;
            }
            else
            {
                location.X = this.Left;
            }

            if (size.Height > this.MinimumSize.Height)
            {
                location.Y = rect.Y + offsetHeight;
            }
            else
            {
                location.Y = this.Top;
            }

            this.SetBounds(location.X, location.Y, size.Width, size.Height);
        }

        private void sizeChageActionFromRightTop(Point p)
        {
            int offsetWidth = p.X - _pointFromMouseDown.X;
            int offsetHeight = p.Y - _pointFromMouseDown.Y;

            Rectangle rect = _formRectangleFromMouseDown;

            Size size = new Size(Math.Max(rect.Width + offsetWidth, this.MinimumSize.Width),
                                 Math.Max(rect.Height - offsetHeight, this.MinimumSize.Height));

            Point location = Point.Empty;

            if (size.Width > this.MinimumSize.Width)
            {
                location.X = rect.X;
            }
            else
            {
                location.X = this.Left;
            }

            if (size.Height > this.MinimumSize.Height)
            {
                location.Y = rect.Y + offsetHeight;
            }
            else
            {
                location.Y = this.Top;
            }

            this.SetBounds(location.X, location.Y, size.Width, size.Height);
        }

        private void sizeChageActionFromLeftBottom(Point p)
        {
            int offsetWidth = p.X - _pointFromMouseDown.X;
            int offsetHeight = p.Y - _pointFromMouseDown.Y;

            Rectangle rect = _formRectangleFromMouseDown;

            Size size = new Size(Math.Max(rect.Width - offsetWidth, this.MinimumSize.Width),
                                 Math.Max(rect.Height + offsetHeight, this.MinimumSize.Height));

            Point location = Point.Empty;

            if (size.Width > this.MinimumSize.Width)
            {
                location.X = rect.X + offsetWidth;
            }
            else
            {
                location.X = this.Left;
            }

            if (size.Height > this.MinimumSize.Height)
            {
                location.Y = rect.Y;
            }
            else
            {
                location.Y = this.Top;
            }

            this.SetBounds(location.X, location.Y, size.Width, size.Height);
        }

        private void sizeChageActionFromRightBottom(Point p)
        {
            int offsetWidth = p.X - _pointFromMouseDown.X;
            int offsetHeight = p.Y - _pointFromMouseDown.Y;

            Rectangle rect = _formRectangleFromMouseDown;

            Size size = new Size(Math.Max(rect.Width + offsetWidth, this.MinimumSize.Width),
                                 Math.Max(rect.Height + offsetHeight, this.MinimumSize.Height));

            Point location = Point.Empty;

            if (size.Width > this.MinimumSize.Width)
            {
                location.X = rect.X;
            }
            else
            {
                location.X = this.Left;
            }

            if (size.Height > this.MinimumSize.Height)
            {
                location.Y = rect.Y;
            }
            else
            {
                location.Y = this.Top;
            }

            this.SetBounds(location.X, location.Y, size.Width, size.Height);
        }

        #endregion

        #endregion
    }
}
