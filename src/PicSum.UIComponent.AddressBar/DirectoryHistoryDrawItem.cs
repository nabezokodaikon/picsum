using PicSum.Job.Common;
using PicSum.Job.Entities;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed partial class DirectoryHistoryDrawItem
        : DropDownDrawItemBase, IDisposable
    {
        private bool disposed = false;
        private readonly Image drawImage = ResourceFiles.SmallArrowDownIcon.Value;

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
                g.FillRectangle(Palette.MOUSE_DOWN_BRUSH, rect);
            }
            else if (base.IsMousePoint)
            {
                g.FillRectangle(Palette.MOUSE_POINT_BRUSH, rect);
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
                Instance<JobCaller>.Value.DirectoryViewHistoryGetJob.Value
                    .StartJob(this.AddressBar, _ =>
                    {
                        if (this.disposed)
                        {
                            return;
                        }

                        this.DirectoryViewHistoryGetJob_Callback(_);
                    });
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

            var srcText = FileUtil.IsSystemRoot(item.DirectoryPath) ?
                item.DirectoryName : item.DirectoryPath;

            var srcTextSize = TextRenderer.MeasureText(srcText, Palette.TEXT_FONT);

            var destText = srcText;
            var destTextSize = srcTextSize;
            var itemWidth = e.ItemRectangle.Width - base.DropDownList.ItemHeight;
            while (destTextSize.Width > itemWidth)
            {
                destText = destText.Substring(0, destText.Length - 1);
                destTextSize = TextRenderer.MeasureText($"{destText}...", Palette.TEXT_FONT);
            }
            destText = srcText == destText ? srcText : $"{destText}...";

            var textRect = new Rectangle(e.ItemRectangle.X + base.DropDownList.ItemHeight,
                                         e.ItemRectangle.Y + (int)((e.ItemRectangle.Height - destTextSize.Height) / 2f),
                                         e.ItemRectangle.Width - base.DropDownList.ItemHeight,
                                         e.ItemRectangle.Height);

            TextRenderer.DrawText(
                e.Graphics,
                destText,
                Palette.TEXT_FONT,
                textRect.Location,
                base.DropDownList.ItemTextBrush.Color,
                TextFormatFlags.Top);
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
            base.DropDownList.BeginUpdate();
            base.Items.Clear();
            base.DropDownList.ClearSelectedItems();
            base.DropDownList.ItemCount = 0;

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

                    width = Math.Max(width, (int)g.MeasureString(item.DirectoryPath + "________", Palette.TEXT_FONT).Width);
                }
            }

            if (base.Items.Count > MAXIMUM_SHOW_ITEM_COUNT)
            {
                width += base.DropDownList.ScrollBarWidth;
            }

            width = Math.Min(
                Math.Max(width, base.AddressBar.Width),
                Screen.FromControl(this.AddressBar).Bounds.Width);

            var height = Math.Min(MAXIMUM_SHOW_ITEM_COUNT * base.DropDownList.ItemHeight,
                                  base.Items.Count * base.DropDownList.ItemHeight);

            base.DropDownList.Size = new Size(width, height);
            base.DropDownList.ItemCount = base.Items.Count;
            base.DropDownList.Show(base.AddressBar, 0, base.AddressBar.Height);
            base.DropDownList.EndUpdate();
        }

    }
}
