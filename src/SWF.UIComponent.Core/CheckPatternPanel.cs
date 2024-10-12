using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;

namespace SWF.UIComponent.Core
{
    [SupportedOSPlatform("windows")]
    public class CheckPatternPanel : Panel
    {


        private int _rectangleSize = 24;
        private readonly SolidBrush brushA = new(Color.FromArgb(255, Color.FromArgb(64, 64, 64)));
        private readonly SolidBrush brushB = new(Color.FromArgb(64, Color.FromArgb(16, 16, 16)));





        public int RectangleSize
        {
            get
            {
                return this._rectangleSize;
            }
            set
            {
                this._rectangleSize = value;
                this.Invalidate();
                this.Update();
            }
        }





        public CheckPatternPanel()
        {
            this.InitializeComponent();
        }





        protected override void OnPaintBackground(PaintEventArgs pevent)
        {

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var sw = Stopwatch.StartNew();

            e.Graphics.SmoothingMode = SmoothingMode.None;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            e.Graphics.CompositingMode = CompositingMode.SourceCopy;

            e.Graphics.FillRectangle(this.brushA, this.ClientRectangle);

            sw.Stop();
            //Console.WriteLine($"[{Thread.CurrentThread.Name}] CheckPatternPanel.OnPaint: {sw.ElapsedMilliseconds} ms");
        }





        private void InitializeComponent()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);
            this.UpdateStyles();
        }

        private void DrawCheckRectangle(Graphics g)
        {
            int w = this.ClientRectangle.Width;
            int h = this.ClientRectangle.Height;

            // グラデーション描画            
            using (LinearGradientBrush b = new LinearGradientBrush(this.ClientRectangle,
                                                                   Color.FromArgb(80, 80, 80),
                                                                   Color.FromArgb(16, 16, 16),
                                                                   LinearGradientMode.ForwardDiagonal))
            {
                g.FillRectangle(b, this.ClientRectangle);
            }

            // チェック描画サイズ取得
            if ((int)(w / this._rectangleSize) % 2 == 1)
            {
                w += this._rectangleSize;
            }

            if ((int)(h / this._rectangleSize) % 2 == 1)
            {
                h += this._rectangleSize;
            }

            // チェック描画領域取得
            var rectsA = new List<Rectangle>();
            var rectsB = new List<Rectangle>();
            bool addA = true;
            for (int x = 0; x <= w; x += this._rectangleSize)
            {
                for (int y = 0; y <= h; y += this._rectangleSize)
                {
                    if (addA)
                    {
                        rectsA.Add(new Rectangle(x, y, this._rectangleSize, this._rectangleSize));
                    }
                    else
                    {
                        rectsB.Add(new Rectangle(x, y, this._rectangleSize, this._rectangleSize));
                    }
                    addA = !addA;
                }
            }

            // チェック描画
            g.FillRectangles(this.brushA, rectsA.ToArray());
            g.FillRectangles(this.brushB, rectsB.ToArray());
        }


    }
}
