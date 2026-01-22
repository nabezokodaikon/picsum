using PicSum.Job.Common;
using PicSum.Job.Entities;
using SWF.Core.Base;
using SWF.Core.Job;
using SWF.Core.ResourceAccessor;
using SWF.Core.StringAccessor;
using SWF.UIComponent.FlowList;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{

    internal sealed partial class SeparatorDrawItem
        : DropDownDrawItemBase, IDisposable
    {
#pragma warning disable CA2213 // リソースを保持する変数。
        private readonly Image _mousePointImage = ResourceFiles.SmallArrowRightIcon.Value;
        private readonly Image _mouseDownImage = ResourceFiles.SmallArrowDownIcon.Value;
#pragma warning restore CA2213

        public DirectoryEntity Directory { get; set; }
        public string SelectedSubDirectoryPath { get; set; }

        public SeparatorDrawItem()
        {

        }

        public new void Dispose()
        {
            if (this._disposed)
            {
                return;
            }

            this._disposed = true;

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

            if (base.IsMouseDown || base.IsDropDown)
            {
                g.FillRectangle(FlowList.LIGHT_MOUSE_POINT_ITEM_BRUSH, rect);
                g.DrawImage(this._mouseDownImage, this.GetImageDrawRectangle(this._mouseDownImage));
            }
            else if (base.IsMousePoint)
            {
                g.FillRectangle(FlowList.LIGHT_MOUSE_POINT_ITEM_BRUSH, rect);
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
                if (!string.IsNullOrEmpty(this.AddressBar.DropDownDirectory)
                    && this.AddressBar.DropDownDirectory == this.Directory.DirectoryPath)
                {
                    base.DropDownList.Close();
                    this.AddressBar.DropDownDirectory = string.Empty;
                    return;
                }

                base.DropDownList.Show(base.AddressBar, (int)this.Left, (int)this.Bottom);
                var param = new ValueParameter<string>(this.Directory.DirectoryPath);
                Instance<JobCaller>.Value.EnqueueSubDirectoriesGetJob(this.AddressBar, param, _ =>
                    {
                        if (this._disposed)
                        {
                            return;
                        }

                        this.GetSubDirectoryJob_Callback(_);
                    });
                this.AddressBar.DropDownDirectory = this.Directory.DirectoryPath;
                this.AddressBar.IsOverflowDropDown = false;
                this.AddressBar.IsHistoryDropDown = false;
            }
        }

        public override void OnMouseClick(MouseEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e, nameof(e));
        }

        protected override void DrawDropDownItem(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            var scale = WindowUtil.GetCurrentWindowScale(this.AddressBar);

            if (e.IsMousePoint && e.IsSelected)
            {
                e.Graphics.FillRectangle(FlowList.LIGHT_SELECTED_ITEM_BRUSH, e.ItemRectangle);
            }
            else if (e.IsMousePoint)
            {
                e.Graphics.FillRectangle(FlowList.LIGHT_MOUSE_POINT_ITEM_BRUSH, e.ItemRectangle);
            }
            else if (e.IsSelected)
            {
                e.Graphics.FillRectangle(FlowList.LIGHT_SELECTED_ITEM_BRUSH, e.ItemRectangle);
            }

            var item = base.Items[e.ItemIndex];

            if (item.DirectoryIcon != null)
            {
                var iconSize = Math.Min(
                    base.DropDownList.ItemHeight,
                    item.DirectoryIcon.Width * scale);

                var iconPoint = (base.DropDownList.ItemHeight - iconSize) / 2f;

                var iconRect = new RectangleF(
                    e.ItemRectangle.X + iconPoint,
                    e.ItemRectangle.Y + iconPoint,
                    iconSize,
                    iconSize);

                e.Graphics.DrawImage(item.DirectoryIcon, iconRect);
            }

            var textFont = this.GetFont(item.DirectoryPath, scale);
            var text = item.DirectoryName;
            var textRect = new Rectangle(
                e.ItemRectangle.X + base.DropDownList.ItemHeight,
                e.ItemRectangle.Y,
                e.ItemRectangle.Width - base.DropDownList.ItemHeight,
                e.ItemRectangle.Height);

            var flags = TextFormatFlags.Left |
                        TextFormatFlags.VerticalCenter |
                        TextFormatFlags.EndEllipsis;

            TextRenderer.DrawText(
                e.Graphics,
                text,
                textFont,
                textRect,
                FlowList.LIGHT_ITEM_TEXT_COLOR,
                flags);
        }

        private RectangleF GetImageDrawRectangle(Image img)
        {
            var scale = WindowUtil.GetCurrentWindowScale(this.AddressBar);
            var margin = 16 * scale;
            var w = Math.Min(img.Width * scale, this.Height) - margin;
            var h = Math.Min(img.Height * scale, this.Height) - margin;
            var x = base.X + (base.Width - w) / 2f;
            var y = base.Y + (base.Height - w) / 2f;
            return new RectangleF(x, y, w, h);
        }

        private Font GetFont(string directoryPath, float scale)
        {
            if (StringUtil.CompareFilePath(directoryPath, this.SelectedSubDirectoryPath))
            {
                return Fonts.GetBoldFont(Fonts.Size.Medium, scale);
            }
            else
            {
                return Fonts.GetRegularFont(Fonts.Size.Medium, scale);
            }
        }

        private void GetSubDirectoryJob_Callback(ListResult<FileShallowInfoEntity> e)
        {
            base.DropDownList.BeginUpdate();
            base.Items.Clear();
            base.DropDownList.ClearSelectedItems();
            base.DropDownList.ItemCount = 0;
            base.DropDownList.ItemHeight = (int)this.GetDropDownItemHeight();

            var scale = WindowUtil.GetCurrentWindowScale(base.AddressBar);
            var font = Fonts.GetRegularFont(Fonts.Size.Medium, scale);
            var width = this.GetMinimumDropDownWidth();

            foreach (var info in e)
            {
                var item = new DirectoryEntity
                {
                    DirectoryPath = info.FilePath,
                    DirectoryName = info.FileName,
                    DirectoryIcon = info.SmallIcon
                };
                base.Items.Add(item);

                width = Math.Max(
                    width,
                    TextRenderer.MeasureText(item.DirectoryName + "______", font).Width + item.DirectoryIcon.Width);
            }

            if (base.Items.Count > MAXIMUM_SHOW_ITEM_COUNT)
            {
                width += base.DropDownList.ItemHeight * 1.5f;
            }

            var screen = Screen.FromControl(base.AddressBar);
            width = Math.Min(width, screen.Bounds.Width);

            var height = Math.Min(
                MAXIMUM_SHOW_ITEM_COUNT * base.DropDownList.ItemHeight,
                base.Items.Count * base.DropDownList.ItemHeight);

            base.DropDownList.Size = new Size((int)width, height);
            base.DropDownList.ItemCount = base.Items.Count;

            var selectedItem = base.Items
                .FirstOrDefault(item => StringUtil.CompareFilePath(item.DirectoryPath, this.SelectedSubDirectoryPath));
            if (selectedItem != null)
            {
                base.DropDownList.SelectItem(base.Items.IndexOf(selectedItem));
            }

            base.DropDownList.EndUpdate();
        }
    }
}
