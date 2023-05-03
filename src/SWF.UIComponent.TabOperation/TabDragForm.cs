using SWF.Common;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// ドラッグ中のコンテンツを表示するフォーム
    /// </summary>
    internal class TabDragForm : Form
    {
        #region 定数・列挙

        private const int OUTLINE_OFFSET = 1;
        private const int DRAW_TAB_WIDHT_OFFSET = 8;
        private const int CAPTURE_WIDTH = 320;

        #endregion

        #region イベント・デリゲート

        #endregion

        #region インスタンス変数

        private TabPalette _tabPalette = null;
        private TabDropForm _tabDropForm = null;
        private TabDrawArea _tabDrawArea = null;
        private DrawTabEventArgs _drawTabEventArgs = null;

        private Bitmap _regionImage = null;
        private Action<DrawTabEventArgs> _drawTabContentsMethod = null;

        #endregion

        #region パブリックプロパティ

        #endregion

        #region 継承プロパティ

        #endregion

        #region プライベートプロパティ

        private TabPalette tabPalette
        {
            get
            {
                if (_tabPalette == null)
                {
                    _tabPalette = new TabPalette();
                }

                return _tabPalette;
            }
        }

        private TabDropForm tabDropForm
        {
            get
            {
                if (_tabDropForm == null)
                {
                    _tabDropForm = new TabDropForm();
                }

                return _tabDropForm;
            }
        }

        private TabDrawArea tabDrawArea
        {
            get
            {
                if (_tabDrawArea == null)
                {
                    _tabDrawArea = new TabDrawArea();
                    _tabDrawArea.X = DRAW_TAB_WIDHT_OFFSET;
                    _tabDrawArea.Y = 0;
                }

                return _tabDrawArea;
            }
        }

        private DrawTabEventArgs drawTabEventArgs
        {
            get
            {
                if (_drawTabEventArgs == null)
                {
                    _drawTabEventArgs = new DrawTabEventArgs();
                }

                return _drawTabEventArgs;
            }
        }

        #endregion

        #region コンストラクタ

        public TabDragForm()
        {
            initializeComponent();
        }

        #endregion

        #region パブリックメソッド

        public void SetLocation(int xOffset, int yOffset)
        {
            Point screenPoint = Cursor.Position;
            this.Location = new Point(screenPoint.X - DRAW_TAB_WIDHT_OFFSET - xOffset, screenPoint.Y - yOffset);

            Rectangle topRect = ScreenUtil.GetTopRect(tabDropForm.Size);
            if (topRect.Contains(screenPoint))
            {
                tabDropForm.Location = new Point(topRect.X, topRect.Y);
                tabDropForm.SetMaximumImage();
                tabDropForm.Visible = true;
                return;
            }

            Rectangle leftRect = ScreenUtil.GetLeftRect(tabDropForm.Size);
            if (leftRect.Contains(screenPoint))
            {
                tabDropForm.Location = new Point(leftRect.X, leftRect.Y);
                tabDropForm.SetLeftImage();
                tabDropForm.Visible = true;
                return;
            }

            Rectangle rightRect = ScreenUtil.GetRightRect(tabDropForm.Size);
            if (rightRect.Contains(screenPoint))
            {
                tabDropForm.Location = new Point(rightRect.X, rightRect.Y);
                tabDropForm.SetRightImage();
                tabDropForm.Visible = true;
                return;
            }

            tabDropForm.Visible = false;
        }

        public void SetTab(TabInfo tab)
        {
            if (tab == null)
            {
                throw new ArgumentNullException("tab");
            }

            if (tab.Owner == null)
            {
                throw new ArgumentException("タブはどこにも所有されていません。", "tab");
            }

            if (_regionImage != null)
            {
                throw new Exception("領域のイメージが初期化されていません。");
            }

            using (Bitmap contentsCap = tab.GetContentsCapture())
            {
                Size contentsSize = getContentsSize(contentsCap);
                Size regionSize = getRegionSize(contentsSize);
                Bitmap regionImage = new Bitmap(regionSize.Width, regionSize.Height);

                using (Graphics g = Graphics.FromImage(regionImage))
                {
                    g.SmoothingMode = SmoothingMode.None;
                    g.InterpolationMode = InterpolationMode.Low;

                    Rectangle outlineRect = getOutlineRectangle(contentsSize);
                    g.FillRectangle(tab.Owner.OutlineBrush, outlineRect);

                    Rectangle contentsRect = getContentsRectangle(outlineRect);
                    g.DrawImage(contentsCap, contentsRect);

                    tabDrawArea.DrawActiveTab(g);
                }

                _regionImage = regionImage;
            }

            drawTabEventArgs.Font = tabPalette.TitleFont;
            drawTabEventArgs.TitleColor = tabPalette.TitleColor;
            drawTabEventArgs.TitleFormatFlags = tabPalette.TitleFormatFlags;
            drawTabEventArgs.TextRectangle = tabDrawArea.GetContentsRectangle();
            drawTabEventArgs.IconRectangle = tabDrawArea.GetIconRectangle(tab.Icon);
            drawTabEventArgs.CloseButtonRectangle = tabDrawArea.GetCloseButtonRectangle();
            drawTabEventArgs.TextStyle = DrawTextUtil.TextStyle.Glowing;

            _drawTabContentsMethod = tab.DrawingTabContents;

            this.Size = _regionImage.Size;
            this.Region = ImageUtil.GetRegion(_regionImage, Color.FromArgb(0, 0, 0, 0));
        }

        public void Clear()
        {
            tabDropForm.Visible = false;

            _drawTabContentsMethod = null;

            if (_regionImage != null)
            {
                _regionImage.Dispose();
                _regionImage = null;
            }
        }

        #endregion

        #region 継承メソッド

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_drawTabContentsMethod == null ||
                _regionImage == null)
            {
                return;
            }

            e.Graphics.DrawImage(_regionImage, 0, 0, _regionImage.Width, _regionImage.Height);

            drawTabEventArgs.Graphics = e.Graphics;
            _drawTabContentsMethod(drawTabEventArgs);
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            this.SetStyle(ControlStyles.DoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.ResizeRedraw, true);

            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.Opacity = 0.75d;
        }

        private Size getContentsSize(Bitmap contentsCap)
        {
            double scale = CAPTURE_WIDTH / (double)contentsCap.Width;
            int w = (int)(contentsCap.Width * scale);
            int h = (int)(contentsCap.Height * scale);
            return new Size(w, h);
        }

        private Size getRegionSize(Size contentsSize)
        {
            int w = contentsSize.Width + OUTLINE_OFFSET * 2;
            int h = tabDrawArea.Height + contentsSize.Height + OUTLINE_OFFSET;
            return new Size(w, h);
        }

        private Rectangle getOutlineRectangle(Size contentsSize)
        {
            int w = contentsSize.Width + OUTLINE_OFFSET * 2;
            int h = contentsSize.Height + OUTLINE_OFFSET * 2;
            int x = 0;
            int y = tabDrawArea.Height - OUTLINE_OFFSET;
            return new Rectangle(x, y, w, h);
        }

        private Rectangle getContentsRectangle(Rectangle outlineRectangle)
        {
            int l = outlineRectangle.Left + OUTLINE_OFFSET;
            int t = outlineRectangle.Top + OUTLINE_OFFSET;
            int r = outlineRectangle.Right - OUTLINE_OFFSET;
            int b = outlineRectangle.Bottom - OUTLINE_OFFSET;
            return Rectangle.FromLTRB(l, t, r, b);
        }

        #endregion
    }
}
