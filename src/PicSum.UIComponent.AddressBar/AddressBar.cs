using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Jobs;
using PicSum.Job.Results;
using PicSum.UIComponent.AddressBar.Properties;
using SWF.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{
    [SupportedOSPlatform("windows")]
    public sealed class AddressBar
        : Control
    {
        #region 定数・列挙

        private const int INNER_OFFSET = 1;

        #endregion

        #region イベント・デリゲート

        public event EventHandler<SelectedDirectoryEventArgs> SelectedDirectory;

        #endregion

        #region インスタンス変数

        private readonly int dropDownItemWidth = Resources.SmallArrowDown.Width;
        private readonly Palette palette = new();
        private readonly OverflowDrawItem overflowItem = new();
        private readonly DirectoryHistoryDrawItem directoryHistoryItem = new();
        private TwoWayJob<AddressInfoGetJob, ValueParameter<string>, AddressInfoGetResult> getAddressInfoJob = null;
        private string currentDirectoryPath = null;
        private readonly List<DrawItemBase> addressItems = [];
        private DrawItemBase mousePointItem = null;
        private DrawItemBase mouseDownItem = null;

        #endregion

        #region パブリックプロパティ

        public Color TextColor
        {
            get
            {
                return this.palette.TextColor;
            }
        }

        public Color MousePointColor
        {
            get
            {
                return this.palette.MousePointColor;
            }
        }

        public Color MouseDownColor
        {
            get
            {
                return this.palette.MouseDownColor;
            }
        }

        public Color OutlineColor
        {
            get
            {
                return this.palette.OutlineColor;
            }
        }

        public Color InnerColor
        {
            get
            {
                return this.palette.InnerColor;
            }
        }

        public StringTrimming TextTrimming
        {
            get
            {
                return this.palette.TextTrimming;
            }
            set
            {
                this.palette.TextTrimming = value;
            }
        }

        public StringAlignment TextAlignment
        {
            get
            {
                return this.palette.TextAlignment;
            }
            set
            {
                this.palette.TextAlignment = value;
            }
        }

        public StringAlignment TextLineAlignment
        {
            get
            {
                return this.palette.TextLineAlignment;
            }
            set
            {
                this.palette.TextLineAlignment = value;
            }
        }

        public StringFormatFlags TextFormatFlags
        {
            get
            {
                return this.palette.TextFormatFlags;
            }
            set
            {
                this.palette.TextFormatFlags = value;
            }
        }

        #endregion

        #region プライベートプロパティ

        private TwoWayJob<AddressInfoGetJob, ValueParameter<string>, AddressInfoGetResult> GetAddressInfoProcess
        {
            get
            {
                if (this.getAddressInfoJob == null)
                {
                    this.getAddressInfoJob = new();
                    this.getAddressInfoJob
                        .Callback(this.GetAddressInfoProcess_Callback)
                        .Catch(ex =>
                        {
                            this.currentDirectoryPath = string.Empty;
                            this.ClearAddressItems();
                            this.Invalidate();
                        })
                        .StartThread();
                }

                return this.getAddressInfoJob;
            }
        }

        #endregion

        #region コンストラクタ

        public AddressBar()
        {
            this.InitializeComponent();
        }

        #endregion

        #region パブリックメソッド

        public void SetAddress(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("アドレスバーに空白は設定できません。", nameof(filePath));
            }

            if (FileUtil.IsFile(filePath))
            {
                if (FileUtil.GetParentDirectoryPath(filePath) == this.currentDirectoryPath)
                {
                    return;
                }
            }
            else
            {
                if (filePath == this.currentDirectoryPath)
                {
                    return;
                }
            }

            var param = new ValueParameter<string>(filePath);

            this.GetAddressInfoProcess.StartJob(param);
        }

        #endregion

        #region 継承メソッド

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.getAddressInfoJob != null)
                {
                    this.getAddressInfoJob.Dispose();
                    this.getAddressInfoJob = null;
                }

                this.overflowItem.Dispose();
                this.directoryHistoryItem.Dispose();

                this.ClearAddressItems();
            }

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
            e.Graphics.FillRectangle(this.palette.OutLineBrush, this.ClientRectangle);
            e.Graphics.FillRectangle(this.palette.InnerBrush, this.GetInnerRectangle());

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
                if (this.mouseDownItem.Equals(drawItem))
                {
                    this.mouseDownItem.OnMouseClick(e);
                }
            }

            base.OnMouseClick(e);
        }

        #endregion

        #region プライベートメソッド

        private void InitializeComponent()
        {
            this.overflowItem.AddressBar = this;
            this.overflowItem.Palette = this.palette;

            this.directoryHistoryItem.AddressBar = this;
            this.directoryHistoryItem.Palette = this.palette;

            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer |
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

        private void SetItemsRectangle()
        {
            this.overflowItem.ClearRectangle();
            this.overflowItem.Items.Clear();

            var innerRect = this.GetInnerRectangle();
            this.directoryHistoryItem.Left = innerRect.Right - this.dropDownItemWidth;
            this.directoryHistoryItem.Top = innerRect.Y;
            this.directoryHistoryItem.Width = this.dropDownItemWidth;
            this.directoryHistoryItem.Height = innerRect.Height;

            var addressRect = this.GetAddressRect();

            if (this.addressItems != null)
            {
                var right = addressRect.Right;

                using (var g = this.CreateGraphics())
                {
                    for (var i = this.addressItems.Count - 1; i > -1; i--)
                    {
                        var drawItem = this.addressItems[i];

                        if (drawItem.GetType() == typeof(DirectoryDrawItem))
                        {
                            drawItem.Width = (int)(g.MeasureString((drawItem as DirectoryDrawItem).Directory.DirectoryName + "__", this.palette.TextFont).Width);
                        }
                        else if (drawItem.GetType() == typeof(SeparatorDrawItem))
                        {
                            drawItem.Width = this.dropDownItemWidth;
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
                                this.overflowItem.Width = this.dropDownItemWidth;
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

        private List<DrawItemBase> CreateAddressItems(AddressInfoGetResult addressInfo)
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
                    Palette = this.palette
                };
                directoryDraw.SelectedDirectory += new(this.DrawItem_SelectedDirectory);
                items.Add(directoryDraw);

                var sepDraw = new SeparatorDrawItem
                {
                    AddressBar = this,
                    Directory = directory,
                    Palette = this.palette
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
                    Palette = this.palette
                };
                directoryDraw.SelectedDirectory += new(this.DrawItem_SelectedDirectory);
                items.Add(directoryDraw);

                var sepDraw = new SeparatorDrawItem
                {
                    AddressBar = this,
                    Directory = directory,
                    Palette = this.palette
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
                    Palette = this.palette,
                };
                directoryDraw.SelectedDirectory += new(this.DrawItem_SelectedDirectory);
                items.Add(directoryDraw);
            }

            return items;
        }

        private void ClearAddressItems()
        {
            foreach (IDisposable item in this.addressItems.Cast<IDisposable>())
            {
                item.Dispose();
            }

            this.addressItems.Clear();
        }

        private Rectangle GetInnerRectangle()
        {
            var x = INNER_OFFSET;
            var y = INNER_OFFSET;
            var w = this.ClientRectangle.Width - INNER_OFFSET * 2;
            var h = this.ClientRectangle.Height - INNER_OFFSET * 2;
            return new Rectangle(x, y, w, h);
        }

        private Rectangle GetAddressRect()
        {
            var x = INNER_OFFSET;
            var y = INNER_OFFSET;
            var w = this.ClientRectangle.Width - INNER_OFFSET * 2 - this.dropDownItemWidth;
            var h = this.ClientRectangle.Height - INNER_OFFSET * 2;
            return new Rectangle(x, y, w, h);
        }

        private bool SetMousePointItem(DrawItemBase newDrawItem)
        {
            var ret = false;
            if (newDrawItem != null)
            {
                if (!newDrawItem.Equals(this.mousePointItem))
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

            this.directoryHistoryItem.IsMousePoint = this.directoryHistoryItem.Equals(this.mousePointItem);
            this.overflowItem.IsMousePoint = this.overflowItem.Equals(this.mousePointItem);
            foreach (var drawItem in this.addressItems)
            {
                drawItem.IsMousePoint = drawItem.Equals(this.mousePointItem);
            }

            return ret;
        }

        private bool SetMouseDownItem(DrawItemBase newDrawItem)
        {
            var ret = false;
            if (newDrawItem != null)
            {
                if (!newDrawItem.Equals(this.mouseDownItem))
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

            this.directoryHistoryItem.IsMouseDown = this.directoryHistoryItem.Equals(this.mouseDownItem);
            this.overflowItem.IsMouseDown = this.overflowItem.Equals(this.mouseDownItem);
            foreach (var drawItem in this.addressItems)
            {
                drawItem.IsMouseDown = drawItem.Equals(this.mouseDownItem);
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

        #endregion

        #region イベント

        private void OnSelectedDirectory(SelectedDirectoryEventArgs e)
        {
            this.SelectedDirectory?.Invoke(this, e);
        }

        private void GetAddressInfoProcess_Callback(AddressInfoGetResult e)
        {
            this.ClearAddressItems();

            this.currentDirectoryPath = e.DirectoryPath;
            this.addressItems.AddRange(this.CreateAddressItems(e));

            this.Invalidate();
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

        #endregion
    }
}
