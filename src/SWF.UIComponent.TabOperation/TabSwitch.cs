using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using SWF.Core.Resource;
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
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class TabSwitch
        : Control
    {
        // タブの高さ
        private int GetTabHeight()
        {
            const int TAB_DEFAULT_HEIGHT = 29;
            var form = this.GetForm();
            var scale = AppConstants.GetCurrentWindowScale(form);
            return (int)(TAB_DEFAULT_HEIGHT * scale);
        }

        // タブ同士のの間隔
        private int GetTabMargin()
        {
            const float TAB_MARGIN = 2;
            var form = this.GetForm();
            var scale = AppConstants.GetCurrentWindowScale(form);
            return (int)(TAB_MARGIN * scale);
        }

        // タブの最大幅
        private int GetTabMaximumWidth()
        {
            const float TAB_MAXIMUM_WIDTH = 256;
            var form = this.GetForm();
            var scale = AppConstants.GetCurrentWindowScale(form);
            return (int)(TAB_MAXIMUM_WIDTH * scale);
        }

        // タブの最小幅
        private int GetTabMinimumWidth()
        {
            const float TAB_MINIMUM_WIDTH = 1;
            var form = this.GetForm();
            var scale = AppConstants.GetCurrentWindowScale(form);
            return (int)(TAB_MINIMUM_WIDTH * scale);
        }

        // タブ領域の間隔
        private int GetTabsMargin()
        {
            const float TABS_MARGIN = 8;
            var form = this.GetForm();
            var scale = AppConstants.GetCurrentWindowScale(form);
            return (int)(TABS_MARGIN * scale);
        }

        private int GetTabRightMargin()
        {
            const int TAB_RIGHT_DEFAULT_MARGIN = 32;
            var form = this.GetForm();
            var scale = AppConstants.GetCurrentWindowScale(form);
            return (int)(TAB_RIGHT_DEFAULT_MARGIN * scale);
        }

        public static int GetTabCloseButtonCanDrawWidth(Control owner)
        {
            ArgumentNullException.ThrowIfNull(owner, nameof(owner));

            const float TAB_CLOSE_BUTTON_CAN_DRAW_WIDTH = 64;
            var scale = AppConstants.GetCurrentWindowScale(owner);
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
        private readonly List<TabInfo> tabList = [];

        // マウスダウンしたタブ
        private TabInfo mouseDownTab = null;

        // マウスポイントされているタブ
        private TabInfo mousePointTab = null;

        // アクティブなタブ
        private TabInfo activeTab = null;

        // ドロップされている座標
        private Point? dropPoint = null;

        // タブ追加ボタン病が暮らす
        private readonly AddTabButtonDrawArea addTabButtonDrawArea;

        // コンテンツ描画クラス
        private readonly PageDrawArea pageDrawArea;

        private readonly Font defaultFont = new("Yu Gothic UI", 10F);
        private readonly Dictionary<float, Font> fontCache = [];

        private int GetTabsRightOffset()
        {
            var size = AppConstants.GetControlBoxSize(this.GetForm().Handle);
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

        public bool IsBeginTabDragOperation
        {
            get
            {
                return TabDragOperation.IsBegin;
            }
        }

        public TabSwitch()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);
            this.UpdateStyles();

            this.addTabButtonDrawArea = new(this);
            this.pageDrawArea = new(this);
        }

        public TabInfo[] GetInactiveTabs()
        {
            var tabs = this.tabList
                    .Where(tab => tab != this.activeTab)
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

            if (this.tabList.Contains(tab))
            {
                throw new ArgumentException("既に含まれているタブを追加しようとしました。", nameof(tab));
            }

            tab.Owner = this;

            this.tabList.Add(tab);

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

            if (this.activeTab != null)
            {
                this.activeTab.ClearPage();
                this.activeTab.OverwritePage(param);

                var container = this.GetContainer();
                container.SuspendLayout();
                container.ClearPage();
                container.SetPage(this.activeTab.Page);
                container.ResumeLayout(false);

                return (T)this.activeTab.Page;
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
            if (this.activeTab == null)
            {
                throw new NullReferenceException("アクティブなタブが存在しません。");
            }

            if (!this.activeTab.HasPreviewPage)
            {
                throw new InvalidOperationException("コンテンツの前の履歴が存在しません。");
            }

            this.activeTab.ClearPage();
            this.activeTab.CreatePreviewPage();

            var container = this.GetContainer();
            container.SuspendLayout(); ;
            container.ClearPage();
            container.SetPage(this.activeTab.Page);
            container.ResumeLayout(false);

            this.Invalidate();

            return (T)this.activeTab.Page;
        }

        /// <summary>
        /// 履歴の次のページを表示します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T SetNextPageHistory<T>() where T : PagePanel
        {
            if (this.activeTab == null)
            {
                throw new NullReferenceException("アクティブなタブが存在しません。");
            }

            if (!this.activeTab.HasNextPage)
            {
                throw new InvalidOperationException("ページの次の履歴が存在しません。");
            }

            this.activeTab.ClearPage();
            this.activeTab.CreateNextPage();

            var container = this.GetContainer();
            container.SuspendLayout();
            container.ClearPage();
            container.SetPage(this.activeTab.Page);
            container.ResumeLayout(false);

            return (T)this.activeTab.Page;
        }

        /// <summary>
        /// 現在のページのクローンを取得します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T CloneCurrentPage<T>()
            where T : PagePanel
        {
            if (this.activeTab == null)
            {
                throw new NullReferenceException("アクティブなタブが存在しません。");
            }

            if (!this.activeTab.HasPage)
            {
                throw new InvalidOperationException("ページが存在しません。");
            }

            this.activeTab.ClearPage();
            this.activeTab.CloneCurrentPage();

            var container = this.GetContainer();
            container.SuspendLayout();
            container.ClearPage();
            container.SetPage(this.activeTab.Page);
            container.ResumeLayout(false);

            return (T)this.activeTab.Page;
        }

        /// <summary>
        /// タブを削除します。
        /// </summary>
        /// <param name="tab"></param>
        public void RemoveTab(TabInfo tab)
        {
            ArgumentNullException.ThrowIfNull(tab, nameof(tab));

            if (!this.tabList.Contains(tab))
            {
                throw new ArgumentException("含まれていないタブを削除しようとしました。", nameof(tab));
            }

            var nextActiveTab = this.GetNextTab(tab) ?? this.GetPreviewTab(tab);

            if (this.SetActiveTab(nextActiveTab))
            {
                this.OnActiveTabChanged(EventArgs.Empty);
            }

            if (tab == this.mousePointTab)
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
                    var screenPoint = Cursor.Position;
                    var form = this.GetForm();

                    if (ScreenUtil.GetLeftBorderRect().Contains(screenPoint))
                    {
                        var scale = AppConstants.GetCurrentWindowScale(this);
                        var screenRect = Screen.GetWorkingArea(screenPoint);
                        var w = (int)(screenRect.Width / 2f + 14 * scale);
                        var h = (int)(screenRect.Height + 8 * scale);
                        var x = (int)(screenRect.Left - 7 * scale);
                        var y = screenRect.Top;
                        this.OnTabDropouted(new TabDropoutedEventArgs(tab, new Point(x, y), new Size(w, h), FormWindowState.Normal));
                    }
                    else if (ScreenUtil.GetRightBorderRect().Contains(screenPoint))
                    {
                        var scale = AppConstants.GetCurrentWindowScale(this);
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

            this.mouseDownTab = null;
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

        protected override void OnHandleCreated(EventArgs e)
        {
            var form = this.GetForm();
            TabDragOperation.AddForm(form);

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

                foreach (var font in this.fontCache.Values)
                {
                    font.Dispose();
                }
                this.fontCache.Clear();
                this.defaultFont.Dispose();
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

            base.OnPaintBackground(pevent);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;

            foreach (var tab in this.tabList.FindAll((t) => t != this.activeTab))
            {
                this.DrawTab(tab, e.Graphics);
            }

            this.GetAddTabButtonDrawMethod()(e.Graphics);

            this.pageDrawArea.Draw(e.Graphics);

            if (this.activeTab != null)
            {
                this.DrawTab(this.activeTab, e.Graphics);
            }

            this.DrawDropPoint(e.Graphics);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (TabDragOperation.IsBegin
                && !TabDragOperation.TabDragForm.Visible
                && TabDragOperation.Tab.Owner == this
                && TabDragOperation.TabDragForm.TabSwitch != null)
            {
                TabDragOperation.TabDragForm.TabSwitch.CallEndTabDragOperation();
            }

            base.OnLostFocus(e);
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
                var form = this.GetForm();
                if (form == Form.ActiveForm)
                {
                    if (this.GetHeaderRectangle().Contains(e.X, e.Y))
                    {
                        this.Focus();
                    }
                }

                var tab = this.GetTabFromPoint(e.X, e.Y);
                this.SetMousePointTab(tab);
                this.InvalidateHeader();

                if (tab == null && e.Button == MouseButtons.Left)
                {
                    WinApiMembers.ReleaseCapture();
                    _ = WinApiMembers.SendMessage(form.Handle, WinApiMembers.WM_NCLBUTTONDOWN, WinApiMembers.HTCAPTION, 0);
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
            else if (this.addTabButtonDrawArea.Page(e.X, e.Y))
            {
                this.InvalidateHeader();
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.CallEndTabDragOperation();

            base.OnMouseUp(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            var tab = this.GetTabFromPoint(e.X, e.Y);
            if (tab != null && tab == this.mouseDownTab)
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
            else if (this.addTabButtonDrawArea.Page(e.X, e.Y))
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
                var tabAddButtonRect = new RectangleF(
                    this.addTabButtonDrawArea.X, this.addTabButtonDrawArea.Y, this.addTabButtonDrawArea.Width, this.addTabButtonDrawArea.Height);
                if (tab == null
                    && this.GetHeaderRectangle().Contains(e.X, e.Y)
                    && !tabAddButtonRect.Contains(e.X, e.Y))
                {
                    this.OnBackgroundMouseLeftDoubleClick(EventArgs.Empty);
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

                this.OnActiveTabChanged(EventArgs.Empty);
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
                    if (tab.DrawArea.Contains(dropX, dropY))
                    {
                        if (this.SetActiveTab(tab))
                        {
                            this.OnActiveTabChanged(EventArgs.Empty);
                            break;
                        }
                    }
                }

                drgevent.Effect = DragDropEffects.Copy;
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
                foreach (var tab in this.tabList)
                {
                    if (tab.DrawArea.Contains(dropX, dropY))
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

        private Font GetFont(float scale)
        {
            if (this.fontCache.TryGetValue(scale, out var font))
            {
                return font;
            }

            var newFont = new Font(this.defaultFont.FontFamily, this.defaultFont.Size * scale);
            this.fontCache.Add(scale, newFont);
            return newFont;
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
                if (tab != this.mousePointTab)
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
                if (tab != this.activeTab)
                {
                    var container = this.GetContainer();
                    container.SuspendLayout();
                    container.ClearPage();
                    container.SetPage(tab.Page);
                    container.ResumeLayout(false);

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
                container.ClearPage();
                container.ResumeLayout(false);

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
            return this.tabList.FirstOrDefault<TabInfo>((t) => t.DrawArea.Contains(x, y));
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
                throw new NullReferenceException("フォーム内にコンテンツコンテナが存在しません。");
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
                throw new Exception("タブスイッチはフォームに格納されていません。");
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
            var w = rect.Width - tabsRightOffset - tabsMargin * 2 - tabMargin - this.addTabButtonDrawArea.Width;
            var h = this.GetTabHeight();
            return new RectangleF(x, y, w, h);
        }

        private void SetTabsDrawArea()
        {
            if (TabDragOperation.IsBegin)
            {
                this.tabList.Sort((a, b) => this.SortTabList(a, b));
                var rect = this.GetTabsRectangle();
                var w = this.GetTabWidth();
                var x = rect.X;
                var tabMargin = this.GetTabMargin();
                foreach (var tab in this.tabList)
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
                foreach (var tab in this.tabList)
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

            if (this.tabList.Count > 0)
            {
                var tab = this.tabList.Last();
                if (TabDragOperation.IsTarget(tab))
                {
                    if (this.tabList.Count > 1)
                    {
                        var index = this.tabList.IndexOf(tab) - 1;
                        this.addTabButtonDrawArea.X = this.tabList[index].DrawArea.Right + tabsMargin;
                    }
                    else
                    {
                        this.addTabButtonDrawArea.X = this.ClientRectangle.X + tabsMargin;
                    }
                }
                else
                {
                    this.addTabButtonDrawArea.X = tab.DrawArea.Right + tabsMargin;
                }

                var rect = this.GetTabsRectangle();
                if (this.addTabButtonDrawArea.Right > rect.Right)
                {
                    var tabMargin = this.GetTabMargin();
                    this.addTabButtonDrawArea.X = rect.Right + tabMargin;
                }
            }
            else
            {
                this.addTabButtonDrawArea.X = this.ClientRectangle.X + tabsMargin;
            }
        }

        private float GetTabWidth()
        {
            var tabMargin = this.GetTabMargin();
            var tabMaximumWidth = this.GetTabMaximumWidth();
            var rect = this.GetTabsRectangle();

            var defAllTabW = tabMaximumWidth * this.tabList.Count + tabMargin * (this.tabList.Count - 1);

            if (defAllTabW > rect.Width)
            {
                float tabW;
                if (this.tabList.Count > 0)
                {
                    tabW = (rect.Width - tabMargin * (this.tabList.Count - 1)) / (float)this.tabList.Count;
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

            var scale = AppConstants.GetCurrentWindowScale(this);
            var font = this.GetFont(scale);
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
            if (this.dropPoint == null)
            {
                return;
            }

            const int ICON_SIZE = 32;
            const int ICON_MARGIN = 8;

            var dropX = this.dropPoint.Value.X;
            var dropY = this.dropPoint.Value.Y;

            var tabMargin = this.GetTabMargin();
            foreach (var tab in this.tabList)
            {
                if (tab.DrawArea.Contains(dropX, dropY))
                {
                    if (this.IsTabLeftDrop(dropX, tab))
                    {
                        var img = ResourceFiles.DropArrowIcon.Value;
                        var scale = AppConstants.GetCurrentWindowScale(this);
                        var size = Math.Min(ICON_SIZE * scale, img.Width * scale) - ICON_MARGIN * scale;
                        var x = (tab.DrawArea.Left - tabMargin / 2f) - size / 2f;
                        var y = 0;
                        g.DrawImage(img, x, y, size, size);
                        return;
                    }
                    else if (this.IsTabRightDrop(dropX, tab))
                    {
                        var img = ResourceFiles.DropArrowIcon.Value;
                        var scale = AppConstants.GetCurrentWindowScale(this);
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
            if (tab == this.activeTab)
            {
                return tab.DrawArea.DrawActiveTab;
            }
            else if (tab == this.mousePointTab)
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
                if (tab == this.activeTab)
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
                if (tab == this.activeTab)
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
            if (this.addTabButtonDrawArea.Page(p))
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

    }
}
