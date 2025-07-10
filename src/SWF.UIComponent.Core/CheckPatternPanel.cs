using SWF.Core.Base;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;

namespace SWF.UIComponent.Core
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public partial class CheckPatternPanel : Panel
    {
        private const int RECTANGLE_DEFAULT_SIZE = 24;
        private static readonly SolidBrush BRUSH_A = new(Color.FromArgb(48, 48, 48));
        private static readonly SolidBrush BRUSH_B = new(Color.FromArgb(24, 24, 24));

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

        public CheckPatternPanel()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);
            this.SetStyle(
                ControlStyles.Selectable,
                false);
            this.UpdateStyles();

            //this.BackColor = BRUSH_A.Color;
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //using (TimeMeasuring.Run(false, "CheckPatternPanel.OnPaintBackground"))
            //{
            //    base.OnPaintBackground(pevent);
            //}
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (TimeMeasuring.Run(false, "CheckPatternPanel.OnPaint"))
            {
                e.Graphics.SmoothingMode = SmoothingMode.None;
                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                e.Graphics.CompositingMode = CompositingMode.SourceCopy;

                e.Graphics.FillRectangle(BRUSH_A, this.ClientRectangle);
                //this.DrawCheckRectangle(e.Graphics);
            }
        }

        private void DrawCheckRectangle(Graphics g)
        {
            var w = this.ClientRectangle.Width;
            var h = this.ClientRectangle.Height;

            // チェック描画サイズ取得
            var scale = WindowUtil.GetCurrentWindowScale(this);
            var size = (int)(RECTANGLE_DEFAULT_SIZE * scale);
            if ((w / size) % 2 == 1)
            {
                w += size;
            }

            if ((h / size) % 2 == 1)
            {
                h += size;
            }

            // チェック描画領域取得
            var rectsA = new List<Rectangle>();
            var rectsB = new List<Rectangle>();
            var addA = true;
            for (var x = 0; x <= w; x += size)
            {
                for (var y = 0; y <= h; y += size)
                {
                    if (addA)
                    {
                        rectsA.Add(new Rectangle(x, y, size, size));
                    }
                    else
                    {
                        rectsB.Add(new Rectangle(x, y, size, size));
                    }
                    addA = !addA;
                }
            }

            // チェック描画
            g.FillRectangles(BRUSH_A, rectsA.ToArray());
            g.FillRectangles(BRUSH_B, rectsB.ToArray());
        }
    }
}
