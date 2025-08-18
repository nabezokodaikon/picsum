using SWF.Core.Base;
using SWF.Core.ResourceAccessor;
using SWF.UIComponent.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using WinApi;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブ切替コントロール
    /// </summary>

    public sealed partial class TabSwitch
        : BasePaintingControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new string Text
        {
            get
            {
                return base.Text;
            }
            private set
            {
                base.Text = value;
            }
        }

        // タブの高さ
        private int GetTabHeight()
        {
            const int TAB_DEFAULT_HEIGHT = 29;
            var form = this.GetForm();
            var scale = WindowUtil.GetCurrentWindowScale(form);
            return (int)(TAB_DEFAULT_HEIGHT * scale);
        }

        // タブ同士のの間隔
        private int GetTabMargin()
        {
            const float TAB_MARGIN = 2;
            var form = this.GetForm();
            var scale = WindowUtil.GetCurrentWindowScale(form);
            return (int)(TAB_MARGIN * scale);
        }

        // タブの最大幅
        private int GetTabMaximumWidth()
        {
            const float TAB_MAXIMUM_WIDTH = 256;
            var form = this.GetForm();
            var scale = WindowUtil.GetCurrentWindowScale(form);
            return (int)(TAB_MAXIMUM_WIDTH * scale);
        }

        // タブの最小幅
        private int GetTabMinimumWidth()
        {
            const float TAB_MINIMUM_WIDTH = 1;
            var form = this.GetForm();
            var scale = WindowUtil.GetCurrentWindowScale(form);
            return (int)(TAB_MINIMUM_WIDTH * scale);
        }

        // タブ領域の間隔
        private int GetTabsMargin()
        {
            const float TABS_MARGIN = 8;
            var form = this.GetForm();
            var scale = WindowUtil.GetCurrentWindowScale(form);
            return (int)(TABS_MARGIN * scale);
        }

        private int GetTabRightMargin()
        {
            const int TAB_RIGHT_DEFAULT_MARGIN = 32;
            var form = this.GetForm();
            var scale = WindowUtil.GetCurrentWindowScale(form);
            return (int)(TAB_RIGHT_DEFAULT_MARGIN * scale);
        }

        public static int GetTabCloseButtonCanDrawWidth(Control owner)
        {
            ArgumentNullException.ThrowIfNull(owner, nameof(owner));

            const float TAB_CLOSE_BUTTON_CAN_DRAW_WIDTH = 64;
            var scale = WindowUtil.GetCurrentWindowScale(owner);
            return (int)(TAB_CLOSE_BUTTON_CAN_DRAW_WIDTH * scale);
        }

        // タブを移動するとみなす重なりの比率
        private const float TAB_DRAG_OVERLAP_RATE = 0.4f;

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

        // タブ情報リスト
        private readonly List<TabInfo> _tabList = [];

        // マウスダウンしたタブ
        private TabInfo _mouseDownTab = null;

        // マウスポイントされているタブ
        private TabInfo _mousePointTab = null;

        // アクティブなタブ
        private TabInfo _activeTab = null;

        // ドロップされている座標
        private Point? _dropPoint = null;

        // タブ追加ボタン病が暮らす
        private readonly AddTabButtonDrawArea _addTabButtonDrawArea;

        // コンテンツ描画クラス
        private readonly PageDrawArea _pageDrawArea;

        private int GetTabsRightOffset()
        {
            var size = WindowUtil.GetControlBoxSize(this.GetForm().Handle);
            var margin = this.GetTabRightMargin();
            return size.Width + margin;
        }

        /// <summary>
        /// タブの存在を確認します。
        /// </summary>
        public bool HasTab
        {
            get
            {
                return (this._tabList.Count > 0);
            }
        }

        /// <summary>
        /// アクティブタブ
        /// </summary>
        public TabInfo ActiveTab
        {
            get
            {
                return this._activeTab;
            }
        }

        /// <summary>
        /// アクティブタブのインデックス
        /// </summary>
        public int ActiveTabIndex
        {
            get
            {
                if (this._activeTab != null)
                {
                    return this._tabList.IndexOf(this._activeTab);
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
                return this._tabList.Count;
            }
        }

        public bool IsBeginTabDragOperation
        {
            get
            {
                return TabDragOperation.IsBegin;
            }
        }

        public TabSwitch()
        {
            this._addTabButtonDrawArea = new(this);
            this._pageDrawArea = new(this);

            this.Loaded += this.TabSwitch_Loaded;
            this.Invalidated += this.TabSwitch_Invalidated;
            this.Paint += this.TabSwitch_Paint;
            this.LostFocus += this.TabSwitch_LostFocus;
            this.MouseLeave += this.TabSwitch_MouseLeave;
            this.MouseMove += this.TabSwitch_MouseMove;
            this.MouseDown += this.TabSwitch_MouseDown;
            this.MouseUp += this.TabSwitch_MouseUp;
            this.MouseClick += this.TabSwitch_MouseClick;
            this.MouseDoubleClick += this.TabSwitch_MouseDoubleClick;
            this.MouseWheel += this.TabSwitch_MouseWheel;
            this.DragOver += this.TabSwitch_DragOver;
            this.DragLeave += this.TabSwitch_DragLeave;
            this.DragDrop += this.TabSwitch_DragDrop;
        }

        public TabInfo[] GetInactiveTabs()
        {
            var tabs = this._tabList
                    .Where(tab => tab != this._activeTab)
                    .ToArray();
            return tabs;
        }

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
            ArgumentNullException.ThrowIfNull(tab, nameof(tab));

            if (this._tabList.Contains(tab))
            {
                throw new ArgumentException("既に含まれているタブを追加しようとしました。", nameof(tab));
            }

            tab.Owner = this;

            this._tabList.Add(tab);

            if (this.SetActiveTab(tab))
            {
                this.OnActiveTabChanged(EventArgs.Empty);
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
            ArgumentOutOfRangeException.ThrowIfLessThan(index, 0, nameof(index));
            ArgumentNullException.ThrowIfNull(tab, nameof(tab));

            if (this._tabList.Contains(tab))
            {
                throw new ArgumentException("既に含まれているタブを挿入しようとしました。", nameof(tab));
            }

            if (index > this._tabList.Count - 1)
            {
                this.AddTab(tab);
            }
            else
            {
                tab.Owner = this;

                this._tabList.Insert(index, tab);

                if (this.SetActiveTab(tab))
                {
                    this.OnActiveTabChanged(EventArgs.Empty);
                }

                this.Invalidate();
            }
        }

        /// <summary>
        /// タブにページを追加します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public T AddTab<T>(IPageParameter param) where T : PagePanel
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            var tab = new TabInfo(this, param);
            this.AddTab(tab);
            return (T)tab.Page;
        }

        /// <summary>
        /// タブにページを挿入します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T InsertTab<T>(int index, IPageParameter param)
            where T : PagePanel
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            var tab = new TabInfo(this, param);
            this.InsertTab(index, tab);
            return (T)tab.Page;
        }

        /// <summary>
        /// タブにページを上書きします。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public T OverwriteTab<T>(IPageParameter param) where T : PagePanel
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            if (this._activeTab != null)
            {
                this._activeTab.ClearPage();
                this._activeTab.OverwritePage(param);

                var container = this.GetContainer();
                container.SuspendLayout();
                container.ClearPage();
                container.SetPage(this._activeTab.Page);
                container.ResumeLayout(false);

                return (T)this._activeTab.Page;
            }
            else
            {
                return this.AddTab<T>(param);
            }
        }

        /// <summary>
        /// 履歴の前のページを表示します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T SetPreviewHistory<T>() where T : PagePanel
        {
            if (this._activeTab == null)
            {
                throw new InvalidOperationException("アクティブなタブが存在しません。");
            }

            if (!this._activeTab.HasPreviewPage)
            {
                throw new InvalidOperationException("コンテンツの前の履歴が存在しません。");
            }

            this._activeTab.ClearPage();
            this._activeTab.CreatePreviewPage();

            var container = this.GetContainer();
            container.SuspendLayout(); ;
            container.ClearPage();
            container.SetPage(this._activeTab.Page);
            container.ResumeLayout(false);

            this.Invalidate();

            return (T)this._activeTab.Page;
        }

        /// <summary>
        /// 履歴の次のページを表示します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T SetNextPageHistory<T>() where T : PagePanel
        {
            if (this._activeTab == null)
            {
                throw new InvalidOperationException("アクティブなタブが存在しません。");
            }

            if (!this._activeTab.HasNextPage)
            {
                throw new InvalidOperationException("ページの次の履歴が存在しません。");
            }

            this._activeTab.ClearPage();
            this._activeTab.CreateNextPage();

            var container = this.GetContainer();
            container.SuspendLayout();
            container.ClearPage();
            container.SetPage(this._activeTab.Page);
            container.ResumeLayout(false);

            return (T)this._activeTab.Page;
        }

        /// <summary>
        /// 現在のページのクローンを取得します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T CloneCurrentPage<T>()
            where T : PagePanel
        {
            if (this._activeTab == null)
            {
                throw new InvalidOperationException("アクティブなタブが存在しません。");
            }

            if (!this._activeTab.HasPage)
            {
                throw new InvalidOperationException("ページが存在しません。");
            }

            this._activeTab.ClearPage();
            this._activeTab.CloneCurrentPage();

            var container = this.GetContainer();
            container.SuspendLayout();
            container.ClearPage();
            container.SetPage(this._activeTab.Page);
            container.ResumeLayout(false);

            return (T)this._activeTab.Page;
        }

        /// <summary>
        /// タブを削除します。
        /// </summary>
        /// <param name="tab"></param>
        public void RemoveTab(TabInfo tab)
        {
            ArgumentNullException.ThrowIfNull(tab, nameof(tab));

            if (!this._tabList.Contains(tab))
            {
                throw new ArgumentException("含まれていないタブを削除しようとしました。", nameof(tab));
            }

            var nextActiveTab = this.GetNextTab(tab) ?? this.GetPreviewTab(tab);

            if (this.SetActiveTab(nextActiveTab))
            {
                this.OnActiveTabChanged(EventArgs.Empty);
            }

            if (tab == this._mousePointTab)
            {
                this._mousePointTab = null;
            }

            tab.Owner = null;

            this._tabList.Remove(tab);

            this.Invalidate();
        }

        /// <summary>
        /// アクティブなタブを削除します。
        /// </summary>
        public void RemoveActiveTab()
        {
            if (this._activeTab == null)
            {
                return;
            }

            this.RemoveTab(this._activeTab);
        }

        /// <summary>
        /// アクティブタブをセットします。
        /// </summary>
        /// <param name="index"></param>
        public void SetActiveTab(int index)
        {
            if (index < 0 || this._tabList.Count - 1 < index)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (this.SetActiveTab(this._tabList[index]))
            {
                this.Invalidate();
                this.OnActiveTabChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// ヘッダ領域を再描画します。
        /// </summary>
        public void InvalidateHeader()
        {
            this.Invalidate(this.GetHeaderRectangle());
        }

        public void CallEndTabDragOperation()
        {
            if (TabDragOperation.IsBegin)
            {
                var tab = TabDragOperation.EndTabDragOperation();
                if (tab.Owner != null)
                {
                    this.InvalidateHeader();
                    this.OnTabDropouted(new TabDropoutedEventArgs(tab));
                }
                else
                {
                    var scale = WindowUtil.GetCurrentWindowScale(this);
                    var screenPoint = Cursor.Position;
                    var form = this.GetForm();

                    if (ScreenUtil.GetLeftBorderRect(scale).Contains(screenPoint))
                    {
                        var screenRect = Screen.GetWorkingArea(screenPoint);
                        var w = (int)(screenRect.Width / 2f + 14 * scale);
                        var h = (int)(screenRect.Height + 8 * scale);
                        var x = (int)(screenRect.Left - 7 * scale);
                        var y = screenRect.Top;
                        this.OnTabDropouted(new TabDropoutedEventArgs(tab, new Point(x, y), new Size(w, h), FormWindowState.Normal));
                    }
                    else if (ScreenUtil.GetRightBorderRect(scale).Contains(screenPoint))
                    {
                        var screenRect = Screen.GetWorkingArea(screenPoint);
                        var w = (int)(screenRect.Width / 2f + 14 * scale);
                        var h = (int)(screenRect.Height + 8 * scale);
                        var x = screenRect.X + (int)(screenRect.Width / 2f) - (int)(7 * scale);
                        var y = screenRect.Top;
                        this.OnTabDropouted(new TabDropoutedEventArgs(tab, new Point(x, y), new Size(w, h), FormWindowState.Normal));
                    }
                    else if (form.WindowState == FormWindowState.Normal)
                    {
                        // マウスカーソルの位置にタブが来るようにずらします。
                        this.OnTabDropouted(new TabDropoutedEventArgs(tab, new Point(screenPoint.X - 128, screenPoint.Y - 24), form.ClientSize, FormWindowState.Normal));
                    }
                    else if (form.WindowState == FormWindowState.Maximized)
                    {
                        // マウスカーソルの位置にタブが来るようにずらします。
                        this.OnTabDropouted(new TabDropoutedEventArgs(tab, new Point(screenPoint.X - 128, screenPoint.Y - 24), form.RestoreBounds.Size, FormWindowState.Normal));
                    }
                    else
                    {
                        throw new NotImplementedException("未定義のタブ操作です。");
                    }
                }
            }

            this._mouseDownTab = null;
        }

        /// <summary>
        /// タブのスクリーン領域を取得します。
        /// </summary>
        /// <returns></returns>
        internal RectangleF GetTabsScreenRectangle()
        {
            var rect = this.GetTabsRectangle();
            var p = this.PointToScreen(new Point((int)rect.X, (int)rect.Y));
            var x = p.X;
            var y = p.Y;
            var w = rect.Width;
            var h = rect.Height;
            return new RectangleF(x, y, w, h);
        }

        /// <summary>
        /// タブのクライアント領域を取得します。
        /// </summary>
        /// <returns></returns>
        internal RectangleF GetTabsClientRectangle()
        {
            return this.GetTabsRectangle();
        }

        private void TabSwitch_Loaded(object sender, EventArgs e)
        {
            var form = this.GetForm();
            TabDragOperation.AddForm(form);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var tab in this._tabList)
                {
                    tab.Close();
                }
            }

            base.Dispose(disposing);
        }

        private void TabSwitch_Invalidated(object sender, InvalidateEventArgs e)
        {
            this.SetTabsDrawArea();
            this.SetAddTabButtonDrawArea();
        }

        private void TabSwitch_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;

            foreach (var tab in this._tabList.FindAll((t) => t != this._activeTab))
            {
                this.DrawTab(tab, e.Graphics);
            }

            this.GetAddTabButtonDrawMethod()(e.Graphics);

            this._pageDrawArea.Draw(e.Graphics);

            if (this._activeTab != null)
            {
                this.DrawTab(this._activeTab, e.Graphics);
            }

            this.DrawDropPoint(e.Graphics);
        }

        private void TabSwitch_LostFocus(object sender, EventArgs e)
        {
            if (TabDragOperation.IsBegin
                && !TabDragOperation.TabDragForm.Visible
                && TabDragOperation.Tab.Owner == this
                && TabDragOperation.TabDragForm.TabSwitch != null)
            {
                TabDragOperation.TabDragForm.TabSwitch.CallEndTabDragOperation();
            }
        }

        private void TabSwitch_MouseLeave(object sender, EventArgs e)
        {
            if (this._mousePointTab != null)
            {
                this.SetMousePointTab(null);
            }

            this._mouseDownTab = null;
            this.InvalidateHeader();
        }

        private void TabSwitch_MouseMove(object sender, MouseEventArgs e)
        {
            if (TabDragOperation.IsBegin)
            {
                TabDragOperation.MoveTab();
            }
            else
            {
                var form = this.GetForm();
                if (form == Form.ActiveForm)
                {
                    if (this.GetHeaderRectangle().Contains(e.X, e.Y))
                    {
                        //this.Focus();
                    }
                }

                var tab = this.GetTabFromPoint(e.X, e.Y);
                this.SetMousePointTab(tab);
                this.InvalidateHeader();

                if (tab == null && !this._addTabButtonDrawArea.Page(e.X, e.Y) && e.Button == MouseButtons.Left)
                {
                    WinApiMembers.ReleaseCapture();
                    _ = WinApiMembers.SendMessage(form.Handle, WinApiMembers.WM_NCLBUTTONDOWN, WinApiMembers.HTCAPTION, 0);
                }
            }
        }

        private void TabSwitch_MouseDown(object sender, MouseEventArgs e)
        {
            var tab = this.GetTabFromPoint(e.X, e.Y);
            if (tab != null)
            {
                this._mouseDownTab = tab;

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
                                this.OnActiveTabChanged(EventArgs.Empty);
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
            else if (this._addTabButtonDrawArea.Page(e.X, e.Y))
            {
                this.InvalidateHeader();
            }
        }

        private void TabSwitch_MouseUp(object sender, MouseEventArgs e)
        {
            this.CallEndTabDragOperation();
        }

        private void TabSwitch_MouseClick(object sender, MouseEventArgs e)
        {
            var tab = this.GetTabFromPoint(e.X, e.Y);
            if (tab != null && tab == this._mouseDownTab)
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
            else if (this._addTabButtonDrawArea.Page(e.X, e.Y))
            {
                this.OnAddTabButtonMouseClick(e);
            }
        }

        private void TabSwitch_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var tab = this.GetTabFromPoint(e.X, e.Y);
                var tabAddButtonRect = new RectangleF(
                    this._addTabButtonDrawArea.X, this._addTabButtonDrawArea.Y, this._addTabButtonDrawArea.Width, this._addTabButtonDrawArea.Height);
                if (tab == null
                    && this.GetHeaderRectangle().Contains(e.X, e.Y)
                    && !tabAddButtonRect.Contains(e.X, e.Y))
                {
                    this.OnBackgroundMouseLeftDoubleClick(EventArgs.Empty);
                }
            }
        }

        private void TabSwitch_MouseWheel(object sender, MouseEventArgs e)
        {
            if (this._tabList.Count > 1)
            {
                if (this._activeTab == null)
                {
                    throw new InvalidOperationException("アクティブなタブがNULLです。");
                }

                if (this.GetHeaderRectangle().Contains(e.X, e.Y))
                {
                    var index = this._tabList.IndexOf(this._activeTab);
                    if (e.Delta > 0)
                    {
                        if (index - 1 < 0)
                        {
                            this.SetActiveTab(this._tabList.Last());
                        }
                        else
                        {
                            this.SetActiveTab(this._tabList[index - 1]);
                        }
                    }
                    else
                    {
                        if (index + 1 > this._tabList.Count - 1)
                        {
                            this.SetActiveTab(this._tabList.First());
                        }
                        else
                        {
                            this.SetActiveTab(this._tabList[index + 1]);
                        }
                    }

                    this.InvalidateHeader();
                }

                this.OnActiveTabChanged(EventArgs.Empty);
            }
        }

        private void TabSwitch_DragOver(object sender, DragEventArgs e)
        {
            this._dropPoint = this.PointToClient(new Point(e.X, e.Y));
            if (this.GetHeaderRectangle().Contains(this._dropPoint.Value))
            {
                var dropX = this._dropPoint.Value.X;
                var dropY = this._dropPoint.Value.Y;
                foreach (var tab in this._tabList)
                {
                    if (tab.DrawArea.Contains(dropX, dropY))
                    {
                        if (this.SetActiveTab(tab))
                        {
                            this.OnActiveTabChanged(EventArgs.Empty);
                            break;
                        }
                    }
                }

                e.Effect = DragDropEffects.Copy;
                this.OnTabAreaDragOver(e);
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }

            this.InvalidateHeader();
        }

        private void TabSwitch_DragLeave(object sender, EventArgs e)
        {
            this._dropPoint = null;
            this.Invalidate();
        }

        private void TabSwitch_DragDrop(object sender, DragEventArgs e)
        {
            if (!this._dropPoint.HasValue)
            {
                throw new InvalidOperationException("ドロップされている座標がNULLです。");
            }

            if (this.GetHeaderRectangle().Contains(this._dropPoint.Value))
            {
                var dropX = this._dropPoint.Value.X;
                var dropY = this._dropPoint.Value.Y;
                var isExecuteEvent = false;
                foreach (var tab in this._tabList)
                {
                    if (tab.DrawArea.Contains(dropX, dropY))
                    {
                        if (this.IsTabLeftDrop(dropX, tab))
                        {
                            this.OnTabAreaDragDrop(new TabAreaDragEventArgs(false, this._tabList.IndexOf(tab), e));
                            isExecuteEvent = true;
                            break;
                        }
                        else if (this.IsTabRightDrop(dropX, tab))
                        {
                            this.OnTabAreaDragDrop(new TabAreaDragEventArgs(false, this._tabList.IndexOf(tab) + 1, e));
                            isExecuteEvent = true;
                            break;
                        }
                        else
                        {
                            this.OnTabAreaDragDrop(new TabAreaDragEventArgs(true, this._tabList.IndexOf(tab), e));
                            isExecuteEvent = true;
                            break;
                        }
                    }
                }

                if (!isExecuteEvent)
                {
                    this.OnTabAreaDragDrop(new TabAreaDragEventArgs(false, this._tabList.Count, e));
                }
            }

            this._dropPoint = null;
        }

        private void OnActiveTabChanged(EventArgs e)
        {
            this.ActiveTabChanged?.Invoke(this, e);

        }

        private void OnTabCloseButtonClick(TabEventArgs e)
        {
            this.TabCloseButtonClick?.Invoke(this, e);
        }

        private void OnTabDropouted(TabDropoutedEventArgs e)
        {
            this.TabDropouted?.Invoke(this, e);
        }

        private void OnBackgroundMouseLeftDoubleClick(EventArgs e)
        {
            this.BackgroundMouseDoubleLeftClick?.Invoke(this, e);
        }

        private void OnTabAreaDragOver(DragEventArgs e)
        {
            this.TabAreaDragOver?.Invoke(this, e);
        }

        private void OnTabAreaDragDrop(TabAreaDragEventArgs e)
        {
            this.TabAreaDragDrop?.Invoke(this, e);
        }

        private void OnAddTabButtonMouseClick(MouseEventArgs e)
        {
            this.AddTabButtonMouseClick?.Invoke(this, e);
        }

        private bool SetMousePointTab(TabInfo tab)
        {
            if (tab != null)
            {
                if (tab != this._mousePointTab)
                {
                    this._mousePointTab = tab;
                    return true;
                }
            }
            else
            {
                if (this._mousePointTab != null)
                {
                    this._mousePointTab = null;
                    return true;
                }
            }

            return false;
        }

        private bool SetActiveTab(TabInfo tab)
        {
            if (tab != null)
            {
                if (tab != this._activeTab)
                {
                    var container = this.GetContainer();
                    container.SuspendLayout();
                    container.ClearPage();
                    container.SetPage(tab.Page);
                    container.ResumeLayout(false);

                    this._activeTab = tab;
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
                container.ClearPage();
                container.ResumeLayout(false);

                if (this._activeTab != null)
                {
                    this._activeTab = null;
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
            var index = this._tabList.IndexOf(tab);

            if (index > 0)
            {
                return this._tabList[index - 1];
            }
            else
            {
                return null;
            }
        }

        private TabInfo GetNextTab(TabInfo tab)
        {
            var index = this._tabList.IndexOf(tab);

            if (index < this._tabList.Count - 1)
            {
                return this._tabList[index + 1];
            }
            else
            {
                return null;
            }
        }

        private TabInfo GetTabFromPoint(int x, int y)
        {
            return this._tabList.FirstOrDefault<TabInfo>((t) => t.DrawArea.Contains(x, y));
        }

        private PageContainer GetContainer()
        {
            var form = this.GetForm();
            var container = this.GetContainer(form);
            if (container != null)
            {
                return container;
            }
            else
            {
                throw new NotSupportedException("フォーム内にコンテンツコンテナが存在しません。");
            }
        }

        private PageContainer GetContainer(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                if (child is PageContainer container)
                {
                    return container;
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

            if (ctl is Form form)
            {
                return form;
            }
            else
            {
                throw new InvalidOperationException("タブスイッチはフォームに格納されていません。");
            }
        }

        private Rectangle GetHeaderRectangle()
        {
            var rect = this.ClientRectangle;
            var x = rect.X;
            var y = rect.Y;
            var w = rect.Width;
            var h = this.GetTabHeight();
            return new Rectangle(x, y, w, h);
        }

        private RectangleF GetTabsRectangle()
        {
            var tabMargin = this.GetTabMargin();
            var tabsMargin = this.GetTabsMargin();
            var tabsRightOffset = this.GetTabsRightOffset();
            var rect = this.ClientRectangle;
            var x = rect.X + tabsMargin;
            var y = rect.Y;
            var w = rect.Width - tabsRightOffset - tabsMargin * 2 - tabMargin - this._addTabButtonDrawArea.Width;
            var h = this.GetTabHeight();
            return new RectangleF(x, y, w, h);
        }

        private void SetTabsDrawArea()
        {
            if (TabDragOperation.IsBegin)
            {
                this._tabList.Sort((a, b) => this.SortTabList(a, b));
                var rect = this.GetTabsRectangle();
                var w = this.GetTabWidth();
                var x = rect.X;
                var tabMargin = this.GetTabMargin();
                foreach (var tab in this._tabList)
                {
                    if (!TabDragOperation.IsTarget(tab))
                    {
                        tab.DrawArea.X = x;
                        tab.DrawArea.Y = rect.Y;
                    }
                    tab.DrawArea.Width = w;
                    x += (tab.DrawArea.Width + tabMargin);
                }
            }
            else
            {
                var rect = this.GetTabsRectangle();
                var w = this.GetTabWidth();
                var x = rect.X;
                var tabMargin = this.GetTabMargin();
                foreach (var tab in this._tabList)
                {
                    tab.DrawArea.X = x;
                    tab.DrawArea.Y = rect.Y;
                    tab.DrawArea.Width = w;
                    x += (w + tabMargin);
                }
            }
        }

        private void SetAddTabButtonDrawArea()
        {
            var tabsMargin = this.GetTabsMargin();

            if (this._tabList.Count > 0)
            {
                var tab = this._tabList.Last();
                if (TabDragOperation.IsTarget(tab))
                {
                    if (this._tabList.Count > 1)
                    {
                        var index = this._tabList.IndexOf(tab) - 1;
                        this._addTabButtonDrawArea.X = this._tabList[index].DrawArea.Right + tabsMargin;
                    }
                    else
                    {
                        this._addTabButtonDrawArea.X = this.ClientRectangle.X + tabsMargin;
                    }
                }
                else
                {
                    this._addTabButtonDrawArea.X = tab.DrawArea.Right + tabsMargin;
                }

                var rect = this.GetTabsRectangle();
                if (this._addTabButtonDrawArea.Right > rect.Right)
                {
                    var tabMargin = this.GetTabMargin();
                    this._addTabButtonDrawArea.X = rect.Right + tabMargin;
                }
            }
            else
            {
                this._addTabButtonDrawArea.X = this.ClientRectangle.X + tabsMargin;
            }
        }

        private float GetTabWidth()
        {
            var tabMargin = this.GetTabMargin();
            var tabMaximumWidth = this.GetTabMaximumWidth();
            var rect = this.GetTabsRectangle();

            var defAllTabW = tabMaximumWidth * this._tabList.Count + tabMargin * (this._tabList.Count - 1);

            if (defAllTabW > rect.Width)
            {
                float tabW;
                if (this._tabList.Count > 0)
                {
                    tabW = (rect.Width - tabMargin * (this._tabList.Count - 1)) / (float)this._tabList.Count;
                }
                else
                {
                    tabW = rect.Width;
                }

                var tabMinimumWidth = this.GetTabMinimumWidth();
                return Math.Max(tabW, tabMinimumWidth);
            }
            else
            {
                return tabMaximumWidth;
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
                    else
                    {
                        return -1;
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
                    else
                    {
                        return 1;
                    }
                }
            }

            return (int)(a.DrawArea.X - b.DrawArea.X);
        }

        private void DrawTab(TabInfo tab, Graphics g)
        {
            this.GetDrawTabMethod(tab)(this, g);
            this.GetDrawCloseButtonMethod(tab)(this, g);
            if (tab.Icon == null)
            {
                return;
            }

            var form = this.GetForm();
            var tabCloseButtonCanDrawWidth = GetTabCloseButtonCanDrawWidth(form);
            if (tab.DrawArea.Width <= tabCloseButtonCanDrawWidth)
            {
                return;
            }

            var scale = WindowUtil.GetCurrentWindowScale(this);
            var font = Fonts.GetRegularFont(Fonts.Size.Medium, scale);
            var args = new DrawTabEventArgs
            {
                Graphics = g,
                Font = font,
                TitleColor = TabPalette.TITLE_COLOR,
                TitleFormatFlags = TabPalette.TITLE_FORMAT_FLAGS,
                TextRectangle = tab.DrawArea.GetPageRectangle(),
                IconRectangle = tab.DrawArea.GetIconRectangle(scale),
                CloseButtonRectangle = tab.DrawArea.GetCloseButtonRectangle(),
            };
            tab.DrawingTabPage(args);

        }

        private void DrawDropPoint(Graphics g)
        {
            if (this._dropPoint == null)
            {
                return;
            }

            const int ICON_SIZE = 32;
            const int ICON_MARGIN = 8;

            var dropX = this._dropPoint.Value.X;
            var dropY = this._dropPoint.Value.Y;

            var tabMargin = this.GetTabMargin();
            foreach (var tab in this._tabList)
            {
                if (tab.DrawArea.Contains(dropX, dropY))
                {
                    if (this.IsTabLeftDrop(dropX, tab))
                    {
                        var img = ResourceFiles.DropArrowIcon.Value;
                        var scale = WindowUtil.GetCurrentWindowScale(this);
                        var size = Math.Min(ICON_SIZE * scale, img.Width * scale) - ICON_MARGIN * scale;
                        var x = (tab.DrawArea.Left - tabMargin / 2f) - size / 2f;
                        var y = 0;
                        g.DrawImage(img, x, y, size, size);
                        return;
                    }
                    else if (this.IsTabRightDrop(dropX, tab))
                    {
                        var img = ResourceFiles.DropArrowIcon.Value;
                        var scale = WindowUtil.GetCurrentWindowScale(this);
                        var size = Math.Min(ICON_SIZE * scale, img.Width * scale) - ICON_MARGIN * scale;
                        var x = (tab.DrawArea.Right - tabMargin / 2f) - size / 2f;
                        var y = 0;
                        g.DrawImage(img, x, y, size, size);
                        return;
                    }
                }
            }
        }

        private Action<TabSwitch, Graphics> GetDrawTabMethod(TabInfo tab)
        {
            if (tab == this._activeTab)
            {
                return tab.DrawArea.DrawActiveTab;
            }
            else if (tab == this._mousePointTab)
            {
                return tab.DrawArea.DrawMousePointTab;
            }
            else
            {
                return tab.DrawArea.DrawInactiveTab;
            }
        }

        private Action<TabSwitch, Graphics> GetDrawCloseButtonMethod(TabInfo tab)
        {
            var p = this.PointToClient(Cursor.Position);
            var rect = tab.DrawArea.GetCloseButtonRectangle();

            if (rect.Contains(p))
            {
                if (tab == this._activeTab)
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
                if (tab == this._activeTab)
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
            if (this._addTabButtonDrawArea.Page(p))
            {
                return this._addTabButtonDrawArea.DrawMousePointImage;
            }
            else
            {
                return this._addTabButtonDrawArea.DrawInactiveImage;
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

    }
}
