using PicSum.Job.Common;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using SWF.Core.ResourceAccessor;
using SWF.Core.StringAccessor;
using SWF.UIComponent.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{

    public sealed partial class AddressBar
        : BasePaintingControl, ISender
    {
        public event EventHandler<SelectedDirectoryEventArgs> SelectedDirectory;

        private bool _disposed = false;
        private readonly OverflowDrawItem _overflowItem = new();
        private readonly DirectoryHistoryDrawItem _directoryHistoryItem = new();
        private string _currentDirectoryPath = null;
        private readonly List<DrawItemBase> _addressItems = [];

#pragma warning disable CA2213 // 描画対象を一時保持する。
        private DrawItemBase _mousePointItem = null;
        private DrawItemBase _mouseDownItem = null;
#pragma warning restore CA2213

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal string DropDownDirectory { get; set; } = string.Empty;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool IsOverflowDropDown { get; set; } = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool IsHistoryDropDown { get; set; } = false;

        public AddressBar()
        {
            this._overflowItem.AddressBar = this;
            this._directoryHistoryItem.AddressBar = this;

            this._overflowItem.DropDownOpened += new(this.DrawItem_DropDownOpened);
            this._overflowItem.DropDownClosed += new(this.DrawItem_DropDownClosed);
            this._overflowItem.SelectedDirectory += new(this.DrawItem_SelectedDirectory);

            this._directoryHistoryItem.DropDownOpened += new(this.DrawItem_DropDownOpened);
            this._directoryHistoryItem.DropDownClosed += new(this.DrawItem_DropDownClosed);
            this._directoryHistoryItem.SelectedDirectory += new(this.DrawItem_SelectedDirectory);

            this.LostFocus += this.AddressBar_LostFocus;
            this.MouseLeave += this.AddressBar_MouseLeave;
            this.MouseDown += this.AddressBar_MouseDown;
            this.MouseUp += this.AddressBar_MouseUp;
            this.MouseMove += this.AddressBar_MouseMove;
            this.MouseClick += this.AddressBar_MouseClick;
            this.Paint += this.AddressBar_Paint;
            this.Resize += this.AddressBar_Resize;
        }

        public void SetAddress(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this.DropDownDirectory = string.Empty;
            this.IsOverflowDropDown = false;
            this.IsHistoryDropDown = false;

            if (FileUtil.IsExistsFile(filePath))
            {
                var dir = FileUtil.GetParentDirectoryPath(filePath);
                if (dir == this._currentDirectoryPath)
                {
                    return;
                }
                else
                {
                    this._currentDirectoryPath = dir;
                }
            }
            else
            {
                if (filePath == this._currentDirectoryPath)
                {
                    return;
                }
                else
                {
                    this._currentDirectoryPath = filePath;
                }
            }

            var param = new ValueParameter<string>(filePath);

            Instance<JobCaller>.Value.AddressInfoGetJob.Value
                .StartJob(this, param, _ =>
                {
                    if (this._disposed)
                    {
                        return;
                    }

                    this.GetAddressInfoJob_Callback(_);
                });
        }

        protected override void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this._overflowItem.Dispose();
                this._directoryHistoryItem.Dispose();

                this.ClearAddressItems();
            }

            this._disposed = true;

            base.Dispose(disposing);
        }

        public new void Invalidate(Rectangle rc)
        {
            if (this.IsDisposed)
            {
                return;
            }

            this.SetItemsRectangle();
            base.Invalidate(rc);
        }

        public new void Invalidate()
        {
            this.Invalidate(this.ClientRectangle);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //base.OnPaintBackground(pevent);
        }

        private void AddressBar_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void AddressBar_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.Invalid;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;

            e.Graphics.FillRectangle(Palette.OUT_LINE_BRUSH, this.ClientRectangle);
            e.Graphics.FillRectangle(Palette.INNER_BRUSH, this.GetInnerRectangle());

            this._directoryHistoryItem.Draw(e.Graphics);

            if (this._overflowItem.Items.Count > 0)
            {
                this._overflowItem.Draw(e.Graphics);
            }

            foreach (var drawItem in this._addressItems)
            {
                if (drawItem.Left >= this._overflowItem.Right)
                {
                    drawItem.Draw(e.Graphics);
                }
            }
        }

        private void AddressBar_LostFocus(object sender, EventArgs e)
        {
            this.DropDownDirectory = string.Empty;
            this.IsOverflowDropDown = false;
            this.IsHistoryDropDown = false;

            this.SetMousePointItem(null);
            this.SetMouseDownItem(null);
            this.Invalidate();
        }

        private void AddressBar_MouseLeave(object sender, EventArgs e)
        {
            this.DropDownDirectory = string.Empty;
            this.IsOverflowDropDown = false;
            this.IsHistoryDropDown = false;

            this.SetMousePointItem(null);
            this.SetMouseDownItem(null);
            this.Invalidate();
        }

        private void AddressBar_MouseDown(object sender, MouseEventArgs e)
        {
            this.Focus();
            this.SetMousePointItem(null);

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

            this._mouseDownItem?.OnMouseDown(e);
        }

        private void AddressBar_MouseUp(object sender, MouseEventArgs e)
        {
            this.SetMousePointItem(null);
            this.SetMouseDownItem(null);
            this.Invalidate();
        }

        private void AddressBar_MouseMove(object sender, MouseEventArgs e)
        {
            var drawItem = this.GetItemFromPoint(e.X, e.Y);
            if (this.SetMousePointItem(drawItem))
            {
                this.Invalidate();
            }
        }

        private void AddressBar_MouseClick(object sender, MouseEventArgs e)
        {
            if (this._mouseDownItem != null)
            {
                var drawItem = this.GetItemFromPoint(e.X, e.Y);
                if (this._mouseDownItem == drawItem)
                {
                    this._mouseDownItem.OnMouseClick(e);
                }
            }
        }

        private float GetInnerOffset()
        {
            const int INNER_OFFSET = 1;
            var scale = WindowUtil.GetCurrentWindowScale(this);
            return INNER_OFFSET * scale;
        }

        private float GetDropDownItemWidth()
        {
            var dropDownItemWidth = ResourceFiles.SmallArrowDownIcon.Value.Width;
            var scale = WindowUtil.GetCurrentWindowScale(this);
            return Math.Min(dropDownItemWidth * scale, this.Height);
        }

        private void SetItemsRectangle()
        {
            this._overflowItem.ClearRectangle();
            this._overflowItem.Items.Clear();

            var innerRect = this.GetInnerRectangle();
            var dropDownItemWidth = this.GetDropDownItemWidth();
            this._directoryHistoryItem.Left = innerRect.Right - dropDownItemWidth;
            this._directoryHistoryItem.Top = innerRect.Y;
            this._directoryHistoryItem.Width = dropDownItemWidth;
            this._directoryHistoryItem.Height = innerRect.Height;

            var addressRect = this.GetAddressRect();

            if (this._addressItems != null)
            {
                using (var g = this.CreateGraphics())
                {
                    var scale = WindowUtil.GetCurrentWindowScale(this);
                    var font = FontCacher.GetRegularGdiFont(FontCacher.Size.Medium, scale);
                    var right = addressRect.Right - this._directoryHistoryItem.Width;
                    for (var i = this._addressItems.Count - 1; i > -1; i--)
                    {
                        var drawItem = this._addressItems[i];

                        if (drawItem.GetType() == typeof(DirectoryDrawItem))
                        {
                            drawItem.Width = g.MeasureString((drawItem as DirectoryDrawItem).Directory.DirectoryName + "__", font).Width;
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
                foreach (var drawItem in this._addressItems)
                {
                    if (drawItem.Left < addressRect.Left)
                    {
                        if (drawItem.GetType() == typeof(DirectoryDrawItem))
                        {
                            var directoryDrawItem = (DirectoryDrawItem)drawItem;
                            this._overflowItem.Items.Add(directoryDrawItem.Directory);
                            if (this._overflowItem.Items.Count == 1)
                            {
                                this._overflowItem.Left = left;
                                this._overflowItem.Top = addressRect.Y;
                                this._overflowItem.Width = dropDownItemWidth;
                                this._overflowItem.Height = addressRect.Height;
                                left = this._overflowItem.Right;
                            }
                        }
                    }
                    else
                    {
                        if (drawItem.GetType() == typeof(SeparatorDrawItem))
                        {
                            var sepDrawItem = (SeparatorDrawItem)drawItem;
                            var count = this._overflowItem.Items
                                .Count(dir => StringUtil.CompareFilePath(dir.DirectoryPath, sepDrawItem.Directory.DirectoryPath));
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
            foreach (var item in this._addressItems.Cast<IDisposable>())
            {
                item.Dispose();
            }

            this._addressItems.Clear();
        }

        private RectangleF GetInnerRectangle()
        {
            var innerOffset = this.GetInnerOffset();
            var x = innerOffset;
            var y = innerOffset;
            var w = this.ClientRectangle.Width - innerOffset * 2;
            var h = this.ClientRectangle.Height - innerOffset * 2;
            return new RectangleF(x, y, w, h);
        }

        private RectangleF GetAddressRect()
        {
            var innerOffset = this.GetInnerOffset();
            var dropDownItemWidth = this.GetDropDownItemWidth();
            var x = innerOffset;
            var y = innerOffset;
            var w = this.ClientRectangle.Width - innerOffset * 2 - dropDownItemWidth;
            var h = this.ClientRectangle.Height - innerOffset * 2;
            return new RectangleF(x, y, w, h);
        }

        private bool SetMousePointItem(DrawItemBase newDrawItem)
        {
            var ret = false;
            if (newDrawItem != null)
            {
                if (newDrawItem != this._mousePointItem)
                {
                    this._mousePointItem = newDrawItem;
                    ret = true;
                }
            }
            else
            {
                if (this._mousePointItem != null)
                {
                    this._mousePointItem = null;
                    ret = true;
                }
            }

            this._directoryHistoryItem.IsMousePoint = this._directoryHistoryItem == this._mousePointItem;
            this._overflowItem.IsMousePoint = this._overflowItem == this._mousePointItem;
            foreach (var drawItem in this._addressItems)
            {
                drawItem.IsMousePoint = drawItem == this._mousePointItem;
            }

            return ret;
        }

        private bool SetMouseDownItem(DrawItemBase newDrawItem)
        {
            var ret = false;
            if (newDrawItem != null)
            {
                if (newDrawItem != this._mouseDownItem)
                {
                    this._mouseDownItem = newDrawItem;
                    ret = true;
                }
            }
            else
            {
                if (this._mouseDownItem != null)
                {
                    this._mouseDownItem = null;
                    ret = true;
                }
            }

            this._directoryHistoryItem.IsMouseDown = this._directoryHistoryItem == this._mouseDownItem;
            this._overflowItem.IsMouseDown = this._overflowItem == this._mouseDownItem;
            foreach (var drawItem in this._addressItems)
            {
                drawItem.IsMouseDown = drawItem == this._mouseDownItem;
            }

            return ret;
        }

        private DrawItemBase GetItemFromPoint(float x, float y)
        {
            if (this._overflowItem.GetRectangle().Contains(x, y))
            {
                return this._overflowItem;
            }
            else if (this._directoryHistoryItem.GetRectangle().Contains(x, y))
            {
                return this._directoryHistoryItem;
            }
            else if (this._addressItems != null)
            {
                foreach (var drawItem in this._addressItems)
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

            this._addressItems.AddRange(this.CreateAddressItems(e));

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
