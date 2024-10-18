using System.Runtime.Versioning;

namespace SWF.UIComponent.Core
{
    [SupportedOSPlatform("windows")]
    public partial class ToolButton : Button
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

        private bool isLeftClick = false;
        private ToolButtonRegionType regionType = ToolButtonRegionType.Default;
        private Func<Rectangle>? getRectangleMethod;

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
                return this.regionType;
            }
            set
            {
                this.regionType = value;

                switch (value)
                {
                    case ToolButtonRegionType.Left:
                        this.getRectangleMethod = new Func<Rectangle>(this.GetLeftRectangle);
                        break;
                    case ToolButtonRegionType.Top:
                        this.getRectangleMethod = new Func<Rectangle>(this.GetTopRectangle);
                        break;
                    case ToolButtonRegionType.Right:
                        this.getRectangleMethod = new Func<Rectangle>(this.GetRightRectangle);
                        break;
                    case ToolButtonRegionType.Bottom:
                        this.getRectangleMethod = new Func<Rectangle>(this.GetBottomRectangle);
                        break;
                    case ToolButtonRegionType.HorizonCenter:
                        this.getRectangleMethod = new Func<Rectangle>(this.GetHorizonCenterRectangle);
                        break;
                    case ToolButtonRegionType.VerticalCenter:
                        this.getRectangleMethod = new Func<Rectangle>(this.GetVerticalCenterRectangle);
                        break;
                    case ToolButtonRegionType.Default:
                        break;
                    default:
                        this.getRectangleMethod = new Func<Rectangle>(this.GetDefaultRectangle);
                        break;
                }

                this.Region = this.GetRegion();
                this.Refresh();
            }
        }

        public ToolButton()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.StandardClick |
                ControlStyles.UserPaint,
                true);

            this.SetStyle(
                ControlStyles.Selectable,
                false);

            this.UpdateStyles();

            this.getRectangleMethod = new Func<Rectangle>(this.GetDefaultRectangle);
            this.Region = this.GetRegion();
        }

        public Rectangle GetRegionBounds()
        {
            if (this.getRectangleMethod == null)
            {
                throw new NullReferenceException("描画領域取得メソッドがNullです。");
            }

            var rect = this.getRectangleMethod();
            var x = this.Bounds.Left + rect.Left;
            var y = this.Bounds.Top + rect.Top;
            var w = rect.Width;
            var h = rect.Height;
            return new Rectangle(x, y, w, h);
        }

        protected override void OnResize(EventArgs e)
        {
            this.Region = this.GetRegion();
            this.Invalidate();
            this.Update();
            base.OnResize(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.isLeftClick = !this.isLeftClick;
                if (this.isLeftClick)
                {
                    base.OnMouseClick(e);
                }
            }
            else
            {
                base.OnMouseClick(e);
            }
        }

        private Rectangle GetDefaultRectangle()
        {
            var l = 0;
            var t = 0;
            var r = this.Width;
            var b = this.Height;
            return Rectangle.FromLTRB(l, t, r, b);
        }

        private Rectangle GetLeftRectangle()
        {
            var l = 0;
            var t = 0;
            var r = this.Width - REGION_OFFSET;
            var b = this.Height;
            return Rectangle.FromLTRB(l, t, r, b);
        }

        private Rectangle GetTopRectangle()
        {
            var l = 0;
            var t = 0;
            var r = this.Width;
            var b = this.Height - REGION_OFFSET;
            return Rectangle.FromLTRB(l, t, r, b);
        }

        private Rectangle GetRightRectangle()
        {
            var l = REGION_OFFSET;
            var t = 0;
            var r = this.Width;
            var b = this.Height;
            return Rectangle.FromLTRB(l, t, r, b);
        }

        private Rectangle GetBottomRectangle()
        {
            var l = 0;
            var t = REGION_OFFSET;
            var r = this.Width;
            var b = this.Height;
            return Rectangle.FromLTRB(l, t, r, b);
        }

        private Rectangle GetHorizonCenterRectangle()
        {
            var l = REGION_OFFSET;
            var t = 0;
            var r = this.Width - REGION_OFFSET;
            var b = this.Height;
            return Rectangle.FromLTRB(l, t, r, b);
        }

        private Rectangle GetVerticalCenterRectangle()
        {
            var l = 0;
            var t = REGION_OFFSET;
            var r = this.Width;
            var b = this.Height - REGION_OFFSET;
            return Rectangle.FromLTRB(l, t, r, b);
        }

        private Region GetRegion()
        {
            if (this.getRectangleMethod == null)
            {
                throw new NullReferenceException("描画領域取得メソッドがNullです。");
            }

            return new Region(this.getRectangleMethod());
        }
    }
}
