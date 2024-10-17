using SWF.Core.ImageAccessor;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using System.Windows.Forms;
using WinApi;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// ドラッグ中のコンテンツを表示するフォーム
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed partial class TabDragForm
        : Form
    {

        private const float OUTLINE_OFFSET = 0.5f;
        private const float DRAW_TAB_WIDHT_OFFSET = 8;
        private const float CAPTURE_WIDTH = 320;

        private TabPalette tabPalette = null;
        private TabDropForm tabDropForm = null;
        private TabDrawArea tabDrawArea = null;
        private DrawTabEventArgs drawTabEventArgs = null;
        private TabSwitch tabSwitch = null;
        private Bitmap regionImage = null;
        private Action<DrawTabEventArgs> drawTabPageMethod = null;

        public TabSwitch TabSwitch
        {
            get
            {
                return this.tabSwitch;
            }
        }

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

        public TabDragForm()
        {
            this.InitializeComponent();
        }

        public void SetLocation(float xOffset, float yOffset)
        {
            var screenPoint = Cursor.Position;
            this.Location = new Point(screenPoint.X - (int)DRAW_TAB_WIDHT_OFFSET - (int)xOffset, screenPoint.Y - (int)yOffset);

            var leftBorderRect = ScreenUtil.GetLeftBorderRect();
            if (leftBorderRect.Contains(screenPoint))
            {
                var leftRect = ScreenUtil.GetLeftRect(this.TabDropForm.Size);
                this.TabDropForm.Location = new Point(leftRect.X, leftRect.Y);
                this.TabDropForm.SetLeftImage();
                WinApiMembers.SetWindowPos(
                    this.TabDropForm.Handle,
                    WinApiMembers.HWND_TOP,
                    leftRect.X, leftRect.Y, leftRect.Width, leftRect.Height,
                    WinApiMembers.SWP_NOSIZE | WinApiMembers.SWP_NOACTIVATE | WinApiMembers.SWP_SHOWWINDOW);
                return;
            }

            var rightBorderRect = ScreenUtil.GetRightBorderRect();
            if (rightBorderRect.Contains(screenPoint))
            {
                var rightRect = ScreenUtil.GetRightRect(this.TabDropForm.Size);
                this.TabDropForm.Location = new Point(rightRect.X, rightRect.Y);
                this.TabDropForm.SetRightImage();
                WinApiMembers.SetWindowPos(
                    this.TabDropForm.Handle,
                    WinApiMembers.HWND_TOP,
                    rightRect.X, rightRect.Y, rightRect.Width, rightRect.Height,
                    WinApiMembers.SWP_NOSIZE | WinApiMembers.SWP_NOACTIVATE | WinApiMembers.SWP_SHOWWINDOW);
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
                var regionImage = new Bitmap((int)regionSize.Width, (int)regionSize.Height);

                using (var g = Graphics.FromImage(regionImage))
                {
                    g.SmoothingMode = SmoothingMode.None;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = CompositingQuality.HighSpeed;
                    g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                    g.CompositingMode = CompositingMode.SourceOver;

                    var outlineRect = this.GetOutlineRectangle(pageSize);
                    //g.FillRectangle(tab.Owner.OutlineBrush, outlineRect);

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

        private void InitializeComponent()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);
            this.UpdateStyles();

            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.Opacity = 0.75d;
        }

        private SizeF GetPageSize(Bitmap pageCap)
        {
            var scale = CAPTURE_WIDTH / (float)pageCap.Width;
            var w = pageCap.Width * scale;
            var h = pageCap.Height * scale;
            return new SizeF(w, h);
        }

        private SizeF GetRegionSize(SizeF pageSize)
        {
            var w = pageSize.Width + OUTLINE_OFFSET * 2;
            var h = this.TabDrawArea.Height + pageSize.Height + OUTLINE_OFFSET;
            return new SizeF(w, h);
        }

        private RectangleF GetOutlineRectangle(SizeF pageSize)
        {
            var w = pageSize.Width + OUTLINE_OFFSET * 2;
            var h = pageSize.Height + OUTLINE_OFFSET * 2;
            var x = 0;
            var y = this.TabDrawArea.Height - OUTLINE_OFFSET;
            return new RectangleF(x, y, w, h);
        }

        private RectangleF GetPageRectangle(RectangleF outlineRectangle)
        {
            var x = outlineRectangle.X + OUTLINE_OFFSET;
            var y = outlineRectangle.Y + OUTLINE_OFFSET;
            var w = outlineRectangle.Width - OUTLINE_OFFSET * 2f;
            var h = outlineRectangle.Height - OUTLINE_OFFSET * 2f;
            return new RectangleF(x, y, w, h);
        }

    }
}
