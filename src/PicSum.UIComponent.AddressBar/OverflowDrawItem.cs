using System;
using System.Drawing;
using System.Windows.Forms;
using PicSum.UIComponent.AddressBar.Properties;
using PicSum.UIComponent.Common;

namespace PicSum.UIComponent.AddressBar
{
    class OverflowDrawItem : DropDownDrawItemBase, IDisposable
    {
        #region インスタンス変数

        private Image _mousePointImage = Resources.SmallArrowLeft;
        private Image _mouseDownImage = Resources.SmallArrowDown;

        #endregion

        #region コンストラクタ

        public OverflowDrawItem()
        {
            initializeComponent();
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
                throw new ArgumentNullException("g");
            }

            Rectangle rect = GetRectangle();

            if (base.IsMouseDown || IsDropDown)
            {
                g.FillRectangle(base.Palette.MouseDownBrush, rect);
                g.DrawImage(_mouseDownImage, getImageDrawRectangle(_mouseDownImage));
            }
            else if (base.IsMousePoint)
            {
                g.FillRectangle(base.Palette.MousePointBrush, rect);
                g.DrawImage(_mousePointImage, getImageDrawRectangle(_mousePointImage));
            }
            else
            {
                g.DrawImage(_mousePointImage, getImageDrawRectangle(_mousePointImage));
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
                int width = MINIMUM_DROPDOWN_WIDHT;

                using (Graphics g = DropDownList.CreateGraphics())
                {
                    foreach (FolderEntity folder in base.Items)
                    {
                        width = Math.Max(width, (int)g.MeasureString(folder.FolderName + "________", base.Palette.TextFont).Width);
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
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
        }

        protected override void drawDropDownItem(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (e.IsFocus || e.IsMousePoint || e.IsSelected)
            {
                e.Graphics.FillRectangle(DropDownList.SelectedItemBrush, e.ItemRectangle);
            }

            FolderEntity item = base.Items[e.ItemIndex];

            if (item.FolderIcon != null)
            {
                int iconSize = Math.Min(base.DropDownList.ItemHeight, item.FolderIcon.Width);

                int iconPoint = (int)((base.DropDownList.ItemHeight - iconSize) / 2);

                Rectangle iconRect = new Rectangle(e.ItemRectangle.X + iconPoint,
                                                   e.ItemRectangle.Y + iconPoint,
                                                   iconSize,
                                                   iconSize);

                e.Graphics.DrawImage(item.FolderIcon, iconRect);
            }

            Rectangle textRect = new Rectangle(e.ItemRectangle.X + DropDownList.ItemHeight,
                                               e.ItemRectangle.Y,
                                               e.ItemRectangle.Width - DropDownList.ItemHeight,
                                               e.ItemRectangle.Height);

            e.Graphics.DrawString(item.FolderName,
                                  base.Palette.TextFont,
                                  base.DropDownList.ItemTextBrush,
                                  textRect,
                                  base.DropDownList.ItemTextFormat);
        }

        private void initializeComponent()
        {

        }

        private Rectangle getImageDrawRectangle(Image img)
        {
            int w = img.Width;
            int h = img.Height;
            int x = (int)(base.X + (base.Width - img.Width) / 2d);
            int y = (int)(base.Y + (base.Height - img.Height) / 2d);
            return new Rectangle(x, y, w, h);
        }

        #endregion
    }
}
