using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PicSum.UIComponent.AddressBar.Properties;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.Core.Base.Conf;
using SWF.Common;
using PicSum.Task.Result;

namespace PicSum.UIComponent.AddressBar
{
    public class AddressBar : Control
    {
        #region 定数・列挙

        private const int INNER_OFFSET = 1;

        #endregion

        #region イベント・デリゲート

        public event EventHandler<SelectedDirectoryEventArgs> SelectedDirectory;

        #endregion

        #region インスタンス変数

        private readonly int _dropDownItemWidth = Resources.SmallArrowDown.Width;
        private readonly Palette _palette = new Palette();
        private readonly OverflowDrawItem _overflowItem = new OverflowDrawItem();
        private readonly DirectoryHistoryDrawItem _directoryHistoryItem = new DirectoryHistoryDrawItem();
        private IContainer _components = null;
        private TwoWayProcess<GetAddressInfoAsyncFacade, SingleValueEntity<string>, GetAddressInfoResult> _getAddressInfoProcess = null;
        private string _directoryPath = null;
        private readonly List<DrawItemBase> _addressItems = new List<DrawItemBase>();
        private DrawItemBase _mousePointItem = null;
        private DrawItemBase _mouseDownItem = null;

        #endregion

        #region パブリックプロパティ

        public Color TextColor
        {
            get
            {
                return _palette.TextColor;
            }
            set
            {
                _palette.TextColor = value;
            }
        }

        public Color MousePointColor
        {
            get
            {
                return _palette.MousePointColor;
            }
            set
            {
                _palette.MousePointColor = value;
            }
        }

        public Color MouseDownColor
        {
            get
            {
                return _palette.MouseDownColor;
            }
            set
            {
                _palette.MouseDownColor = value;
            }
        }

        public Color OutlineColor
        {
            get
            {
                return _palette.OutlineColor;
            }
            set
            {
                _palette.OutlineColor = value;
            }
        }

        public Color InnerColor
        {
            get
            {
                return _palette.InnerColor;
            }
            set
            {
                _palette.InnerColor = value;
            }
        }

        public StringTrimming TextTrimming
        {
            get
            {
                return _palette.TextTrimming;
            }
            set
            {
                _palette.TextTrimming = value;
            }
        }

        public StringAlignment TextAlignment
        {
            get
            {
                return _palette.TextAlignment;
            }
            set
            {
                _palette.TextAlignment = value;
            }
        }

        public StringAlignment TextLineAlignment
        {
            get
            {
                return _palette.TextLineAlignment;
            }
            set
            {
                _palette.TextLineAlignment = value;
            }
        }

        public StringFormatFlags TextFormatFlags
        {
            get
            {
                return _palette.TextFormatFlags;
            }
            set
            {
                _palette.TextFormatFlags = value;
            }
        }

        #endregion

        #region 継承プロパティ

        #endregion

        #region プライベートプロパティ

        private IContainer components
        {
            get
            {
                if (_components == null)
                {
                    _components = new Container();
                }

                return _components;
            }
        }

        private TwoWayProcess<GetAddressInfoAsyncFacade, SingleValueEntity<string>, GetAddressInfoResult> getAddressInfoProcess
        {
            get
            {
                if (_getAddressInfoProcess == null)
                {
                    _getAddressInfoProcess = TaskManager.CreateTwoWayProcess<GetAddressInfoAsyncFacade, SingleValueEntity<string>, GetAddressInfoResult>(components);
                    getAddressInfoProcess.Callback += new AsyncTaskCallbackEventHandler<GetAddressInfoResult>(getAddressInfoProcess_Callback);
                }

                return _getAddressInfoProcess;
            }
        }

