using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using SWF.Core.ResourceAccessor;
using SWF.UIComponent.Base;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WinApi;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// ドラッグ中のコンテンツを表示するフォーム
    /// </summary>

    internal sealed partial class TabDragForm
        : BaseForm
    {
        private static readonly Color TRANSPARENT_COLOR = Color.FromArgb(0, 0, 0, 0);

        private TabDrawArea _tabDrawArea = null;
        private DrawTabEventArgs _drawTabEventArgs = null;

#pragma warning disable CA2213 // TabInfoを保持しているTabSwitchを保持する変数。
        private TabSwitch _tabSwitch = null;
#pragma warning restore CA2213

        private Bitmap _regionImage = null;
        private Action<DrawTabEventArgs> _drawTabPageMethod = null;

        private float GetOutlineOffset()
        {
            const float OUTLINE_OFFSET = 0.5f;
            var scale = WindowUtil.GetCursorWindowScale();
            return OUTLINE_OFFSET * scale;
        }

        private float GetDrawTabWidthOffset()
        {
            const float DRAW_TAB_WIDHT_OFFSET = 8;
            _ = WinApiMembers.WindowFromPoint(
                new WinApiMembers.POINT(Cursor.Position.X, Cursor.Position.Y));
            var scale = WindowUtil.GetCursorWindowScale();
            return DRAW_TAB_WIDHT_OFFSET * scale;
        }

        private float GetCaptureWidth()
        {
            const float CAPTURE_WIDTH = 320;
            var scale = WindowUtil.GetCursorWindowScale();
            return CAPTURE_WIDTH * scale;
        }

        public TabSwitch TabSwitch
        {
            get
            {
                return this._tabSwitch;
            }
        }

        private TabDropForm TabDropForm { get; } = new();

        private TabDrawArea TabDrawArea
        {
            get
            {
                this._tabDrawArea ??= new(this)
                {
                    X = this.GetDrawTabWidthOffset(),
                    Y = 0
                };
                return this._tabDrawArea;
            }
        }

        private DrawTabEventArgs DrawTabEventArgs
        {
            get
            {
                this._drawTabEventArgs ??= new DrawTabEventArgs();
                return this._drawTabEventArgs;
            }
        }

        public TabDragForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.Opacity = 0.75d;

            this.LocationChanged += this.TabDragForm_LocationChanged;
            this.LostFocus += this.TabDragForm_LostFocus;
            this.Paint += this.TabDragForm_Paint;
        }

        public void SetLocation(float xOffset, float yOffset)
        {
            var screenPoint = Cursor.Position;
            this.Location = new Point(screenPoint.X - (int)this.GetDrawTabWidthOffset() - (int)xOffset, screenPoint.Y - (int)yOffset);

            var scale = WindowUtil.GetCurrentWindowScale(this);

            var leftBorderRect = ScreenUtil.GetLeftBorderRect(scale);
            if (leftBorderRect.Contains(screenPoint))
            {
                this.TabDropForm.SetBounds(
                    leftBorderRect.X, leftBorderRect.Y, leftBorderRect.Width, leftBorderRect.Height);
                WinApiMembers.SetWindowPos(
                    this.TabDropForm.Handle,
                    WinApiMembers.HWND_TOP,
                    leftBorderRect.X, leftBorderRect.Y, leftBorderRect.Width, leftBorderRect.Height,
                    WinApiMembers.SWP_NOSIZE | WinApiMembers.SWP_NOACTIVATE | WinApiMembers.SWP_SHOWWINDOW);
                return;
            }

            var rightBorderRect = ScreenUtil.GetRightBorderRect(scale);
            if (rightBorderRect.Contains(screenPoint))
            {
                this.TabDropForm.SetBounds(
                    rightBorderRect.X, rightBorderRect.Y, rightBorderRect.Width, rightBorderRect.Height);
                WinApiMembers.SetWindowPos(
                    this.TabDropForm.Handle,
                    WinApiMembers.HWND_TOP,
                    rightBorderRect.X, rightBorderRect.Y, rightBorderRect.Width, rightBorderRect.Height,
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

            if (this._regionImage != null)
            {
                throw new InvalidOperationException("領域のイメージが初期化されていません。");
            }

            var scale = WindowUtil.GetCursorWindowScale();

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

                    var pageRect = this.GetPageRectangle(pageSize);
                    var outlineRect = this.GetOutlineRectangle(pageRect);
                    g.DrawImage(pageCap, pageRect);
                    g.DrawRectangle(Pens.Black, outlineRect);

                    this.TabDrawArea.DrawActiveTab(g, scale);
                }

                this._regionImage = regionImage;
            }

            this.DrawTabEventArgs.Font = FontCacher.GetRegularGdiFont(FontCacher.Size.Medium, scale);
            this.DrawTabEventArgs.TitleColor = TabPalette.TITLE_COLOR;
            this.DrawTabEventArgs.TitleFormatFlags = TabPalette.TITLE_FORMAT_FLAGS;
            this.DrawTabEventArgs.TextRectangle = this.TabDrawArea.GetPageRectangle();
            this.DrawTabEventArgs.IconRectangle = this.TabDrawArea.GetIconRectangle(scale);
            this.DrawTabEventArgs.CloseButtonRectangle = this.TabDrawArea.GetCloseButtonRectangle();

            this._drawTabPageMethod = tab.DrawingTabPage;
            this.Size = this._regionImage.Size;
            this.Region = ImageUtil.GetRegion(this._regionImage, TRANSPARENT_COLOR);
            this._tabSwitch = tab.Owner;
        }

        public void Clear()
        {
            this.TabDropForm.Visible = false;

            this._drawTabPageMethod = null;

            if (this._regionImage != null)
            {
                this._regionImage.Dispose();
                this._regionImage = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._regionImage?.Dispose();
            }

            this._regionImage = null;

            base.Dispose(disposing);
        }

        private void TabDragForm_LocationChanged(object sender, EventArgs e)
        {
            this.Size = this._regionImage.Size;
        }

        private void TabDragForm_LostFocus(object sender, EventArgs e)
        {
            if (this._tabSwitch != null && this.Visible)
            {
                this._tabSwitch.CallEndTabDragOperation();
            }
        }

        private void TabDragForm_Paint(object sender, PaintEventArgs e)
        {
            if (this._drawTabPageMethod == null ||
                this._regionImage == null)
            {
                return;
            }

            e.Graphics.DrawImage(this._regionImage, 0, 0, this._regionImage.Width, this._regionImage.Height);

            this.DrawTabEventArgs.Graphics = e.Graphics;
            this._drawTabPageMethod(this.DrawTabEventArgs);
        }

        private SizeF GetPageSize(Bitmap pageCap)
        {
            var scale = this.GetCaptureWidth() / (float)pageCap.Width;
            var w = pageCap.Width * scale;
            var h = pageCap.Height * scale;
            return new SizeF(w, h);
        }

        private SizeF GetRegionSize(SizeF pageSize)
        {
            var outlineOffset = this.GetOutlineOffset();
            var w = pageSize.Width + outlineOffset * 2;
            var h = this.TabDrawArea.Height + pageSize.Height + outlineOffset;
            return new SizeF(w, h);
        }

        private RectangleF GetOutlineRectangle(RectangleF pageRectangle)
        {
            var outlineOffset = this.GetOutlineOffset();
            var x = 0;
            var y = this.TabDrawArea.Height - outlineOffset;
            var w = pageRectangle.Width - outlineOffset * 2;
            var h = pageRectangle.Height - outlineOffset * 2;
            return new RectangleF(x, y, w, h);
        }

        private RectangleF GetPageRectangle(SizeF pageSize)
        {
            var x = 0;
            var y = this.TabDrawArea.Height;
            var w = pageSize.Width;
            var h = pageSize.Height;
            return new RectangleF(x, y, w, h);
        }

    }
}
