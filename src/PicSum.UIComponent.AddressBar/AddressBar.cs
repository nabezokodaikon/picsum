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

namespace PicSum.UIComponent.AddressBar
{
    public class AddressBar : Control
    {
        #region 定数・列挙

        private const int INNER_OFFSET = 1;

        #endregion

        #region イベント・デリゲート

        public event EventHandler<SelectedFolderEventArgs> SelectedFolder;

        #endregion

        #region インスタンス変数

        private readonly int _dropDownItemWidth = Resources.SmallArrowDown.Width;
        private readonly Palette _palette = new Palette();
        private readonly OverflowDrawItem _overflowItem = new OverflowDrawItem();
        private readonly FolderHistoryDrawItem _folderHistoryItem = new FolderHistoryDrawItem();
        private IContainer _components = null;
        private TwoWayProcess<GetAddressInfoAsyncFacade, SingleValueEntity<string>, AddressInfo> _getAddressInfoProcess = null;
        private string _folderPath = null;
        private readonly List<DrawItemBase> _addressItems = new List<DrawItemBase>();
        private DrawItemBase _mousePointItem = null;
        private DrawItemBase _mouseDownItem = null;

        #endregion

        #region パブリックプロパティ

        public Font TextFont
        {
            get
            {
                return _palette.TextFont;
            }
            set
            {
                _palette.TextFont = value;
            }
        }

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

        private TwoWayProcess<GetAddressInfoAsyncFacade, SingleValueEntity<string>, AddressInfo> getAddressInfoProcess
        {
            get
            {
                if (_getAddressInfoProcess == null)
                {
                    _getAddressInfoProcess = TaskManager.CreateTwoWayProcess<GetAddressInfoAsyncFacade, SingleValueEntity<string>, AddressInfo>(components);
                    getAddressInfoProcess.Callback += new AsyncTaskCallbackEventHandler<AddressInfo>(getAddressInfoProcess_Callback);
                }

                return _getAddressInfoProcess;
            }
        }

        private FolderContextMenu contextMenu
        {
            get
            {
                 return (FolderContextMenu)this.ContextMenuStrip;
            }
        }

