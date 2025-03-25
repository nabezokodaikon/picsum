using System.Drawing.Drawing2D;
using System.Runtime.Versioning;

namespace SWF.UIComponent.Core
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public partial class CheckPatternPanel : Panel
    {
        private const int RECTANGLE_SIZE = 24;
        private static readonly SolidBrush BRUSH_A = new(Color.FromArgb(32, 32, 32));
        private static readonly SolidBrush BRUSH_B = new(Color.FromArgb(24, 24, 24));

        public CheckPatternPanel()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);
            this.UpdateStyles();
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.None;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            e.Graphics.CompositingMode = CompositingMode.SourceCopy;

            //e.Graphics.FillRectangle(BRUSH_A, this.ClientRectangle);
            this.DrawCheckRectangle(e.Graphics);
        }

        private void DrawCheckRectangle(Graphics g)
        {
            var w = this.ClientRectangle.Width;
            var h = this.ClientRectangle.Height;

            // チェック描画サイズ取得
            if ((int)(w / RECTANGLE_SIZE) % 2 == 1)
            {
                w += RECTANGLE_SIZE;
            }

            if ((int)(h / RECTANGLE_SIZE) % 2 == 1)
            {
                h += RECTANGLE_SIZE;
            }

            // チェック描画領域取得
            var rectsA = new List<Rectangle>();
            var rectsB = new List<Rectangle>();
            var addA = true;
            for (var x = 0; x <= w; x += RECTANGLE_SIZE)
            {
                for (var y = 0; y <= h; y += RECTANGLE_SIZE)
                {
                    if (addA)
                    {
                        rectsA.Add(new Rectangle(x, y, RECTANGLE_SIZE, RECTANGLE_SIZE));
                    }
                    else
                    {
                        rectsB.Add(new Rectangle(x, y, RECTANGLE_SIZE, RECTANGLE_SIZE));
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
