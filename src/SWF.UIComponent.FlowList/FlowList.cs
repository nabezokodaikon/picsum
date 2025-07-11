using SWF.Core.Base;
using SWF.UIComponent.Core;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.FlowList
{
    /// <summary>
    /// フローリストコントロール
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class FlowList
        : UserControl
    {
        // 項目最小サイズ
        public const int MINIMUM_ITEM_SIZE = 16;

        // 項目最大サイズ
        public const int MAXIMUM_ITEM_SIZE = 512;

        private static readonly Color ITEM_TEXT_COLOR = Color.FromArgb(
            SystemColors.ControlText.A,
            SystemColors.ControlText.R,
            SystemColors.ControlText.G,
            SystemColors.ControlText.B);

        private static readonly Color SELECTED_ITEM_COLOR = Color.FromArgb(
            SystemColors.Highlight.A / 8,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B);

        private static readonly Color FOCUS_ITEM_COLOR = Color.FromArgb(
            SystemColors.Highlight.A / 8,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B);

        private static readonly Color MOUSE_POINT_ITEM_COLOR = Color.FromArgb(
            SystemColors.Highlight.A / 8,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B);

        private static readonly Color RECTANGLE_SELECTION_COLOR = Color.FromArgb(
            SystemColors.Highlight.A / 4,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B);

        private static readonly SolidBrush ITEM_TEXT_BRUSH = new(ITEM_TEXT_COLOR);
        private static readonly SolidBrush SELECTED_ITEM_BRUSH = new(SELECTED_ITEM_COLOR);
        private static readonly Pen SELECTED_ITEM_PEN = new(Color.FromArgb(
            255,
            SELECTED_ITEM_COLOR.R,
            SELECTED_ITEM_COLOR.G,
            SELECTED_ITEM_COLOR.B),
            2);
        private static readonly SolidBrush FOUCUS_ITEM_BRUSH = new(FOCUS_ITEM_COLOR);
        private static readonly SolidBrush MOUSE_POINT_ITEM_BRUSH = new(MOUSE_POINT_ITEM_COLOR);
        private static readonly SolidBrush RECTANGLE_SELECTION_BRUSH = new(RECTANGLE_SELECTION_COLOR);
        private static readonly Pen RECTANGLE_SELECTION_PEN = new(Color.FromArgb(
            RECTANGLE_SELECTION_COLOR.A * 2,
            RECTANGLE_SELECTION_COLOR.R,
            RECTANGLE_SELECTION_COLOR.G,
            RECTANGLE_SELECTION_COLOR.B));

        private StringTrimming _itemTextTrimming = StringTrimming.EllipsisCharacter;
        private StringAlignment _itemTextAlignment = StringAlignment.Center;
        private StringAlignment _itemTextLineAlignment = StringAlignment.Center;
        private StringFormatFlags _itemTextFormatFlags = 0;
        private StringFormat _itemTextFormat = null;

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

        // ドラッグフラグ
        private bool _isDrag = false;

        private SolidBrush RectangleSelectionBrush
        {
            get
            {
                return RECTANGLE_SELECTION_BRUSH;
            }
        }

        private Pen RectangleSelectionPen
        {
            get
            {
                return RECTANGLE_SELECTION_PEN;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new BorderStyle BorderStyle
        {
            get
            {
                return base.BorderStyle;
            }
            private set
            {
                base.BorderStyle = value;
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

        public FlowList()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);
            this.UpdateStyles();

            this._scrollBar.Dock = DockStyle.Right;
            this._scrollBar.ValueChanged += new(this.ScrollBar_ValueChanged);
            this._selectedItemIndexs.Change += new(this.SelectedItemIndexs_Change);

            base.BorderStyle = BorderStyle.None;
            this.Controls.Add(this._scrollBar);

            this.KeyDown += this.FlowList_KeyDown;
            this.MouseLeave += this.FlowList_MouseLeave;
            this.MouseDown += this.FlowList_MouseDown;
            this.Invalidated += this.FlowList_Invalidated;
            this.MouseUp += this.FlowList_MouseUp;
            this.MouseMove += this.FlowList_MouseMove;
            this.MouseDoubleClick += this.FlowList_MouseDoubleClick;
            this.MouseWheel += this.FlowList_MouseWheel;
            this.Paint += this.FlowList_Paint;
            this.Resize += this.FlowList_Resize;
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

        private void FlowList_Invalidated(object sender, InvalidateEventArgs e)
        {
            this.SetDrawParameter(false);
        }

        private void FlowList_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void FlowList_Paint(object sender, PaintEventArgs e)
        {
            using (TimeMeasuring.Run(false, "FlowList.FlowList_Paint"))
            {
                if (!this._isDraw)
                {
                    return;
                }

                e.Graphics.SmoothingMode = SmoothingMode.None;
                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                e.Graphics.CompositingMode = CompositingMode.SourceOver;

                if (this._rectangleSelection.IsBegun)
                {
                    this.DrawRectangleSelection(e.Graphics);
                }

                if (this._itemCount > 0)
                {

                    this.DrawItems(e.Graphics);
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

        private void FlowList_MouseLeave(object sender, EventArgs e)
        {
            this._mousePointItemIndex = -1;
            this._isDrag = false;
            this._mouseDownHitTestInfo = new HitTestInfo();

            this.Invalidate();
        }

        private void FlowList_MouseDown(object sender, MouseEventArgs e)
        {
            this.Select();

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
                        this.Invalidate(newRect);
                        if (oldIndex > -1)
                        {
                            var oldRect = this.GetItemDrawRectangle(oldIndex);
                            this.Invalidate(oldRect, true);
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
                        this.Invalidate(oldRect, true);
                    }
                }

                if (e.Button == MouseButtons.Left)
                {
                    // ドラッグとしないマウスの移動範囲を取得します。
                    var moveRect = new Rectangle(this._mouseDownHitTestInfo.DrawRectangle.X - SystemInformation.DragSize.Width / 2,
                                                 this._mouseDownHitTestInfo.DrawRectangle.Y - SystemInformation.DragSize.Height / 2,
                                                 SystemInformation.DragSize.Width,
                                                 SystemInformation.DragSize.Height);
                    if (!moveRect.Contains(e.X, e.Y))
                    {
                        if (this._mouseDownHitTestInfo.IsItem && !this._isDrag)
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
            else if (e.Delta != 0)
            {
                var value = this._scrollBar.Value - (int)(this._itemHeight * 0.8 * (e.Delta / Math.Abs(e.Delta)));
                if (value < this._scrollBar.Minimum)
                {
                    this._scrollBar.Value = this._scrollBar.Minimum;
                }
                else if (value > this._scrollBar.Maximum)
                {
                    this._scrollBar.Value = this._scrollBar.Maximum;
                }
                else
                {
                    this._scrollBar.Value = value;
                }
            }
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
                    this._scrollBar.LargeChange = (int)(this.Height / 2f);
                    this._scrollBar.SmallChange = (int)(this._scrollBar.LargeChange / 8f);
                    this._scrollBar.Maximum = this._drawParameter.ScrollBarMaximum;
                    this._scrollBar.Visible = true;
                }
                else
                {
                    this._scrollBar.Visible = false;
                    this._scrollBar.LargeChange = this._itemHeight;
                    this._scrollBar.SmallChange = this._scrollBar.LargeChange / 8;
                    this._scrollBar.Value = this._scrollBar.Minimum;
                    this._scrollBar.Maximum = this._drawParameter.ScrollBarMaximum;
                }
            }
            else
            {
                this._drawParameter = new DrawParameter();
                this._scrollBar.Visible = false;
                this._scrollBar.LargeChange = this._itemHeight;
                this._scrollBar.SmallChange = this._scrollBar.LargeChange / 8;
                this._scrollBar.Value = this._scrollBar.Minimum;
                this._scrollBar.Maximum = this._drawParameter.ScrollBarMaximum;
            }

            if (isForced)
            {
                this.OnDrawItemChanged(new DrawItemChangedEventArgs(this._drawParameter.DrawFirstItemIndex, this._drawParameter.DrawLastItemIndex));
            }
            else
            {
                if (this._drawParameter.DrawFirstItemIndex == 0 && this._drawParameter.DrawLastItemIndex == 0 ||
                    beforeDrawParameter.DrawFirstItemIndex != this._drawParameter.DrawFirstItemIndex ||
                    beforeDrawParameter.DrawLastItemIndex != this._drawParameter.DrawLastItemIndex)
                {
                    this.OnDrawItemChanged(new DrawItemChangedEventArgs(this._drawParameter.DrawFirstItemIndex, this._drawParameter.DrawLastItemIndex));
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

        private ItemIndexList GetItemIndexsFromVirtualRectangle(Rectangle rect)
        {
            var firstRow = Math.Max(0, this.GetRowFromVirtualY(rect.Y));
            var firstCol = Math.Max(0, this.GetColFromX(rect.X));
            var lastRow = Math.Min(this.GetRowFromVirtualY(rect.Bottom), this._drawParameter.RowCount - 1);
            var lastCol = Math.Min(this.GetColFromX(rect.Right), this._drawParameter.ColCount - 1);

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

        private bool EnsureVisible(int itemIndex)
        {
            var drawRect = this.GetItemDrawRectangle(itemIndex);
            if (drawRect.Top < 0)
            {
                // 上へスクロール
                var virtualRect = this.GetItemVirtualRectangle(itemIndex);
                this._scrollBar.Value = virtualRect.Top - this._itemSpace;
                return true;
            }
            else if (drawRect.Bottom > this.Height)
            {
                // 下へスクロール              
                var virtualRect = this.GetItemVirtualRectangle(itemIndex);
                this._scrollBar.Value = virtualRect.Bottom - this.Height + this._itemSpace;
                return true;
            }
            else
            {
                // 既に表示されています。
                return false;
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

        private Rectangle GetItemVirtualRectangle(int row, int col)
        {
            var x = col * (this._itemWidth + this._drawParameter.ItemSideSpace) + this._drawParameter.ItemSideSpace + this._itemSpace;
            var y = row * this._itemHeight + this._itemSpace;
            var margin = this._itemSpace * 2;
            return new Rectangle(x, y, this._itemWidth - margin, this._itemHeight - margin);
        }

        private Rectangle GetItemVirtualRectangle(int itemIndex)
        {
            var col = itemIndex % this._drawParameter.ColCount;
            var row = Utility.ToRoundDown(itemIndex / (float)this._drawParameter.ColCount);
            return this.GetItemVirtualRectangle(row, col);
        }

        private Rectangle GetItemDrawRectangle(int row, int col)
        {
            var x = col * (this._itemWidth + this._drawParameter.ItemSideSpace) + this._drawParameter.ItemSideSpace + this._itemSpace;
            var y = row * this._itemHeight + this._itemSpace - this._scrollBar.Value;
            var margin = this._itemSpace * 2;
            return new Rectangle(x, y, this._itemWidth - margin, this._itemHeight - margin);
        }

        private Rectangle GetItemDrawRectangle(int itemIndex)
        {
            var col = this._drawParameter.ColCount switch
            {
                0 => 0,
                _ => itemIndex % this._drawParameter.ColCount,
            };
            var row = Utility.ToRoundDown(itemIndex / (float)this._drawParameter.ColCount);
            return this.GetItemDrawRectangle(row, col);
        }

        private void DrawItems(Graphics g)
        {
            for (var itemIndex = this._drawParameter.DrawFirstItemIndex;
                 itemIndex <= this._drawParameter.DrawLastItemIndex;
                 itemIndex++)
            {
                var drawRect = this.GetItemDrawRectangle(itemIndex);

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

                this.OnDrawItem(new DrawItemEventArgs(g, itemIndex, drawRect, isSelected, isMousePoint, isFocus));
            }
        }

        private void DrawRectangleSelection(Graphics g)
        {
            g.FillRectangle(this.RectangleSelectionBrush, this._rectangleSelection.GetDrawRectangle(this._scrollBar.Value));
            g.DrawRectangle(this.RectangleSelectionPen, this._rectangleSelection.GetDrawRectangle(this._scrollBar.Value));
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

                var itemRect = this.GetItemDrawRectangle(newIndex);
                if (itemRect.Top < 0)
                {
                    this._scrollBar.Value = Math.Max(this._scrollBar.Minimum, this._scrollBar.Value - this._itemHeight);
                }

                this.EnsureVisible(newIndex);

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

                var itemRect = this.GetItemDrawRectangle(newIndex);
                if (itemRect.Bottom > this.Height)
                {
                    this._scrollBar.Value = Math.Min(this._scrollBar.Maximum, this._scrollBar.Value + this._itemHeight);
                }

                this.EnsureVisible(newIndex);

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

                var itemRect = this.GetItemDrawRectangle(newIndex);
                if (itemRect.Top < 0)
                {
                    this._scrollBar.Value = Math.Max(this._scrollBar.Minimum, this._scrollBar.Value - this._itemHeight);
                }

                this.EnsureVisible(newIndex);

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

                var itemRect = this.GetItemDrawRectangle(newIndex);
                if (itemRect.Bottom > this.Height)
                {
                    this._scrollBar.Value = Math.Min(this._scrollBar.Maximum, this._scrollBar.Value + this._itemHeight);
                }

                this.EnsureVisible(newIndex);

                this._foucusItemIndex = newIndex;

                return true;
            }
            else
            {
                return false;
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

            this.EnsureVisible(newIndex);

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

            this.EnsureVisible(newIndex);

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

            this.EnsureVisible(newIndex);

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

            this.EnsureVisible(newIndex);

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

        private void SelectedItemIndexs_Change(object sender, EventArgs e)
        {
            this.OnSelectedItemChanged(EventArgs.Empty);
        }
    }
}
