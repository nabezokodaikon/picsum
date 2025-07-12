using SWF.Core.ImageAccessor;
using SWF.Core.ResourceAccessor;
using SWF.UIComponent.Core;
using System;
using System.Collections.Generic;
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
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed partial class TabDragForm
        : Form
    {
        private static readonly Color TRANSPARENT_COLOR = Color.FromArgb(0, 0, 0, 0);

        private TabDropForm _tabDropForm = null;
        private TabDrawArea _tabDrawArea = null;
        private DrawTabEventArgs _drawTabEventArgs = null;
        private TabSwitch _tabSwitch = null;
        private Bitmap _regionImage = null;
        private Action<DrawTabEventArgs> _drawTabPageMethod = null;
        private readonly Font _defaultFont = Fonts.UI_FONT_10;
        private readonly Dictionary<float, Font> _fontCache = [];

        private Font GetFont(float scale)
        {
            if (this._fontCache.TryGetValue(scale, out var font))
            {
                return font;
            }

            var newFont = new Font(this._defaultFont.FontFamily, this._defaultFont.Size * scale);
            this._fontCache.Add(scale, newFont);
            return newFont;
        }

        private float GetOutlineOffset()
        {
            const float OUTLINE_OFFSET = 0.5f;
            var scale = WindowUtil.GetCursorWindowScale();
            return OUTLINE_OFFSET * scale;
        }

        private float GetDrawTabWidthOffset()
        {
            const float DRAW_TAB_WIDHT_OFFSET = 8;
            var hwnd = WinApiMembers.WindowFromPoint(
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

        private TabDropForm TabDropForm
        {
            get
            {
                this._tabDropForm ??= new TabDropForm();
                return this._tabDropForm;
            }
        }

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
            this.AutoScaleMode = AutoScaleMode.Dpi;

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

            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.Opacity = 0.75d;

            this.FormClosing += this.TabDragForm_Closing;
            this.LocationChanged += this.TabDragForm_LocationChanged;
            this.LostFocus += this.TabDragForm_LostFocus;
            this.Paint += this.TabDragForm_Paint;
        }

        public void SetLocation(float xOffset, float yOffset)
        {
            var screenPoint = Cursor.Position;
            this.Location = new Point(screenPoint.X - (int)this.GetDrawTabWidthOffset() - (int)xOffset, screenPoint.Y - (int)yOffset);

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

            if (this._regionImage != null)
            {
                throw new Exception("領域のイメージが初期化されていません。");
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

            this.DrawTabEventArgs.Font = this.GetFont(scale);
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

        private void TabDragForm_Closing(object sender, FormClosingEventArgs e)
        {
            foreach (var font in this._fontCache.Values)
            {
                font.Dispose();
            }
            this._fontCache.Clear();
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
