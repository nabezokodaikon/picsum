using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using SWF.Common;
using SWF.UIComponent.TabOperation.Properties;
using WinApi;
using System.Globalization;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブ切替コントロール
    /// </summary>
    public class TabSwitch : Control
    {
        #region 定数・列挙

        // タブ同士のの間隔
        private const int TabMargin = 2;

        // タブの最大幅
        private const int TabMaximumWidth = 256;

        // タブの最小幅
        private const int TabMinimumWidth = 64;

        // タブ領域の間隔
        internal const int TabsMargin = 8;

        // タブを移動するとみなす重なりの比率
        private const float TabDragOverlapRate = 0.4f;

        #endregion

        #region イベント・デリゲート

        /// <summary>
        /// アクティブタブ変更イベント
        /// </summary>
        public event EventHandler ActiveTabChanged;

        /// <summary>
        /// タブを閉じるボタンをクリックしたイベント
        /// </summary>
        public event EventHandler<TabEventArgs> TabCloseButtonClick;

        /// <summary>
        /// タブをドロップアウトしたイベント。
        /// </summary>
        public event EventHandler<TabDropoutedEventArgs> TabDropouted;

        /// <summary>
        /// 背景を左ダブルクリックした時のイベント
        /// </summary>
        public event EventHandler BackgroundMouseDoubleLeftClick;

        /// <summary>
        /// タブ領域にドラッグオーバーしたイベント
        /// </summary>
        public event EventHandler<DragEventArgs> TabAreaDragOver;

        /// <summary>
        /// タブ領域にドラッグドロップしたイベント
        /// </summary>
        public event EventHandler<TabAreaDragEventArgs> TabAreaDragDrop;

        /// <summary>
        /// タブ追加ボタンをクリックしたイベント
        /// </summary>
        public event EventHandler<MouseEventArgs> AddTabButtonMouseClick;

        #endregion

        #region インスタンス変数

        // タブの高さ
        private readonly int _tabHeight = Resources.ActiveTab.Height;

        // タブ領域右側の差分
        private int _tabsRightOffset = 0;

        // タブ描画パレット
        private readonly TabPalette _tabPalette = new TabPalette();

        // タブ情報リスト
        private readonly List<TabInfo> _tabList = new List<TabInfo>();

        // マウスダウンしたタブ
        private TabInfo _mouseDownTab = null;

        // マウスポイントされているタブ
        private TabInfo _mousePointTab = null;

        // アクティブなタブ
        private TabInfo _activeTab = null;

        // ドロップされている座標
        private Nullable<Point> _dropPoint = null;

        // タブ追加ボタン病が暮らす
        private AddTabButtonDrawArea _addTabButtonDrawArea = new AddTabButtonDrawArea();

        // コンテンツ描画クラス
        private ContentsDrawArea _contentsDrawArea = new ContentsDrawArea();

        #endregion

        #region パブリックプロパティ

        /// <summary>
        /// タブの高さ
        /// </summary>
        public int TabHeight
        {
            get
            {
                return _tabHeight;
            }
        }

        /// <summary>
        /// タブ領域右側の差分
        /// </summary>
        public int TabsRightOffset
        {
            get
            {
                return _tabsRightOffset;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _tabsRightOffset = value;
            }
        }

        /// <summary>
        /// タブの存在を確認します。
        /// </summary>
        public bool HasTab
        {
            get
            {
                return (_tabList.Count > 0);
            }
        }

        /// <summary>
        /// アクティブタブ
        /// </summary>
        public TabInfo ActiveTab
        {
            get
            {
                return _activeTab;
            }
        }

        /// <summary>
        /// アクティブタブのインデックス
        /// </summary>
        public int ActiveTabIndex
        {
            get
            {
                if (_activeTab != null)
                {
                    return _tabList.IndexOf(_activeTab);
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// タブの数
        /// </summary>
        public int TabCount
        {
            get
            {
                return _tabList.Count;
            }
        }

        /// <summary>
        /// 輪郭色
        /// </summary>
        public Color OutlineColor
        {
            get
            {
                return _contentsDrawArea.OutlineColor;
            }
        }

        /// <summary>
        /// 輪郭ブラシ
        /// </summary>
        public SolidBrush OutlineBrush
        {
            get
            {
                return _contentsDrawArea.OutlineBrush;
            }
        }

        /// <summary>
        /// コンテンツ色
        /// </summary>
        public Color ContentsColor
        {
            get
            {
                return _contentsDrawArea.ContentsColor;
            }
        }

        /// <summary>
        /// コンテンツブラシ
        /// </summary>
        public SolidBrush ContentsBrush
        {
            get
            {
                return _contentsDrawArea.ContentsBrush;
            }
        }

        #endregion

        #region コンストラクタ

        public TabSwitch()
        {
            initializeComponent();
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// 格納されているフォームを取得します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetForm<T>() where T : Form
        {
            return (T)getForm();
        }

        /// <summary>
        /// タブを追加します。
        /// </summary>
        /// <param name="tab"></param>
        public void AddTab(TabInfo tab)
        {
            if (tab == null)
            {
                throw new ArgumentNullException("tab");
            }

            if (_tabList.Contains(tab))
            {
                throw new ArgumentException("既に含まれているタブを追加しようとしました。", "tab");
            }

            tab.Owner = this;

            _tabList.Add(tab);

            if (setActiveTab(tab))
            {
                OnActiveTabChanged(new EventArgs());
            }

            this.Invalidate();
        }

        /// <summary>
        /// タブを挿入します。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="tab"></param>
        public void InsertTab(int index, TabInfo tab)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (tab == null)
            {
                throw new ArgumentNullException("tab");
            }

            if (_tabList.Contains(tab))
            {
                throw new ArgumentException("既に含まれているタブを挿入しようとしました。", "tab");
            }

            if (index > _tabList.Count - 1)
            {
                AddTab(tab);
            }
            else
            {
                tab.Owner = this;

                _tabList.Insert(index, tab);

                if (setActiveTab(tab))
                {
                    OnActiveTabChanged(new EventArgs());
                }

                this.Invalidate();
            }
        }

        /// <summary>
        /// タブにコンテンツを追加します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public T AddTab<T>(IContentsParameter param) where T : ContentsPanel
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            TabInfo tab = new TabInfo(param);
            AddTab(tab);
            return (T)tab.Contents;
        }

        /// <summary>
        /// タブにコンテンツを挿入します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T InsertTab<T>(int index, IContentsParameter param) where T : ContentsPanel
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            TabInfo tab = new TabInfo(param);
            InsertTab(index, tab);
            return (T)tab.Contents;
        }

        /// <summary>
        /// タブにコンテンツを上書きします。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public T OverwriteTab<T>(IContentsParameter param) where T : ContentsPanel
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            if (_activeTab != null)
            {
                _activeTab.ClearContents();
                _activeTab.OverwriteContents(param);

                ContentsContainer container = getContainer();
                container.ClearContents();
                container.SetContents(_activeTab.Contents);

                this.Invalidate();

                return (T)_activeTab.Contents;
            }
            else
            {
                return AddTab<T>(param);
            }
        }

        /// <summary>
        /// 履歴の前のコンテンツを表示します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T SetPreviewContentsHistory<T>() where T : ContentsPanel
        {
            if (_activeTab == null)
            {
                throw new NullReferenceException("アクティブなタブが存在しません。");
            }

            if (!_activeTab.HasPreviewContents)
            {
                throw new Exception("コンテンツの前の履歴が存在しません。");
            }

            _activeTab.ClearContents();
            _activeTab.CreatePreviewContents();

            ContentsContainer container = getContainer();
            container.ClearContents();
            container.SetContents(_activeTab.Contents);

            this.Invalidate();

            return (T)_activeTab.Contents;
        }

        /// <summary>
        /// 履歴の次のコンテンツを表示します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T SetNextContentsHistory<T>() where T : ContentsPanel
        {
            if (_activeTab == null)
            {
                throw new NullReferenceException("アクティブなタブが存在しません。");
            }

            if (!_activeTab.HasNextContents)
            {
                throw new Exception("コンテンツの次の履歴が存在しません。");
            }

            _activeTab.ClearContents();
            _activeTab.CreateNextContents();

            ContentsContainer container = getContainer();
            container.ClearContents();
            container.SetContents(_activeTab.Contents);

            this.Invalidate();

            return (T)_activeTab.Contents;
        }

        /// <summary>
        /// タブを削除します。
        /// </summary>
        /// <param name="tab"></param>
        public void RemoveTab(TabInfo tab)
        {
            if (tab == null)
            {
                throw new ArgumentNullException("tab");
            }

            if (!_tabList.Contains(tab))
            {
                throw new ArgumentException("含まれていないタブを削除しようとしました。", "tab");
            }

            TabInfo nextActiveTab = getNextTab(tab);
            if (nextActiveTab == null)
            {
                nextActiveTab = getPreviewTab(tab);
            }

            if (setActiveTab(nextActiveTab))
            {
                OnActiveTabChanged(new EventArgs());
            }

            if (tab.Equals(_mousePointTab))
            {
                _mousePointTab = null;
            }

            tab.Owner = null;

            _tabList.Remove(tab);

            this.Invalidate();
        }

        /// <summary>
        /// アクティブタブをセットします。
        /// </summary>
        /// <param name="index"></param>
        public void SetActiveTab(int index)
        {
            if (index < 0 || _tabList.Count - 1 < index)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (setActiveTab(_tabList[index]))
            {
                this.Invalidate();
                OnActiveTabChanged(new EventArgs());
            }
        }

        /// <summary>
        /// ヘッダ領域を再描画します。
        /// </summary>
        public void InvalidateHeader()
        {
            invalidateHeader();
        }

        /// <summary>
        /// タブのスクリーン領域を取得します。
        /// </summary>
        /// <returns></returns>
        internal Rectangle GetTabsScreenRectangle()
        {
            Rectangle rect = getTabsRectangle();
            Point p = this.PointToScreen(new Point(rect.X, rect.Y));
            int x = p.X;
            int y = p.Y;
            int w = rect.Width;
            int h = rect.Height;
            return new Rectangle(x, y, w, h);
        }

        /// <summary>
        /// タブのクライアント領域を取得します。
        /// </summary>
        /// <returns></returns>
        internal Rectangle GetTabsClientRectangle()
        {
            return getTabsRectangle();
        }

        #endregion

        #region 継承メソッド

        protected override void OnHandleCreated(EventArgs e)
        {
            if (!this.DesignMode)
            {
                Form form = getForm();
                TabDragOperation.AddForm(form);
            }

            base.OnHandleCreated(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (TabInfo tab in _tabList)
                {
                    tab.Close();
                }
            }

            base.Dispose(disposing);
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            setTabsDrawArea();
            setAddTabButtonDrawArea();
            base.OnInvalidated(e);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //base.OnPaintBackground(pevent);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.None;
            e.Graphics.InterpolationMode = InterpolationMode.Low;

            foreach (TabInfo tab in _tabList.FindAll((t) => !t.Equals(_activeTab)))
            {
                drawTab(tab, e.Graphics);
            }

            getAddTabButtonDrawMethod()(e.Graphics);

            _contentsDrawArea.Draw(e.Graphics);

            if (_activeTab != null)
            {
                drawTab(_activeTab, e.Graphics);
            }

            drawDropPoint(e.Graphics);

            base.OnPaint(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (_mousePointTab != null)
            {
                setMousePointTab(null);
            }

            _mouseDownTab = null;
            invalidateHeader();

            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (TabDragOperation.IsBegin)
            {
                TabDragOperation.MoveTab();
            }
            else
            {
                TabInfo tab = getTabFromPoint(e.X, e.Y);
                setMousePointTab(tab);
                invalidateHeader();

                Form form = getForm();
                if (form.Equals(Form.ActiveForm))
                {
                    if (getHeaderRectangle().Contains(e.X, e.Y))
                    {
                        this.Focus();
                    }
                }

                if (tab == null && e.Button == MouseButtons.Left)
                {
                    WinApiMembers.ReleaseCapture();
                    WinApiMembers.SendMessage(form.Handle, WinApiMembers.WM_NCLBUTTONDOWN, WinApiMembers.HTCAPTION, 0);
                }
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            TabInfo tab = getTabFromPoint(e.X, e.Y);
            if (tab != null)
            {
                _mouseDownTab = tab;

                if (tab.DrawArea.GetCloseButtonRectangle().Contains(e.X, e.Y))
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        invalidateHeader();
                    }
                }
                else
                {
                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            if (setActiveTab(tab))
                            {
                                this.Invalidate();
                                OnActiveTabChanged(new EventArgs());
                            }
                            TabDragOperation.BeginTabDragOperation(tab);
                            break;
                        case MouseButtons.Right:
                            break;
                        case MouseButtons.Middle:
                            break;
                    }
                }
            }
            else if (_addTabButtonDrawArea.Contents(e.X, e.Y))
            {
                invalidateHeader();
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (TabDragOperation.IsBegin)
            {
                TabInfo tab = TabDragOperation.EndTabDragOperation();
                if (tab.Owner != null)
                {
                    invalidateHeader();

                    if (!tab.Owner.Equals(this))
                    {
                        OnTabDropouted(new TabDropoutedEventArgs(tab));
                    }
                }
                else
                {
                    Point screenPoint = this.PointToScreen(new Point(e.X, e.Y));
                    Form form = getForm();

                    if (ScreenUtil.GetTopRect(Resources.DropMaximum.Size).Contains(screenPoint))
                    {
                        OnTabDropouted(new TabDropoutedEventArgs(tab, screenPoint, form.ClientSize, FormWindowState.Maximized));
                    }
                    else if (ScreenUtil.GetLeftRect(Resources.DropMaximum.Size).Contains(screenPoint))
                    {
                        Rectangle screenRect = Screen.GetWorkingArea(Cursor.Position);
                        int w = (int)(screenRect.Width / 2f);
                        int h = screenRect.Height;
                        int x = screenRect.Left;
                        int y = screenRect.Top;
                        OnTabDropouted(new TabDropoutedEventArgs(tab, new Point(x, y), new Size(w, h), FormWindowState.Normal));
                    }
                    else if (ScreenUtil.GetRightRect(Resources.DropMaximum.Size).Contains(screenPoint))
                    {
                        Rectangle screenRect = Screen.GetWorkingArea(Cursor.Position);
                        int w = (int)(screenRect.Width / 2f);
                        int h = screenRect.Height;
                        int x = screenRect.Right - w;
                        int y = screenRect.Top;
                        OnTabDropouted(new TabDropoutedEventArgs(tab, new Point(x, y), new Size(w, h), FormWindowState.Normal));
                    }
                    else
                    {
                        OnTabDropouted(new TabDropoutedEventArgs(tab, screenPoint, form.RestoreBounds.Size, FormWindowState.Normal));
                    }
                }
            }

            _mouseDownTab = null;

            base.OnMouseUp(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            TabInfo tab = getTabFromPoint(e.X, e.Y);
            if (tab != null && tab.Equals(_mouseDownTab))
            {
                if (e.Button == MouseButtons.Middle)
                {
                    OnTabCloseButtonClick(new TabEventArgs(tab));
                }
                else
                {
                    RectangleF rect = tab.DrawArea.GetCloseButtonRectangle();
                    if (rect.Contains(e.X, e.Y))
                    {
                        OnTabCloseButtonClick(new TabEventArgs(tab));
                    }
                }
            }
            else if (_addTabButtonDrawArea.Contents(e.X, e.Y))
            {
                OnAddTabButtonMouseClick(e);
            }

            base.OnMouseClick(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                TabInfo tab = getTabFromPoint(e.X, e.Y);
                if (tab == null &&
                    getHeaderRectangle().Contains(e.X, e.Y))
                {
                    OnBackgroundMouseLeftDoubleClick(new EventArgs());
                    //Form form = getForm();
                    //Point p = form.PointToClient(Cursor.Position);
                    //string x = p.X.ToString("X4");
                    //string y = p.Y.ToString("X4");
                    //int lParam = int.Parse(x + y, NumberStyles.AllowHexSpecifier);
                    //WinApiMembers.SendMessage(form.Handle, WinApiMembers.WM_LBUTTONDBLCLK, 0, lParam);
                }
            }

            base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (_tabList.Count > 1)
            {
                if (_activeTab == null)
                {
                    throw new NullReferenceException("アクティブなタブがNULLです。");
                }

                if (getHeaderRectangle().Contains(e.X, e.Y))
                {
                    int index = _tabList.IndexOf(_activeTab);
                    if (e.Delta > 0)
                    {
                        if (index - 1 < 0)
                        {
                            setActiveTab(_tabList.Last());
                        }
                        else
                        {
                            setActiveTab(_tabList[index - 1]);
                        }
                    }
                    else
                    {
                        if (index + 1 > _tabList.Count - 1)
                        {
                            setActiveTab(_tabList.First());
                        }
                        else
                        {
                            setActiveTab(_tabList[index + 1]);
                        }
                    }

                    invalidateHeader();
                }
                else
                {
                    _activeTab.Contents.Focus();
                }
            }
            else if (_activeTab != null)
            {
                _activeTab.Contents.Focus();
            }

            base.OnMouseWheel(e);
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            _dropPoint = this.PointToClient(new Point(drgevent.X, drgevent.Y));
            if (getHeaderRectangle().Contains(_dropPoint.Value))
            {
                int dropX = _dropPoint.Value.X;
                int dropY = _dropPoint.Value.Y;
                foreach (TabInfo tab in _tabList)
                {
                    if (tab.DrawArea.Contents(dropX, dropY))
                    {
                        if (setActiveTab(tab))
                        {
                            OnActiveTabChanged(new EventArgs());
                            break;
                        }
                    }
                }

                drgevent.Effect = DragDropEffects.All;
                OnTabAreaDragOver(drgevent);
            }
            else
            {
                drgevent.Effect = DragDropEffects.None;
            }

            invalidateHeader();
            base.OnDragOver(drgevent);
        }

        protected override void OnDragLeave(EventArgs e)
        {
            _dropPoint = null;
            this.Invalidate();
            base.OnDragLeave(e);
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            if (!_dropPoint.HasValue)
            {
                throw new Exception("ドロップされている座標がNULLです。");
            }

            if (getHeaderRectangle().Contains(_dropPoint.Value))
            {
                int dropX = _dropPoint.Value.X;
                int dropY = _dropPoint.Value.Y;
                bool isExecuteEvent = false;
                foreach (TabInfo tab in _tabList)
                {
                    if (tab.DrawArea.Contents(dropX, dropY))
                    {
                        if (isTabLeftDrop(dropX, tab))
                        {
                            OnTabAreaDragDrop(new TabAreaDragEventArgs(false, _tabList.IndexOf(tab), drgevent));
                            isExecuteEvent = true;
                            break;
                        }
                        else if (isTabRightDrop(dropX, tab))
                        {
                            OnTabAreaDragDrop(new TabAreaDragEventArgs(false, _tabList.IndexOf(tab) + 1, drgevent));
                            isExecuteEvent = true;
                            break;
                        }
                        else
                        {
                            OnTabAreaDragDrop(new TabAreaDragEventArgs(true, _tabList.IndexOf(tab), drgevent));
                            isExecuteEvent = true;
                            break;
                        }
                    }
                }

                if (!isExecuteEvent)
                {
                    OnTabAreaDragDrop(new TabAreaDragEventArgs(false, _tabList.Count, drgevent));
                }
            }

            _dropPoint = null;
            base.OnDragDrop(drgevent);
        }

        protected virtual void OnActiveTabChanged(EventArgs e)
        {
            if (ActiveTabChanged != null)
            {
                ActiveTabChanged(this, e);
            }
        }

        protected virtual void OnTabCloseButtonClick(TabEventArgs e)
        {
            if (TabCloseButtonClick != null)
            {
                TabCloseButtonClick(this, e);
            }
        }

        protected virtual void OnTabDropouted(TabDropoutedEventArgs e)
        {
            if (TabDropouted != null)
            {
                TabDropouted(this, e);
            }
        }

        protected virtual void OnBackgroundMouseLeftDoubleClick(EventArgs e)
        {
            if (BackgroundMouseDoubleLeftClick != null)
            {
                BackgroundMouseDoubleLeftClick(this, e);
            }
        }

        protected virtual void OnTabAreaDragOver(DragEventArgs e)
        {
            if (TabAreaDragOver != null)
            {
                TabAreaDragOver(this, e);
            }
        }

        protected virtual void OnTabAreaDragDrop(TabAreaDragEventArgs e)
        {
            if (TabAreaDragDrop != null)
            {
                TabAreaDragDrop(this, e);
            }
        }

        protected virtual void OnAddTabButtonMouseClick(MouseEventArgs e)
        {
            if (AddTabButtonMouseClick != null)
            {
                AddTabButtonMouseClick(this, e);
            }
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            this.SetStyle(ControlStyles.DoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.ResizeRedraw, true);

            setAddTabButtonDrawArea();
        }

        private bool setMousePointTab(TabInfo tab)
        {
            if (tab != null)
            {
                if (!tab.Equals(_mousePointTab))
                {
                    _mousePointTab = tab;
                    return true;
                }
            }
            else
            {
                if (_mousePointTab != null)
                {
                    _mousePointTab = null;
                    return true;
                }
            }

            return false;
        }

        private bool setActiveTab(TabInfo tab)
        {
            if (tab != null)
            {
                if (!tab.Equals(_activeTab))
                {
                    ContentsContainer container = getContainer();
                    container.SuspendLayout();
                    container.ClearContents();
                    container.SetContents(tab.Contents);
                    container.ResumeLayout();
                    _activeTab = tab;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                ContentsContainer container = getContainer();
                container.SuspendLayout();
                container.ClearContents();
                container.ResumeLayout();
                if (_activeTab != null)
                {
                    _activeTab = null;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private TabInfo getPreviewTab(TabInfo tab)
        {
            int index = _tabList.IndexOf(tab);

            if (index > 0)
            {
                return _tabList[index - 1];
            }
            else
            {
                return null;
            }
        }

        private TabInfo getNextTab(TabInfo tab)
        {
            int index = _tabList.IndexOf(tab);

            if (index < _tabList.Count - 1)
            {
                return _tabList[index + 1];
            }
            else
            {
                return null;
            }
        }

        private TabInfo getTabFromPoint(int x, int y)
        {
            return _tabList.FirstOrDefault<TabInfo>((t) => t.DrawArea.Contents(x, y));
        }

        private ContentsContainer getContainer()
        {
            Form form = getForm();
            ContentsContainer container = getContainer(form);
            if (container != null)
            {
                return container;
            }
            else
            {
                throw new NullReferenceException("フォーム内にコンテンツコンテナが存在しません。");
            }
        }

        private ContentsContainer getContainer(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                if (child is ContentsContainer)
                {
                    return (ContentsContainer)child;
                }
                else if (child.Controls.Count > 0)
                {
                    ContentsContainer result = getContainer(child);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        private Form getForm()
        {
            Control ctl = this;
            while (ctl.Parent != null)
            {
                ctl = ctl.Parent;
            }

            if (ctl is Form)
            {
                return (Form)ctl;
            }
            else
            {
                throw new Exception("タブスイッチはフォームに格納されていません。");
            }
        }

        private void invalidateHeader()
        {
            this.Invalidate(getHeaderRectangle());
        }

        private Rectangle getHeaderRectangle()
        {
            Rectangle rect = this.ClientRectangle;
            int x = rect.X;
            int y = rect.Y;
            int w = rect.Width;
            int h = _tabHeight;
            return new Rectangle(x, y, w, h);
        }

        private Rectangle getTabsRectangle()
        {
            Rectangle rect = this.ClientRectangle;
            int x = rect.X + TabsMargin;
            int y = rect.Y;
            int w = rect.Width - _tabsRightOffset - TabsMargin * 2 - TabMargin - _addTabButtonDrawArea.Width;
            int h = _tabHeight;
            return new Rectangle(x, y, w, h);
        }

        private void setTabsDrawArea()
        {
            if (TabDragOperation.IsBegin)
            {
                _tabList.Sort((a, b) => sortTabList(a, b));
                Rectangle rect = getTabsRectangle();
                int x = rect.X;
                foreach (TabInfo tab in _tabList)
                {
                    if (!TabDragOperation.IsTarget(tab))
                    {
                        tab.DrawArea.X = x;
                        tab.DrawArea.Y = rect.Y;
                    }
                    x += (tab.DrawArea.Width + TabMargin);
                }
            }
            else
            {
                Rectangle rect = getTabsRectangle();
                int w = getTabWidth();
                int x = rect.X;
                foreach (TabInfo tab in _tabList)
                {
                    tab.DrawArea.X = x;
                    tab.DrawArea.Y = rect.Y;
                    tab.DrawArea.Width = w;
                    x += (w + TabMargin);
                }
            }
        }

        private void setAddTabButtonDrawArea()
        {
            if (_tabList.Count > 0)
            {
                TabInfo tab = _tabList.Last();
                if (TabDragOperation.IsTarget(tab))
                {
                    if (_tabList.Count > 1)
                    {
                        int index = _tabList.IndexOf(tab) - 1;
                        _addTabButtonDrawArea.X = _tabList[index].DrawArea.Right + TabsMargin;
                    }
                    else
                    {
                        _addTabButtonDrawArea.X = this.ClientRectangle.X + TabsMargin;
                    }
                }
                else
                {
                    _addTabButtonDrawArea.X = tab.DrawArea.Right + TabsMargin;
                }

                Rectangle rect = getTabsRectangle();
                if (_addTabButtonDrawArea.Right > rect.Right)
                {
                    _addTabButtonDrawArea.X = rect.Right + TabMargin;
                }
            }
            else
            {
                _addTabButtonDrawArea.X = this.ClientRectangle.X + TabsMargin;
            }
        }

        private int getTabWidth()
        {
            RectangleF rect = getTabsRectangle();

            float defAllTabW = TabMaximumWidth * _tabList.Count + TabMargin * (_tabList.Count - 1);

            if (defAllTabW > rect.Width)
            {
                float tabW = (rect.Width - TabMargin * (_tabList.Count - 1)) / _tabList.Count;
                return (int)Math.Max(tabW, TabMinimumWidth);
            }
            else
            {
                return TabMaximumWidth;
            }
        }

        private int sortTabList(TabInfo a, TabInfo b)
        {
            if (TabDragOperation.IsTarget(a))
            {
                float margin = b.DrawArea.Width * TabDragOverlapRate;
                if (margin > Math.Abs(a.DrawArea.X - b.DrawArea.X))
                {
                    if (a.DrawArea.X > b.DrawArea.X)
                    {
                        return (int)((a.DrawArea.X - margin) - b.DrawArea.X);
                    }
                    else if (a.DrawArea.X < b.DrawArea.X)
                    {
                        return (int)((a.DrawArea.X + margin) - b.DrawArea.X);
                    }
                }
            }
            else if (TabDragOperation.IsTarget(b))
            {
                float margin = a.DrawArea.Width * TabDragOverlapRate;
                if (margin > Math.Abs(a.DrawArea.X - b.DrawArea.X))
                {
                    if (a.DrawArea.X > b.DrawArea.X)
                    {
                        return (int)(a.DrawArea.X - (b.DrawArea.X + margin));
                    }
                    else if (a.DrawArea.X < b.DrawArea.X)
                    {
                        return (int)(a.DrawArea.X - (b.DrawArea.X - margin));
                    }
                }
            }

            return (int)(a.DrawArea.X - b.DrawArea.X);
        }

        private void drawTab(TabInfo tab, Graphics g)
        {
            getDrawTabMethod(tab)(g);
            getDrawCloseButtonMethod(tab)(g);

            DrawTabEventArgs args = new DrawTabEventArgs();
            args.Graphics = g;
            args.Font = _tabPalette.TitleFont;
            args.TitleColor = _tabPalette.TitleColor;
            args.TitleFormatFlags = _tabPalette.TitleFormatFlags;
            args.TextRectangle = tab.DrawArea.GetContentsRectangle();
            args.IconRectangle = tab.DrawArea.GetIconRectangle(tab.Icon);
            args.CloseButtonRectangle = tab.DrawArea.GetCloseButtonRectangle();
            args.TextStyle = DrawTextUtil.TextStyle.Glowing;
            tab.DrawingTabContents(args);
        }

        private void drawDropPoint(Graphics g)
        {
            if (_dropPoint == null)
            {
                return;
            }

            int dropX = _dropPoint.Value.X;
            int dropY = _dropPoint.Value.Y;

            foreach (TabInfo tab in _tabList)
            {
                if (tab.DrawArea.Contents(dropX, dropY))
                {
                    if (isTabLeftDrop(dropX, tab))
                    {
                        Image img = Resources.DropArrow;
                        float x = (tab.DrawArea.Left - TabMargin / 2f) - img.Width / 2f;
                        float y = 0;
                        g.DrawImage(img, x, y, img.Width, img.Height);
                        return;
                    }
                    else if (isTabRightDrop(dropX, tab))
                    {
                        Image img = Resources.DropArrow;
                        float x = (tab.DrawArea.Right - TabMargin / 2f) - img.Width / 2f;
                        float y = 0;
                        g.DrawImage(img, x, y, img.Width, img.Height);
                        return;
                    }
                }
            }
        }

        private Action<Graphics> getDrawTabMethod(TabInfo tab)
        {
            if (tab.Equals(_activeTab))
            {
                return tab.DrawArea.DrawActiveTab;
            }
            else if (tab.Equals(_mousePointTab))
            {
                return tab.DrawArea.DrawMousePointTab;
            }
            else
            {
                return tab.DrawArea.DrawInactiveTab;
            }
        }

        private Action<Graphics> getDrawCloseButtonMethod(TabInfo tab)
        {
            Point p = this.PointToClient(Cursor.Position);
            RectangleF rect = tab.DrawArea.GetCloseButtonRectangle();

            if (rect.Contains(p))
            {
                if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left)
                {
                    return tab.DrawArea.DrawMouseDownTabCloseButton;
                }
                else
                {
                    return tab.DrawArea.DrawMousePointTabCloseButton;
                }
            }
            else
            {
                return tab.DrawArea.DrawInactiveTabCloseButton;
            }
        }

        private Action<Graphics> getAddTabButtonDrawMethod()
        {
            Point p = this.PointToClient(Cursor.Position);
            if (_addTabButtonDrawArea.Contents(p))
            {
                if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left)
                {
                    return _addTabButtonDrawArea.DrawMouseDownImage;
                }
                else
                {
                    return _addTabButtonDrawArea.DrawMousePointImage;
                }
            }
            else
            {
                return _addTabButtonDrawArea.DrawInactiveImage;
            }
        }

        private bool isTabLeftDrop(int x, TabInfo tab)
        {
            return x > tab.DrawArea.Left && tab.DrawArea.Left + tab.DrawArea.Width / 3f > x;
        }

        private bool isTabRightDrop(int x, TabInfo tab)
        {
            return tab.DrawArea.Right > x && x > tab.DrawArea.Right - tab.DrawArea.Width / 3f;
        }

        #endregion
    }
}
