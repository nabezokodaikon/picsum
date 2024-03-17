using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.Common
{
    [SupportedOSPlatform("windows")]
    public class ToolButton : Button
    {
        private const int REGION_OFFSET = 4;

        public enum ToolButtonRegionType
        {
            Default = 0,
            Left = 1,
            Top = 2,
            Right = 3,
            Bottom = 4,
            HorizonCenter = 5,
            VerticalCenter = 6
        }

        private bool _isLeftClick = false;
        private ToolButtonRegionType _regionType = ToolButtonRegionType.Default;
        private Func<Rectangle> _getRectangleMethod = null;

        public int RegionOffset
        {
            get
            {
                return REGION_OFFSET;
            }
        }

        public ToolButtonRegionType RegionType
        {
            get
            {
                return this._regionType;
            }
            set
            {
                this._regionType = value;

                switch (value)
                {
                    case ToolButtonRegionType.Left:
                        this._getRectangleMethod = new Func<Rectangle>(this.getLeftRectangle);
                        break;
                    case ToolButtonRegionType.Top:
                        this._getRectangleMethod = new Func<Rectangle>(this.getTopRectangle);
                        break;
                    case ToolButtonRegionType.Right:
                        this._getRectangleMethod = new Func<Rectangle>(this.getRightRectangle);
                        break;
                    case ToolButtonRegionType.Bottom:
                        this._getRectangleMethod = new Func<Rectangle>(this.getBottomRectangle);
                        break;
                    case ToolButtonRegionType.HorizonCenter:
                        this._getRectangleMethod = new Func<Rectangle>(this.getHorizonCenterRectangle);
                        break;
                    case ToolButtonRegionType.VerticalCenter:
                        this._getRectangleMethod = new Func<Rectangle>(this.getVerticalCenterRectangle);
                        break;
                    default:
                        this._getRectangleMethod = new Func<Rectangle>(this.getDefaultRectangle);
                        break;
                }

                this.Region = this.getRegion();
                this.Refresh();
            }
        }

        public ToolButton()
        {
            this.initializeComponent();
        }

        public Rectangle GetRegionBounds()
        {
            Rectangle rect = this._getRectangleMethod();
            int x = this.Bounds.Left + rect.Left;
            int y = this.Bounds.Top + rect.Top;
            int w = rect.Width;
            int h = rect.Height;
            return new Rectangle(x, y, w, h);
        }

        protected override void OnResize(EventArgs e)
        {
            this.Region = this.getRegion();
            this.Invalidate();
            base.OnResize(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this._isLeftClick = !this._isLeftClick;
                if (this._isLeftClick)
                {
                    base.OnMouseClick(e);
                }
            }
            else
            {
                base.OnMouseClick(e);
            }
        }

        private void initializeComponent()
        {
            this.SetStyle(ControlStyles.DoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.StandardClick, true);

            this.SetStyle(ControlStyles.Selectable, false);

            this._getRectangleMethod = new Func<Rectangle>(this.getDefaultRectangle);
            this.Region = this.getRegion();
        }

        private Rectangle getDefaultRectangle()
        {
            int l = 0;
            int t = 0;
            int r = this.Width;
            int b = this.Height;
            return Rectangle.FromLTRB(l, t, r, b);
        }

        private Rectangle getLeftRectangle()
        {
            int l = 0;
            int t = 0;
            int r = this.Width - REGION_OFFSET;
            int b = this.Height;
            return Rectangle.FromLTRB(l, t, r, b);
        }

        private Rectangle getTopRectangle()
        {
            int l = 0;
            int t = 0;
            int r = this.Width;
            int b = this.Height - REGION_OFFSET;
            return Rectangle.FromLTRB(l, t, r, b);
        }

        private Rectangle getRightRectangle()
        {
            int l = REGION_OFFSET;
            int t = 0;
            int r = this.Width;
            int b = this.Height;
            return Rectangle.FromLTRB(l, t, r, b);
        }

        private Rectangle getBottomRectangle()
        {
            int l = 0;
            int t = REGION_OFFSET;
            int r = this.Width;
            int b = this.Height;
            return Rectangle.FromLTRB(l, t, r, b);
        }

        private Rectangle getHorizonCenterRectangle()
        {
            int l = REGION_OFFSET;
            int t = 0;
            int r = this.Width - REGION_OFFSET;
            int b = this.Height;
            return Rectangle.FromLTRB(l, t, r, b);
        }

        private Rectangle getVerticalCenterRectangle()
        {
            int l = 0;
            int t = REGION_OFFSET;
            int r = this.Width;
            int b = this.Height - REGION_OFFSET;
            return Rectangle.FromLTRB(l, t, r, b);
        }

        private Region getRegion()
        {
            return new Region(this._getRectangleMethod());
        }
    }
}
