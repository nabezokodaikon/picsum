using SWF.Core.Base;
using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed partial class OverflowDrawItem
        : DropDownDrawItemBase, IDisposable
    {

        private readonly Image mousePointImage = ResourceFiles.SmallArrowLeftIcon.Value;
        private readonly Image mouseDownImage = ResourceFiles.SmallArrowDownIcon.Value;

        public OverflowDrawItem()
        {

        }

        public new void Dispose()
        {
            base.Dispose();
        }

        public override void Draw(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            var rect = this.GetRectangle();

            if (base.IsMouseDown || this.IsDropDown)
            {
                g.FillRectangle(Palette.MOUSE_DOWN_BRUSH, rect);
                g.DrawImage(this.mouseDownImage, this.GetImageDrawRectangle(this.mouseDownImage));
            }
            else if (base.IsMousePoint)
            {
                g.FillRectangle(Palette.MOUSE_POINT_BRUSH, rect);
                g.DrawImage(this.mousePointImage, this.GetImageDrawRectangle(this.mousePointImage));
            }
            else
            {
                g.DrawImage(this.mousePointImage, this.GetImageDrawRectangle(this.mousePointImage));
            }
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e, nameof(e));

            if (e.Button == MouseButtons.Left)
            {
                var width = MINIMUM_DROPDOWN_WIDHT;

                using (var g = this.DropDownList.CreateGraphics())
                {
                    foreach (var directory in base.Items)
                    {
                        width = Math.Max(width, (int)g.MeasureString(directory.DirectoryName + "________", Palette.TEXT_FONT).Width);
                    }
                }

                if (base.Items.Count > MAXIMUM_SHOW_ITEM_COUNT)
                {
                    width += base.DropDownList.ScrollBarWidth;
                }

                var height = Math.Min(MAXIMUM_SHOW_ITEM_COUNT * base.DropDownList.ItemHeight,
                                      base.Items.Count * base.DropDownList.ItemHeight);

                base.DropDownList.BeginUpdate();
                base.DropDownList.Size = new Size(width + base.DropDownList.ItemHeight, height);
                base.DropDownList.ClearSelectedItems();
                base.DropDownList.ItemCount = 0;
                base.DropDownList.ItemCount = base.Items.Count;
                base.DropDownList.Show(base.AddressBar, base.Left, base.Bottom);
                base.DropDownList.EndUpdate();
            }
        }

        public override void OnMouseClick(MouseEventArgs e)
        {

        }

        protected override void DrawDropDownItem(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (e.IsFocus || e.IsMousePoint || e.IsSelected)
            {
                e.Graphics.FillRectangle(this.DropDownList.SelectedItemBrush, e.ItemRectangle);
            }

            var item = base.Items[e.ItemIndex];

            if (item.DirectoryIcon != null)
            {
                var iconSize = Math.Min(base.DropDownList.ItemHeight, item.DirectoryIcon.Width);

                var iconPoint = (base.DropDownList.ItemHeight - iconSize) / 2f;

                var iconRect = new RectangleF(e.ItemRectangle.X + iconPoint,
                                              e.ItemRectangle.Y + iconPoint,
                                              iconSize,
                                              iconSize);

                e.Graphics.DrawImage(item.DirectoryIcon, iconRect);
            }

            var text = item.DirectoryName;
            var textSize = TextRenderer.MeasureText(text, Palette.TEXT_FONT);
            var textRect = new RectangleF(e.ItemRectangle.X + this.DropDownList.ItemHeight,
                                          e.ItemRectangle.Y,
                                          e.ItemRectangle.Width - this.DropDownList.ItemHeight,
                                          e.ItemRectangle.Height);

            TextRenderer.DrawText(
                e.Graphics,
                text,
                Palette.TEXT_FONT,
                new Point((int)textRect.Location.X, (int)(textRect.Location.Y + (textRect.Height - textSize.Height) / 2f)),
                Palette.TEXT_BRUSH.Color,
                TextFormatFlags.Top);
        }

        private RectangleF GetImageDrawRectangle(Image img)
        {
            var w = img.Width;
            var h = img.Height;
            var x = base.X + (base.Width - img.Width) / 2f;
            var y = base.Y + (base.Height - img.Height) / 2f;
            return new RectangleF(x, y, w, h);
        }

    }
}
