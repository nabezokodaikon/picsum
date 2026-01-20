using PicSum.Job.Common;
using PicSum.Job.Entities;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using SWF.Core.ResourceAccessor;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{

    internal sealed partial class DirectoryHistoryDrawItem
        : DropDownDrawItemBase, IDisposable
    {
#pragma warning disable CA2213 // リソースを保持する。
        private readonly Image _drawImage = ResourceFiles.SmallArrowDownIcon.Value;
#pragma warning restore CA2213

        public DirectoryHistoryDrawItem()
        {

        }

        public new void Dispose()
        {
            if (this._disposed)
            {
                return;
            }

            base.Dispose();
            this._disposed = true;

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

            if (base.IsMouseDown || base.IsDropDown)
            {
                g.FillRectangle(Palette.MOUSE_DOWN_BRUSH, rect);
            }
            else if (base.IsMousePoint)
            {
                g.FillRectangle(Palette.MOUSE_POINT_BRUSH, rect);
            }

            g.DrawImage(
                this._drawImage,
                this.GetImageDrawRectangle(this._drawImage),
                new RectangleF(0, 0, this._drawImage.Width, this._drawImage.Height),
                GraphicsUnit.Pixel);
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e, nameof(e));

            if (e.Button == MouseButtons.Left)
            {
                if (this.AddressBar.IsHistoryDropDown)
                {
                    base.DropDownList.Close();
                    this.AddressBar.IsHistoryDropDown = false;
                    return;
                }

                Instance<JobCaller>.Value.EnqueueDirectoryViewHistoryGetJob(this.AddressBar, _ =>
                    {
                        if (this._disposed)
                        {
                            return;
                        }

                        this.DirectoryViewHistoryGetJob_Callback(_);
                        this.AddressBar.DropDownDirectory = string.Empty;
                        this.AddressBar.IsOverflowDropDown = false;
                        this.AddressBar.IsHistoryDropDown = true;
                    });
            }
        }

        public override void OnMouseClick(MouseEventArgs e)
        {

        }

        protected override void DrawDropDownItem(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            var scale = WindowUtil.GetCurrentWindowScale(this.AddressBar);

            if (e.IsMousePoint)
            {
                e.Graphics.FillRectangle(base.DropDownList.MousePointItemBrush, e.ItemRectangle);
            }

            var item = base.Items[e.ItemIndex];

            if (item.DirectoryIcon != null)
            {
                var iconSize = Math.Min(base.DropDownList.ItemHeight, item.DirectoryIcon.Width * scale);

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

            var font = Fonts.GetRegularFont(Fonts.Size.Medium, scale);
            var srcTextSize = TextRenderer.MeasureText(srcText, font);

            var destText = srcText;
            var destTextSize = srcTextSize;
            var itemWidth = e.ItemRectangle.Width - base.DropDownList.ItemHeight;
            while (destTextSize.Width > itemWidth)
            {
                destText = destText[..^1];
                destTextSize = TextRenderer.MeasureText($"{destText}...", font);
            }
            destText = srcText == destText ? srcText : $"{destText}...";

            var textRect = new Rectangle(e.ItemRectangle.X + base.DropDownList.ItemHeight,
                                         e.ItemRectangle.Y + (int)((e.ItemRectangle.Height - destTextSize.Height) / 2f),
                                         e.ItemRectangle.Width - base.DropDownList.ItemHeight,
                                         e.ItemRectangle.Height);

            TextRenderer.DrawText(
                e.Graphics,
                destText,
                font,
                textRect.Location,
                base.DropDownList.ItemTextBrush.Color,
                TextFormatFlags.Top);
        }

        private RectangleF GetImageDrawRectangle(Image img)
        {
            var scale = WindowUtil.GetCurrentWindowScale(this.AddressBar);
            var margin = 16 * scale;
            var w = Math.Min(img.Width * scale, this.Height) - margin;
            var h = Math.Min(img.Height * scale, this.Height) - margin;
            var x = (base.X + (base.Width - w) / 2f);
            var y = (base.Y + (base.Height - h) / 2f);
            return new RectangleF(x, y, w, h);
        }

        private void DirectoryViewHistoryGetJob_Callback(ListResult<FileShallowInfoEntity> e)
        {
            base.DropDownList.BeginUpdate();
            base.Items.Clear();
            base.DropDownList.ClearSelectedItems();
            base.DropDownList.ItemCount = 0;
            base.DropDownList.ItemHeight = (int)this.GetDropDownItemHeight();

            var width = 0f;

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

                    var scale = WindowUtil.GetCurrentWindowScale(base.DropDownList);
                    var font = Fonts.GetRegularFont(Fonts.Size.Medium, scale);
                    width = Math.Max(width, g.MeasureString(item.DirectoryPath + "________", font).Width);
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

            base.DropDownList.Size = new Size((int)width, height);
            base.DropDownList.ItemCount = base.Items.Count;
            base.DropDownList.Show(base.AddressBar, 0, base.AddressBar.Height);
            base.DropDownList.EndUpdate();
        }

    }
}
