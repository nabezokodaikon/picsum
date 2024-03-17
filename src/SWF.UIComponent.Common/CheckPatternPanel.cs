using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.Common
{
    [SupportedOSPlatform("windows")]
    public class CheckPatternPanel : Panel
    {
        #region 定数・列挙

        #endregion

        #region イベント・デリゲート

        #endregion

        #region インスタンス変数

        private int _rectangleSize = 24;
        private SolidBrush _brushA = new SolidBrush(Color.FromArgb(64, Color.FromArgb(48, 48, 48)));
        private SolidBrush _brushB = new SolidBrush(Color.FromArgb(64, Color.FromArgb(16, 16, 16)));

        #endregion

        #region パブリックプロパティ

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
            }
        }

        #endregion

        #region 継承プロパティ

        #endregion

        #region プライベートプロパティ

        #endregion

        #region コンストラクタ

        public CheckPatternPanel()
        {
            this.initializeComponent();
        }

        #endregion

        #region パブリックメソッド

        #endregion

        #region 継承メソッド

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.None;
            e.Graphics.InterpolationMode = InterpolationMode.Low;
            e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;

            this.drawCheckRectangle(e.Graphics);
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            this.SetStyle(ControlStyles.DoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.ResizeRedraw, true);
        }

        private void drawCheckRectangle(Graphics g)
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
            List<Rectangle> rectsA = new List<Rectangle>();
            List<Rectangle> rectsB = new List<Rectangle>();
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
            g.FillRectangles(this._brushA, rectsA.ToArray());
            g.FillRectangles(this._brushB, rectsB.ToArray());
        }

        #endregion
    }
}
