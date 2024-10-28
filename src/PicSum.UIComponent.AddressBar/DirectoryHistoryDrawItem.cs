using PicSum.Job.Common;
using PicSum.Job.Entities;
using PicSum.UIComponent.AddressBar.Properties;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{
    [SupportedOSPlatform("windows")]
    internal sealed partial class DirectoryHistoryDrawItem
        : DropDownDrawItemBase, IDisposable
    {
        private bool disposed = false;
        private readonly Image drawImage = Resources.SmallArrowDown;

        public DirectoryHistoryDrawItem()
        {

        }

        public new void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;

            base.Dispose();
        }

        public override void Draw(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            var rect = this.GetRectangle();

            if (base.IsMouseDown || base.IsDropDown)
            {
                g.FillRectangle(base.Palette.MouseDownBrush, rect);
            }
            else if (base.IsMousePoint)
            {
                g.FillRectangle(base.Palette.MousePointBrush, rect);
            }

            g.DrawImage(
                this.drawImage,
                this.GetImageDrawRectangle(this.drawImage),
                new RectangleF(0, 0, this.drawImage.Width, this.drawImage.Height),
                GraphicsUnit.Pixel);
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e, nameof(e));

            if (e.Button == MouseButtons.Left)
            {
                base.Items.Clear();
                var width = Math.Max(MINIMUM_DROPDOWN_WIDHT, base.AddressBar.Width);
                var height = MAXIMUM_SHOW_ITEM_COUNT * base.DropDownList.ItemHeight;
                base.DropDownList.Size = new Size(width, height);
                base.DropDownList.ClearSelectedItems();
                base.DropDownList.ItemCount = 0;
                base.DropDownList.Show(base.AddressBar, 0, base.AddressBar.Height);

                CommonJobs.Instance.DirectoryViewHistoryGetJob
                    .Callback(_ =>
                    {
                        if (this.disposed)
                        {
                            return;
                        }

                        this.DirectoryViewHistoryGetJob_Callback(_);
                    })
                    .StartJob(this.AddressBar);
            }
        }

        public override void OnMouseClick(MouseEventArgs e)
        {

        }

        protected override void DrawDropDownItem(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (e.IsFocus || e.IsMousePoint || e.IsSelected)
            {
                e.Graphics.FillRectangle(base.DropDownList.SelectedItemBrush, e.ItemRectangle);
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

                e.Graphics.DrawImage(
                    item.DirectoryIcon,
                    iconRect,
                    new RectangleF(0, 0, item.DirectoryIcon.Width, item.DirectoryIcon.Height),
                    GraphicsUnit.Pixel);
            }

            var textRect = new RectangleF(e.ItemRectangle.X + base.DropDownList.ItemHeight,
                                          e.ItemRectangle.Y,
                                          e.ItemRectangle.Width - base.DropDownList.ItemHeight,
                                          e.ItemRectangle.Height);

            var dirPath = FileUtil.IsSystemRoot(item.DirectoryPath) ?
                item.DirectoryName : item.DirectoryPath;

            e.Graphics.DrawString(dirPath,
                                  base.Palette.TextFont,
                                  base.DropDownList.ItemTextBrush,
                                  textRect,
                                  base.DropDownList.ItemTextFormat);
        }

        private RectangleF GetImageDrawRectangle(Image img)
        {
            var w = img.Width;
            var h = img.Height;
            var x = (base.X + (base.Width - img.Width) / 2f);
            var y = (base.Y + (base.Height - img.Height) / 2f);
            return new RectangleF(x, y, w, h);
        }

        private void DirectoryViewHistoryGetJob_Callback(ListResult<FileShallowInfoEntity> e)
        {
            var width = 0;

            using (var g = base.DropDownList.CreateGraphics())
            {
                foreach (var info in e)
                {
                    var item = new DirectoryEntity
                    {
                        DirectoryPath = info.FilePath,
                        DirectoryName = info.FileName,
                        DirectoryIcon = info.SmallIcon
                    };
                    base.Items.Add(item);

                    width = Math.Max(width, (int)g.MeasureString(item.DirectoryPath + "________", base.Palette.TextFont).Width);
                }
            }

            if (base.Items.Count > MAXIMUM_SHOW_ITEM_COUNT)
            {
                width += base.DropDownList.ScrollBarWidth;
            }

            width = Math.Max(width, base.AddressBar.Width);

            var height = Math.Min(MAXIMUM_SHOW_ITEM_COUNT * base.DropDownList.ItemHeight,
                                  base.Items.Count * base.DropDownList.ItemHeight);

            base.DropDownList.Size = new Size(width, height);
            base.DropDownList.ItemCount = base.Items.Count;
        }

    }
}
