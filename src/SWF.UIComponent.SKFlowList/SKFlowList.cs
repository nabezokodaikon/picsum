using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SWF.Core.Base;
using SWF.UIComponent.Base;
using SWF.UIComponent.FlowList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.SKFlowList
{
    /// <summary>
    /// フローリストコントロール
    /// </summary>

    public sealed class SKFlowList
        : SKControl
    {
        public event EventHandler<SKDrawItemEventArgs> SKDrawItem;
        public event EventHandler<SKDrawItemsEventArgs> SKDrawItems;
        public event EventHandler<SKDrawItemChangedEventArgs> DrawItemChanged;
        public event EventHandler SelectedItemChanged;
        public event EventHandler<MouseEventArgs> ItemMouseClick;
        public event EventHandler<MouseEventArgs> ItemMouseDoubleClick;
        public event EventHandler<MouseEventArgs> BackgroundMouseClick;
        public event EventHandler ItemExecute;
        public event EventHandler ItemDelete;
        public event EventHandler ItemCopy;
        public event EventHandler ItemCut;
        public event EventHandler DragStart;

        public const int SCROLL_BAR_DEFAULT_WIDTH = 17;

        // 項目最小サイズ
        public const int MINIMUM_ITEM_SIZE = 16;

        private static readonly Size DRAG_SIZE = GetDragSize();

        private static Size GetDragSize()
        {
            var size = SystemInformation.DragSize.Width * 16;
            return new Size(size, size);
        }

        // 描画フラグ
        private bool _isDraw = true;

        // 複数選択可能フラグ
        private bool _isMultiSelect = false;

        // 行リストフラグ
        private bool _isLileList = false;

        // 項目数
        private int _itemCount = 0;

        // 項目幅
        private int _itemWidth = MINIMUM_ITEM_SIZE;

        // 項目高さ
        private int _itemHeight = MINIMUM_ITEM_SIZE;

        // 項目間余白
        private int _itemSpace = 0;

        // 描画パラメータ
        private DrawParameter _drawParameter = new();

        // 垂直スクロールバー
        private readonly VScrollBarEx _scrollBar = new();

        // フォーカスされている項目のインデックス
        private int _foucusItemIndex = -1;

        // マウスポイントされている項目のインデックス
        private int _mousePointItemIndex = -1;

        // 選択されている項目インデックスのリスト
        private readonly ItemIndexList _selectedItemIndexs = new();

        // 短形選択クラス
        private readonly RectangleSelection _rectangleSelection = new();

        // マウスダウンした座標情報
        private HitTestInfo _mouseDownHitTestInfo = new();
        private SKRectI _dragJudgementRectangle = new();

        // ドラッグフラグ
        private bool _isDrag = false;

        // スクロール関連
        private const int ANIMATION_DURATION_MS = 300;
        private readonly AnimationTimer _animationTimer = new();
        private Stopwatch _animationStopwatch;
        private double _animationStartValue;
        private double _currentScrollPosition;
        private int _targetVerticalScroll;

        private bool _isRunningPaintSurfaceEvent = false;
        private bool _needsRepaint = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanKeyDown { get; set; } = true;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsLight { get; set; } = true;


        /// <summary>
        /// スクロールバー表示フラグ
        /// </summary>
        public bool IsScrollBarVisible
        {
            get
            {
                return this._scrollBar.Visible;
            }
        }

        public int ScrollValue
        {
            get
            {
                return this._scrollBar.Value;
            }
        }

        /// <summary>
        /// 項目数
        /// </summary>
        [Category("項目表示"), DefaultValue(0)]
        public int ItemCount
        {
            get
            {
                return this._itemCount;
            }
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 0, nameof(value));

                if (this._rectangleSelection.IsBegun)
                {
                    this._rectangleSelection.EndSelection();
                }

                this._selectedItemIndexs.Clear();

                this._mousePointItemIndex = -1;

                if (this._foucusItemIndex > value - 1)
                {
                    this._foucusItemIndex = -1;
                }

                this._itemCount = value;

                this.Invalidate();
            }
        }

        /// <summary>
        /// 項目幅
        /// </summary>
        [Category("項目表示"), DefaultValue(MINIMUM_ITEM_SIZE)]
        public int ItemWidth
        {
            get
            {
                return this._itemWidth;
            }
            set
            {
                if (this._rectangleSelection.IsBegun)
                {
                    throw new InvalidOperationException("短形選択中は設定できません。");
                }

                ArgumentOutOfRangeException.ThrowIfLessThan(value, MINIMUM_ITEM_SIZE);

                this._itemWidth = value;

                this.Invalidate();
            }
        }

        /// <summary>
        /// 項目高さ
        /// </summary>
        [Category("項目表示"), DefaultValue(MINIMUM_ITEM_SIZE)]
        public int ItemHeight
        {
            get
            {
                return this._itemHeight;
            }
            set
            {
                if (this._rectangleSelection.IsBegun)
                {
                    throw new InvalidOperationException("短形選択中は設定できません。");
                }

                ArgumentOutOfRangeException.ThrowIfLessThan(value, MINIMUM_ITEM_SIZE);

                this._itemHeight = value;

                this.Invalidate();
            }
        }

        /// <summary>
        /// 項目間余白
        /// </summary>
        [Category("項目表示"), DefaultValue(0)]
        public int ItemSpace
        {
            get
            {
                return this._itemSpace;
            }
            set
            {
                if (this._rectangleSelection.IsBegun)
                {
                    throw new InvalidOperationException("短形選択中は設定できません。");
                }

                ArgumentOutOfRangeException.ThrowIfLessThan(value, 0, nameof(value));

                this._itemSpace = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// 行リストフラグ
        /// </summary>
        [Category("項目表示"), DefaultValue(false)]
        public bool IsLileList
        {
            get
            {
                return this._isLileList;
            }
            set
            {
                if (this._rectangleSelection.IsBegun)
                {
                    throw new InvalidOperationException("短形選択中は設定できません。");
                }

                this._isLileList = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// 複数選択フラグ
        /// </summary>
        [Category("項目動作"), DefaultValue(false)]
        public bool IsMultiSelect
        {
            get
            {
                return this._isMultiSelect;
            }
            set
            {
                if (this._rectangleSelection.IsBegun)
                {
                    throw new InvalidOperationException("短形選択中は設定できません。");
                }

                this._selectedItemIndexs.Clear();
                this._isMultiSelect = value;
                this._rectangleSelection.IsUse = value;
                this.Invalidate();
            }
        }

        public bool IsRunningScrollAnimation
        {
            get
            {
                return this._animationTimer.Enabled;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return base.ContextMenuStrip;
            }
            set
            {
                if (base.ContextMenuStrip != null)
                {
                    base.ContextMenuStrip.Opening -= this.ContextMenuStrip_Opening;
                }

                base.ContextMenuStrip = value;

                if (base.ContextMenuStrip != null)
                {
                    base.ContextMenuStrip.Opening += this.ContextMenuStrip_Opening;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float MouseWheelRate { get; set; } = 1f;

        public SKFlowList()
        {
            this.DoubleBuffered = true;

            this._animationTimer.Tick += this.AnimationTick;

            this._scrollBar.Dock = DockStyle.Right;
            this._scrollBar.ValueChanged += new(this.ScrollBar_ValueChanged);
            this._scrollBar.Scroll += this.ScrollBar_Scroll;
            this._selectedItemIndexs.Change += new(this.SelectedItemIndexs_Change);

            this.Controls.Add(this._scrollBar);

            this.KeyDown += this.FlowList_KeyDown;
            this.MouseLeave += this.FlowList_MouseLeave;
            this.MouseDown += this.FlowList_MouseDown;
            this.MouseUp += this.FlowList_MouseUp;
            this.MouseMove += this.FlowList_MouseMove;
            this.MouseDoubleClick += this.FlowList_MouseDoubleClick;
            this.MouseWheel += this.FlowList_MouseWheel;
            this.PaintSurface += this.FlowList_PaintSurface;
            this.Resize += this.FlowList_Resize;
            this.LostFocus += this.FlowList_LostFocus;
        }

        /// <summary>
        /// 描画領域をリフレッシュします。
        /// </summary>
        public new void Refresh()
        {
            this._drawParameter = new DrawParameter();
            this.Invalidate();
        }

        /// <summary>
        /// 描画を停止します。
        /// </summary>
        public void BeginUpdate()
        {
            this._isDraw = false;
        }

        /// <summary>
        /// 描画を開始します。
        /// </summary>
        public void EndUpdate()
        {
            this._isDraw = true;
            this.Invalidate();
        }

        /// <summary>
        /// 項目のサイズを設定します。
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetItemSize(int width, int height)
        {
            if (this._rectangleSelection.IsBegun)
            {
                throw new InvalidOperationException("短形選択中は設定できません。");
            }

            ArgumentOutOfRangeException.ThrowIfLessThan(width, MINIMUM_ITEM_SIZE);
            ArgumentOutOfRangeException.ThrowIfLessThan(height, MINIMUM_ITEM_SIZE);

            this._itemWidth = width;
            this._itemHeight = height;

            this.Invalidate();
        }

        /// <summary>
        /// 選択項目をクリアします。
        /// </summary>
        public void ClearSelectedItems()
        {
            this._selectedItemIndexs.Clear();
            this._foucusItemIndex = -1;
            this._mousePointItemIndex = -1;
            this.Invalidate();
        }

        /// <summary>
        /// 指定した項目を選択します。
        /// </summary>
        /// <param name="itemIndex"></param>
        public void SelectItem(int itemIndex)
        {
            if (this._rectangleSelection.IsBegun)
            {
                throw new InvalidOperationException("短形選択中は設定できません。");
            }

            if (itemIndex < 0 || this._itemCount - 1 < itemIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(itemIndex));
            }

            this._foucusItemIndex = itemIndex;

            this._selectedItemIndexs.Clear();
            this._selectedItemIndexs.Add(itemIndex);

            this.EnsureVisible(itemIndex, false);

            this.Invalidate();

            this.OnSelectedItemChanged(EventArgs.Empty);
        }

        public void SelectItem(int itemIndex, ScrollParameter scrollInfo)
        {
            if (this._rectangleSelection.IsBegun)
            {
                throw new InvalidOperationException("短形選択中は設定できません。");
            }

            if (itemIndex < 0 || this._itemCount - 1 < itemIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(itemIndex));
            }

            this._foucusItemIndex = itemIndex;

            this._selectedItemIndexs.Clear();
            this._selectedItemIndexs.Add(itemIndex);

            if (scrollInfo.FlowListSize == this.Size
                && scrollInfo.ItemSize.Width == this.ItemWidth
                && scrollInfo.ItemSize.Height == this.ItemHeight)
            {
                this._scrollBar.Value = scrollInfo.ScrollValue;
            }
            else
            {
                this.EnsureVisible(itemIndex, false);
            }

            this.Invalidate();

            this.OnSelectedItemChanged(EventArgs.Empty);
        }

        /// <summary>
        /// 選択している項目のインデックスを取得します。
        /// </summary>
        /// <returns></returns>
        public int[] GetSelectedIndexs()
        {
            if (this._rectangleSelection.IsBegun)
            {
                return [];
            }

            return this._selectedItemIndexs.GetList();
        }

        /// <summary>
        /// クライアント座標から、項目のインデックスを取得します。
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <returns>座標上に項目が存在する場合は、項目のインデックス。存在しない場合は-1を返します。</returns>
        public int IndexFromPoint(int x, int y)
        {
            var ht = this.GetHitTestFromDrawPoint(x, y);
            return ht.ItemIndex;
        }

        /// <summary>
        /// 項目を再描画します。
        /// </summary>
        /// <param name="itemIndex">項目インデックス</param>
        public void InvalidateFromItemIndex(int itemIndex)
        {
            if (itemIndex < 0 || this._itemCount - 1 < itemIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(itemIndex));
            }

            var rect = this.GetItemDrawRectangle(itemIndex);
            this.Invalidate(rect.ToDrawingRect());
        }

        public new void Invalidate(Rectangle rc)
        {
            if (this.IsDisposed)
            {
                return;
            }

            this.SetDrawParameter(false);

            base.Invalidate(rc);
        }

        public new void Invalidate()
        {
            this.Invalidate(this.ClientRectangle);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._animationTimer.Stop();
                this._animationTimer.Dispose();
                this._scrollBar.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            var kcode = keyData & Keys.KeyCode;

            if (kcode == Keys.Up ||
                kcode == Keys.Down ||
                kcode == Keys.Left ||
                kcode == Keys.Right)
            {
                return true;
            }

            return base.IsInputKey(keyData);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this._isRunningPaintSurfaceEvent)
            {
                this._needsRepaint = true;
                return;
            }

            base.OnPaint(e);
            this._needsRepaint = false;
        }

        private void FlowList_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void FlowList_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            using (Measuring.Time(false, "SKFlowList.FlowList_PaintSurface"))
            {
                this._isRunningPaintSurfaceEvent = true;
                this._needsRepaint = false;

                using var recorder = new SKPictureRecorder();
                using var canvas = recorder.BeginRecording(e.Info.Rect);

                if (this.IsLight)
                {
                    canvas.Clear(SKFlowListResources.LIGHT_BACKGROUND_PAINT.Color);
                }
                else
                {
                    canvas.Clear(SKFlowListResources.DARK_BACKGROUND_PAINT.Color);
                }

                if (!this._isDraw)
                {
                    return;
                }

                if (this._rectangleSelection.IsBegun)
                {
                    this.DrawRectangleSelection(canvas);
                }

                if (this._itemCount > 0)
                {
                    var infoList = new List<SKDrawItemInfo>(
                        Math.Max(0, this._drawParameter.DrawLastItemIndex - this._drawParameter.DrawFirstItemIndex + 1));

                    using (Measuring.Time(false, "SKFlowList.FlowList_PaintSurface Calculating drawing items"))
                    {
                        for (var itemIndex = this._drawParameter.DrawFirstItemIndex;
                             itemIndex <= this._drawParameter.DrawLastItemIndex;
                             itemIndex++)
                        {
                            var drawRect = this.GetItemDrawRectangle(itemIndex);
                            if (!e.Surface.Canvas.LocalClipBounds.IntersectsWith(drawRect))
                            {
                                continue;
                            }

                            bool isSelected;
                            if (this._rectangleSelection.IsBegun)
                            {
                                isSelected = this._selectedItemIndexs.Contains(itemIndex) ||
                                    this._rectangleSelection.VirtualRectangle.IntersectsWith(this.GetItemVirtualRectangle(itemIndex));
                            }
                            else
                            {
                                isSelected = this._selectedItemIndexs.Contains(itemIndex);
                            }

                            var isMousePoint = this._mousePointItemIndex == itemIndex;
                            var isFocus = this._foucusItemIndex == itemIndex;

                            var arg = new SKDrawItemEventArgs(
                                canvas,
                                itemIndex,
                                drawRect,
                                isSelected,
                                isMousePoint,
                                isFocus);
                            this.OnSKDrawItem(arg);

                            var info = new SKDrawItemInfo(
                                itemIndex,
                                drawRect,
                                isSelected,
                                isMousePoint,
                                isFocus);

                            infoList.Add(info);
                        }
                    }

                    if (infoList.Count > 0)
                    {
                        this.OnSKDrawItems(new SKDrawItemsEventArgs(
                            canvas,
                            e.Surface.Canvas.LocalClipBounds,
                            [.. infoList]));
                    }
                }

                using (Measuring.Time(false, "SKFlowList.FlowList_PaintSurface DrawPicture"))
                {
                    using var recordedMap = recorder.EndRecording();
                    e.Surface.Canvas.DrawPicture(recordedMap);
                }

                this._isRunningPaintSurfaceEvent = false;

                if (this._needsRepaint)
                {
                    this.Invalidate();
                }
            }
        }

        private void FlowList_KeyDown(object sender, KeyEventArgs e)
        {
            if (!this.CanKeyDown)
            {
                return;
            }

            if (this._itemCount > 0 && !this._rectangleSelection.IsBegun)
            {
                this._selectedItemIndexs.BeginUpdate();

                try
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Left:
                            if (this.LeftKeyDown())
                            {
                                this.Invalidate();
                            }
                            break;
                        case Keys.Right:
                            if (this.RightKeyDown())
                            {
                                this.Invalidate();
                            }
                            break;
                        case Keys.Up:
                            if (this.UpKeyDown())
                            {
                                this.Invalidate();
                            }
                            break;
                        case Keys.Down:
                            if (this.DownKeyDown())
                            {
                                this.Invalidate();
                            }
                            break;
                        case Keys.Space:
                            this.SpaceKeyDown();
                            this.Invalidate();
                            break;
                        case Keys.ProcessKey:
                            this.SpaceKeyDown();
                            this.Invalidate();
                            break;
                        case Keys.PageUp:
                            this.PageUpKeyDown();
                            this.Invalidate();
                            break;
                        case Keys.PageDown:
                            this.PageDownKeyDown();
                            this.Invalidate();
                            break;
                        case Keys.Home:
                            this.HomeKeyDown();
                            this.Invalidate();
                            break;
                        case Keys.End:
                            this.EndKeyDown();
                            this.Invalidate();
                            break;
                        case Keys.A:
                            if (this._isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
                            {
                                this.SelectAll();
                                this.Invalidate();
                            }
                            break;
                        case Keys.Enter:
                            if (this._selectedItemIndexs.Count > 0)
                            {
                                this.OnItemExecute(EventArgs.Empty);
                            }
                            break;
                        case Keys.Delete:
                            if (this._selectedItemIndexs.Count > 0)
                            {
                                if (this._selectedItemIndexs.Contains(this._foucusItemIndex))
                                {
                                    this.OnItemDelete(EventArgs.Empty);
                                }
                                else
                                {
                                    this.OnItemDelete(EventArgs.Empty);
                                }
                            }
                            break;
                        case Keys.C:
                            if (this._selectedItemIndexs.Count > 0)
                            {
                                if (this._selectedItemIndexs.Contains(this._foucusItemIndex))
                                {
                                    this.OnItemCopy(EventArgs.Empty);
                                }
                                else
                                {
                                    this.OnItemCopy(EventArgs.Empty);
                                }
                            }
                            break;
                        case Keys.X:
                            if (this._selectedItemIndexs.Count > 0)
                            {
                                if (this._selectedItemIndexs.Contains(this._foucusItemIndex))
                                {
                                    this.OnItemCut(EventArgs.Empty);
                                }
                                else
                                {
                                    this.OnItemCut(EventArgs.Empty);
                                }
                            }
                            break;
                    }
                }
                finally
                {
                    this._selectedItemIndexs.EndUpdate();
                }
            }
        }

        private void FlowList_LostFocus(object sender, EventArgs e)
        {
            if (this._rectangleSelection.IsBegun)
            {
                this._rectangleSelection.EndSelection();
            }

            this._mousePointItemIndex = -1;
            this._isDrag = false;
            this._mouseDownHitTestInfo = new HitTestInfo();

            this.Invalidate();
        }

        private void FlowList_MouseLeave(object sender, EventArgs e)
        {
            if (this._rectangleSelection.IsBegun)
            {
                this._rectangleSelection.EndSelection();
            }

            this._mousePointItemIndex = -1;
            this._isDrag = false;
            this._mouseDownHitTestInfo = new HitTestInfo();

            this.Invalidate();
        }

        private void FlowList_MouseDown(object sender, MouseEventArgs e)
        {
            if (this._itemCount < 1)
            {
                return;
            }
            else if (this._rectangleSelection.IsBegun)
            {
                return;
            }
            else if (e.Button == MouseButtons.Left)
            {
                // 左ボタン
                this._selectedItemIndexs.BeginUpdate();
                try
                {
                    this.LeftMouseDown(e.X, e.Y);
                }
                finally
                {
                    this.Invalidate();
                    this._selectedItemIndexs.EndUpdate();
                }

                // マウスダウンした座標情報を保持します。
                this._mouseDownHitTestInfo = this.GetHitTestFromDrawPoint(e.X, e.Y);

                var dragSize = DRAG_SIZE;
                var x = e.X - dragSize.Width / 2f;
                var y = e.Y - dragSize.Height / 2f;
                this._dragJudgementRectangle = new(
                    (int)x,
                    (int)y,
                    (int)(x + dragSize.Width),
                    (int)(y + dragSize.Height));
            }
            else if (e.Button == MouseButtons.Right)
            {
                // 右ボタン
                this._selectedItemIndexs.BeginUpdate();
                try
                {
                    this.RightMouseDown(e.X, e.Y);
                }
                finally
                {
                    this.Invalidate();
                    this._selectedItemIndexs.EndUpdate();
                }
            }
            else if (e.Button == MouseButtons.Middle)
            {
                // ミドルボタン
                this._selectedItemIndexs.BeginUpdate();
                try
                {
                    this.MiddleMouseDown(e.X, e.Y);
                }
                finally
                {
                    this.Invalidate();
                    this._selectedItemIndexs.EndUpdate();
                }
            }
        }

        private void FlowList_MouseUp(object sender, MouseEventArgs e)
        {
            if (this._itemCount < 1)
            {
                return;
            }

            if (this._rectangleSelection.IsBegun)
            {
                var itemIndexs = this.GetItemIndexsFromVirtualRectangle(this._rectangleSelection.VirtualRectangle);
                this._rectangleSelection.EndSelection();
                this._selectedItemIndexs.Union(itemIndexs);
                this.Invalidate();
            }
            else
            {
                var ht = this.GetHitTestFromDrawPoint(e.X, e.Y);
                if (ht.IsItem)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        if (!this._isDrag &&
                            (Control.ModifierKeys & Keys.Shift) != Keys.Shift &&
                            (Control.ModifierKeys & Keys.Control) != Keys.Control)
                        {
                            this._selectedItemIndexs.BeginUpdate();
                            try
                            {
                                this._selectedItemIndexs.Clear();
                                this._selectedItemIndexs.Add(ht.ItemIndex);
                            }
                            finally
                            {
                                this.Invalidate();
                                this._selectedItemIndexs.EndUpdate();
                            }
                        }
                    }

                    if (this._selectedItemIndexs.Count > 0)
                    {
                        this.OnItemMouseClick(e);
                    }
                }
                else
                {
                    this.OnBackgroundMouseClick(e);
                }
            }

            this._isDrag = false;
            this._mouseDownHitTestInfo = new HitTestInfo();
        }

        private void FlowList_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._itemCount < 1)
            {
                return;
            }
            else if (this._rectangleSelection.IsBegun)
            {
                // 短形選択中
                this._rectangleSelection.ChangeSelection(e.X, e.Y, this._scrollBar.Value);

                if (this._scrollBar.Value > 0 && 0 > e.Y)
                {
                    // 上へスクロールします。
                    this._scrollBar.Value = Math.Max(this._scrollBar.Minimum, this._scrollBar.Value + e.Y);
                }
                else if (this._scrollBar.Value < this._scrollBar.Maximum && this.Height < e.Y)
                {
                    // 下へスクロールします。
                    this._scrollBar.Value = Math.Min(this._scrollBar.Maximum, this._scrollBar.Value + (e.Y - this.Height));
                }
                else
                {
                    this.Invalidate();
                }
            }
            else
            {
                var ht = this.GetHitTestFromDrawPoint(e.X, e.Y);
                if (ht.IsItem)
                {
                    var oldIndex = this._mousePointItemIndex;
                    if (ht.ItemIndex != oldIndex)
                    {
                        this._mousePointItemIndex = ht.ItemIndex;
                        var newRect = this.GetItemDrawRectangle(ht.ItemIndex);
                        this.Invalidate(newRect.ToDrawingRect());
                        if (oldIndex > -1)
                        {
                            var oldRect = this.GetItemDrawRectangle(oldIndex);
                            this.Invalidate(oldRect.ToDrawingRect());
                        }
                    }
                }
                else
                {
                    var oldIndex = this._mousePointItemIndex;
                    this._mousePointItemIndex = -1;
                    if (oldIndex > -1)
                    {
                        var oldRect = this.GetItemDrawRectangle(oldIndex);
                        this.Invalidate(oldRect.ToDrawingRect());
                    }
                }

                if (e.Button == MouseButtons.Left)
                {
                    if (!this._dragJudgementRectangle.Contains(e.X, e.Y)
                        && this._mouseDownHitTestInfo.IsItem && !this._isDrag)
                    {
                        // 項目のドラッグを開始します。
                        this._isDrag = true;
                        this.OnDragStart(EventArgs.Empty);
                    }
                    else if (!this._mouseDownHitTestInfo.IsItem)
                    {
                        // 短形選択を開始します。
                        this._rectangleSelection.BeginSelection(e.X, e.Y, this._scrollBar.Value);
                    }
                }
            }
        }

        private void FlowList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this._itemCount < 1)
            {
                return;
            }
            else if (this._rectangleSelection.IsBegun)
            {
                return;
            }
            else if (e.Button == MouseButtons.Left)
            {
                var ht = this.GetHitTestFromDrawPoint(e.X, e.Y);
                if (ht.IsItem)
                {
                    if (this._selectedItemIndexs.Count > 0)
                    {
                        this.OnItemMouseDoubleClick(e);
                    }
                }
            }
        }

        private void FlowList_MouseWheel(object sender, MouseEventArgs e)
        {
            if (this._itemCount < 1)
            {
                return;
            }

            if (e is HandledMouseEventArgs hme)
            {
                hme.Handled = true;
            }

            int scrollDelta;
            if (e.Delta == 0)
            {
                scrollDelta = (int)(this._itemHeight * this.MouseWheelRate);
            }
            else
            {
                scrollDelta = (int)(this._itemHeight * this.MouseWheelRate * (e.Delta / Math.Abs(e.Delta)));
            }

            if (!this._animationTimer.Enabled)
            {
                this._targetVerticalScroll = this._scrollBar.Value;
            }

            this._targetVerticalScroll -= scrollDelta;
            if (this._targetVerticalScroll < this._scrollBar.Minimum)
            {
                this._targetVerticalScroll = this._scrollBar.Minimum;
            }
            else if (this._targetVerticalScroll > this._scrollBar.Maximum)
            {
                this._targetVerticalScroll = this._scrollBar.Maximum;
            }

            this.StartAnimation();
        }

        private void AnimationTick()
        {
            if (this.IsDisposed)
            {
                return;
            }

            var elapsed = this._animationStopwatch.Elapsed.TotalMilliseconds;
            var progress = Math.Min(1.0, elapsed / ANIMATION_DURATION_MS);

            if (progress >= 1.0)
            {
                this._currentScrollPosition = this._targetVerticalScroll;
                this._scrollBar.Value = this._targetVerticalScroll;
                this._animationTimer.Stop();
                this.SetDrawParameter(true);
                return;
            }

            var easedProgress = 1 - Math.Pow(1 - progress, 3);
            var distance = this._targetVerticalScroll - this._animationStartValue;
            this._currentScrollPosition = this._animationStartValue + (distance * easedProgress);

            var intValue = (int)Math.Round(this._currentScrollPosition);
            if (intValue != this._scrollBar.Value)
            {
                this._scrollBar.Value = Math.Max(
                    this._scrollBar.Minimum,
                    Math.Min(this._scrollBar.Maximum, intValue));
            }

            this.Invalidate();
        }

        /// <summary>
        /// スクロールバーの幅を取得します。
        /// </summary>
        public int GetScrollBarWidth()
        {
            var scale = WindowUtil.GetCurrentWindowScale(this);
            var width = (int)(SCROLL_BAR_DEFAULT_WIDTH * scale);
            if (width != this._scrollBar.Width)
            {
                this._scrollBar.Width = width;
            }

            return width;
        }

        public void SetDrawParameter(bool isForced)
        {
            var beforeDrawParameter = this._drawParameter;
            var scrollBarWidth = this.GetScrollBarWidth();

            if (this._itemCount > 0)
            {
                var width = (this._scrollBar.Visible) ? this.Width - scrollBarWidth : this.Width;
                if (this._isLileList)
                {
                    this._itemWidth = width;
                }

                var colCount = Math.Max(Utility.ToRoundDown(width / (float)this._itemWidth), 1);
                var rowCount = Utility.ToRoundUp(this._itemCount / (float)colCount);
                var virtualHeight = this._itemHeight * rowCount;
                var itemSideSpace = Math.Max((int)((width - this.ItemWidth * colCount) / (float)(colCount + 1)), 0);
                int scrollBarMaximum;
                if (virtualHeight <= this.Height)
                {
                    scrollBarMaximum = this._scrollBar.Minimum;
                }
                else
                {
                    scrollBarMaximum = virtualHeight - this.Height;
                    width = this.Width - scrollBarWidth;
                    if (this._isLileList)
                    {
                        this._itemWidth = width;
                    }
                }

                var drawFirstRow = Utility.ToRoundDown(this._scrollBar.Value / (float)this._itemHeight);
                var drawRowCount = Utility.ToRoundUp((this.Height - ((drawFirstRow + 1) * this._itemHeight - this._scrollBar.Value)) / (float)this._itemHeight);
                var drawLastRow = drawFirstRow + drawRowCount + 1;
                var drawFirstItemIndex = drawFirstRow * colCount;
                var drawLastItemIndex = drawLastRow * colCount - 1;
                for (var row = drawLastRow; row >= drawFirstRow; row--)
                {
                    if (drawLastItemIndex < this._itemCount)
                    {
                        break;
                    }

                    for (var colIndex = colCount - 1; colIndex >= 0; colIndex--)
                    {
                        if (drawLastItemIndex < this._itemCount)
                        {
                            break;
                        }

                        drawLastItemIndex--;
                    }
                }

                if (drawLastItemIndex < 0)
                {
                    drawLastItemIndex = 0;
                }

                this._drawParameter = new DrawParameter(rowCount, colCount,
                                                        drawFirstRow, drawLastRow,
                                                        drawFirstItemIndex, drawLastItemIndex,
                                                        itemSideSpace,
                                                        scrollBarMaximum);

                if (this._drawParameter.ScrollBarMaximum > 0)
                {
                    this._scrollBar.LargeChange = this._itemHeight * 2;
                    this._scrollBar.SmallChange = (int)(this._scrollBar.LargeChange / 4f);
                    this._scrollBar.Maximum = this._drawParameter.ScrollBarMaximum;
                    this._scrollBar.Value = Math.Min(this._scrollBar.Value, this._scrollBar.Maximum);
                    this._scrollBar.Visible = true;
                }
                else
                {
                    this._scrollBar.Visible = false;
                    this._scrollBar.LargeChange = this._itemHeight * 2;
                    this._scrollBar.SmallChange = (int)(this._scrollBar.LargeChange / 4f);
                    this._scrollBar.Value = this._scrollBar.Minimum;
                    this._scrollBar.Maximum = this._drawParameter.ScrollBarMaximum;
                }
            }
            else
            {
                this._drawParameter = new DrawParameter();
                this._scrollBar.Visible = false;
                this._scrollBar.LargeChange = this._itemHeight * 2;
                this._scrollBar.SmallChange = (int)(this._scrollBar.LargeChange / 4f);
                this._scrollBar.Value = this._scrollBar.Minimum;
                this._scrollBar.Maximum = this._drawParameter.ScrollBarMaximum;
            }

            if (isForced)
            {
                this.OnDrawItemChanged(new SKDrawItemChangedEventArgs(this._drawParameter.DrawFirstItemIndex, this._drawParameter.DrawLastItemIndex));
            }
            else
            {
                if (this._drawParameter.DrawFirstItemIndex == 0 && this._drawParameter.DrawLastItemIndex == 0 ||
                    beforeDrawParameter.DrawFirstItemIndex != this._drawParameter.DrawFirstItemIndex ||
                    beforeDrawParameter.DrawLastItemIndex != this._drawParameter.DrawLastItemIndex)
                {
                    this.OnDrawItemChanged(new SKDrawItemChangedEventArgs(this._drawParameter.DrawFirstItemIndex, this._drawParameter.DrawLastItemIndex));
                }
            }
        }

        private HitTestInfo GetHitTestFromDrawPoint(int x, int y)
        {
            var col = this.GetColFromX(x);
            if (col > this._drawParameter.ColCount - 1 || col < 0)
            {
                return new HitTestInfo();
            }

            var row = this.GetRowFromDrawY(y);
            if (row > this._drawParameter.RowCount - 1 || row < 0)
            {
                return new HitTestInfo();
            }

            var itemIndex = this._drawParameter.ColCount * row + col;
            if (itemIndex >= this._itemCount)
            {
                return new HitTestInfo();
            }

            var drawRect = this.GetItemDrawRectangle(row, col);
            var virtualRect = this.GetItemVirtualRectangle(itemIndex);
            var isItem = drawRect.Contains(x, y);
            var isSelected = this._selectedItemIndexs.Contains(itemIndex);
            var isMousePoint = this._mousePointItemIndex == itemIndex;
            var isFocus = this._foucusItemIndex == itemIndex;

            return new HitTestInfo(itemIndex, row, col, virtualRect, drawRect, isItem, isSelected, isMousePoint, isFocus);
        }

        private ItemIndexList GetItemIndexsFromVirtualRectangle(SKRectI rect)
        {
            var firstRow = Math.Max(0, this.GetRowFromVirtualY((int)rect.Top));
            var firstCol = Math.Max(0, this.GetColFromX((int)rect.Left));
            var lastRow = Math.Min(this.GetRowFromVirtualY((int)rect.Bottom), this._drawParameter.RowCount - 1);
            var lastCol = Math.Min(this.GetColFromX((int)rect.Right), this._drawParameter.ColCount - 1);

            Cell getFirstCell()
            {
                for (var row = firstRow; row <= lastRow; row++)
                {
                    for (var col = firstCol; col <= lastCol; col++)
                    {
                        var itemIndex = this._drawParameter.ColCount * row + col;
                        if (itemIndex < this._itemCount)
                        {
                            var itemRect = this.GetItemVirtualRectangle(itemIndex);
                            if (rect.IntersectsWith(itemRect))
                            {
                                return new Cell(row, col);
                            }
                        }
                    }
                }

                return null;
            }

            Cell getLastCell()
            {
                for (var row = lastRow; row >= firstRow; row--)
                {
                    for (var col = lastCol; col >= firstCol; col--)
                    {
                        var itemIndex = this._drawParameter.ColCount * row + col;
                        var itemRect = this.GetItemVirtualRectangle(itemIndex);
                        if (rect.IntersectsWith(itemRect))
                        {
                            // 項目数を上回る場合があります。
                            return new Cell(row, col);
                        }
                    }
                }

                throw new InvalidOperationException("最後のセルが取得できません。");
            }

            var firstCell = getFirstCell();
            if (firstCell == null)
            {
                return new ItemIndexList();
            }

            var lastCell = getLastCell();

            var list = new ItemIndexList();

            for (var row = firstCell.Row; row <= lastCell.Row; row++)
            {
                for (var col = firstCell.Col; col <= lastCell.Col; col++)
                {
                    var itemIndex = this._drawParameter.ColCount * row + col;
                    if (itemIndex < this._itemCount)
                    {
                        list.Add(itemIndex);
                    }
                    else
                    {
                        return list;
                    }
                }
            }

            return list;
        }

        private bool EnsureVisible(int itemIndex, bool isScroll)
        {
            if (isScroll)
            {
                var drawRect = this.GetItemDrawRectangle(itemIndex);
                if (drawRect.Top < 0)
                {
                    var virtualRect = this.GetItemVirtualRectangle(itemIndex);
                    if (!this._animationTimer.Enabled)
                    {
                        this._targetVerticalScroll = virtualRect.Top - this._itemSpace;
                    }
                    else
                    {
                        this._targetVerticalScroll += virtualRect.Top - this._itemSpace - this._targetVerticalScroll;
                    }

                    this.StartAnimation();
                    return true;
                }
                else if (drawRect.Bottom > this.Height)
                {
                    var virtualRect = this.GetItemVirtualRectangle(itemIndex);
                    if (!this._animationTimer.Enabled)
                    {
                        this._targetVerticalScroll = virtualRect.Bottom - this.Height + this._itemSpace;
                    }
                    else
                    {
                        this._targetVerticalScroll += virtualRect.Bottom - this.Height + this._itemSpace - this._targetVerticalScroll;
                    }

                    this.StartAnimation();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                var drawRect = this.GetItemDrawRectangle(itemIndex);
                if (drawRect.Top < 0)
                {
                    var virtualRect = this.GetItemVirtualRectangle(itemIndex);
                    this._scrollBar.Value = virtualRect.Top - this._itemSpace;
                    return true;
                }
                else if (drawRect.Bottom > this.Height)
                {
                    var virtualRect = this.GetItemVirtualRectangle(itemIndex);
                    this._scrollBar.Value = virtualRect.Bottom - this.Height + this._itemSpace;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void SelectAll()
        {
            if (this._rectangleSelection.IsBegun)
            {
                throw new InvalidOperationException("短形選択中は全選択を行いません。");
            }
            else
            {
                this._selectedItemIndexs.Clear();
                for (var i = 0; i < this._itemCount; i++)
                {
                    this._selectedItemIndexs.Add(i);
                }
            }
        }

        private SKRectI GetItemVirtualRectangle(int row, int col)
        {
            var x = col * (this._itemWidth + this._drawParameter.ItemSideSpace) + this._drawParameter.ItemSideSpace + this._itemSpace;
            var y = row * this._itemHeight + this._itemSpace;
            var margin = this._itemSpace * 2;
            return SKRectI.Create(x, y, this._itemWidth - margin, this._itemHeight - margin);
        }

        private SKRectI GetItemVirtualRectangle(int itemIndex)
        {
            var col = itemIndex % this._drawParameter.ColCount;
            var row = Utility.ToRoundDown(itemIndex / (float)this._drawParameter.ColCount);
            return this.GetItemVirtualRectangle(row, col);
        }

        private SKRectI GetItemDrawRectangle(int row, int col)
        {
            var x = col * (this._itemWidth + this._drawParameter.ItemSideSpace) + this._drawParameter.ItemSideSpace + this._itemSpace;
            var y = row * this._itemHeight + this._itemSpace - this._scrollBar.Value;
            var margin = this._itemSpace * 2;
            return SKRectI.Create(x, y, this._itemWidth - margin, this._itemHeight - margin);
        }

        private SKRectI GetItemDrawRectangle(int itemIndex)
        {
            var col = this._drawParameter.ColCount switch
            {
                0 => 0,
                _ => itemIndex % this._drawParameter.ColCount,
            };
            var row = Utility.ToRoundDown(itemIndex / (float)this._drawParameter.ColCount);
            return this.GetItemDrawRectangle(row, col);
        }

        private void DrawRectangleSelection(SKCanvas canvas)
        {
            var rect = this._rectangleSelection.GetDrawRectangle(this._scrollBar.Value);

            canvas.DrawRect(rect, SKFlowListResources.DARK_RECTANGLE_SELECTION_FILL_PAINT);
            canvas.DrawRect(rect, SKFlowListResources.GetDarkRectangleSelectionStrokePatint(this));
        }

        private int GetRowFromVirtualY(int y)
        {
            return Utility.ToRoundDown(y / (float)this._itemHeight);
        }

        private int GetRowFromDrawY(int y)
        {
            return Utility.ToRoundDown((y + this._scrollBar.Value) / (float)this._itemHeight);
        }

        private int GetColFromX(int x)
        {
            return Utility.ToRoundDown((x - this._drawParameter.ItemSideSpace - this._itemSpace) / (float)(this._itemWidth + this._drawParameter.ItemSideSpace));
        }

        private bool LeftKeyDown()
        {
            if (this._itemCount == 0)
            {
                throw new InvalidOperationException("項目が存在しません。");
            }

            var newIndex = this._foucusItemIndex - 1;
            if (newIndex > -1)
            {
                if (this._isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    // Shiftキー押下
                    this._selectedItemIndexs.AddRange(newIndex);
                }
                else if (this._isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    // Ctrlキー押下
                }
                else
                {
                    // 修飾キー無し
                    this._selectedItemIndexs.Clear();
                    this._selectedItemIndexs.Add(newIndex);
                }

                this.EnsureVisible(newIndex, true);

                this._foucusItemIndex = newIndex;

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool RightKeyDown()
        {
            if (this._itemCount == 0)
            {
                throw new InvalidOperationException("項目が存在しません。");
            }

            var newIndex = this._foucusItemIndex + 1;
            if (newIndex < this._itemCount)
            {
                if (this._isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    // Shiftキー押下
                    this._selectedItemIndexs.AddRange(newIndex);
                }
                else if (this._isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    // Ctrlキー押下
                }
                else
                {
                    // 修飾キー無し
                    this._selectedItemIndexs.Clear();
                    this._selectedItemIndexs.Add(newIndex);
                }

                this.EnsureVisible(newIndex, true);

                this._foucusItemIndex = newIndex;

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool UpKeyDown()
        {
            if (this._itemCount == 0)
            {
                throw new InvalidOperationException("項目が存在しません。");
            }

            var newIndex = this._foucusItemIndex - this._drawParameter.ColCount;
            if (newIndex > -1)
            {
                if (this._isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    // Shiftキー押下
                    this._selectedItemIndexs.AddRange(newIndex);
                }
                else if (this._isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    // Ctrlキー押下
                }
                else
                {
                    // 修飾キー無し
                    this._selectedItemIndexs.Clear();
                    this._selectedItemIndexs.Add(newIndex);
                }

                this.EnsureVisible(newIndex, true);

                this._foucusItemIndex = newIndex;

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool DownKeyDown()
        {
            if (this._itemCount == 0)
            {
                throw new InvalidOperationException("項目が存在しません。");
            }

            var newIndex = this._foucusItemIndex + this._drawParameter.ColCount;

            if (newIndex < this._itemCount)
            {
                if (this._isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    // Shiftキー押下
                    this._selectedItemIndexs.AddRange(newIndex);
                }
                else if (this._isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    // Ctrlキー押下
                }
                else
                {
                    // 修飾キー無し
                    this._selectedItemIndexs.Clear();
                    this._selectedItemIndexs.Add(newIndex);
                }

                this.EnsureVisible(newIndex, true);

                this._foucusItemIndex = newIndex;

                return true;
            }
            else
            {
                return false;
            }
        }

        private void StartAnimation()
        {
            this._currentScrollPosition = this._scrollBar.Value;
            this._animationStartValue = this._currentScrollPosition;
            this._animationStopwatch = Stopwatch.StartNew();

            if (!this._animationTimer.Enabled)
            {
                this._animationTimer.Start(DisplayUtil.GetAnimationInterval(this));
            }
        }

        private void SpaceKeyDown()
        {
            if (!this._selectedItemIndexs.Contains(this._foucusItemIndex))
            {
                this._selectedItemIndexs.Add(this._foucusItemIndex);
            }
            else
            {
                this._selectedItemIndexs.Remove(this._foucusItemIndex);
            }
        }

        private void PageUpKeyDown()
        {
            if (this._itemCount == 0)
            {
                throw new InvalidOperationException("項目が存在しません。");
            }

            var newIndex = this._foucusItemIndex - this._drawParameter.ColCount * (this._drawParameter.DrawLastRow - this._drawParameter.DrawFirstRow - 1);
            if (newIndex < 0)
            {
                newIndex = 0;
            }

            if (this._isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                // Shiftキー押下
                this._selectedItemIndexs.AddRange(newIndex);
            }
            else if (this._isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                // Ctrlキー押下
            }
            else
            {
                // 修飾キー無し
                this._selectedItemIndexs.Clear();
                this._selectedItemIndexs.Add(newIndex);
            }

            this.EnsureVisible(newIndex, true);

            this._foucusItemIndex = newIndex;
        }

        private void PageDownKeyDown()
        {
            if (this._itemCount == 0)
            {
                throw new InvalidOperationException("項目が存在しません。");
            }

            var newIndex = this._foucusItemIndex + this._drawParameter.ColCount * (this._drawParameter.DrawLastRow - this._drawParameter.DrawFirstRow - 1);
            if (newIndex >= this._itemCount)
            {
                newIndex = this._itemCount - 1;
            }

            if (this._isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                // Shiftキー押下
                this._selectedItemIndexs.AddRange(newIndex);
            }
            else if (this._isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                // Ctrlキー押下
            }
            else
            {
                // 修飾キー無し
                this._selectedItemIndexs.Clear();
                this._selectedItemIndexs.Add(newIndex);
            }

            this.EnsureVisible(newIndex, true);

            this._foucusItemIndex = newIndex;
        }

        private void HomeKeyDown()
        {
            if (this._itemCount == 0)
            {
                throw new InvalidOperationException("項目が存在しません。");
            }

            var newIndex = 0;
            if (this._isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                // Shiftキー押下
                this._selectedItemIndexs.AddRange(newIndex);
            }
            else if (this._isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                // Ctrlキー押下
            }
            else
            {
                // 修飾キー無し
                this._selectedItemIndexs.Clear();
                this._selectedItemIndexs.Add(newIndex);
            }

            this.EnsureVisible(newIndex, true);

            this._foucusItemIndex = newIndex;
        }

        private void EndKeyDown()
        {
            if (this._itemCount == 0)
            {
                throw new InvalidOperationException("項目が存在しません。");
            }

            var newIndex = this._itemCount - 1;
            if (this._isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                // Shiftキー押下
                this._selectedItemIndexs.AddRange(newIndex);
            }
            else if (this._isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                // Ctrlキー押下
            }
            else
            {
                // 修飾キー無し
                this._selectedItemIndexs.Clear();
                this._selectedItemIndexs.Add(newIndex);
            }

            this.EnsureVisible(newIndex, true);

            this._foucusItemIndex = newIndex;
        }

        private void LeftMouseDown(int x, int y)
        {
            var ht = this.GetHitTestFromDrawPoint(x, y);

            if (this._isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                // Shiftキー押下
                if (ht.IsItem)
                {
                    this._selectedItemIndexs.AddRange(ht.ItemIndex);
                    this._foucusItemIndex = ht.ItemIndex;
                }
            }
            else if (this._isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                // Ctrlキー押下
                if (ht.IsItem)
                {
                    if (!this._selectedItemIndexs.Contains(ht.ItemIndex))
                    {
                        this._selectedItemIndexs.Add(ht.ItemIndex);
                    }
                    else
                    {
                        this._selectedItemIndexs.Remove(ht.ItemIndex);
                    }

                    this._foucusItemIndex = ht.ItemIndex;
                }
            }
            else
            {
                // 修飾キー無し
                if (ht.IsItem)
                {
                    if (this._selectedItemIndexs.Contains(ht.ItemIndex))
                    {
                        this._foucusItemIndex = ht.ItemIndex;
                    }
                    else
                    {
                        this._selectedItemIndexs.Clear();
                        this._selectedItemIndexs.Add(ht.ItemIndex);
                        this._foucusItemIndex = ht.ItemIndex;
                    }
                }
                else
                {
                    this._selectedItemIndexs.Clear();
                }
            }
        }

        private void RightMouseDown(int x, int y)
        {
            var ht = this.GetHitTestFromDrawPoint(x, y);

            if (ht.IsItem)
            {
                if (ht.IsSelected)
                {
                    this._foucusItemIndex = ht.ItemIndex;
                }
                else
                {
                    this._selectedItemIndexs.Clear();
                    this._selectedItemIndexs.Add(ht.ItemIndex);
                    this._foucusItemIndex = ht.ItemIndex;
                }
            }
            else
            {
                this._selectedItemIndexs.Clear();
            }
        }

        private void MiddleMouseDown(int x, int y)
        {
            var ht = this.GetHitTestFromDrawPoint(x, y);

            if (ht.IsItem)
            {
                if (ht.IsSelected)
                {
                    this._foucusItemIndex = ht.ItemIndex;
                }
                else
                {
                    this._selectedItemIndexs.Clear();
                    this._selectedItemIndexs.Add(ht.ItemIndex);
                    this._foucusItemIndex = ht.ItemIndex;
                }
            }
            else
            {
                this._selectedItemIndexs.Clear();
            }
        }

        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (this._itemCount < 1)
            {
                e.Cancel = true;
            }
            else if (this._rectangleSelection.IsBegun)
            {
                e.Cancel = true;
            }
        }

        private void ScrollBar_ValueChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.LargeIncrement
                || e.Type == ScrollEventType.SmallIncrement
                || e.Type == ScrollEventType.LargeDecrement
                || e.Type == ScrollEventType.SmallDecrement)
            {
                if (!this._animationTimer.Enabled)
                {
                    this._targetVerticalScroll = e.NewValue;
                    e.NewValue = e.OldValue;
                }
                else
                {
                    var scrollDelta = (e.NewValue - e.OldValue);
                    this._targetVerticalScroll += scrollDelta;
                    e.NewValue = e.OldValue;
                }

                if (this._targetVerticalScroll < this._scrollBar.Minimum)
                {
                    this._targetVerticalScroll = this._scrollBar.Minimum;
                }
                else if (this._targetVerticalScroll > this._scrollBar.Maximum)
                {
                    this._targetVerticalScroll = this._scrollBar.Maximum;
                }

                this.StartAnimation();
            }
            else if (e.Type == ScrollEventType.ThumbTrack)
            {
                this._animationTimer.Stop();
            }
        }

        private void SelectedItemIndexs_Change(object sender, EventArgs e)
        {
            this.OnSelectedItemChanged(EventArgs.Empty);
        }

        private void OnSKDrawItem(SKDrawItemEventArgs e)
        {
            this.SKDrawItem?.Invoke(this, e);
        }

        private void OnDrawItemChanged(SKDrawItemChangedEventArgs e)
        {
            this.DrawItemChanged?.Invoke(this, e);
        }

        private void OnSKDrawItems(SKDrawItemsEventArgs e)
        {
            this.SKDrawItems?.Invoke(this, e);
        }

        private void OnSelectedItemChanged(EventArgs e)
        {
            this.SelectedItemChanged?.Invoke(this, e);
        }

        private void OnItemMouseClick(MouseEventArgs e)
        {
            this.ItemMouseClick?.Invoke(this, e);
        }

        private void OnItemMouseDoubleClick(MouseEventArgs e)
        {
            this.ItemMouseDoubleClick?.Invoke(this, e);
        }

        private void OnBackgroundMouseClick(MouseEventArgs e)
        {
            this.BackgroundMouseClick?.Invoke(this, e);
        }

        private void OnItemExecute(EventArgs e)
        {
            this.ItemExecute?.Invoke(this, e);
        }

        private void OnItemDelete(EventArgs e)
        {
            this.ItemDelete?.Invoke(this, e);
        }

        private void OnItemCopy(EventArgs e)
        {
            this.ItemCopy?.Invoke(this, e);
        }

        private void OnItemCut(EventArgs e)
        {
            this.ItemCut?.Invoke(this, e);
        }

        private void OnDragStart(EventArgs e)
        {
            this.DragStart?.Invoke(this, e);
        }
    }
}