        private FolderDrawItem mousePointFolderItem
        {
            get
            {
                if (_mousePointItem != null &&
                    _mousePointItem.GetType() == typeof(FolderDrawItem))
                {
                    return (FolderDrawItem)_mousePointItem;
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
                _folderHistoryItem.Dispose();

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

            _folderHistoryItem.Draw(e.Graphics);

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
            if (drawItem is FolderDrawItem &&
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

        protected virtual void OnSelectedFolder(SelectedFolderEventArgs e)
        {
            if (SelectedFolder != null)
            {
                SelectedFolder(this, e);
            }
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            _overflowItem.AddressBar = this;
            _overflowItem.Palette = _palette;

            _folderHistoryItem.AddressBar = this;
            _folderHistoryItem.Palette = _palette;

            this.SetStyle(ControlStyles.DoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.ResizeRedraw, true);

            this.ContextMenuStrip = new FolderContextMenu();

            contextMenu.Opening += new CancelEventHandler(contextMenu_Opening);
            contextMenu.ActiveTabOpen += new EventHandler(contextMenu_ActiveTabOpen);
            contextMenu.NewTabOpen += new EventHandler(contextMenu_NewTabOpen);
            contextMenu.OtherWindowOpen += new EventHandler(contextMenu_OtherWindowOpen);
            contextMenu.NewWindowOpen += new EventHandler(contextMenu_NewWindowOpen);

            _overflowItem.DropDownOpened += new EventHandler(drawItem_DropDownOpened);
            _overflowItem.DropDownClosed += new EventHandler(drawItem_DropDownClosed);
            _overflowItem.SelectedFolder += new EventHandler<SelectedFolderEventArgs>(drawItem_SelectedFolder);

            _folderHistoryItem.DropDownOpened += new EventHandler(drawItem_DropDownOpened);
            _folderHistoryItem.DropDownClosed += new EventHandler(drawItem_DropDownClosed);
            _folderHistoryItem.SelectedFolder += new EventHandler<SelectedFolderEventArgs>(drawItem_SelectedFolder);
        }

        private void setItemsRectangle()
        {
            _overflowItem.ClearRectangle();
            _overflowItem.Items.Clear();

            Rectangle innerRect = getInnerRectangle();
            _folderHistoryItem.Left = innerRect.Right - _dropDownItemWidth;
            _folderHistoryItem.Top = innerRect.Y;
            _folderHistoryItem.Width = _dropDownItemWidth;
            _folderHistoryItem.Height = innerRect.Height;

            Rectangle addressRect = getAddressRect();

            if (_addressItems != null)
            {
                int right = addressRect.Right;

                using (Graphics g = this.CreateGraphics())
                {
                    for (int i = _addressItems.Count - 1; i > -1; i--)
                    {
                        DrawItemBase drawItem = _addressItems[i];

                        if (drawItem.GetType() == typeof(FolderDrawItem))
                        {
                            drawItem.Width = (int)(g.MeasureString((drawItem as FolderDrawItem).Folder.FolderName + "__", _palette.TextFont).Width);
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
                        if (drawItem.GetType() == typeof(FolderDrawItem))
                        {
                            FolderDrawItem folderDrawItem = (FolderDrawItem)drawItem;
                            _overflowItem.Items.Add(folderDrawItem.Folder);
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
                            if (_overflowItem.Items.Count(folder => folder.FolderPath.Equals(sepDrawItem.Folder.FolderPath, StringComparison.Ordinal)) > 0)
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

        private IList<DrawItemBase> createAddressItems(AddressInfo addressInfo)
        {
            List<DrawItemBase> items = new List<DrawItemBase>();

            for (int i = 0; i < addressInfo.FolderList.Count - 1; i++)
            {
                FileShallowInfoEntity info = addressInfo.FolderList[i];
                FolderEntity folder = new FolderEntity();
                folder.FolderPath = info.FilePath;
                folder.FolderName = info.FileName;
                folder.FolderIcon = info.SmallIcon;

                FolderDrawItem folderDraw = new FolderDrawItem();
                folderDraw.AddressBar = this;
                folderDraw.Folder = folder;
                folderDraw.Palette = _palette;
                folderDraw.SelectedFolder += new EventHandler<SelectedFolderEventArgs>(drawItem_SelectedFolder);
                items.Add(folderDraw);

                SeparatorDrawItem sepDraw = new SeparatorDrawItem();
                sepDraw.AddressBar = this;
                sepDraw.Folder = folder;
                sepDraw.Palette = _palette;
                if (i + 1 < addressInfo.FolderList.Count)
                {
                    sepDraw.SelectedSubFolderPath = addressInfo.FolderList[i + 1].FilePath;
                }
                sepDraw.SelectedFolder += new EventHandler<SelectedFolderEventArgs>(drawItem_SelectedFolder);
                items.Add(sepDraw);
            }

            if (addressInfo.HasSubFolder)
            {
                FileShallowInfoEntity info = addressInfo.FolderList.Last();
                FolderEntity folder = new FolderEntity();
                folder.FolderPath = info.FilePath;
                folder.FolderName = info.FileName;
                folder.FolderIcon = info.SmallIcon;

                FolderDrawItem folderDraw = new FolderDrawItem();
                folderDraw.AddressBar = this;
                folderDraw.Folder = folder;
                folderDraw.Palette = _palette;
                folderDraw.SelectedFolder += new EventHandler<SelectedFolderEventArgs>(drawItem_SelectedFolder);
                items.Add(folderDraw);

                SeparatorDrawItem sepDraw = new SeparatorDrawItem();
                sepDraw.AddressBar = this;
                sepDraw.Folder = folder;
                sepDraw.Palette = _palette;
                sepDraw.SelectedFolder += new EventHandler<SelectedFolderEventArgs>(drawItem_SelectedFolder);
                items.Add(sepDraw);
            }
            else
            {
                FileShallowInfoEntity info = addressInfo.FolderList.Last();
                FolderEntity folder = new FolderEntity();
                folder.FolderPath = info.FilePath;
                folder.FolderName = info.FileName;
                folder.FolderIcon = info.SmallIcon;

                FolderDrawItem folderDraw = new FolderDrawItem();
                folderDraw.AddressBar = this;
                folderDraw.Folder = folder;
                folderDraw.Palette = _palette;
                folderDraw.SelectedFolder += new EventHandler<SelectedFolderEventArgs>(drawItem_SelectedFolder);
                items.Add(folderDraw);
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

            _folderHistoryItem.IsMousePoint = _folderHistoryItem.Equals(_mousePointItem);
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

            _folderHistoryItem.IsMouseDown = _folderHistoryItem.Equals(_mouseDownItem);
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
            else if (_folderHistoryItem.GetRectangle().Contains(x, y))
            {
                return _folderHistoryItem;
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

        private void getAddressInfoProcess_Callback(object sender, AddressInfo e)
        {
            clearAddressItems();

            _folderPath = e.FolderPath;
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

        private void drawItem_SelectedFolder(object sender, SelectedFolderEventArgs e)
        {
            OnSelectedFolder(e);
        }

        private void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (mousePointFolderItem == null)
            {
                e.Cancel = true;
            }
        }

        private void contextMenu_ActiveTabOpen(object sender, EventArgs e)
        {
            OnSelectedFolder(new SelectedFolderEventArgs(ContentsOpenType.OverlapTab, mousePointFolderItem.Folder.FolderPath));
        }

        private void contextMenu_NewTabOpen(object sender, EventArgs e)
        {
            OnSelectedFolder(new SelectedFolderEventArgs(ContentsOpenType.AddTab, mousePointFolderItem.Folder.FolderPath));
        }

        private void contextMenu_OtherWindowOpen(object sender, EventArgs e)
        {
            OnSelectedFolder(new SelectedFolderEventArgs(ContentsOpenType.OtherWindow, mousePointFolderItem.Folder.FolderPath));
        }

        private void contextMenu_NewWindowOpen(object sender, EventArgs e)
        {
            OnSelectedFolder(new SelectedFolderEventArgs(ContentsOpenType.NewWindow, mousePointFolderItem.Folder.FolderPath));
        }

        #endregion
    }
}
