using PicSum.Job.Common;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class AddressBar
        : Control, ISender
    {
        public event EventHandler<SelectedDirectoryEventArgs> SelectedDirectory;

        private bool disposed = false;
        private readonly OverflowDrawItem overflowItem = new();
        private readonly DirectoryHistoryDrawItem directoryHistoryItem = new();
        private string currentDirectoryPath = null;
        private readonly List<DrawItemBase> addressItems = [];
        private DrawItemBase mousePointItem = null;
        private DrawItemBase mouseDownItem = null;
        private readonly Font defaultFont = new("Yu Gothic UI", 10F);
        private readonly Dictionary<float, Font> regularFontCache = [];
        private readonly Dictionary<float, Font> boldFontCache = [];

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal string DropDownDirectory { get; set; } = string.Empty;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool IsOverflowDropDown { get; set; } = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool IsHistoryDropDown { get; set; } = false;

        public AddressBar()
        {
            this.overflowItem.AddressBar = this;
            this.directoryHistoryItem.AddressBar = this;

            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);
            this.UpdateStyles();

            this.overflowItem.DropDownOpened += new(this.DrawItem_DropDownOpened);
            this.overflowItem.DropDownClosed += new(this.DrawItem_DropDownClosed);
            this.overflowItem.SelectedDirectory += new(this.DrawItem_SelectedDirectory);

            this.directoryHistoryItem.DropDownOpened += new(this.DrawItem_DropDownOpened);
            this.directoryHistoryItem.DropDownClosed += new(this.DrawItem_DropDownClosed);
            this.directoryHistoryItem.SelectedDirectory += new(this.DrawItem_SelectedDirectory);
        }

        public void SetAddress(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this.DropDownDirectory = string.Empty;
            this.IsOverflowDropDown = false;
            this.IsHistoryDropDown = false;

            if (FileUtil.IsFile(filePath))
            {
                var dir = FileUtil.GetParentDirectoryPath(filePath);
                if (dir == this.currentDirectoryPath)
                {
                    return;
                }
                else
                {
                    this.currentDirectoryPath = dir;
                }
            }
            else
            {
                if (filePath == this.currentDirectoryPath)
                {
                    return;
                }
                else
                {
                    this.currentDirectoryPath = filePath;
                }
            }

            var param = new ValueParameter<string>(filePath);

            Instance<JobCaller>.Value.AddressInfoGetJob.Value
                .StartJob(this, param, _ =>
                {
                    if (this.disposed)
                    {
                        return;
                    }

                    this.GetAddressInfoJob_Callback(_);
                });
        }

        internal Font GetRegularFont(float scale)
        {
            if (this.regularFontCache.TryGetValue(scale, out var font))
            {
                return font;
            }

            var newFont = new Font(this.defaultFont.FontFamily, this.defaultFont.Size * scale);
            this.regularFontCache.Add(scale, newFont);
            return newFont;
        }

        internal Font GetBoldFont(float scale)
        {
            if (this.boldFontCache.TryGetValue(scale, out var font))
            {
                return font;
            }

            var newFont = new Font(this.defaultFont.FontFamily, this.defaultFont.Size * scale, FontStyle.Bold);
            this.boldFontCache.Add(scale, newFont);
            return newFont;
        }

        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.overflowItem.Dispose();
                this.directoryHistoryItem.Dispose();

                this.ClearAddressItems();

                foreach (var font in this.regularFontCache.Values)
                {
                    font.Dispose();
                }
                this.regularFontCache.Clear();

                foreach (var font in this.boldFontCache.Values)
                {
                    font.Dispose();
                }
                this.boldFontCache.Clear();

                this.defaultFont.Dispose();
            }

            this.disposed = true;

            base.Dispose(disposing);
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            this.SetItemsRectangle();
            base.OnInvalidated(e);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //base.OnPaintBackground(pevent);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;

            e.Graphics.FillRectangle(Palette.OUT_LINE_BRUSH, this.ClientRectangle);
            e.Graphics.FillRectangle(Palette.INNER_BRUSH, this.GetInnerRectangle());

            this.directoryHistoryItem.Draw(e.Graphics);

            if (this.overflowItem.Items.Count > 0)
            {
                this.overflowItem.Draw(e.Graphics);
            }

            foreach (var drawItem in this.addressItems)
            {
                if (drawItem.Left >= this.overflowItem.Right)
                {
                    drawItem.Draw(e.Graphics);
                }
            }

            base.OnPaint(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.DropDownDirectory = string.Empty;
            this.IsOverflowDropDown = false;
            this.IsHistoryDropDown = false;

            this.SetMouseDownItem(null);
            this.Invalidate();

            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            var drawItem = this.GetItemFromPoint(e.X, e.Y);
            if (drawItem is DirectoryDrawItem &&
                (e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle))
            {
                this.SetMouseDownItem(drawItem);
            }
            else if (drawItem is DropDownDrawItemBase &&
                     e.Button == MouseButtons.Left)
            {
                this.SetMouseDownItem(drawItem);
            }
            else
            {
                this.SetMouseDownItem(null);
            }

            this.Invalidate();

            this.mouseDownItem?.OnMouseDown(e);

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.SetMouseDownItem(null);
            this.Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            var drawItem = this.GetItemFromPoint(e.X, e.Y);
            if (this.SetMousePointItem(drawItem))
            {
                this.Invalidate();
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (this.mouseDownItem != null)
            {
                var drawItem = this.GetItemFromPoint(e.X, e.Y);
                if (this.mouseDownItem == drawItem)
                {
                    this.mouseDownItem.OnMouseClick(e);
                }
            }

            base.OnMouseClick(e);
        }

        private int GetInnerOffset()
        {
            const int INNER_OFFSET = 1;
            var scale = AppConstants.GetCurrentWindowScale(this);
            return (int)(INNER_OFFSET * scale);
        }

        private int GetDropDownItemWidth()
        {
            var dropDownItemWidth = ResourceFiles.SmallArrowDownIcon.Value.Width;
            var scale = AppConstants.GetCurrentWindowScale(this);
            return Math.Min((int)(dropDownItemWidth * scale), this.Height);
        }

        private void SetItemsRectangle()
        {
            this.overflowItem.ClearRectangle();
            this.overflowItem.Items.Clear();

            var innerRect = this.GetInnerRectangle();
            var dropDownItemWidth = this.GetDropDownItemWidth();
            this.directoryHistoryItem.Left = innerRect.Right - dropDownItemWidth;
            this.directoryHistoryItem.Top = innerRect.Y;
            this.directoryHistoryItem.Width = dropDownItemWidth;
            this.directoryHistoryItem.Height = innerRect.Height;

            var addressRect = this.GetAddressRect();

            if (this.addressItems != null)
            {
                var right = addressRect.Right;
                var scale = AppConstants.GetCurrentWindowScale(this);
                using (var g = this.CreateGraphics())
                {
                    var font = this.GetRegularFont(scale);
                    for (var i = this.addressItems.Count - 1; i > -1; i--)
                    {
                        var drawItem = this.addressItems[i];

                        if (drawItem.GetType() == typeof(DirectoryDrawItem))
                        {
                            drawItem.Width = (int)(g.MeasureString((drawItem as DirectoryDrawItem).Directory.DirectoryName + "__", font).Width);
                        }
                        else if (drawItem.GetType() == typeof(SeparatorDrawItem))
                        {
                            drawItem.Width = dropDownItemWidth;
                        }

                        drawItem.Left = right - drawItem.Width;
                        drawItem.Top = addressRect.Y;
                        drawItem.Height = addressRect.Height;

                        right = drawItem.Left;
                    }
                }

                var left = addressRect.Left;
                foreach (var drawItem in this.addressItems)
                {
                    if (drawItem.Left < addressRect.Left)
                    {
                        if (drawItem.GetType() == typeof(DirectoryDrawItem))
                        {
                            var directoryDrawItem = (DirectoryDrawItem)drawItem;
                            this.overflowItem.Items.Add(directoryDrawItem.Directory);
                            if (this.overflowItem.Items.Count == 1)
                            {
                                this.overflowItem.Left = left;
                                this.overflowItem.Top = addressRect.Y;
                                this.overflowItem.Width = dropDownItemWidth;
                                this.overflowItem.Height = addressRect.Height;
                                left = this.overflowItem.Right;
                            }
                        }
                    }
                    else
                    {
                        if (drawItem.GetType() == typeof(SeparatorDrawItem))
                        {
                            var sepDrawItem = (SeparatorDrawItem)drawItem;
                            var count = this.overflowItem.Items
                                .Count(dir => dir.DirectoryPath.Equals(sepDrawItem.Directory.DirectoryPath, StringComparison.Ordinal));
                            if (count > 0)
                            {
                                drawItem.Left = -1;
                                continue;
                            }
                        }

                        drawItem.Left = left;
                        left = drawItem.Right;
                    }
                }
            }
        }

        private DrawItemBase[] CreateAddressItems(AddressInfoGetResult addressInfo)
        {
            var items = new List<DrawItemBase>();

            for (var i = 0; i < addressInfo.DirectoryList.Count - 1; i++)
            {
                var info = addressInfo.DirectoryList[i];
                var directory = new DirectoryEntity
                {
                    DirectoryPath = info.FilePath,
                    DirectoryName = info.FileName,
                    DirectoryIcon = info.SmallIcon
                };

                var directoryDraw = new DirectoryDrawItem
                {
                    AddressBar = this,
                    Directory = directory,
                };
                directoryDraw.SelectedDirectory += new(this.DrawItem_SelectedDirectory);
                items.Add(directoryDraw);

                var sepDraw = new SeparatorDrawItem
                {
                    AddressBar = this,
                    Directory = directory,
                };
                if (i + 1 < addressInfo.DirectoryList.Count)
                {
                    sepDraw.SelectedSubDirectoryPath = addressInfo.DirectoryList[i + 1].FilePath;
                }
                sepDraw.SelectedDirectory += new(this.DrawItem_SelectedDirectory);
                items.Add(sepDraw);
            }

            if (addressInfo.HasSubDirectory)
            {
                var info = addressInfo.DirectoryList.Last();
                var directory = new DirectoryEntity
                {
                    DirectoryPath = info.FilePath,
                    DirectoryName = info.FileName,
                    DirectoryIcon = info.SmallIcon
                };

                var directoryDraw = new DirectoryDrawItem
                {
                    AddressBar = this,
                    Directory = directory,
                };
                directoryDraw.SelectedDirectory += new(this.DrawItem_SelectedDirectory);
                items.Add(directoryDraw);

                var sepDraw = new SeparatorDrawItem
                {
                    AddressBar = this,
                    Directory = directory,
                };
                sepDraw.SelectedDirectory += new(this.DrawItem_SelectedDirectory);
                items.Add(sepDraw);
            }
            else
            {
                var info = addressInfo.DirectoryList.Last();
                var directory = new DirectoryEntity
                {
                    DirectoryPath = info.FilePath,
                    DirectoryName = info.FileName,
                    DirectoryIcon = info.SmallIcon
                };

                var directoryDraw = new DirectoryDrawItem
                {
                    AddressBar = this,
                    Directory = directory,
                };
                directoryDraw.SelectedDirectory += new(this.DrawItem_SelectedDirectory);
                items.Add(directoryDraw);
            }

            return [.. items];
        }

        private void ClearAddressItems()
        {
            foreach (var item in this.addressItems.Cast<IDisposable>())
            {
                item.Dispose();
            }

            this.addressItems.Clear();
        }

        private Rectangle GetInnerRectangle()
        {
            var innerOffset = this.GetInnerOffset();
            var x = innerOffset;
            var y = innerOffset;
            var w = this.ClientRectangle.Width - innerOffset * 2;
            var h = this.ClientRectangle.Height - innerOffset * 2;
            return new Rectangle(x, y, w, h);
        }

        private Rectangle GetAddressRect()
        {
            var innerOffset = this.GetInnerOffset();
            var dropDownItemWidth = this.GetDropDownItemWidth();
            var x = innerOffset;
            var y = innerOffset;
            var w = this.ClientRectangle.Width - innerOffset * 2 - dropDownItemWidth;
            var h = this.ClientRectangle.Height - innerOffset * 2;
            return new Rectangle(x, y, w, h);
        }

        private bool SetMousePointItem(DrawItemBase newDrawItem)
        {
            var ret = false;
            if (newDrawItem != null)
            {
                if (newDrawItem != this.mousePointItem)
                {
                    this.mousePointItem = newDrawItem;
                    ret = true;
                }
            }
            else
            {
                if (this.mousePointItem != null)
                {
                    this.mousePointItem = null;
                    ret = true;
                }
            }

            this.directoryHistoryItem.IsMousePoint = this.directoryHistoryItem == this.mousePointItem;
            this.overflowItem.IsMousePoint = this.overflowItem == this.mousePointItem;
            foreach (var drawItem in this.addressItems)
            {
                drawItem.IsMousePoint = drawItem == this.mousePointItem;
            }

            return ret;
        }

        private bool SetMouseDownItem(DrawItemBase newDrawItem)
        {
            var ret = false;
            if (newDrawItem != null)
            {
                if (newDrawItem != this.mouseDownItem)
                {
                    this.mouseDownItem = newDrawItem;
                    ret = true;
                }
            }
            else
            {
                if (this.mouseDownItem != null)
                {
                    this.mouseDownItem = null;
                    ret = true;
                }
            }

            this.directoryHistoryItem.IsMouseDown = this.directoryHistoryItem == this.mouseDownItem;
            this.overflowItem.IsMouseDown = this.overflowItem == this.mouseDownItem;
            foreach (var drawItem in this.addressItems)
            {
                drawItem.IsMouseDown = drawItem == this.mouseDownItem;
            }

            return ret;
        }

        private DrawItemBase GetItemFromPoint(int x, int y)
        {
            if (this.overflowItem.GetRectangle().Contains(x, y))
            {
                return this.overflowItem;
            }
            else if (this.directoryHistoryItem.GetRectangle().Contains(x, y))
            {
                return this.directoryHistoryItem;
            }
            else if (this.addressItems != null)
            {
                foreach (var drawItem in this.addressItems)
                {
                    if (drawItem.GetRectangle().Contains(x, y))
                    {
                        return drawItem;
                    }
                }
            }

            return null;
        }

        private void OnSelectedDirectory(SelectedDirectoryEventArgs e)
        {
            this.SelectedDirectory?.Invoke(this, e);
        }

        private void GetAddressInfoJob_Callback(AddressInfoGetResult e)
        {
            this.ClearAddressItems();

            this.addressItems.AddRange(this.CreateAddressItems(e));

            this.Invalidate();
            this.Update();
        }

        private void DrawItem_DropDownOpened(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void DrawItem_DropDownClosed(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void DrawItem_SelectedDirectory(object sender, SelectedDirectoryEventArgs e)
        {
            this.OnSelectedDirectory(e);
        }
    }
}
