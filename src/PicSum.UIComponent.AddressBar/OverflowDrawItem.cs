using PicSum.UIComponent.AddressBar.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{
    internal sealed class OverflowDrawItem
        : DropDownDrawItemBase, IDisposable
    {
        #region インスタンス変数

        private Image mousePointImage = Resources.SmallArrowLeft;
        private Image mouseDownImage = Resources.SmallArrowDown;

        #endregion

        #region コンストラクタ

        public OverflowDrawItem()
        {

        }

        #endregion

        #region メソッド

        public new void Dispose()
        {
            base.Dispose();
        }

        public override void Draw(Graphics g)
        {
            if (g == null)
            {
                throw new ArgumentNullException(nameof(g));
            }

            var rect = this.GetRectangle();

            if (base.IsMouseDown || this.IsDropDown)
            {
                g.FillRectangle(base.Palette.MouseDownBrush, rect);
                g.DrawImage(this.mouseDownImage, this.GetImageDrawRectangle(this.mouseDownImage));
            }
            else if (base.IsMousePoint)
            {
                g.FillRectangle(base.Palette.MousePointBrush, rect);
                g.DrawImage(this.mousePointImage, this.GetImageDrawRectangle(this.mousePointImage));
            }
            else
            {
                g.DrawImage(this.mousePointImage, this.GetImageDrawRectangle(this.mousePointImage));
            }
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            if (e.Button == MouseButtons.Left)
            {
                var width = MINIMUM_DROPDOWN_WIDHT;

                using (var g = this.DropDownList.CreateGraphics())
                {
                    foreach (var directory in base.Items)
                    {
                        width = Math.Max(width, (int)g.MeasureString(directory.DirectoryName + "________", base.Palette.TextFont).Width);
                    }
                }

                if (base.Items.Count > MAXIMUM_SHOW_ITEM_COUNT)
                {
                    width += base.DropDownList.ScrollBarWidth;
                }

                int height = Math.Min(MAXIMUM_SHOW_ITEM_COUNT * base.DropDownList.ItemHeight,
                                      base.Items.Count * base.DropDownList.ItemHeight);

                base.DropDownList.Size = new Size(width + base.DropDownList.ItemHeight, height);
                base.DropDownList.ClearSelectedItems();
                base.DropDownList.ItemCount = 0;
                base.DropDownList.ItemCount = base.Items.Count;
                base.DropDownList.Show(base.AddressBar, base.Left, base.Bottom);
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

                var iconPoint = (int)((base.DropDownList.ItemHeight - iconSize) / 2);

                var iconRect = new Rectangle(e.ItemRectangle.X + iconPoint,
                                             e.ItemRectangle.Y + iconPoint,
                                             iconSize,
                                             iconSize);

                e.Graphics.DrawImage(item.DirectoryIcon, iconRect);
            }

            var textRect = new Rectangle(e.ItemRectangle.X + this.DropDownList.ItemHeight,
                                         e.ItemRectangle.Y,
                                         e.ItemRectangle.Width - this.DropDownList.ItemHeight,
                                         e.ItemRectangle.Height);

            e.Graphics.DrawString(item.DirectoryName,
                                  base.Palette.TextFont,
                                  base.DropDownList.ItemTextBrush,
                                  textRect,
                                  base.DropDownList.ItemTextFormat);
        }

        private Rectangle GetImageDrawRectangle(Image img)
        {
            var w = img.Width;
            var h = img.Height;
            var x = (int)(base.X + (base.Width - img.Width) / 2d);
            var y = (int)(base.Y + (base.Height - img.Height) / 2d);
            return new Rectangle(x, y, w, h);
        }

        #endregion
    }
}
