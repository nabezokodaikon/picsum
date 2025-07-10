using System.ComponentModel;
using System.Runtime.Versioning;

namespace SWF.UIComponent.Core
{
    [SupportedOSPlatform("windows10.0.17763.0")]
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

        private bool _isLeftClick = false;
        private ToolButtonRegionType _regionType = ToolButtonRegionType.Default;
        private Func<Rectangle>? _getRectangleMethod;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new FlatStyle FlatStyle
        {
            get
            {
                return base.FlatStyle;
            }
            private set
            {
                base.FlatStyle = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            private set
            {
                base.BackColor = value;
            }
        }

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
        public new bool TabStop
        {
            get
            {
                return base.TabStop;
            }
            private set
            {
                base.TabStop = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool UseVisualStyleBackColor
        {
            get
            {
                return base.UseVisualStyleBackColor;
            }
            private set
            {
                base.UseVisualStyleBackColor = value;
            }
        }

        public int RegionOffset
        {
            get
            {
                return REGION_OFFSET;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
                        this._getRectangleMethod = new Func<Rectangle>(this.GetLeftRectangle);
                        break;
                    case ToolButtonRegionType.Top:
                        this._getRectangleMethod = new Func<Rectangle>(this.GetTopRectangle);
                        break;
                    case ToolButtonRegionType.Right:
                        this._getRectangleMethod = new Func<Rectangle>(this.GetRightRectangle);
                        break;
                    case ToolButtonRegionType.Bottom:
                        this._getRectangleMethod = new Func<Rectangle>(this.GetBottomRectangle);
                        break;
                    case ToolButtonRegionType.HorizonCenter:
                        this._getRectangleMethod = new Func<Rectangle>(this.GetHorizonCenterRectangle);
                        break;
                    case ToolButtonRegionType.VerticalCenter:
                        this._getRectangleMethod = new Func<Rectangle>(this.GetVerticalCenterRectangle);
                        break;
                    case ToolButtonRegionType.Default:
                        break;
                    default:
                        this._getRectangleMethod = new Func<Rectangle>(this.GetDefaultRectangle);
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
                ControlStyles.ResizeRedraw |
                ControlStyles.StandardClick |
                ControlStyles.UserPaint |
                ControlStyles.SupportsTransparentBackColor,
                true);
            this.SetStyle(
                ControlStyles.Selectable,
                false);
            this.UpdateStyles();

            this._getRectangleMethod = new Func<Rectangle>(this.GetDefaultRectangle);
            this.Region = this.GetRegion();

            this.UseVisualStyleBackColor = false;
            this.TabIndex = 0;
            this.TabStop = false;
            this.BackColor = Color.FromArgb(250, 250, 250);
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.FlatAppearance.MouseDownBackColor = Color.FromArgb(250, 250, 250);
        }

        public Rectangle GetRegionBounds()
        {
            if (this._getRectangleMethod == null)
            {
                throw new NullReferenceException("描画領域取得メソッドがNullです。");
            }

            var rect = this._getRectangleMethod();
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
            if (this._getRectangleMethod == null)
            {
                throw new NullReferenceException("描画領域取得メソッドがNullです。");
            }

            return new Region(this._getRectangleMethod());
        }
    }
}
