using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SWF.UIComponent.FlowList
{
    /// <summary>
    /// フローリストコントロール
    /// </summary>
    public partial class FlowList : PictureBox
    {
        #region 定数・列挙

        // 項目最小サイズ
        public const int MinimumItemSize = 16;

        // 項目最大サイズ
        public const int MaximumItemSize = 512;

        #endregion

        #region インスタンス変数

        // 描画フラグ
        private bool _isDraw = true;

        // 複数選択可能フラグ
        private bool _isMultiSelect = false;

        // 行リストフラグ
        private bool _isLileList = false;

        // 項目数
        private int _itemCount = 0;

        // 項目幅
        private int _itemWidth = MinimumItemSize;

        // 項目高さ
        private int _itemHeight = MinimumItemSize;

        // 項目間余白
        private int _itemSpace = 0;

        // 描画パラメータ
        private DrawParameter _drawParameter = new DrawParameter();

        // 垂直スクロールバー
        private VScrollBarEx _scrollBar = new VScrollBarEx();

        // フォーカスされている項目のインデックス
        private int _foucusItemIndex = -1;

        // マウスポイントされている項目のインデックス
        private int _mousePointItemIndex = -1;

        // 選択されている項目インデックスのリスト
        private ItemIndexList _selectedItemIndexs = new ItemIndexList();

        // 短形選択クラス
        private readonly RectangleSelection _rectangleSelection = new RectangleSelection();

        // マウスダウンした座標情報
        private HitTestInfo _mouseDownHitTestInfo = new HitTestInfo();

        // ドラッグフラグ
        private bool _isDrag = false;

        #region 描画オブジェクト

        private Color _itemTextColor = Color.FromArgb(
            SystemColors.ControlText.A, 
            SystemColors.ControlText.R, 
            SystemColors.ControlText.G, 
            SystemColors.ControlText.B);

        private Color _selectedItemColor = Color.FromArgb(
            SystemColors.Highlight.A / 8, 
            SystemColors.Highlight.R, 
            SystemColors.Highlight.G, 
            SystemColors.Highlight.B);
        
        private Color _focusItemColor = Color.FromArgb(
            SystemColors.Highlight.A / 8,
            SystemColors.Highlight.R, 
            SystemColors.Highlight.G, 
            SystemColors.Highlight.B);
        
        private Color _mousePointItemColor = Color.FromArgb(
            SystemColors.Highlight.A / 8,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B);
        
        private Color _rectangleSelectionColor = Color.FromArgb(
            SystemColors.Highlight.A / 4,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B);

        private StringTrimming _itemTextTrimming = StringTrimming.EllipsisCharacter;
        private StringAlignment _itemTextAlignment = StringAlignment.Center;
        private StringAlignment _itemTextLineAlignment = StringAlignment.Center;
        private StringFormatFlags _itemTextFormatFlags = 0;

        private SolidBrush _itemTextBrush = null;
        private SolidBrush _selectedItemBrush = null;
        private Pen _selectedItemPen = null;
        private SolidBrush _foucusItemBrush = null;
        private SolidBrush _mousePointItemBrush = null;
        private SolidBrush _rectangleSelectionBrush = null;
        private Pen _rectangleSelectionPen = null;
        private StringFormat _itemTextFormat = null;

        #endregion

        #endregion

        #region プライベートプロパティ

        private SolidBrush rectangleSelectionBrush
        {
            get
            {
                if (_rectangleSelectionBrush == null)
                {
                    _rectangleSelectionBrush = new SolidBrush(_rectangleSelectionColor);
                }

                return _rectangleSelectionBrush;
            }
        }

        private Pen rectangleSelectionPen
        {
            get
            {
                if (_rectangleSelectionPen == null)
                {
                    _rectangleSelectionPen = new Pen(Color.FromArgb(
                        _rectangleSelectionColor.A * 2, 
                        _rectangleSelectionColor.R, 
                        _rectangleSelectionColor.G, 
                        _rectangleSelectionColor.B));
                }

                return _rectangleSelectionPen;
            }
        }

        #endregion

        #region コンストラクタ

        public FlowList()
        {
            initializeComponent();
        }

        #endregion

        #region 継承メソッド

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            setDrawParameter();

            base.OnInvalidated(e);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (!_isDraw)
            {
                return;
            }

            base.OnPaintBackground(pevent);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!_isDraw)
            {
                return;
            }

            e.Graphics.InterpolationMode = InterpolationMode.Low;
            e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
            e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;

            if (_rectangleSelection.IsBegun)
            {
                drawRectangleSelection(e.Graphics);
            }

            if (_itemCount > 0)
            {
                drawItems(e.Graphics);
            }

            base.OnPaint(e);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if ((keyData & Keys.Alt) != Keys.Alt)
            {
                Keys kcode = keyData & Keys.KeyCode;

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

            if (_itemCount > 0 && !_rectangleSelection.IsBegun)
            {
                _selectedItemIndexs.BeginUpdate();

                try
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Left:
                            if (leftKeyDown())
                            {
                                this.Invalidate();
                            }
                            break;
                        case Keys.Right:
                            if (rightKeyDown())
                            {
                                this.Invalidate();
                            }
                            break;
                        case Keys.Up:
                            if (upKeyDown())
                            {
                                this.Invalidate();
                            }
                            break;
                        case Keys.Down:
                            if (downKeyDown())
                            {
                                this.Invalidate();
                            }
                            break;
                        case Keys.Space:
                            spaceKeyDown();
                            this.Invalidate();
                            break;
                        case Keys.ProcessKey:
                            spaceKeyDown();
                            this.Invalidate();
                            break;
                        case Keys.PageUp:
                            pageUpKeyDown();
                            this.Invalidate();
                            break;
                        case Keys.PageDown:
                            pageDownKeyDown();
                            this.Invalidate();
                            break;
                        case Keys.Home:
                            homeKeyDown();
                            this.Invalidate();
                            break;
                        case Keys.End:
                            endKeyDown();
                            this.Invalidate();
                            break;
                        case Keys.A:
                            if (_isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
                            {
                                selectAll();
                                this.Invalidate();
                            }
                            break;
                        case Keys.Enter:
                            if (_selectedItemIndexs.Count > 0)
                            {
                                OnItemExecute(new EventArgs());
                            }
                            break;
                        case Keys.Delete:
                            if (_selectedItemIndexs.Count > 0)
                            {
                                if (_selectedItemIndexs.Contains(_foucusItemIndex))
                                {
                                    OnItemDelete(new EventArgs());
                                }
                                else
                                {
                                    OnItemDelete(new EventArgs());
                                }
                            }
                            break;
                        case Keys.C:
                            if (_selectedItemIndexs.Count > 0)
                            {
                                if (_selectedItemIndexs.Contains(_foucusItemIndex))
                                {
                                    OnItemCopy(new EventArgs());
                                }
                                else
                                {
                                    OnItemCopy(new EventArgs());
                                }
                            }
                            break;
                        case Keys.X:
                            if (_selectedItemIndexs.Count > 0)
                            {
                                if (_selectedItemIndexs.Contains(_foucusItemIndex))
                                {
                                    OnItemCut(new EventArgs());
                                }
                                else
                                {
                                    OnItemCut(new EventArgs());
                                }
                            }
                            break;
                    }
                }
                finally
                {
                    _selectedItemIndexs.EndUpdate();
                }
            }

            base.OnKeyDown(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _mousePointItemIndex = -1;
            _isDrag = false;
            _mouseDownHitTestInfo = new HitTestInfo();

            this.Invalidate();

            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Select();

            if (e.Button == MouseButtons.Left)
            {
                // 左ボタン
                _selectedItemIndexs.BeginUpdate();
                try
                {
                    leftMouseDown(e.X, e.Y);
                }
                finally
                {
                    this.Invalidate();
                    _selectedItemIndexs.EndUpdate();
                }

                // マウスダウンした座標情報を保持します。
                _mouseDownHitTestInfo = getHitTestFromDrawPoint(e.X, e.Y);
            }
            else if (e.Button == MouseButtons.Right)
            {
                // 右ボタン
                _selectedItemIndexs.BeginUpdate();
                try
                {
                    rightMouseDown(e.X, e.Y);
                }
                finally
                {
                    this.Invalidate();
                    _selectedItemIndexs.EndUpdate();
                }
            }
            else if (e.Button == MouseButtons.Middle)
            {
                // ミドルボタン
                _selectedItemIndexs.BeginUpdate();
                try
                {
                    middleMouseDown(e.X, e.Y);
                }
                finally
                {
                    this.Invalidate();
                    _selectedItemIndexs.EndUpdate();
                }
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_rectangleSelection.IsBegun)
            {
                ItemIndexList itemIndexs = getItemIndexsFromVirtualRectangle(_rectangleSelection.VirtualRectangle);
                _rectangleSelection.EndSelection();
                _selectedItemIndexs.Union(itemIndexs);
                this.Invalidate();
            }
            else
            {
                HitTestInfo ht = getHitTestFromDrawPoint(e.X, e.Y);
                if (ht.IsItem)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        if (!_isDrag &&
                            (Control.ModifierKeys & Keys.Shift) != Keys.Shift &&
                            (Control.ModifierKeys & Keys.Control) != Keys.Control)
                        {
                            _selectedItemIndexs.BeginUpdate();
                            try
                            {
                                _selectedItemIndexs.Clear();
                                _selectedItemIndexs.Add(ht.ItemIndex);
                            }
                            finally
                            {
                                this.Invalidate();
                                _selectedItemIndexs.EndUpdate();
                            }
                        }
                    }

                    if (_selectedItemIndexs.Count > 0)
                    {
                        OnItemMouseClick(e);
                    }
                }
                else
                {
                    OnBackgroundMouseClick(e);
                }
            }

            _isDrag = false;
            _mouseDownHitTestInfo = new HitTestInfo();

            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_rectangleSelection.IsBegun)
            {
                // 短形選択中
                _rectangleSelection.ChangeSelection(e.X, e.Y, _scrollBar.Value);

                if (_scrollBar.Value > 0 && 0 > e.Y)
                {
                    // 上へスクロールします。
                    _scrollBar.Value = Math.Max(_scrollBar.Minimum, _scrollBar.Value + e.Y);
                }
                else if (_scrollBar.Value < _scrollBar.Maximum && this.Height < e.Y)
                {
                    // 下へスクロールします。
                    _scrollBar.Value = Math.Min(_scrollBar.Maximum, _scrollBar.Value + (e.Y - this.Height));
                }
                else
                {
                    this.Invalidate();
                }
            }
            else
            {
                HitTestInfo ht = getHitTestFromDrawPoint(e.X, e.Y);
                if (ht.IsItem)
                {
                    int oldIndex = _mousePointItemIndex;
                    if (ht.ItemIndex != oldIndex)
                    {
                        _mousePointItemIndex = ht.ItemIndex;
                        Rectangle newRect = getItemDrawRectangle(ht.ItemIndex);
                        this.Invalidate(newRect);
                        if (oldIndex > -1)
                        {
                            Rectangle oldRect = getItemDrawRectangle(oldIndex);
                            this.Invalidate(oldRect, true);
                        }
                    }
                }
                else
                {
                    int oldIndex = _mousePointItemIndex;
                    _mousePointItemIndex = -1;
                    if (oldIndex > -1)
                    {
                        Rectangle oldRect = getItemDrawRectangle(oldIndex);
                        this.Invalidate(oldRect, true);
                    }
                }

                if (e.Button == MouseButtons.Left)
                {
                    // ドラッグとしないマウスの移動範囲を取得します。
                    Rectangle moveRect = new Rectangle(_mouseDownHitTestInfo.DrawRectangle.X - SystemInformation.DragSize.Width / 2,
                                                       _mouseDownHitTestInfo.DrawRectangle.Y - SystemInformation.DragSize.Height / 2,
                                                       SystemInformation.DragSize.Width,
                                                       SystemInformation.DragSize.Height);
                    if (!moveRect.Contains(e.X, e.Y))
                    {
                        if (_mouseDownHitTestInfo.IsItem && !_isDrag)
                        {
                            // 項目のドラッグを開始します。
                            _isDrag = true;
                            OnDragStart(new EventArgs());
                        }
                        else if (!_mouseDownHitTestInfo.IsItem)
                        {
                            // 短形選択を開始します。
                            _rectangleSelection.BeginSelection(e.X, e.Y, _scrollBar.Value);
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
                HitTestInfo ht = getHitTestFromDrawPoint(e.X, e.Y);
                if (ht.IsItem)
                {
                    if (_selectedItemIndexs.Count > 0)
                    {
                        OnItemMouseDoubleClick(e);
                    }
                }
            }

            base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            mouseWheelProcess(e);
            base.OnMouseWheel(e);
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            this.SetStyle(ControlStyles.DoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.ResizeRedraw, true);

            _scrollBar.Dock = DockStyle.Right;
            _scrollBar.ValueChanged += new EventHandler(_scrollBar_ValueChanged);

            _selectedItemIndexs.Change += new EventHandler(_selectedItemIndexs_Change);

            this.Controls.Add(_scrollBar);
        }

        private void setDrawParameter()
        {
            DrawParameter beforeDrawParameter = _drawParameter;

            if (_itemCount > 0)
            {
                int width = this.Width;

                if (_isLileList)
                {
                    _itemWidth = width;
                }

                int colCount = Utility.ToRoundDown(width / (double)_itemWidth);
                int rowCount = Utility.ToRoundUp(_itemCount / (double)colCount);
                int virtualHeight = _itemHeight * rowCount;
                int scrollBarMaximum;
                int itemSideSpace;
                if (virtualHeight <= this.Height)
                {
                    itemSideSpace = (int)((width - ItemWidth * colCount) / (double)(colCount + 1));
                    scrollBarMaximum = _scrollBar.Minimum;
                }
                else
                {
                    width = this.Width - _scrollBar.Width;

                    if (_isLileList)
                    {
                        _itemWidth = width;
                    }

                    colCount = Utility.ToRoundDown(width / (double)_itemWidth);
                    rowCount = Utility.ToRoundUp(_itemCount / (double)colCount);
                    virtualHeight = _itemHeight * rowCount;
                    itemSideSpace = (int)((width - ItemWidth * colCount) / (double)(colCount + 1));
                    scrollBarMaximum = virtualHeight - this.Height;
                }

                int drawFirstRow = Utility.ToRoundDown(_scrollBar.Value / (double)_itemHeight);
                int drawRowCount = Utility.ToRoundUp((this.Height - ((drawFirstRow + 1) * _itemHeight - _scrollBar.Value)) / (double)_itemHeight);
                int drawLastRow = drawFirstRow + drawRowCount + 1;
                int drawFirstItemIndex = drawFirstRow * colCount;
                int drawLastItemIndex = drawLastRow * colCount - 1;
                for (int row = drawLastRow; row >= drawFirstRow; row--)
                {
                    if (drawLastItemIndex < _itemCount)
                    {
                        break;
                    }

                    for (int colIndex = colCount - 1; colIndex >= 0; colIndex--)
                    {
                        if (drawLastItemIndex < _itemCount)
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

                _drawParameter = new DrawParameter(rowCount, colCount,
                                                   drawFirstRow, drawLastRow,
                                                   drawFirstItemIndex, drawLastItemIndex,
                                                   itemSideSpace,
                                                   scrollBarMaximum);

                if (_drawParameter.ScrollBarMaximum > 0)
                {
                    _scrollBar.LargeChange = (int)(this.Height / 2);
                    _scrollBar.Maximum = _drawParameter.ScrollBarMaximum;
                    _scrollBar.Visible = true;
                }
                else
                {
                    _scrollBar.Visible = false;
                    _scrollBar.LargeChange = _itemHeight;
                    _scrollBar.Value = _scrollBar.Minimum;
                    _scrollBar.Maximum = _drawParameter.ScrollBarMaximum;
                }
            }
            else
            {
                _drawParameter = new DrawParameter();
                _scrollBar.Visible = false;
                _scrollBar.LargeChange = _itemHeight;
                _scrollBar.Value = _scrollBar.Minimum;
                _scrollBar.Maximum = _drawParameter.ScrollBarMaximum;
            }

            if (beforeDrawParameter.DrawFirstItemIndex != _drawParameter.DrawFirstItemIndex ||
                beforeDrawParameter.DrawLastItemIndex != _drawParameter.DrawLastItemIndex)
            {
                OnDrawItemChanged(new DrawItemChangedEventArgs(_drawParameter.DrawFirstItemIndex, _drawParameter.DrawLastItemIndex));
            }
        }

        private HitTestInfo getHitTestFromDrawPoint(int x, int y)
        {
            int col = getColFromX(x);
            if (col > _drawParameter.ColCount - 1 || col < 0)
            {
                return new HitTestInfo();
            }

            int row = getRowFromDrawY(y);
            if (row > _drawParameter.RowCount - 1 || row < 0)
            {
                return new HitTestInfo();
            }

            int itemIndex = _drawParameter.ColCount * row + col;
            if (itemIndex >= _itemCount)
            {
                return new HitTestInfo();
            }

            Rectangle drawRect = getItemDrawRectangle(row, col);
            Rectangle virtualRect = getItemVirtualRectangle(itemIndex);
            bool isItem = drawRect.Contains(x, y);
            bool isSelected = _selectedItemIndexs.Contains(itemIndex);
            bool isMousePoint = _mousePointItemIndex == itemIndex;
            bool isFocus = _foucusItemIndex == itemIndex;

            return new HitTestInfo(itemIndex, row, col, virtualRect, drawRect, isItem, isSelected, isMousePoint, isFocus);
        }

        private ItemIndexList getItemIndexsFromVirtualRectangle(Rectangle rect)
        {
            int firstRow = Math.Max(0, getRowFromVirtualY(rect.Y));
            int firstCol = Math.Max(0, getColFromX(rect.X));
            int lastRow = Math.Min(getRowFromVirtualY(rect.Bottom), _drawParameter.RowCount - 1);
            int lastCol = Math.Min(getColFromX(rect.Right), _drawParameter.ColCount - 1);

            Func<Cell> getFirstCell = () =>
            {
                for (int row = firstRow; row <= lastRow; row++)
                {
                    for (int col = firstCol; col <= lastCol; col++)
                    {
                        int itemIndex = _drawParameter.ColCount * row + col;
                        if (itemIndex < _itemCount)
                        {
                            Rectangle itemRect = getItemVirtualRectangle(itemIndex);
                            if (rect.IntersectsWith(itemRect))
                            {
                                return new Cell(row, col);
                            }
                        }
                    }
                }

                return null;
            };

            Func<Cell> getLastCell = () =>
            {
                for (int row = lastRow; row >= firstRow; row--)
                {
                    for (int col = lastCol; col >= firstCol; col--)
                    {
                        int itemIndex = _drawParameter.ColCount * row + col;
                        Rectangle itemRect = getItemVirtualRectangle(itemIndex);
                        if (rect.IntersectsWith(itemRect))
                        {
                            // 項目数を上回る場合があります。
                            return new Cell(row, col);
                        }
                    }
                }

                throw new Exception("最後のセルが取得できません。");
            };

            Cell firstCell = getFirstCell();
            if (firstCell == null)
            {
                return new ItemIndexList();
            }

            Cell lastCell = getLastCell();

            ItemIndexList list = new ItemIndexList();

            for (int row = firstCell.Row; row <= lastCell.Row; row++)
            {
                for (int col = firstCell.Col; col <= lastCell.Col; col++)
                {
                    int itemIndex = _drawParameter.ColCount * row + col;
                    if (itemIndex < _itemCount)
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

        private bool ensureVisible(int itemIndex)
        {
            Rectangle drawRect = getItemDrawRectangle(itemIndex);
            if (drawRect.Top < 0)
            {
                // 上へスクロール
                Rectangle virtualRect = getItemVirtualRectangle(itemIndex);
                _scrollBar.Value = virtualRect.Top - _itemSpace;
                return true;
            }
            else if (drawRect.Bottom > this.Height)
            {
                // 下へスクロール              
                Rectangle virtualRect = getItemVirtualRectangle(itemIndex);
                _scrollBar.Value = virtualRect.Bottom - this.Height + _itemSpace;
                return true;
            }
            else
            {
                // 既に表示されています。
                return false;
            }
        }

        private void selectAll()
        {
            if (_rectangleSelection.IsBegun)
            {
                throw new Exception("短形選択中は全選択を行いません。");
            }
            else
            {
                _selectedItemIndexs.Clear();
                for (int i = 0; i < _itemCount; i++)
                {
                    _selectedItemIndexs.Add(i);
                }
            }
        }

        private void invalidateFromItemIndex(int itemIndex)
        {
            Rectangle rect = getItemDrawRectangle(itemIndex);
            this.Invalidate(rect);
        }

        private void mouseWheelProcess(MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                int value = _scrollBar.Value - (int)(_itemHeight * 0.8 * (e.Delta / Math.Abs(e.Delta)));
                if (value < _scrollBar.Minimum)
                {
                    _scrollBar.Value = _scrollBar.Minimum;
                }
                else if (value > _scrollBar.Maximum)
                {
                    _scrollBar.Value = _scrollBar.Maximum;
                }
                else
                {
                    _scrollBar.Value = value;
                }
            }
        }

        #region 項目の描画領域を取得します。

        private Rectangle getItemVirtualRectangle(int row, int col)
        {
            int x = col * (_itemWidth + _drawParameter.ItemSideSpace) + _drawParameter.ItemSideSpace + _itemSpace;
            int y = row * _itemHeight + _itemSpace;
            int margin = _itemSpace * 2;
            return new Rectangle(x, y, _itemWidth - margin, _itemHeight - margin);
        }

        private Rectangle getItemVirtualRectangle(int itemIndex)
        {
            int col = itemIndex % _drawParameter.ColCount;
            int row = Utility.ToRoundDown(itemIndex / (double)_drawParameter.ColCount);
            return getItemVirtualRectangle(row, col);
        }

        private Rectangle getItemDrawRectangle(int row, int col)
        {
            int x = col * (_itemWidth + _drawParameter.ItemSideSpace) + _drawParameter.ItemSideSpace + _itemSpace;
            int y = row * _itemHeight + _itemSpace - _scrollBar.Value;
            int margin = _itemSpace * 2;
            return new Rectangle(x, y, _itemWidth - margin, _itemHeight - margin);
        }

        private Rectangle getItemDrawRectangle(int itemIndex)
        {
            int col = itemIndex % _drawParameter.ColCount;
            int row = Utility.ToRoundDown(itemIndex / (double)_drawParameter.ColCount);
            return getItemDrawRectangle(row, col);
        }

        #endregion

        #region 描画メソッド

        private void drawItems(Graphics g)
        {
            for (int itemIndex = _drawParameter.DrawFirstItemIndex;
                 itemIndex <= _drawParameter.DrawLastItemIndex;
                 itemIndex++)
            {
                Rectangle drawRect = getItemDrawRectangle(itemIndex);

                bool isSelected;
                if (_rectangleSelection.IsBegun)
                {
                    isSelected = _selectedItemIndexs.Contains(itemIndex) || _rectangleSelection.VirtualRectangle.IntersectsWith(getItemVirtualRectangle(itemIndex));
                }
                else
                {
                    isSelected = _selectedItemIndexs.Contains(itemIndex);
                }

                bool isMousePoint = _mousePointItemIndex == itemIndex;
                bool isFocus = _foucusItemIndex == itemIndex;

                OnDrawItem(new DrawItemEventArgs(g, itemIndex, drawRect, isSelected, isMousePoint, isFocus));
            }
        }

        private void drawRectangleSelection(Graphics g)
        {
            g.FillRectangle(rectangleSelectionBrush, _rectangleSelection.GetDrawRectangle(_scrollBar.Value));
            g.DrawRectangle(rectangleSelectionPen, _rectangleSelection.GetDrawRectangle(_scrollBar.Value));
        }

        #endregion

        #region 座標情報取得メソッド

        private int getRowFromVirtualY(int y)
        {
            return Utility.ToRoundDown(y / (double)_itemHeight);
        }

        private int getRowFromDrawY(int y)
        {
            return Utility.ToRoundDown((y + _scrollBar.Value) / (double)_itemHeight);
        }

        private int getColFromX(int x)
        {
            return Utility.ToRoundDown((x - _drawParameter.ItemSideSpace - _itemSpace) / (double)(_itemWidth + _drawParameter.ItemSideSpace));
        }

        #endregion

        #region キーダウン操作

        private bool leftKeyDown()
        {
            if (_itemCount == 0)
            {
                throw new Exception("項目が存在しません。");
            }

            int newIndex = _foucusItemIndex - 1;
            if (newIndex > -1)
            {
                if (_isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    // Shiftキー押下
                    _selectedItemIndexs.AddRange(newIndex);
                }
                else if (_isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    // Ctrlキー押下
                }
                else
                {
                    // 修飾キー無し
                    _selectedItemIndexs.Clear();
                    _selectedItemIndexs.Add(newIndex);
                }

                Rectangle itemRect = getItemDrawRectangle(newIndex);
                if (itemRect.Top < 0)
                {
                    _scrollBar.Value -= _itemHeight;
                }

                ensureVisible(newIndex);

                _foucusItemIndex = newIndex;

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool rightKeyDown()
        {
            if (_itemCount == 0)
            {
                throw new Exception("項目が存在しません。");
            }

            int newIndex = _foucusItemIndex + 1;
            if (newIndex < _itemCount)
            {
                if (_isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    // Shiftキー押下
                    _selectedItemIndexs.AddRange(newIndex);
                }
                else if (_isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    // Ctrlキー押下
                }
                else
                {
                    // 修飾キー無し
                    _selectedItemIndexs.Clear();
                    _selectedItemIndexs.Add(newIndex);
                }

                Rectangle itemRect = getItemDrawRectangle(newIndex);
                if (itemRect.Bottom > this.Height)
                {
                    _scrollBar.Value += _itemHeight;
                }

                ensureVisible(newIndex);

                _foucusItemIndex = newIndex;

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool upKeyDown()
        {
            if (_itemCount == 0)
            {
                throw new Exception("項目が存在しません。");
            }

            int newIndex = _foucusItemIndex - _drawParameter.ColCount;
            if (newIndex > -1)
            {
                if (_isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    // Shiftキー押下
                    _selectedItemIndexs.AddRange(newIndex);
                }
                else if (_isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    // Ctrlキー押下
                }
                else
                {
                    // 修飾キー無し
                    _selectedItemIndexs.Clear();
                    _selectedItemIndexs.Add(newIndex);
                }

                Rectangle itemRect = getItemDrawRectangle(newIndex);
                if (itemRect.Top < 0)
                {
                    _scrollBar.Value = Math.Max(_scrollBar.Minimum, _scrollBar.Value - _itemHeight);
                }

                ensureVisible(newIndex);

                _foucusItemIndex = newIndex;

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool downKeyDown()
        {
            if (_itemCount == 0)
            {
                throw new Exception("項目が存在しません。");
            }

            int newIndex = _foucusItemIndex + _drawParameter.ColCount;

            if (newIndex < _itemCount)
            {
                if (_isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    // Shiftキー押下
                    _selectedItemIndexs.AddRange(newIndex);
                }
                else if (_isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    // Ctrlキー押下
                }
                else
                {
                    // 修飾キー無し
                    _selectedItemIndexs.Clear();
                    _selectedItemIndexs.Add(newIndex);
                }

                Rectangle itemRect = getItemDrawRectangle(newIndex);
                if (itemRect.Bottom > this.Height)
                {
                    _scrollBar.Value = Math.Min(_scrollBar.Maximum, _scrollBar.Value + _itemHeight);
                }

                ensureVisible(newIndex);

                _foucusItemIndex = newIndex;

                return true;
            }
            else
            {
                return false;
            }
        }

        private void spaceKeyDown()
        {
            if (!_selectedItemIndexs.Contains(_foucusItemIndex))
            {
                _selectedItemIndexs.Add(_foucusItemIndex);
            }
            else
            {
                _selectedItemIndexs.Remove(_foucusItemIndex);
            }
        }

        private void pageUpKeyDown()
        {
            if (_itemCount == 0)
            {
                throw new Exception("項目が存在しません。");
            }

            int newIndex = _foucusItemIndex - _drawParameter.ColCount * (_drawParameter.DrawLastRow - _drawParameter.DrawFirstRow - 1);
            if (newIndex < 0)
            {
                newIndex = 0;
            }

            if (_isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                // Shiftキー押下
                _selectedItemIndexs.AddRange(newIndex);
            }
            else if (_isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                // Ctrlキー押下
            }
            else
            {
                // 修飾キー無し
                _selectedItemIndexs.Clear();
                _selectedItemIndexs.Add(newIndex);
            }

            ensureVisible(newIndex);

            _foucusItemIndex = newIndex;
        }

        private void pageDownKeyDown()
        {
            if (_itemCount == 0)
            {
                throw new Exception("項目が存在しません。");
            }

            int newIndex = _foucusItemIndex + _drawParameter.ColCount * (_drawParameter.DrawLastRow - _drawParameter.DrawFirstRow - 1);
            if (newIndex >= _itemCount)
            {
                newIndex = _itemCount - 1;
            }

            if (_isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                // Shiftキー押下
                _selectedItemIndexs.AddRange(newIndex);
            }
            else if (_isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                // Ctrlキー押下
            }
            else
            {
                // 修飾キー無し
                _selectedItemIndexs.Clear();
                _selectedItemIndexs.Add(newIndex);
            }

            ensureVisible(newIndex);

            _foucusItemIndex = newIndex;
        }

        private void homeKeyDown()
        {
            if (_itemCount == 0)
            {
                throw new Exception("項目が存在しません。");
            }

            int newIndex = 0;
            if (_isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                // Shiftキー押下
                _selectedItemIndexs.AddRange(newIndex);
            }
            else if (_isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                // Ctrlキー押下
            }
            else
            {
                // 修飾キー無し
                _selectedItemIndexs.Clear();
                _selectedItemIndexs.Add(newIndex);
            }

            ensureVisible(newIndex);

            _foucusItemIndex = newIndex;
        }

        private void endKeyDown()
        {
            if (_itemCount == 0)
            {
                throw new Exception("項目が存在しません。");
            }

            int newIndex = _itemCount - 1;
            if (_isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                // Shiftキー押下
                _selectedItemIndexs.AddRange(newIndex);
            }
            else if (_isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                // Ctrlキー押下
            }
            else
            {
                // 修飾キー無し
                _selectedItemIndexs.Clear();
                _selectedItemIndexs.Add(newIndex);
            }

            ensureVisible(newIndex);

            _foucusItemIndex = newIndex;
        }

        #endregion

        #region マウスダウン操作

        private void leftMouseDown(int x, int y)
        {
            HitTestInfo ht = getHitTestFromDrawPoint(x, y);

            if (_isMultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                // Shiftキー押下
                if (ht.IsItem)
                {
                    _selectedItemIndexs.AddRange(ht.ItemIndex);
                    _foucusItemIndex = ht.ItemIndex;
                }
            }
            else if (_isMultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                // Ctrlキー押下
                if (ht.IsItem)
                {
                    if (!_selectedItemIndexs.Contains(ht.ItemIndex))
                    {
                        _selectedItemIndexs.Add(ht.ItemIndex);
                    }
                    else
                    {
                        _selectedItemIndexs.Remove(ht.ItemIndex);
                    }

                    _foucusItemIndex = ht.ItemIndex;
                }
            }
            else
            {
                // 修飾キー無し
                if (ht.IsItem)
                {
                    if (_selectedItemIndexs.Contains(ht.ItemIndex))
                    {
                        _foucusItemIndex = ht.ItemIndex;
                    }
                    else
                    {
                        _selectedItemIndexs.Clear();
                        _selectedItemIndexs.Add(ht.ItemIndex);
                        _foucusItemIndex = ht.ItemIndex;
                    }
                }
                else
                {
                    _selectedItemIndexs.Clear();
                }
            }
        }

        private void rightMouseDown(int x, int y)
        {
            HitTestInfo ht = getHitTestFromDrawPoint(x, y);

            if (ht.IsItem)
            {
                if (ht.IsSelected)
                {
                    _foucusItemIndex = ht.ItemIndex;
                }
                else
                {
                    _selectedItemIndexs.Clear();
                    _selectedItemIndexs.Add(ht.ItemIndex);
                    _foucusItemIndex = ht.ItemIndex;
                }
            }
            else
            {
                _selectedItemIndexs.Clear();
            }
        }

        private void middleMouseDown(int x, int y)
        {
            HitTestInfo ht = getHitTestFromDrawPoint(x, y);

            if (ht.IsItem)
            {
                if (ht.IsSelected)
                {
                    _foucusItemIndex = ht.ItemIndex;
                }
                else
                {
                    _selectedItemIndexs.Clear();
                    _selectedItemIndexs.Add(ht.ItemIndex);
                    _foucusItemIndex = ht.ItemIndex;
                }
            }
            else
            {
                _selectedItemIndexs.Clear();
            }
        }

        #endregion

        #endregion

        #region スクロールバーイベント

        private void _scrollBar_ValueChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        #endregion

        #region 選択されている項目インデックスのリストイベント

        private void _selectedItemIndexs_Change(object sender, EventArgs e)
        {
            OnSelectedItemChanged(new EventArgs());
        }

        #endregion
    }
}
