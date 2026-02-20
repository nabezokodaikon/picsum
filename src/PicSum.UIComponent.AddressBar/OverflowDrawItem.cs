using SkiaSharp;
using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using SWF.Core.ResourceAccessor;
using SWF.UIComponent.FlowList;
using SWF.UIComponent.SKFlowList;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{

    internal sealed partial class OverflowDrawItem
        : DropDownDrawItemBase, IDisposable
    {
#pragma warning disable CA2213 // リソースを保持する変数。
        private readonly Image _mousePointImage = ResourceFiles.SmallArrowLeftIcon.Value;
        private readonly Image _mouseDownImage = ResourceFiles.SmallArrowDownIcon.Value;
#pragma warning restore CA2213

        public OverflowDrawItem()
        {

        }

        public new void Dispose()
        {
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        public override void Draw(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            if (this._disposed)
            {
                return;
            }

            var rect = this.GetRectangle();

            if (base.IsMouseDown || this.IsDropDown)
            {
                g.FillRectangle(FlowListResources.LIGHT_MOUSE_POINT_ITEM_BRUSH, rect);
                g.DrawImage(this._mouseDownImage, this.GetImageDrawRectangle(this._mouseDownImage));
            }
            else if (base.IsMousePoint)
            {
                g.FillRectangle(FlowListResources.LIGHT_MOUSE_POINT_ITEM_BRUSH, rect);
                g.DrawImage(this._mousePointImage, this.GetImageDrawRectangle(this._mousePointImage));
            }
            else
            {
                g.DrawImage(this._mousePointImage, this.GetImageDrawRectangle(this._mousePointImage));
            }
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e, nameof(e));

            if (e.Button == MouseButtons.Left)
            {
                if (this.AddressBar.IsOverflowDropDown)
                {
                    base.DropDownList.Close();
                    this.AddressBar.IsOverflowDropDown = false;
                    return;
                }

                base.DropDownList.BeginUpdate();
                base.DropDownList.ItemHeight = (int)this.GetDropDownItemHeight();

                var scale = WindowUtil.GetCurrentWindowScale(base.AddressBar);
                var font = FontCacher.GetRegularGdiFont(FontCacher.Size.Medium, scale);
                var width = this.GetMinimumDropDownWidth();

                foreach (var directory in base.Items)
                {
                    width = Math.Max(
                        width,
                        TextRenderer.MeasureText(directory.DirectoryName + "______", font).Width + directory.DirectoryIcon.Width);
                }

                if (base.Items.Count > MAXIMUM_SHOW_ITEM_COUNT)
                {
                    width += (int)(base.DropDownList.ItemHeight * 1.5f);
                }

                var screen = Screen.FromControl(base.AddressBar);
                width = (int)(Math.Min(width, screen.Bounds.Width));

                var height = Math.Min(
                    MAXIMUM_SHOW_ITEM_COUNT * base.DropDownList.ItemHeight,
                    base.Items.Count * base.DropDownList.ItemHeight);

                base.DropDownList.Size = new Size((int)width, height);
                base.DropDownList.ClearSelectedItems();
                base.DropDownList.ItemCount = 0;
                base.DropDownList.ItemCount = base.Items.Count;
                base.DropDownList.Show(base.AddressBar, (int)base.Left, (int)base.Bottom);
                base.DropDownList.EndUpdate();

                this.AddressBar.DropDownDirectory = string.Empty;
                this.AddressBar.IsOverflowDropDown = true;
                this.AddressBar.IsHistoryDropDown = false;
            }
        }

        public override void OnMouseClick(MouseEventArgs e)
        {

        }

        protected override void DrawDropDownItem(SKDrawItemEventArgs e)
        {
            var scale = WindowUtil.GetCurrentWindowScale(this.AddressBar);
            var font = FontCacher.GetRegularSKFont(FontCacher.Size.Medium, scale);

            if (e.IsMousePoint)
            {
                e.Canvas.DrawRect(e.ItemRectangle, SKFlowListResources.LIGHT_MOUSE_POINT_FILL_PAINT);
            }

            var item = base.Items[e.ItemIndex];

            if (item.DirectoryIcon != null)
            {
                var iconSize = Math.Min(
                    base.DropDownList.ItemHeight,
                    item.DirectoryIcon.Width * scale);

                var iconPoint = (base.DropDownList.ItemHeight - iconSize) / 2f;

                var iconRect = SKRectI.Create(
                    (int)(e.ItemRectangle.Left + iconPoint),
                    (int)(e.ItemRectangle.Top + iconPoint),
                    (int)iconSize,
                    (int)iconSize);

                item.DirectoryIcon.Draw(e.Canvas, AddressBarResources.ICON_PAINT, iconRect);
            }

            var text = item.DirectoryName;

            var textRect = SKRect.Create(
                e.ItemRectangle.Left + this.DropDownList.ItemHeight,
                e.ItemRectangle.Top,
                e.ItemRectangle.Width - this.DropDownList.ItemHeight,
                e.ItemRectangle.Height);

            SkiaUtil.DrawText(
                e.Canvas,
                SKFlowListResources.LIGHT_TEXT_PAINT,
                font,
                text,
                textRect,
                SKTextAlign.Left,
                1);
        }

        private RectangleF GetImageDrawRectangle(Image img)
        {
            var scale = WindowUtil.GetCurrentWindowScale(this.AddressBar);
            var margin = 16 * scale;
            var w = Math.Min(img.Width * scale, this.Height) - margin;
            var h = Math.Min(img.Height * scale, this.Height) - margin;
            var x = base.X + (base.Width - w) / 2f;
            var y = base.Y + (base.Height - h) / 2f;
            return new RectangleF(x, y, w, h);
        }
    }
}