        private DirectoryDrawItem mousePointDirectoryItem
        {
            get
            {
                if (_mousePointItem != null &&
                    _mousePointItem.GetType() == typeof(DirectoryDrawItem))
                {
                    return (DirectoryDrawItem)_mousePointItem;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region コンストラクタ

        public AddressBar()
        {
            initializeComponent();
        }

        #endregion

        #region パブリックメソッド

        public void SetAddress()
        {
            SetAddress(string.Empty);
        }

        public void SetAddress(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            getAddressInfoProcess.Cancel();

            SingleValueEntity<string> param = new SingleValueEntity<string>();
            param.Value = filePath;
            getAddressInfoProcess.Execute(this, param);
        }

        #endregion

        #region 継承メソッド

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_components != null)
                {
                    _components.Dispose();
                }

                _overflowItem.Dispose();
                _directoryHistoryItem.Dispose();

                clearAddressItems();
            }

            base.Dispose(disposing);
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            setItemsRectangle();
            base.OnInvalidated(e);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //base.OnPaintBackground(pevent);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(_palette.OutLineBrush, this.ClientRectangle);
            e.Graphics.FillRectangle(_palette.InnerBrush, getInnerRectangle());

            _directoryHistoryItem.Draw(e.Graphics);

            if (_overflowItem.Items.Count > 0)
            {
                _overflowItem.Draw(e.Graphics);
            }

            foreach (DrawItemBase drawItem in _addressItems)
            {
                if (drawItem.Left >= _overflowItem.Right)
                {
                    drawItem.Draw(e.Graphics);
                }
            }

            base.OnPaint(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            setMouseDownItem(null);
            this.Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            DrawItemBase drawItem = getItemFromPoint(e.X, e.Y);
            if (drawItem is DirectoryDrawItem &&
                (e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle))
            {
                setMouseDownItem(drawItem);
            }
            else if (drawItem is DropDownDrawItemBase &&
                     e.Button == MouseButtons.Left)
            {
                setMouseDownItem(drawItem);
            }
            else
            {
                setMouseDownItem(null);
            }

            this.Invalidate();

            if (_mouseDownItem != null)
            {
                _mouseDownItem.OnMouseDown(e);
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            setMouseDownItem(null);
            this.Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            DrawItemBase drawItem = getItemFromPoint(e.X, e.Y);
            if (setMousePointItem(drawItem))
            {
                this.Invalidate();
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (_mouseDownItem != null)
            {
                DrawItemBase drawItem = getItemFromPoint(e.X, e.Y);
                if (_mouseDownItem.Equals(drawItem))
                {
                    _mouseDownItem.OnMouseClick(e);
                }
            }

            base.OnMouseClick(e);
        }

        protected virtual void OnSelectedDirectory(SelectedDirectoryEventArgs e)
        {
            if (SelectedDirectory != null)
            {
                SelectedDirectory(this, e);
            }
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            _overflowItem.AddressBar = this;
            _overflowItem.Palette = _palette;

            _directoryHistoryItem.AddressBar = this;
            _directoryHistoryItem.Palette = _palette;

            this.SetStyle(ControlStyles.DoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.ResizeRedraw, true);

            _overflowItem.DropDownOpened += new EventHandler(drawItem_DropDownOpened);
            _overflowItem.DropDownClosed += new EventHandler(drawItem_DropDownClosed);
            _overflowItem.SelectedDirectory += new EventHandler<SelectedDirectoryEventArgs>(drawItem_SelectedDirectory);

            _directoryHistoryItem.DropDownOpened += new EventHandler(drawItem_DropDownOpened);
            _directoryHistoryItem.DropDownClosed += new EventHandler(drawItem_DropDownClosed);
            _directoryHistoryItem.SelectedDirectory += new EventHandler<SelectedDirectoryEventArgs>(drawItem_SelectedDirectory);
        }

        private void setItemsRectangle()
        {
            _overflowItem.ClearRectangle();
            _overflowItem.Items.Clear();

            Rectangle innerRect = getInnerRectangle();
            _directoryHistoryItem.Left = innerRect.Right - _dropDownItemWidth;
            _directoryHistoryItem.Top = innerRect.Y;
            _directoryHistoryItem.Width = _dropDownItemWidth;
            _directoryHistoryItem.Height = innerRect.Height;

            Rectangle addressRect = getAddressRect();

            if (_addressItems != null)
            {
                int right = addressRect.Right;

                using (Graphics g = this.CreateGraphics())
                {
                    for (int i = _addressItems.Count - 1; i > -1; i--)
                    {
                        DrawItemBase drawItem = _addressItems[i];

                        if (drawItem.GetType() == typeof(DirectoryDrawItem))
                        {
                            drawItem.Width = (int)(g.MeasureString((drawItem as DirectoryDrawItem).Directory.DirectoryName + "__", _palette.TextFont).Width);
                        }
                        else if (drawItem.GetType() == typeof(SeparatorDrawItem))
                        {
                            drawItem.Width = _dropDownItemWidth;
                        }

                        drawItem.Left = right - drawItem.Width;
                        drawItem.Top = addressRect.Y;
                        drawItem.Height = addressRect.Height;

                        right = drawItem.Left;
                    }
                }

                int left = addressRect.Left;
                foreach (DrawItemBase drawItem in _addressItems)
                {
                    if (drawItem.Left < addressRect.Left)
                    {
                        if (drawItem.GetType() == typeof(DirectoryDrawItem))
                        {
                            DirectoryDrawItem directoryDrawItem = (DirectoryDrawItem)drawItem;
                            _overflowItem.Items.Add(directoryDrawItem.Directory);
                            if (_overflowItem.Items.Count == 1)
                            {
                                _overflowItem.Left = left;
                                _overflowItem.Top = addressRect.Y;
                                _overflowItem.Width = _dropDownItemWidth;
                                _overflowItem.Height = addressRect.Height;
                                left = _overflowItem.Right;
                            }
                        }
                    }
                    else
                    {
                        if (drawItem.GetType() == typeof(SeparatorDrawItem))
                        {
                            SeparatorDrawItem sepDrawItem = (SeparatorDrawItem)drawItem;
                            if (_overflowItem.Items.Count(directory => directory.DirectoryPath.Equals(sepDrawItem.Directory.DirectoryPath, StringComparison.Ordinal)) > 0)
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

        private IList<DrawItemBase> createAddressItems(GetAddressInfoResult addressInfo)
        {
            List<DrawItemBase> items = new List<DrawItemBase>();

            for (int i = 0; i < addressInfo.DirectoryList.Count - 1; i++)
            {
                FileShallowInfoEntity info = addressInfo.DirectoryList[i];
                DirectoryEntity directory = new DirectoryEntity();
                directory.DirectoryPath = info.FilePath;
                directory.DirectoryName = info.FileName;
                directory.DirectoryIcon = info.SmallIcon;

                DirectoryDrawItem directoryDraw = new DirectoryDrawItem();
                directoryDraw.AddressBar = this;
                directoryDraw.Directory = directory;
                directoryDraw.Palette = _palette;
                directoryDraw.SelectedDirectory += new EventHandler<SelectedDirectoryEventArgs>(drawItem_SelectedDirectory);
                items.Add(directoryDraw);

                SeparatorDrawItem sepDraw = new SeparatorDrawItem();
                sepDraw.AddressBar = this;
                sepDraw.Directory = directory;
                sepDraw.Palette = _palette;
                if (i + 1 < addressInfo.DirectoryList.Count)
                {
                    sepDraw.SelectedSubDirectoryPath = addressInfo.DirectoryList[i + 1].FilePath;
                }
                sepDraw.SelectedDirectory += new EventHandler<SelectedDirectoryEventArgs>(drawItem_SelectedDirectory);
                items.Add(sepDraw);
            }

            if (addressInfo.HasSubDirectory)
            {
                FileShallowInfoEntity info = addressInfo.DirectoryList.Last();
                DirectoryEntity directory = new DirectoryEntity();
                directory.DirectoryPath = info.FilePath;
                directory.DirectoryName = info.FileName;
                directory.DirectoryIcon = info.SmallIcon;

                DirectoryDrawItem directoryDraw = new DirectoryDrawItem();
                directoryDraw.AddressBar = this;
                directoryDraw.Directory = directory;
                directoryDraw.Palette = _palette;
                directoryDraw.SelectedDirectory += new EventHandler<SelectedDirectoryEventArgs>(drawItem_SelectedDirectory);
                items.Add(directoryDraw);

                SeparatorDrawItem sepDraw = new SeparatorDrawItem();
                sepDraw.AddressBar = this;
                sepDraw.Directory = directory;
                sepDraw.Palette = _palette;
                sepDraw.SelectedDirectory += new EventHandler<SelectedDirectoryEventArgs>(drawItem_SelectedDirectory);
                items.Add(sepDraw);
            }
            else
            {
                FileShallowInfoEntity info = addressInfo.DirectoryList.Last();
                DirectoryEntity directory = new DirectoryEntity();
                directory.DirectoryPath = info.FilePath;
                directory.DirectoryName = info.FileName;
                directory.DirectoryIcon = info.SmallIcon;

                DirectoryDrawItem directoryDraw = new DirectoryDrawItem();
                directoryDraw.AddressBar = this;
                directoryDraw.Directory = directory;
                directoryDraw.Palette = _palette;
                directoryDraw.SelectedDirectory += new EventHandler<SelectedDirectoryEventArgs>(drawItem_SelectedDirectory);
                items.Add(directoryDraw);
            }

            return items;
        }

        private void clearAddressItems()
        {
            foreach (IDisposable item in _addressItems)
            {
                item.Dispose();
            }

            _addressItems.Clear();
        }

        private Rectangle getInnerRectangle()
        {
            int x = INNER_OFFSET;
            int y = INNER_OFFSET;
            int w = this.ClientRectangle.Width - INNER_OFFSET * 2;
            int h = this.ClientRectangle.Height - INNER_OFFSET * 2;
            return new Rectangle(x, y, w, h);
        }

        private Rectangle getAddressRect()
        {
            int x = INNER_OFFSET;
            int y = INNER_OFFSET;
            int w = this.ClientRectangle.Width - INNER_OFFSET * 2 - _dropDownItemWidth;
            int h = this.ClientRectangle.Height - INNER_OFFSET * 2;
            return new Rectangle(x, y, w, h);
        }

        private bool setMousePointItem(DrawItemBase newDrawItem)
        {
            bool ret = false;
            if (newDrawItem != null)
            {
                if (!newDrawItem.Equals(_mousePointItem))
                {
                    _mousePointItem = newDrawItem;
                    ret = true;
                }
            }
            else
            {
                if (_mousePointItem != null)
                {
                    _mousePointItem = null;
                    ret = true;
                }
            }

            _directoryHistoryItem.IsMousePoint = _directoryHistoryItem.Equals(_mousePointItem);
            _overflowItem.IsMousePoint = _overflowItem.Equals(_mousePointItem);
            foreach (DrawItemBase drawItem in _addressItems)
            {
                drawItem.IsMousePoint = drawItem.Equals(_mousePointItem);
            }

            return ret;
        }

        private bool setMouseDownItem(DrawItemBase newDrawItem)
        {
            bool ret = false;
            if (newDrawItem != null)
            {
                if (!newDrawItem.Equals(_mouseDownItem))
                {
                    _mouseDownItem = newDrawItem;
                    ret = true;
                }
            }
            else
            {
                if (_mouseDownItem != null)
                {
                    _mouseDownItem = null;
                    ret = true;
                }
            }

            _directoryHistoryItem.IsMouseDown = _directoryHistoryItem.Equals(_mouseDownItem);
            _overflowItem.IsMouseDown = _overflowItem.Equals(_mouseDownItem);
            foreach (DrawItemBase drawItem in _addressItems)
            {
                drawItem.IsMouseDown = drawItem.Equals(_mouseDownItem);
            }

            return ret;
        }

        private DrawItemBase getItemFromPoint(int x, int y)
        {
            if (_overflowItem.GetRectangle().Contains(x, y))
            {
                return _overflowItem;
            }
            else if (_directoryHistoryItem.GetRectangle().Contains(x, y))
            {
                return _directoryHistoryItem;
            }
            else if (_addressItems != null)
            {
                foreach (DrawItemBase drawItem in _addressItems)
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

        private void getAddressInfoProcess_Callback(object sender, GetAddressInfoResult e)
        {
            if (e.GetAddressInfoException != null)
            {
                ExceptionUtil.ShowErrorDialog(e.GetAddressInfoException);
                return;
            }

            clearAddressItems();

            _directoryPath = e.DirectoryPath;
            _addressItems.AddRange(createAddressItems(e));

            this.Invalidate();
        }

        private void drawItem_DropDownOpened(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void drawItem_DropDownClosed(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void drawItem_SelectedDirectory(object sender, SelectedDirectoryEventArgs e)
        {
            OnSelectedDirectory(e);
        }

        #endregion
    }
}
