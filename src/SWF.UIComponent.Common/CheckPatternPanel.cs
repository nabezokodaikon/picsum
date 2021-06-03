using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SWF.UIComponent.Common
{
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
                return _rectangleSize;
            }
            set
            {
                _rectangleSize = value;
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
            initializeComponent();
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

            drawCheckRectangle(e.Graphics);
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
            if ((int)(w / _rectangleSize) % 2 == 1)
            {
                w += _rectangleSize;
            }

            if ((int)(h / _rectangleSize) % 2 == 1)
            {
                h += _rectangleSize;
            }

            // チェック描画領域取得
            List<Rectangle> rectsA = new List<Rectangle>();
            List<Rectangle> rectsB = new List<Rectangle>();
            bool addA = true;
            for (int x = 0; x <= w; x += _rectangleSize)
            {
                for (int y = 0; y <= h; y += _rectangleSize)
                {
                    if (addA)
                    {
                        rectsA.Add(new Rectangle(x, y, _rectangleSize, _rectangleSize));
                    }
                    else
                    {
                        rectsB.Add(new Rectangle(x, y, _rectangleSize, _rectangleSize));
                    }
                    addA = !addA;
                }
            }

            // チェック描画
            g.FillRectangles(_brushA, rectsA.ToArray());
            g.FillRectangles(_brushB, rectsB.ToArray());
        }

        #endregion
    }
}
