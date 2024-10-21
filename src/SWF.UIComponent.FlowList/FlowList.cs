using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.FlowList
{
    /// <summary>
    /// フローリストコントロール
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed partial class FlowList
        : UserControl
    {

        // 項目最小サイズ
        public const int MINIMUM_ITEM_SIZE = 16;

        // 項目最大サイズ
        public const int MAXIMUM_ITEM_SIZE = 512;

        // 描画フラグ
        private bool isDraw = true;

        // 複数選択可能フラグ
        private bool isMultiSelect = false;

        // 行リストフラグ
        private bool isLileList = false;

        // 項目数
        private int itemCount = 0;

        // 項目幅
        private int itemWidth = MINIMUM_ITEM_SIZE;

        // 項目高さ
        private int itemHeight = MINIMUM_ITEM_SIZE;

        // 項目間余白
        private int itemSpace = 0;

        // 描画パラメータ
        private DrawParameter drawParameter = new();

        // 垂直スクロールバー
        private readonly VScrollBarEx scrollBar = new();

        // フォーカスされている項目のインデックス
        private int foucusItemIndex = -1;

        // マウスポイントされている項目のインデックス
        private int mousePointItemIndex = -1;

        // 選択されている項目インデックスのリスト
        private readonly ItemIndexList selectedItemIndexs = new();

        // 短形選択クラス
        private readonly RectangleSelection rectangleSelection = new();

        // マウスダウンした座標情報
        private HitTestInfo mouseDownHitTestInfo = new();

        // ドラッグフラグ
        private bool isDrag = false;

        private readonly Color itemTextColor = Color.FromArgb(
            SystemColors.ControlText.A,
            SystemColors.ControlText.R,
            SystemColors.ControlText.G,
            SystemColors.ControlText.B);

        private readonly Color selectedItemColor = Color.FromArgb(
            SystemColors.Highlight.A / 8,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B);

        private readonly Color focusItemColor = Color.FromArgb(
            SystemColors.Highlight.A / 8,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B);

        private readonly Color mousePointItemColor = Color.FromArgb(
            SystemColors.Highlight.A / 8,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B);

        private readonly Color rectangleSelectionColor = Color.FromArgb(
            SystemColors.Highlight.A / 4,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B);

        private StringTrimming itemTextTrimming = StringTrimming.EllipsisCharacter;
        private StringAlignment itemTextAlignment = StringAlignment.Center;
        private StringAlignment itemTextLineAlignment = StringAlignment.Center;
        private StringFormatFlags itemTextFormatFlags = 0;

        private SolidBrush itemTextBrush = null;
        private SolidBrush selectedItemBrush = null;
        private Pen selectedItemPen = null;
        private SolidBrush foucusItemBrush = null;
        private SolidBrush mousePointItemBrush = null;
        private SolidBrush rectangleSelectionBrush = null;
        private Pen rectangleSelectionPen = null;
        private StringFormat itemTextFormat = null;

        private SolidBrush RectangleSelectionBrush
        {
            get
            {
                this.rectangleSelectionBrush ??= new(this.rectangleSelectionColor);
                return this.rectangleSelectionBrush;
            }
        }

        private Pen RectangleSelectionPen
        {
            get
            {
                this.rectangleSelectionPen ??= new(Color.FromArgb(
                    this.rectangleSelectionColor.A * 2,
                    this.rectangleSelectionColor.R,
                    this.rectangleSelectionColor.G,
                    this.rectangleSelectionColor.B));
                return this.rectangleSelectionPen;
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

            this.scrollBar.Dock = DockStyle.Right;
            this.scrollBar.ValueChanged += new(this.ScrollBar_ValueChanged);
            this.selectedItemIndexs.Change += new(this.SelectedItemIndexs_Change);

            this.Controls.Add(this.scrollBar);
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            this.SetDrawParameter();

            base.OnInvalidated(e);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (!this.isDraw)
            {
                return;
            }

            base.OnPaintBackground(pevent);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!this.isDraw)
            {
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.None;
            e.Graphics.InterpolationMode = InterpolationMode.Low;
            e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;

            if (this.rectangleSelection.IsBegun)
            {
                this.DrawRectangleSelection(e.Graphics);
            }

            if (this.itemCount > 0)
            {
                this.DrawItems(e.Graphics);
            }

            base.OnPaint(e);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if ((keyData & Keys.Alt) != Keys.Alt)
            {
                var kcode = keyData & Keys.KeyCode;

                if (kcode == Keys.Up ||
                    kcode == Keys.Down ||
                    kcode == Keys.Left ||
                    kcode == Keys.Right)
                {
                    return true;
                }
            }

            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!this.CanKeyDown)
            {
                base.OnKeyDown(e);
                return;
            }

            if (this.itemCount > 0 && !this.rectangleSelection.IsBegun)
            {
                this.selectedItemIndexs.BeginUpdate();

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
                            if (this.isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
                            {
                                this.SelectAll();
                                this.Invalidate();
                            }
                            break;
                        case Keys.Enter:
                            if (this.selectedItemIndexs.Count > 0)
                            {
                                this.OnItemExecute(EventArgs.Empty);
                            }
                            break;
                        case Keys.Delete:
                            if (this.selectedItemIndexs.Count > 0)
                            {
                                if (this.selectedItemIndexs.Contains(this.foucusItemIndex))
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
                            if (this.selectedItemIndexs.Count > 0)
                            {
                                if (this.selectedItemIndexs.Contains(this.foucusItemIndex))
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
                            if (this.selectedItemIndexs.Count > 0)
                            {
                                if (this.selectedItemIndexs.Contains(this.foucusItemIndex))
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
                    this.selectedItemIndexs.EndUpdate();
                }
            }

            base.OnKeyDown(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.mousePointItemIndex = -1;
            this.isDrag = false;
            this.mouseDownHitTestInfo = new HitTestInfo();

            this.Invalidate();

            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Select();

            if (e.Button == MouseButtons.Left)
            {
                // 左ボタン
                this.selectedItemIndexs.BeginUpdate();
                try
                {
                    this.LeftMouseDown(e.X, e.Y);
                }
                finally
                {
                    this.Invalidate();
                    this.selectedItemIndexs.EndUpdate();
                }

                // マウスダウンした座標情報を保持します。
                this.mouseDownHitTestInfo = this.GetHitTestFromDrawPoint(e.X, e.Y);
            }
            else if (e.Button == MouseButtons.Right)
            {
                // 右ボタン
                this.selectedItemIndexs.BeginUpdate();
                try
                {
                    this.RightMouseDown(e.X, e.Y);
                }
                finally
                {
                    this.Invalidate();
                    this.selectedItemIndexs.EndUpdate();
                }
            }
            else if (e.Button == MouseButtons.Middle)
            {
                // ミドルボタン
                this.selectedItemIndexs.BeginUpdate();
                try
                {
                    this.MiddleMouseDown(e.X, e.Y);
                }
                finally
                {
                    this.Invalidate();
                    this.selectedItemIndexs.EndUpdate();
                }
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (this.rectangleSelection.IsBegun)
            {
                var itemIndexs = this.GetItemIndexsFromVirtualRectangle(this.rectangleSelection.VirtualRectangle);
                this.rectangleSelection.EndSelection();
                this.selectedItemIndexs.Union(itemIndexs);
                this.Invalidate();
            }
            else
            {
                var ht = this.GetHitTestFromDrawPoint(e.X, e.Y);
                if (ht.IsItem)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        if (!this.isDrag &&
                            (Control.ModifierKeys & Keys.Shift) != Keys.Shift &&
                            (Control.ModifierKeys & Keys.Control) != Keys.Control)
                        {
                            this.selectedItemIndexs.BeginUpdate();
                            try
                            {
                                this.selectedItemIndexs.Clear();
                                this.selectedItemIndexs.Add(ht.ItemIndex);
                            }
                            finally
                            {
                                this.Invalidate();
                                this.selectedItemIndexs.EndUpdate();
                            }
                        }
                    }

                    if (this.selectedItemIndexs.Count > 0)
                    {
                        this.OnItemMouseClick(e);
                    }
                }
                else
                {
                    this.OnBackgroundMouseClick(e);
                }
            }

            this.isDrag = false;
            this.mouseDownHitTestInfo = new HitTestInfo();

            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.rectangleSelection.IsBegun)
            {
                // 短形選択中
                this.rectangleSelection.ChangeSelection(e.X, e.Y, this.scrollBar.Value);

                if (this.scrollBar.Value > 0 && 0 > e.Y)
                {
                    // 上へスクロールします。
                    this.scrollBar.Value = Math.Max(this.scrollBar.Minimum, this.scrollBar.Value + e.Y);
                }
                else if (this.scrollBar.Value < this.scrollBar.Maximum && this.Height < e.Y)
                {
                    // 下へスクロールします。
                    this.scrollBar.Value = Math.Min(this.scrollBar.Maximum, this.scrollBar.Value + (e.Y - this.Height));
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
                    var oldIndex = this.mousePointItemIndex;
                    if (ht.ItemIndex != oldIndex)
                    {
                        this.mousePointItemIndex = ht.ItemIndex;
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
                    var oldIndex = this.mousePointItemIndex;
                    this.mousePointItemIndex = -1;
                    if (oldIndex > -1)
                    {
                        var oldRect = this.GetItemDrawRectangle(oldIndex);
                        this.Invalidate(oldRect, true);
                    }
                }

                if (e.Button == MouseButtons.Left)
                {
                    // ドラッグとしないマウスの移動範囲を取得します。
                    var moveRect = new Rectangle(this.mouseDownHitTestInfo.DrawRectangle.X - SystemInformation.DragSize.Width / 2,
                                                 this.mouseDownHitTestInfo.DrawRectangle.Y - SystemInformation.DragSize.Height / 2,
                                                 SystemInformation.DragSize.Width,
                                                 SystemInformation.DragSize.Height);
                    if (!moveRect.Contains(e.X, e.Y))
                    {
                        if (this.mouseDownHitTestInfo.IsItem && !this.isDrag)
                        {
                            // 項目のドラッグを開始します。
                            this.isDrag = true;
                            this.OnDragStart(EventArgs.Empty);
                        }
                        else if (!this.mouseDownHitTestInfo.IsItem)
                        {
                            // 短形選択を開始します。
                            this.rectangleSelection.BeginSelection(e.X, e.Y, this.scrollBar.Value);
                        }
                    }
                }
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var ht = this.GetHitTestFromDrawPoint(e.X, e.Y);
                if (ht.IsItem)
                {
                    if (this.selectedItemIndexs.Count > 0)
                    {
                        this.OnItemMouseDoubleClick(e);
                    }
                }
            }

            base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                var value = this.scrollBar.Value - (int)(this.itemHeight * 0.8 * (e.Delta / Math.Abs(e.Delta)));
                if (value < this.scrollBar.Minimum)
                {
                    this.scrollBar.Value = this.scrollBar.Minimum;
                }
                else if (value > this.scrollBar.Maximum)
                {
                    this.scrollBar.Value = this.scrollBar.Maximum;
                }
                else
                {
                    this.scrollBar.Value = value;
                }
            }

            base.OnMouseWheel(e);
        }

        private void SetDrawParameter()
        {
            var beforeDrawParameter = this.drawParameter;

            if (this.itemCount > 0)
            {
                var width = (this.scrollBar.Visible) ? this.Width - this.scrollBar.Width : this.Width;
                if (this.isLileList)
                {
                    this.itemWidth = width;
                }

                var colCount = Math.Max(Utility.ToRoundDown(width / (float)this.itemWidth), 1);
                var rowCount = Utility.ToRoundUp(this.itemCount / (float)colCount);
                var virtualHeight = this.itemHeight * rowCount;
                var itemSideSpace = Math.Max((int)((width - this.ItemWidth * colCount) / (float)(colCount + 1)), 0);
                int scrollBarMaximum;
                if (virtualHeight <= this.Height)
                {
                    scrollBarMaximum = this.scrollBar.Minimum;
                }
                else
                {
                    scrollBarMaximum = virtualHeight - this.Height;
                    width = this.Width - this.scrollBar.Width;
                    if (this.isLileList)
                    {
                        this.itemWidth = width;
                    }
                }

                var drawFirstRow = Utility.ToRoundDown(this.scrollBar.Value / (float)this.itemHeight);
                var drawRowCount = Utility.ToRoundUp((this.Height - ((drawFirstRow + 1) * this.itemHeight - this.scrollBar.Value)) / (float)this.itemHeight);
                var drawLastRow = drawFirstRow + drawRowCount + 1;
                var drawFirstItemIndex = drawFirstRow * colCount;
                var drawLastItemIndex = drawLastRow * colCount - 1;
                for (var row = drawLastRow; row >= drawFirstRow; row--)
                {
                    if (drawLastItemIndex < this.itemCount)
                    {
                        break;
                    }

                    for (var colIndex = colCount - 1; colIndex >= 0; colIndex--)
                    {
                        if (drawLastItemIndex < this.itemCount)
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

                this.drawParameter = new DrawParameter(rowCount, colCount,
                                                        drawFirstRow, drawLastRow,
                                                        drawFirstItemIndex, drawLastItemIndex,
                                                        itemSideSpace,
                                                        scrollBarMaximum);

                if (this.drawParameter.ScrollBarMaximum > 0)
                {
                    this.scrollBar.LargeChange = (int)(this.Height / 2f);
                    this.scrollBar.SmallChange = (int)(this.scrollBar.LargeChange / 8f);
                    this.scrollBar.Maximum = this.drawParameter.ScrollBarMaximum;
                    this.scrollBar.Visible = true;
                }
                else
                {
                    this.scrollBar.Visible = false;
                    this.scrollBar.LargeChange = this.itemHeight;
                    this.scrollBar.SmallChange = this.scrollBar.LargeChange / 8;
                    this.scrollBar.Value = this.scrollBar.Minimum;
                    this.scrollBar.Maximum = this.drawParameter.ScrollBarMaximum;
                }
            }
            else
            {
                this.drawParameter = new DrawParameter();
                this.scrollBar.Visible = false;
                this.scrollBar.LargeChange = this.itemHeight;
                this.scrollBar.SmallChange = this.scrollBar.LargeChange / 8;
                this.scrollBar.Value = this.scrollBar.Minimum;
                this.scrollBar.Maximum = this.drawParameter.ScrollBarMaximum;
            }

            if (this.drawParameter.DrawFirstItemIndex == 0 && this.drawParameter.DrawLastItemIndex == 0 ||
                beforeDrawParameter.DrawFirstItemIndex != this.drawParameter.DrawFirstItemIndex ||
                beforeDrawParameter.DrawLastItemIndex != this.drawParameter.DrawLastItemIndex)
            {
                this.OnDrawItemChanged(new DrawItemChangedEventArgs(this.drawParameter.DrawFirstItemIndex, this.drawParameter.DrawLastItemIndex));
            }
        }

        private HitTestInfo GetHitTestFromDrawPoint(int x, int y)
        {
            var col = this.GetColFromX(x);
            if (col > this.drawParameter.ColCount - 1 || col < 0)
            {
                return new HitTestInfo();
            }

            var row = this.GetRowFromDrawY(y);
            if (row > this.drawParameter.RowCount - 1 || row < 0)
            {
                return new HitTestInfo();
            }

            var itemIndex = this.drawParameter.ColCount * row + col;
            if (itemIndex >= this.itemCount)
            {
                return new HitTestInfo();
            }

            var drawRect = this.GetItemDrawRectangle(row, col);
            var virtualRect = this.GetItemVirtualRectangle(itemIndex);
            var isItem = drawRect.Contains(x, y);
            var isSelected = this.selectedItemIndexs.Contains(itemIndex);
            var isMousePoint = this.mousePointItemIndex == itemIndex;
            var isFocus = this.foucusItemIndex == itemIndex;

            return new HitTestInfo(itemIndex, row, col, virtualRect, drawRect, isItem, isSelected, isMousePoint, isFocus);
        }

        private ItemIndexList GetItemIndexsFromVirtualRectangle(Rectangle rect)
        {
            var firstRow = Math.Max(0, this.GetRowFromVirtualY(rect.Y));
            var firstCol = Math.Max(0, this.GetColFromX(rect.X));
            var lastRow = Math.Min(this.GetRowFromVirtualY(rect.Bottom), this.drawParameter.RowCount - 1);
            var lastCol = Math.Min(this.GetColFromX(rect.Right), this.drawParameter.ColCount - 1);

            Cell getFirstCell()
            {
                for (var row = firstRow; row <= lastRow; row++)
                {
                    for (var col = firstCol; col <= lastCol; col++)
                    {
                        var itemIndex = this.drawParameter.ColCount * row + col;
                        if (itemIndex < this.itemCount)
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
                        var itemIndex = this.drawParameter.ColCount * row + col;
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
                    var itemIndex = this.drawParameter.ColCount * row + col;
                    if (itemIndex < this.itemCount)
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
                this.scrollBar.Value = virtualRect.Top - this.itemSpace;
                return true;
            }
            else if (drawRect.Bottom > this.Height)
            {
                // 下へスクロール              
                var virtualRect = this.GetItemVirtualRectangle(itemIndex);
                this.scrollBar.Value = virtualRect.Bottom - this.Height + this.itemSpace;
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
            if (this.rectangleSelection.IsBegun)
            {
                throw new InvalidOperationException("短形選択中は全選択を行いません。");
            }
            else
            {
                this.selectedItemIndexs.Clear();
                for (var i = 0; i < this.itemCount; i++)
                {
                    this.selectedItemIndexs.Add(i);
                }
            }
        }

        private Rectangle GetItemVirtualRectangle(int row, int col)
        {
            var x = col * (this.itemWidth + this.drawParameter.ItemSideSpace) + this.drawParameter.ItemSideSpace + this.itemSpace;
            var y = row * this.itemHeight + this.itemSpace;
            var margin = this.itemSpace * 2;
            return new Rectangle(x, y, this.itemWidth - margin, this.itemHeight - margin);
        }

        private Rectangle GetItemVirtualRectangle(int itemIndex)
        {
            var col = itemIndex % this.drawParameter.ColCount;
            var row = Utility.ToRoundDown(itemIndex / (float)this.drawParameter.ColCount);
            return this.GetItemVirtualRectangle(row, col);
        }

        private Rectangle GetItemDrawRectangle(int row, int col)
        {
            var x = col * (this.itemWidth + this.drawParameter.ItemSideSpace) + this.drawParameter.ItemSideSpace + this.itemSpace;
            var y = row * this.itemHeight + this.itemSpace - this.scrollBar.Value;
            var margin = this.itemSpace * 2;
            return new Rectangle(x, y, this.itemWidth - margin, this.itemHeight - margin);
        }

        private Rectangle GetItemDrawRectangle(int itemIndex)
        {
            var col = this.drawParameter.ColCount switch
            {
                0 => 0,
                _ => itemIndex % this.drawParameter.ColCount,
            };
            var row = Utility.ToRoundDown(itemIndex / (float)this.drawParameter.ColCount);
            return this.GetItemDrawRectangle(row, col);
        }

        private void DrawItems(Graphics g)
        {
            for (var itemIndex = this.drawParameter.DrawFirstItemIndex;
                 itemIndex <= this.drawParameter.DrawLastItemIndex;
                 itemIndex++)
            {
                var drawRect = this.GetItemDrawRectangle(itemIndex);

                bool isSelected;
                if (this.rectangleSelection.IsBegun)
                {
                    isSelected = this.selectedItemIndexs.Contains(itemIndex) ||
                        this.rectangleSelection.VirtualRectangle.IntersectsWith(this.GetItemVirtualRectangle(itemIndex));
                }
                else
                {
                    isSelected = this.selectedItemIndexs.Contains(itemIndex);
                }

                var isMousePoint = this.mousePointItemIndex == itemIndex;
                var isFocus = this.foucusItemIndex == itemIndex;

                this.OnDrawItem(new DrawItemEventArgs(g, itemIndex, drawRect, isSelected, isMousePoint, isFocus));
            }
        }

        private void DrawRectangleSelection(Graphics g)
        {
            g.FillRectangle(this.RectangleSelectionBrush, this.rectangleSelection.GetDrawRectangle(this.scrollBar.Value));
            g.DrawRectangle(this.RectangleSelectionPen, this.rectangleSelection.GetDrawRectangle(this.scrollBar.Value));
        }

        private int GetRowFromVirtualY(int y)
        {
            return Utility.ToRoundDown(y / (float)this.itemHeight);
        }

        private int GetRowFromDrawY(int y)
        {
            return Utility.ToRoundDown((y + this.scrollBar.Value) / (float)this.itemHeight);
        }

        private int GetColFromX(int x)
        {
            return Utility.ToRoundDown((x - this.drawParameter.ItemSideSpace - this.itemSpace) / (float)(this.itemWidth + this.drawParameter.ItemSideSpace));
        }

        private bool LeftKeyDown()
        {
            if (this.itemCount == 0)
            {
                throw new InvalidOperationException("項目が存在しません。");
            }

            var newIndex = this.foucusItemIndex - 1;
            if (newIndex > -1)
            {
                if (this.isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    // Shiftキー押下
                    this.selectedItemIndexs.AddRange(newIndex);
                }
                else if (this.isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    // Ctrlキー押下
                }
                else
                {
                    // 修飾キー無し
                    this.selectedItemIndexs.Clear();
                    this.selectedItemIndexs.Add(newIndex);
                }

                var itemRect = this.GetItemDrawRectangle(newIndex);
                if (itemRect.Top < 0)
                {
                    this.scrollBar.Value -= this.itemHeight;
                }

                this.EnsureVisible(newIndex);

                this.foucusItemIndex = newIndex;

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool RightKeyDown()
        {
            if (this.itemCount == 0)
            {
                throw new InvalidOperationException("項目が存在しません。");
            }

            var newIndex = this.foucusItemIndex + 1;
            if (newIndex < this.itemCount)
            {
                if (this.isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    // Shiftキー押下
                    this.selectedItemIndexs.AddRange(newIndex);
                }
                else if (this.isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    // Ctrlキー押下
                }
                else
                {
                    // 修飾キー無し
                    this.selectedItemIndexs.Clear();
                    this.selectedItemIndexs.Add(newIndex);
                }

                var itemRect = this.GetItemDrawRectangle(newIndex);
                if (itemRect.Bottom > this.Height)
                {
                    this.scrollBar.Value += this.itemHeight;
                }

                this.EnsureVisible(newIndex);

                this.foucusItemIndex = newIndex;

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool UpKeyDown()
        {
            if (this.itemCount == 0)
            {
                throw new InvalidOperationException("項目が存在しません。");
            }

            var newIndex = this.foucusItemIndex - this.drawParameter.ColCount;
            if (newIndex > -1)
            {
                if (this.isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    // Shiftキー押下
                    this.selectedItemIndexs.AddRange(newIndex);
                }
                else if (this.isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    // Ctrlキー押下
                }
                else
                {
                    // 修飾キー無し
                    this.selectedItemIndexs.Clear();
                    this.selectedItemIndexs.Add(newIndex);
                }

                var itemRect = this.GetItemDrawRectangle(newIndex);
                if (itemRect.Top < 0)
                {
                    this.scrollBar.Value = Math.Max(this.scrollBar.Minimum, this.scrollBar.Value - this.itemHeight);
                }

                this.EnsureVisible(newIndex);

                this.foucusItemIndex = newIndex;

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool DownKeyDown()
        {
            if (this.itemCount == 0)
            {
                throw new InvalidOperationException("項目が存在しません。");
            }

            var newIndex = this.foucusItemIndex + this.drawParameter.ColCount;

            if (newIndex < this.itemCount)
            {
                if (this.isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    // Shiftキー押下
                    this.selectedItemIndexs.AddRange(newIndex);
                }
                else if (this.isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    // Ctrlキー押下
                }
                else
                {
                    // 修飾キー無し
                    this.selectedItemIndexs.Clear();
                    this.selectedItemIndexs.Add(newIndex);
                }

                var itemRect = this.GetItemDrawRectangle(newIndex);
                if (itemRect.Bottom > this.Height)
                {
                    this.scrollBar.Value = Math.Min(this.scrollBar.Maximum, this.scrollBar.Value + this.itemHeight);
                }

                this.EnsureVisible(newIndex);

                this.foucusItemIndex = newIndex;

                return true;
            }
            else
            {
                return false;
            }
        }

        private void SpaceKeyDown()
        {
            if (!this.selectedItemIndexs.Contains(this.foucusItemIndex))
            {
                this.selectedItemIndexs.Add(this.foucusItemIndex);
            }
            else
            {
                this.selectedItemIndexs.Remove(this.foucusItemIndex);
            }
        }

        private void PageUpKeyDown()
        {
            if (this.itemCount == 0)
            {
                throw new InvalidOperationException("項目が存在しません。");
            }

            var newIndex = this.foucusItemIndex - this.drawParameter.ColCount * (this.drawParameter.DrawLastRow - this.drawParameter.DrawFirstRow - 1);
            if (newIndex < 0)
            {
                newIndex = 0;
            }

            if (this.isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                // Shiftキー押下
                this.selectedItemIndexs.AddRange(newIndex);
            }
            else if (this.isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                // Ctrlキー押下
            }
            else
            {
                // 修飾キー無し
                this.selectedItemIndexs.Clear();
                this.selectedItemIndexs.Add(newIndex);
            }

            this.EnsureVisible(newIndex);

            this.foucusItemIndex = newIndex;
        }

        private void PageDownKeyDown()
        {
            if (this.itemCount == 0)
            {
                throw new InvalidOperationException("項目が存在しません。");
            }

            var newIndex = this.foucusItemIndex + this.drawParameter.ColCount * (this.drawParameter.DrawLastRow - this.drawParameter.DrawFirstRow - 1);
            if (newIndex >= this.itemCount)
            {
                newIndex = this.itemCount - 1;
            }

            if (this.isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                // Shiftキー押下
                this.selectedItemIndexs.AddRange(newIndex);
            }
            else if (this.isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                // Ctrlキー押下
            }
            else
            {
                // 修飾キー無し
                this.selectedItemIndexs.Clear();
                this.selectedItemIndexs.Add(newIndex);
            }

            this.EnsureVisible(newIndex);

            this.foucusItemIndex = newIndex;
        }

        private void HomeKeyDown()
        {
            if (this.itemCount == 0)
            {
                throw new InvalidOperationException("項目が存在しません。");
            }

            var newIndex = 0;
            if (this.isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                // Shiftキー押下
                this.selectedItemIndexs.AddRange(newIndex);
            }
            else if (this.isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                // Ctrlキー押下
            }
            else
            {
                // 修飾キー無し
                this.selectedItemIndexs.Clear();
                this.selectedItemIndexs.Add(newIndex);
            }

            this.EnsureVisible(newIndex);

            this.foucusItemIndex = newIndex;
        }

        private void EndKeyDown()
        {
            if (this.itemCount == 0)
            {
                throw new InvalidOperationException("項目が存在しません。");
            }

            var newIndex = this.itemCount - 1;
            if (this.isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                // Shiftキー押下
                this.selectedItemIndexs.AddRange(newIndex);
            }
            else if (this.isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                // Ctrlキー押下
            }
            else
            {
                // 修飾キー無し
                this.selectedItemIndexs.Clear();
                this.selectedItemIndexs.Add(newIndex);
            }

            this.EnsureVisible(newIndex);

            this.foucusItemIndex = newIndex;
        }

        private void LeftMouseDown(int x, int y)
        {
            var ht = this.GetHitTestFromDrawPoint(x, y);

            if (this.isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                // Shiftキー押下
                if (ht.IsItem)
                {
                    this.selectedItemIndexs.AddRange(ht.ItemIndex);
                    this.foucusItemIndex = ht.ItemIndex;
                }
            }
            else if (this.isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                // Ctrlキー押下
                if (ht.IsItem)
                {
                    if (!this.selectedItemIndexs.Contains(ht.ItemIndex))
                    {
                        this.selectedItemIndexs.Add(ht.ItemIndex);
                    }
                    else
                    {
                        this.selectedItemIndexs.Remove(ht.ItemIndex);
                    }

                    this.foucusItemIndex = ht.ItemIndex;
                }
            }
            else
            {
                // 修飾キー無し
                if (ht.IsItem)
                {
                    if (this.selectedItemIndexs.Contains(ht.ItemIndex))
                    {
                        this.foucusItemIndex = ht.ItemIndex;
                    }
                    else
                    {
                        this.selectedItemIndexs.Clear();
                        this.selectedItemIndexs.Add(ht.ItemIndex);
                        this.foucusItemIndex = ht.ItemIndex;
                    }
                }
                else
                {
                    this.selectedItemIndexs.Clear();
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
                    this.foucusItemIndex = ht.ItemIndex;
                }
                else
                {
                    this.selectedItemIndexs.Clear();
                    this.selectedItemIndexs.Add(ht.ItemIndex);
                    this.foucusItemIndex = ht.ItemIndex;
                }
            }
            else
            {
                this.selectedItemIndexs.Clear();
            }
        }

        private void MiddleMouseDown(int x, int y)
        {
            var ht = this.GetHitTestFromDrawPoint(x, y);

            if (ht.IsItem)
            {
                if (ht.IsSelected)
                {
                    this.foucusItemIndex = ht.ItemIndex;
                }
                else
                {
                    this.selectedItemIndexs.Clear();
                    this.selectedItemIndexs.Add(ht.ItemIndex);
                    this.foucusItemIndex = ht.ItemIndex;
                }
            }
            else
            {
                this.selectedItemIndexs.Clear();
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
