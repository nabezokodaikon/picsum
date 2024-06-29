using System.Runtime.Versioning;

namespace SWF.UIComponent.Core
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

        private bool isLeftClick = false;
        private ToolButtonRegionType regionType = ToolButtonRegionType.Default;
        private Func<Rectangle> getRectangleMethod = null;

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
            this.InitializeComponent();
        }

        public Rectangle GetRegionBounds()
        {
            Rectangle rect = this.getRectangleMethod();
            int x = this.Bounds.Left + rect.Left;
            int y = this.Bounds.Top + rect.Top;
            int w = rect.Width;
            int h = rect.Height;
            return new Rectangle(x, y, w, h);
        }

        protected override void OnResize(EventArgs e)
        {
            this.Region = this.GetRegion();
            this.Invalidate();
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

        private void InitializeComponent()
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

        private Rectangle GetDefaultRectangle()
        {
            int l = 0;
            int t = 0;
            int r = this.Width;
            int b = this.Height;
            return Rectangle.FromLTRB(l, t, r, b);
        }

        private Rectangle GetLeftRectangle()
        {
            int l = 0;
            int t = 0;
            int r = this.Width - REGION_OFFSET;
            int b = this.Height;
            return Rectangle.FromLTRB(l, t, r, b);
        }

        private Rectangle GetTopRectangle()
        {
            int l = 0;
            int t = 0;
            int r = this.Width;
            int b = this.Height - REGION_OFFSET;
            return Rectangle.FromLTRB(l, t, r, b);
        }

        private Rectangle GetRightRectangle()
        {
            int l = REGION_OFFSET;
            int t = 0;
            int r = this.Width;
            int b = this.Height;
            return Rectangle.FromLTRB(l, t, r, b);
        }

        private Rectangle GetBottomRectangle()
        {
            int l = 0;
            int t = REGION_OFFSET;
            int r = this.Width;
            int b = this.Height;
            return Rectangle.FromLTRB(l, t, r, b);
        }

        private Rectangle GetHorizonCenterRectangle()
        {
            int l = REGION_OFFSET;
            int t = 0;
            int r = this.Width - REGION_OFFSET;
            int b = this.Height;
            return Rectangle.FromLTRB(l, t, r, b);
        }

        private Rectangle GetVerticalCenterRectangle()
        {
            int l = 0;
            int t = REGION_OFFSET;
            int r = this.Width;
            int b = this.Height - REGION_OFFSET;
            return Rectangle.FromLTRB(l, t, r, b);
        }

        private Region GetRegion()
        {
            return new Region(this.getRectangleMethod());
        }
    }
}
