using SWF.Common;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// ドラッグ中のコンテンツを表示するフォーム
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class TabDragForm
        : Form
    {
        #region 定数・列挙

        private const int OUTLINE_OFFSET = 1;
        private const int DRAW_TAB_WIDHT_OFFSET = 8;
        private const int CAPTURE_WIDTH = 320;

        #endregion

        #region インスタンス変数

        private TabPalette tabPalette = null;
        private TabDropForm tabDropForm = null;
        private TabDrawArea tabDrawArea = null;
        private DrawTabEventArgs drawTabEventArgs = null;
        private TabSwitch tabSwitch = null;
        private Bitmap regionImage = null;
        private Action<DrawTabEventArgs> drawTabPageMethod = null;

        #endregion

        public TabSwitch TabSwitch
        {
            get
            {
                return tabSwitch;
            }
        }

        #region プライベートプロパティ

        private TabPalette TabPalette
        {
            get
            {
                this.tabPalette ??= new TabPalette();
                return this.tabPalette;
            }
        }

        private TabDropForm TabDropForm
        {
            get
            {
                this.tabDropForm ??= new TabDropForm();
                return this.tabDropForm;
            }
        }

        private TabDrawArea TabDrawArea
        {
            get
            {
                this.tabDrawArea ??= new()
                {
                    X = DRAW_TAB_WIDHT_OFFSET,
                    Y = 0
                };
                return this.tabDrawArea;
            }
        }

        private DrawTabEventArgs DrawTabEventArgs
        {
            get
            {
                this.drawTabEventArgs ??= new DrawTabEventArgs();
                return this.drawTabEventArgs;
            }
        }

        #endregion

        #region コンストラクタ

        public TabDragForm()
        {
            this.InitializeComponent();
        }

        #endregion

        #region パブリックメソッド

        public void SetLocation(int xOffset, int yOffset)
        {
            var screenPoint = Cursor.Position;
            this.Location = new Point(screenPoint.X - DRAW_TAB_WIDHT_OFFSET - xOffset, screenPoint.Y - yOffset);

            var topRect = ScreenUtil.GetTopRect(this.TabDropForm.Size);
            if (topRect.Contains(screenPoint))
            {
                this.TabDropForm.Location = new Point(topRect.X, topRect.Y);
                this.TabDropForm.SetMaximumImage();
                this.TabDropForm.Visible = true;
                return;
            }

            var leftRect = ScreenUtil.GetLeftRect(this.TabDropForm.Size);
            if (leftRect.Contains(screenPoint))
            {
                this.TabDropForm.Location = new Point(leftRect.X, leftRect.Y);
                this.TabDropForm.SetLeftImage();
                this.TabDropForm.Visible = true;
                return;
            }

            var rightRect = ScreenUtil.GetRightRect(this.TabDropForm.Size);
            if (rightRect.Contains(screenPoint))
            {
                this.TabDropForm.Location = new Point(rightRect.X, rightRect.Y);
                this.TabDropForm.SetRightImage();
                this.TabDropForm.Visible = true;
                return;
            }

            this.TabDropForm.Visible = false;
        }

        public void SetTab(TabInfo tab)
        {
            ArgumentNullException.ThrowIfNull(tab, nameof(tab));

            if (tab.Owner == null)
            {
                throw new ArgumentException("タブはどこにも所有されていません。", nameof(tab));
            }

            if (this.regionImage != null)
            {
                throw new Exception("領域のイメージが初期化されていません。");
            }

            using (var pageCap = tab.GetPageCapture())
            {
                var pageSize = this.GetPageSize(pageCap);
                var regionSize = this.GetRegionSize(pageSize);
                var regionImage = new Bitmap(regionSize.Width, regionSize.Height);

                using (var g = Graphics.FromImage(regionImage))
                {
                    g.SmoothingMode = SmoothingMode.None;
                    g.InterpolationMode = InterpolationMode.Low;

                    var outlineRect = this.GetOutlineRectangle(pageSize);
                    g.FillRectangle(tab.Owner.OutlineBrush, outlineRect);

                    var pageRect = this.GetPageRectangle(outlineRect);
                    g.DrawImage(pageCap, pageRect);

                    this.TabDrawArea.DrawActiveTab(g);
                }

                this.regionImage = regionImage;
            }

            this.DrawTabEventArgs.Font = this.TabPalette.TitleFont;
            this.DrawTabEventArgs.TitleColor = this.TabPalette.TitleColor;
            this.DrawTabEventArgs.TitleFormatFlags = this.TabPalette.TitleFormatFlags;
            this.DrawTabEventArgs.TextRectangle = this.TabDrawArea.GetPageRectangle();
            this.DrawTabEventArgs.IconRectangle = this.TabDrawArea.GetIconRectangle(tab.Icon);
            this.DrawTabEventArgs.CloseButtonRectangle = this.TabDrawArea.GetCloseButtonRectangle();
            this.DrawTabEventArgs.TextStyle = DrawTextUtil.TextStyle.Glowing;

            this.drawTabPageMethod = tab.DrawingTabPage;

            this.Size = this.regionImage.Size;
            this.Region = ImageUtil.GetRegion(this.regionImage, Color.FromArgb(0, 0, 0, 0));
            this.tabSwitch = tab.Owner;
        }

        public void Clear()
        {
            this.TabDropForm.Visible = false;

            this.drawTabPageMethod = null;

            if (this.regionImage != null)
            {
                this.regionImage.Dispose();
                this.regionImage = null;
            }
        }

        #endregion

        #region 継承メソッド

        protected override void OnLostFocus(EventArgs e)
        {
            if (this.tabSwitch != null && this.Visible)
            {
                this.tabSwitch.CallEndTabDragOperation();
            }

            base.OnLostFocus(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.drawTabPageMethod == null ||
                this.regionImage == null)
            {
                return;
            }

            e.Graphics.DrawImage(this.regionImage, 0, 0, this.regionImage.Width, this.regionImage.Height);

            this.DrawTabEventArgs.Graphics = e.Graphics;
            this.drawTabPageMethod(this.DrawTabEventArgs);
        }

        #endregion

        #region プライベートメソッド

        private void InitializeComponent()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);
            this.UpdateStyles();

            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.Opacity = 0.75d;
        }

        private Size GetPageSize(Bitmap pageCap)
        {
            var scale = CAPTURE_WIDTH / (double)pageCap.Width;
            var w = (int)(pageCap.Width * scale);
            var h = (int)(pageCap.Height * scale);
            return new Size(w, h);
        }

        private Size GetRegionSize(Size pageSize)
        {
            var w = pageSize.Width + OUTLINE_OFFSET * 2;
            var h = this.TabDrawArea.Height + pageSize.Height + OUTLINE_OFFSET;
            return new Size(w, h);
        }

        private Rectangle GetOutlineRectangle(Size pageSize)
        {
            var w = pageSize.Width + OUTLINE_OFFSET * 2;
            var h = pageSize.Height + OUTLINE_OFFSET * 2;
            var x = 0;
            var y = this.TabDrawArea.Height - OUTLINE_OFFSET;
            return new Rectangle(x, y, w, h);
        }

        private Rectangle GetPageRectangle(Rectangle outlineRectangle)
        {
            var l = outlineRectangle.Left + OUTLINE_OFFSET;
            var t = outlineRectangle.Top + OUTLINE_OFFSET;
            var r = outlineRectangle.Right - OUTLINE_OFFSET;
            var b = outlineRectangle.Bottom - OUTLINE_OFFSET;
            return Rectangle.FromLTRB(l, t, r, b);
        }

        #endregion
    }
}
