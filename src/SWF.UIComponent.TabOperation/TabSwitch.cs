using SWF.Common;
using SWF.UIComponent.TabOperation.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;
using WinApi;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブ切替コントロール
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class TabSwitch
        : Control
    {
        #region 定数・列挙

        // タブ同士のの間隔
        private const int TAB_MARGIN = 2;

        // タブの最大幅
        private const int TAB_MAXIMUM_WIDTH = 256;

        // タブの最小幅
        private const int TAB_MINIMUM_WIDTH = 64;

        // タブ領域の間隔
        internal const int TABS_MARGIN = 8;

        // タブを移動するとみなす重なりの比率
        private const float TAB_DRAG_OVERLAP_RATE = 0.4f;

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
        private readonly int tabHeight = Resources.ActiveTab.Height;

        // タブ領域右側の差分
        private int tabsRightOffset = 0;

        // タブ描画パレット
        private readonly TabPalette tabPalette = new TabPalette();

        // タブ情報リスト
        private readonly List<TabInfo> tabList = new List<TabInfo>();

        // マウスダウンしたタブ
        private TabInfo mouseDownTab = null;

        // マウスポイントされているタブ
        private TabInfo mousePointTab = null;

        // アクティブなタブ
        private TabInfo activeTab = null;

        // ドロップされている座標
        private Nullable<Point> dropPoint = null;

        // タブ追加ボタン病が暮らす
        private AddTabButtonDrawArea addTabButtonDrawArea = new AddTabButtonDrawArea();

        // コンテンツ描画クラス
        private ContentsDrawArea contentsDrawArea = new ContentsDrawArea();

        #endregion

        #region パブリックプロパティ

        /// <summary>
        /// タブの高さ
        /// </summary>
        public int TabHeight
        {
            get
            {
                return this.tabHeight;
            }
        }

        /// <summary>
        /// タブ領域右側の差分
        /// </summary>
        public int TabsRightOffset
        {
            get
            {
                return this.tabsRightOffset;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                this.tabsRightOffset = value;
            }
        }

        /// <summary>
        /// タブの存在を確認します。
        /// </summary>
        public bool HasTab
        {
            get
            {
                return (this.tabList.Count > 0);
            }
        }

        /// <summary>
        /// アクティブタブ
        /// </summary>
        public TabInfo ActiveTab
        {
            get
            {
                return this.activeTab;
            }
        }

        /// <summary>
        /// アクティブタブのインデックス
        /// </summary>
        public int ActiveTabIndex
        {
            get
            {
                if (this.activeTab != null)
                {
                    return this.tabList.IndexOf(this.activeTab);
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
                return this.tabList.Count;
            }
        }

        /// <summary>
        /// 輪郭色
        /// </summary>
        public Color OutlineColor
        {
            get
            {
                return this.contentsDrawArea.OutlineColor;
            }
        }

        /// <summary>
        /// 輪郭ブラシ
        /// </summary>
        public SolidBrush OutlineBrush
        {
            get
            {
                return this.contentsDrawArea.OutlineBrush;
            }
        }

        /// <summary>
        /// コンテンツ色
        /// </summary>
        public Color ContentsColor
        {
            get
            {
                return this.contentsDrawArea.ContentsColor;
            }
        }

        /// <summary>
        /// コンテンツブラシ
        /// </summary>
        public SolidBrush ContentsBrush
        {
            get
            {
                return this.contentsDrawArea.ContentsBrush;
            }
        }

        #endregion

        #region コンストラクタ

        public TabSwitch()
        {
            this.InitializeComponent();
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
            return (T)this.GetForm();
        }

        /// <summary>
        /// タブを追加します。
        /// </summary>
        /// <param name="tab"></param>
        public void AddTab(TabInfo tab)
        {
            if (tab == null)
            {
                throw new ArgumentNullException(nameof(tab));
            }

            if (this.tabList.Contains(tab))
            {
                throw new ArgumentException("既に含まれているタブを追加しようとしました。", nameof(tab));
            }

            tab.Owner = this;

            this.tabList.Add(tab);

            if (this.SetActiveTab(tab))
            {
                this.OnActiveTabChanged(new EventArgs());
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
                throw new ArgumentNullException(nameof(tab));
            }

            if (this.tabList.Contains(tab))
            {
                throw new ArgumentException("既に含まれているタブを挿入しようとしました。", nameof(tab));
            }

            if (index > this.tabList.Count - 1)
            {
                this.AddTab(tab);
            }
            else
            {
                tab.Owner = this;

                this.tabList.Insert(index, tab);

                if (this.SetActiveTab(tab))
                {
                    this.OnActiveTabChanged(new EventArgs());
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
                throw new ArgumentNullException(nameof(param));
            }

            var tab = new TabInfo(param);
            this.AddTab(tab);
            return (T)tab.Contents;
        }

        /// <summary>
        /// タブにコンテンツを挿入します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T InsertTab<T>(int index, IContentsParameter param)
            where T : ContentsPanel
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var tab = new TabInfo(param);
            this.InsertTab(index, tab);
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
                throw new ArgumentNullException(nameof(param));
            }

            if (this.activeTab != null)
            {
                this.activeTab.ClearContents();
                this.activeTab.OverwriteContents(param);

                var container = this.GetContainer();
                container.ClearContents();
                container.SetContents(this.activeTab.Contents);

                this.Invalidate();

                return (T)this.activeTab.Contents;
            }
            else
            {
                return this.AddTab<T>(param);
            }
        }

        /// <summary>
        /// 履歴の前のコンテンツを表示します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T SetPreviewContentsHistory<T>() where T : ContentsPanel
        {
            if (this.activeTab == null)
            {
                throw new NullReferenceException("アクティブなタブが存在しません。");
            }

            if (!this.activeTab.HasPreviewContents)
            {
                throw new InvalidOperationException("コンテンツの前の履歴が存在しません。");
            }

            this.activeTab.ClearContents();
            this.activeTab.CreatePreviewContents();

            var container = this.GetContainer();
            container.ClearContents();
            container.SetContents(this.activeTab.Contents);

            this.Invalidate();

            return (T)this.activeTab.Contents;
        }

        /// <summary>
        /// 履歴の次のコンテンツを表示します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T SetNextContentsHistory<T>() where T : ContentsPanel
        {
            if (this.activeTab == null)
            {
                throw new NullReferenceException("アクティブなタブが存在しません。");
            }

            if (!this.activeTab.HasNextContents)
            {
                throw new InvalidOperationException("コンテンツの次の履歴が存在しません。");
            }

            this.activeTab.ClearContents();
            this.activeTab.CreateNextContents();

            var container = this.GetContainer();
            container.ClearContents();
            container.SetContents(this.activeTab.Contents);

            this.Invalidate();

            return (T)this.activeTab.Contents;
        }

        /// <summary>
        /// 現在のコンテンツのクローンを取得します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T CloneCurrentContents<T>()
            where T : ContentsPanel
        {
            if (this.activeTab == null)
            {
                throw new NullReferenceException("アクティブなタブが存在しません。");
            }

            if (!this.activeTab.HasContents)
            {
                throw new InvalidOperationException("コンテンツが存在しません。");
            }

            this.activeTab.ClearContents();
            this.activeTab.CloneCurrentContents();

            var container = this.GetContainer();
            container.ClearContents();
            container.SetContents(this.activeTab.Contents);

            this.Invalidate();

            return (T)this.activeTab.Contents;
        }

        /// <summary>
        /// タブを削除します。
        /// </summary>
        /// <param name="tab"></param>
        public void RemoveTab(TabInfo tab)
        {
            if (tab == null)
            {
                throw new ArgumentNullException(nameof(tab));
            }

            if (!this.tabList.Contains(tab))
            {
                throw new ArgumentException("含まれていないタブを削除しようとしました。", nameof(tab));
            }

            var nextActiveTab = this.GetNextTab(tab);
            if (nextActiveTab == null)
            {
                nextActiveTab = this.GetPreviewTab(tab);
            }

            if (this.SetActiveTab(nextActiveTab))
            {
                this.OnActiveTabChanged(new EventArgs());
            }

            if (tab.Equals(this.mousePointTab))
            {
                this.mousePointTab = null;
            }

            tab.Owner = null;

            this.tabList.Remove(tab);

            this.Invalidate();
        }

        /// <summary>
        /// アクティブなタブを削除します。
        /// </summary>
        public void RemoveActiveTab()
        {
            if (this.activeTab == null)
            {
                return;
            }

            this.RemoveTab(this.activeTab);
        }

        /// <summary>
        /// アクティブタブをセットします。
        /// </summary>
        /// <param name="index"></param>
        public void SetActiveTab(int index)
        {
            if (index < 0 || this.tabList.Count - 1 < index)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (this.SetActiveTab(this.tabList[index]))
            {
                this.Invalidate();
                this.OnActiveTabChanged(new EventArgs());
            }
        }

        /// <summary>
        /// ヘッダ領域を再描画します。
        /// </summary>
        public void InvalidateHeader()
        {
            this.Invalidate(this.GetHeaderRectangle());
        }

        /// <summary>
        /// タブのスクリーン領域を取得します。
        /// </summary>
        /// <returns></returns>
        internal Rectangle GetTabsScreenRectangle()
        {
            var rect = this.GetTabsRectangle();
            var p = this.PointToScreen(new Point(rect.X, rect.Y));
            var x = p.X;
            var y = p.Y;
            var w = rect.Width;
            var h = rect.Height;
            return new Rectangle(x, y, w, h);
        }

        /// <summary>
        /// タブのクライアント領域を取得します。
        /// </summary>
        /// <returns></returns>
        internal Rectangle GetTabsClientRectangle()
        {
            return this.GetTabsRectangle();
        }

        #endregion

        #region 継承メソッド

        protected override void OnHandleCreated(EventArgs e)
        {
            if (!this.DesignMode)
            {
                var form = this.GetForm();
                TabDragOperation.AddForm(form);
            }

            base.OnHandleCreated(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var tab in this.tabList)
                {
                    tab.Close();
                }
            }

            base.Dispose(disposing);
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            this.SetTabsDrawArea();
            this.SetAddTabButtonDrawArea();
            base.OnInvalidated(e);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //base.OnPaintBackground(pevent);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.Low;

            foreach (var tab in this.tabList.FindAll((t) => !t.Equals(this.activeTab)))
            {
                this.DrawTab(tab, e.Graphics);
            }

            this.GetAddTabButtonDrawMethod()(e.Graphics);

            this.contentsDrawArea.Draw(e.Graphics);

            if (this.activeTab != null)
            {
                this.DrawTab(this.activeTab, e.Graphics);
            }

            this.DrawDropPoint(e.Graphics);

            base.OnPaint(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (this.mousePointTab != null)
            {
                this.SetMousePointTab(null);
            }

            this.mouseDownTab = null;
            this.InvalidateHeader();

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
                var tab = this.GetTabFromPoint(e.X, e.Y);
                this.SetMousePointTab(tab);
                this.InvalidateHeader();

                var form = this.GetForm();
                if (form.Equals(Form.ActiveForm))
                {
                    if (this.GetHeaderRectangle().Contains(e.X, e.Y))
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
            var tab = this.GetTabFromPoint(e.X, e.Y);
            if (tab != null)
            {
                this.mouseDownTab = tab;

                if (tab.DrawArea.GetCloseButtonRectangle().Contains(e.X, e.Y))
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        this.InvalidateHeader();
                    }
                }
                else
                {
                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            if (this.SetActiveTab(tab))
                            {
                                this.Invalidate();
                                this.OnActiveTabChanged(new EventArgs());
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
            else if (this.addTabButtonDrawArea.Contents(e.X, e.Y))
            {
                this.InvalidateHeader();
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (TabDragOperation.IsBegin)
            {
                var tab = TabDragOperation.EndTabDragOperation();
                if (tab.Owner != null)
                {
                    this.InvalidateHeader();

                    if (!tab.Owner.Equals(this))
                    {
                        this.OnTabDropouted(new TabDropoutedEventArgs(tab));
                    }
                }
                else
                {
                    var screenPoint = this.PointToScreen(new Point(e.X, e.Y));
                    var form = this.GetForm();

                    if (ScreenUtil.GetTopRect(Resources.DropMaximum.Size).Contains(screenPoint))
                    {
                        this.OnTabDropouted(new TabDropoutedEventArgs(tab, screenPoint, form.ClientSize, FormWindowState.Maximized));
                    }
                    else if (ScreenUtil.GetLeftRect(Resources.DropMaximum.Size).Contains(screenPoint))
                    {
                        var screenRect = Screen.GetWorkingArea(Cursor.Position);
                        var w = (int)(screenRect.Width / 2f);
                        var h = screenRect.Height;
                        var x = screenRect.Left;
                        var y = screenRect.Top;
                        this.OnTabDropouted(new TabDropoutedEventArgs(tab, new Point(x, y), new Size(w, h), FormWindowState.Normal));
                    }
                    else if (ScreenUtil.GetRightRect(Resources.DropMaximum.Size).Contains(screenPoint))
                    {
                        var screenRect = Screen.GetWorkingArea(Cursor.Position);
                        var w = (int)(screenRect.Width / 2f);
                        var h = screenRect.Height;
                        var x = screenRect.Right - w;
                        var y = screenRect.Top;
                        this.OnTabDropouted(new TabDropoutedEventArgs(tab, new Point(x, y), new Size(w, h), FormWindowState.Normal));
                    }
                    else
                    {
                        this.OnTabDropouted(new TabDropoutedEventArgs(tab, screenPoint, form.RestoreBounds.Size, FormWindowState.Normal));
                    }
                }
            }

            this.mouseDownTab = null;

            base.OnMouseUp(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            var tab = this.GetTabFromPoint(e.X, e.Y);
            if (tab != null && tab.Equals(this.mouseDownTab))
            {
                if (e.Button == MouseButtons.Middle)
                {
                    this.OnTabCloseButtonClick(new TabEventArgs(tab));
                }
                else
                {
                    var rect = tab.DrawArea.GetCloseButtonRectangle();
                    if (rect.Contains(e.X, e.Y))
                    {
                        this.OnTabCloseButtonClick(new TabEventArgs(tab));
                    }
                }
            }
            else if (this.addTabButtonDrawArea.Contents(e.X, e.Y))
            {
                this.OnAddTabButtonMouseClick(e);
            }

            base.OnMouseClick(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var tab = this.GetTabFromPoint(e.X, e.Y);
                if (tab == null &&
                    this.GetHeaderRectangle().Contains(e.X, e.Y))
                {
                    this.OnBackgroundMouseLeftDoubleClick(new EventArgs());
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
            if (this.tabList.Count > 1)
            {
                if (this.activeTab == null)
                {
                    throw new NullReferenceException("アクティブなタブがNULLです。");
                }

                if (this.GetHeaderRectangle().Contains(e.X, e.Y))
                {
                    var index = this.tabList.IndexOf(this.activeTab);
                    if (e.Delta > 0)
                    {
                        if (index - 1 < 0)
                        {
                            this.SetActiveTab(this.tabList.Last());
                        }
                        else
                        {
                            this.SetActiveTab(this.tabList[index - 1]);
                        }
                    }
                    else
                    {
                        if (index + 1 > this.tabList.Count - 1)
                        {
                            this.SetActiveTab(this.tabList.First());
                        }
                        else
                        {
                            this.SetActiveTab(this.tabList[index + 1]);
                        }
                    }

                    this.InvalidateHeader();
                }
                else
                {
                    this.activeTab.Contents.Focus();
                }

                this.OnActiveTabChanged(new EventArgs());
            }
            else if (this.activeTab != null)
            {
                this.activeTab.Contents.Focus();
            }

            base.OnMouseWheel(e);
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            this.dropPoint = this.PointToClient(new Point(drgevent.X, drgevent.Y));
            if (this.GetHeaderRectangle().Contains(this.dropPoint.Value))
            {
                var dropX = this.dropPoint.Value.X;
                var dropY = this.dropPoint.Value.Y;
                foreach (var tab in this.tabList)
                {
                    if (tab.DrawArea.Contents(dropX, dropY))
                    {
                        if (this.SetActiveTab(tab))
                        {
                            this.OnActiveTabChanged(new EventArgs());
                            break;
                        }
                    }
                }

                drgevent.Effect = DragDropEffects.All;
                this.OnTabAreaDragOver(drgevent);
            }
            else
            {
                drgevent.Effect = DragDropEffects.None;
            }

            this.InvalidateHeader();
            base.OnDragOver(drgevent);
        }

        protected override void OnDragLeave(EventArgs e)
        {
            this.dropPoint = null;
            this.Invalidate();
            base.OnDragLeave(e);
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            if (!this.dropPoint.HasValue)
            {
                throw new Exception("ドロップされている座標がNULLです。");
            }

            if (this.GetHeaderRectangle().Contains(this.dropPoint.Value))
            {
                var dropX = this.dropPoint.Value.X;
                var dropY = this.dropPoint.Value.Y;
                var isExecuteEvent = false;
                foreach (TabInfo tab in this.tabList)
                {
                    if (tab.DrawArea.Contents(dropX, dropY))
                    {
                        if (this.IsTabLeftDrop(dropX, tab))
                        {
                            this.OnTabAreaDragDrop(new TabAreaDragEventArgs(false, this.tabList.IndexOf(tab), drgevent));
                            isExecuteEvent = true;
                            break;
                        }
                        else if (this.IsTabRightDrop(dropX, tab))
                        {
                            this.OnTabAreaDragDrop(new TabAreaDragEventArgs(false, this.tabList.IndexOf(tab) + 1, drgevent));
                            isExecuteEvent = true;
                            break;
                        }
                        else
                        {
                            this.OnTabAreaDragDrop(new TabAreaDragEventArgs(true, this.tabList.IndexOf(tab), drgevent));
                            isExecuteEvent = true;
                            break;
                        }
                    }
                }

                if (!isExecuteEvent)
                {
                    this.OnTabAreaDragDrop(new TabAreaDragEventArgs(false, this.tabList.Count, drgevent));
                }
            }

            this.dropPoint = null;
            base.OnDragDrop(drgevent);
        }

        #endregion

        #region プライベートメソッド

        private void InitializeComponent()
        {
            this.SetStyle(ControlStyles.DoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.ResizeRedraw, true);

            this.SetAddTabButtonDrawArea();
        }


        private void OnActiveTabChanged(EventArgs e)
        {
            if (this.ActiveTabChanged != null)
            {
                this.ActiveTabChanged(this, e);
            }
        }

        private void OnTabCloseButtonClick(TabEventArgs e)
        {
            if (this.TabCloseButtonClick != null)
            {
                this.TabCloseButtonClick(this, e);
            }
        }

        private void OnTabDropouted(TabDropoutedEventArgs e)
        {
            if (this.TabDropouted != null)
            {
                this.TabDropouted(this, e);
            }
        }

        private void OnBackgroundMouseLeftDoubleClick(EventArgs e)
        {
            if (this.BackgroundMouseDoubleLeftClick != null)
            {
                this.BackgroundMouseDoubleLeftClick(this, e);
            }
        }

        private void OnTabAreaDragOver(DragEventArgs e)
        {
            if (this.TabAreaDragOver != null)
            {
                this.TabAreaDragOver(this, e);
            }
        }

        private void OnTabAreaDragDrop(TabAreaDragEventArgs e)
        {
            if (this.TabAreaDragDrop != null)
            {
                this.TabAreaDragDrop(this, e);
            }
        }

        private void OnAddTabButtonMouseClick(MouseEventArgs e)
        {
            if (this.AddTabButtonMouseClick != null)
            {
                this.AddTabButtonMouseClick(this, e);
            }
        }

        private bool SetMousePointTab(TabInfo tab)
        {
            if (tab != null)
            {
                if (!tab.Equals(this.mousePointTab))
                {
                    this.mousePointTab = tab;
                    return true;
                }
            }
            else
            {
                if (this.mousePointTab != null)
                {
                    this.mousePointTab = null;
                    return true;
                }
            }

            return false;
        }

        private bool SetActiveTab(TabInfo tab)
        {
            if (tab != null)
            {
                if (!tab.Equals(this.activeTab))
                {
                    var container = this.GetContainer();
                    container.SuspendLayout();
                    container.ClearContents();
                    container.SetContents(tab.Contents);
                    container.ResumeLayout();
                    this.activeTab = tab;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                var container = this.GetContainer();
                container.SuspendLayout();
                container.ClearContents();
                container.ResumeLayout();
                if (this.activeTab != null)
                {
                    this.activeTab = null;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private TabInfo GetPreviewTab(TabInfo tab)
        {
            var index = this.tabList.IndexOf(tab);

            if (index > 0)
            {
                return this.tabList[index - 1];
            }
            else
            {
                return null;
            }
        }

        private TabInfo GetNextTab(TabInfo tab)
        {
            var index = this.tabList.IndexOf(tab);

            if (index < this.tabList.Count - 1)
            {
                return this.tabList[index + 1];
            }
            else
            {
                return null;
            }
        }

        private TabInfo GetTabFromPoint(int x, int y)
        {
            return this.tabList.FirstOrDefault<TabInfo>((t) => t.DrawArea.Contents(x, y));
        }

        private ContentsContainer GetContainer()
        {
            var form = this.GetForm();
            var container = this.GetContainer(form);
            if (container != null)
            {
                return container;
            }
            else
            {
                throw new NullReferenceException("フォーム内にコンテンツコンテナが存在しません。");
            }
        }

        private ContentsContainer GetContainer(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                if (child is ContentsContainer)
                {
                    return (ContentsContainer)child;
                }
                else if (child.Controls.Count > 0)
                {
                    var result = this.GetContainer(child);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        private Form GetForm()
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

        private Rectangle GetHeaderRectangle()
        {
            var rect = this.ClientRectangle;
            var x = rect.X;
            var y = rect.Y;
            var w = rect.Width;
            var h = this.tabHeight;
            return new Rectangle(x, y, w, h);
        }

        private Rectangle GetTabsRectangle()
        {
            var rect = this.ClientRectangle;
            var x = rect.X + TABS_MARGIN;
            var y = rect.Y;
            var w = rect.Width - this.tabsRightOffset - TABS_MARGIN * 2 - TAB_MARGIN - this.addTabButtonDrawArea.Width;
            var h = this.tabHeight;
            return new Rectangle(x, y, w, h);
        }

        private void SetTabsDrawArea()
        {
            if (TabDragOperation.IsBegin)
            {
                this.tabList.Sort((a, b) => this.SortTabList(a, b));
                var rect = this.GetTabsRectangle();
                var x = rect.X;
                foreach (var tab in this.tabList)
                {
                    if (!TabDragOperation.IsTarget(tab))
                    {
                        tab.DrawArea.X = x;
                        tab.DrawArea.Y = rect.Y;
                    }
                    x += (tab.DrawArea.Width + TAB_MARGIN);
                }
            }
            else
            {
                var rect = this.GetTabsRectangle();
                var w = this.GetTabWidth();
                var x = rect.X;
                foreach (var tab in this.tabList)
                {
                    tab.DrawArea.X = x;
                    tab.DrawArea.Y = rect.Y;
                    tab.DrawArea.Width = w;
                    x += (w + TAB_MARGIN);
                }
            }
        }

        private void SetAddTabButtonDrawArea()
        {
            if (this.tabList.Count > 0)
            {
                var tab = this.tabList.Last();
                if (TabDragOperation.IsTarget(tab))
                {
                    if (this.tabList.Count > 1)
                    {
                        int index = this.tabList.IndexOf(tab) - 1;
                        this.addTabButtonDrawArea.X = this.tabList[index].DrawArea.Right + TABS_MARGIN;
                    }
                    else
                    {
                        this.addTabButtonDrawArea.X = this.ClientRectangle.X + TABS_MARGIN;
                    }
                }
                else
                {
                    this.addTabButtonDrawArea.X = tab.DrawArea.Right + TABS_MARGIN;
                }

                var rect = this.GetTabsRectangle();
                if (this.addTabButtonDrawArea.Right > rect.Right)
                {
                    this.addTabButtonDrawArea.X = rect.Right + TAB_MARGIN;
                }
            }
            else
            {
                this.addTabButtonDrawArea.X = this.ClientRectangle.X + TABS_MARGIN;
            }
        }

        private int GetTabWidth()
        {
            var rect = this.GetTabsRectangle();

            var defAllTabW = TAB_MAXIMUM_WIDTH * this.tabList.Count + TAB_MARGIN * (this.tabList.Count - 1);

            if (defAllTabW > rect.Width)
            {
                float tabW = (rect.Width - TAB_MARGIN * (this.tabList.Count - 1)) / this.tabList.Count;
                return (int)Math.Max(tabW, TAB_MINIMUM_WIDTH);
            }
            else
            {
                return TAB_MAXIMUM_WIDTH;
            }
        }

        private int SortTabList(TabInfo a, TabInfo b)
        {
            if (TabDragOperation.IsTarget(a))
            {
                var margin = b.DrawArea.Width * TAB_DRAG_OVERLAP_RATE;
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
                var margin = a.DrawArea.Width * TAB_DRAG_OVERLAP_RATE;
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

        private void DrawTab(TabInfo tab, Graphics g)
        {
            this.GetDrawTabMethod(tab)(g);
            this.GetDrawCloseButtonMethod(tab)(g);
            if (tab.Icon == null)
            {
                return;
            }

            var args = new DrawTabEventArgs();
            args.Graphics = g;
            args.Font = this.tabPalette.TitleFont;
            args.TitleColor = this.tabPalette.TitleColor;
            args.TitleFormatFlags = this.tabPalette.TitleFormatFlags;
            args.TextRectangle = tab.DrawArea.GetContentsRectangle();
            args.IconRectangle = tab.DrawArea.GetIconRectangle(tab.Icon);
            args.CloseButtonRectangle = tab.DrawArea.GetCloseButtonRectangle();
            args.TextStyle = DrawTextUtil.TextStyle.Glowing;
            tab.DrawingTabContents(args);
        }

        private void DrawDropPoint(Graphics g)
        {
            if (this.dropPoint == null)
            {
                return;
            }

            var dropX = this.dropPoint.Value.X;
            var dropY = this.dropPoint.Value.Y;

            foreach (var tab in this.tabList)
            {
                if (tab.DrawArea.Contents(dropX, dropY))
                {
                    if (this.IsTabLeftDrop(dropX, tab))
                    {
                        var img = Resources.DropArrow;
                        var x = (tab.DrawArea.Left - TAB_MARGIN / 2f) - img.Width / 2f;
                        var y = 0;
                        g.DrawImage(img, x, y, img.Width, img.Height);
                        return;
                    }
                    else if (this.IsTabRightDrop(dropX, tab))
                    {
                        var img = Resources.DropArrow;
                        var x = (tab.DrawArea.Right - TAB_MARGIN / 2f) - img.Width / 2f;
                        var y = 0;
                        g.DrawImage(img, x, y, img.Width, img.Height);
                        return;
                    }
                }
            }
        }

        private Action<Graphics> GetDrawTabMethod(TabInfo tab)
        {
            if (tab.Equals(this.activeTab))
            {
                return tab.DrawArea.DrawActiveTab;
            }
            else if (tab.Equals(this.mousePointTab))
            {
                return tab.DrawArea.DrawMousePointTab;
            }
            else
            {
                return tab.DrawArea.DrawInactiveTab;
            }
        }

        private Action<Graphics> GetDrawCloseButtonMethod(TabInfo tab)
        {
            var p = this.PointToClient(Cursor.Position);
            var rect = tab.DrawArea.GetCloseButtonRectangle();

            if (rect.Contains(p))
            {
                if (tab.Equals(this.activeTab))
                {
                    return tab.DrawArea.DrawActiveMousePointTabCloseButton;
                }
                else
                {
                    return tab.DrawArea.DrawInactiveMousePointTabCloseButton;
                }

            }
            else
            {
                if (tab.Equals(this.activeTab))
                {
                    return tab.DrawArea.DrawActiveTabCloseButton;
                }
                else
                {
                    return tab.DrawArea.DrawInactiveTabCloseButton;
                }
            }
        }

        private Action<Graphics> GetAddTabButtonDrawMethod()
        {
            var p = this.PointToClient(Cursor.Position);
            if (this.addTabButtonDrawArea.Contents(p))
            {
                return this.addTabButtonDrawArea.DrawMousePointImage;
            }
            else
            {
                return this.addTabButtonDrawArea.DrawInactiveImage;
            }
        }

        private bool IsTabLeftDrop(int x, TabInfo tab)
        {
            return x > tab.DrawArea.Left && tab.DrawArea.Left + tab.DrawArea.Width / 3f > x;
        }

        private bool IsTabRightDrop(int x, TabInfo tab)
        {
            return tab.DrawArea.Right > x && x > tab.DrawArea.Right - tab.DrawArea.Width / 3f;
        }

        #endregion
    }
}
