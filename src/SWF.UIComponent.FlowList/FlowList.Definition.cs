using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.FlowList
{
    /// <summary>
    /// フローリストコントロール
    /// </summary>
    public partial class FlowList
    {
        #region イベント・デリゲート

        public event EventHandler<DrawItemEventArgs> DrawItem;

        public event EventHandler<DrawItemChangedEventArgs> DrawItemChanged;

        public event EventHandler SelectedItemChanged;

        public event EventHandler<MouseEventArgs> ItemMouseClick;

        public event EventHandler<MouseEventArgs> ItemMouseDoubleClick;

        public event EventHandler<MouseEventArgs> BackgroundMouseClick;

        public event EventHandler ItemExecute;

        public event EventHandler ItemDelete;

        public event EventHandler ItemCopy;

        public event EventHandler ItemCut;

        public event EventHandler DragStart;

        #endregion

        #region パブリックプロパティ

        public bool CanKeyDown { get; set; } = true;

        /// <summary>
        /// スクロールバーの幅
        /// </summary>
        public int ScrollBarWidth
        {
            get
            {
                return _scrollBar.Width;
            }
        }

        /// <summary>
        /// スクロールバー表示フラグ
        /// </summary>
        public bool IsScrollBarVisible
        {
            get
            {
                return _scrollBar.Visible;
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
                return _itemCount;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                if (_rectangleSelection.IsBegun)
                {
                    _rectangleSelection.EndSelection();
                }

                _selectedItemIndexs.Clear();

                _mousePointItemIndex = -1;

                if (_foucusItemIndex > value - 1)
                {
                    _foucusItemIndex = -1;
                }

                _itemCount = value;

                this.Invalidate();
            }
        }

        /// <summary>
        /// 項目幅
        /// </summary>
        [Category("項目表示"), DefaultValue(MinimumItemSize)]
        public int ItemWidth
        {
            get
            {
                return _itemWidth;
            }
            set
            {
                if (_rectangleSelection.IsBegun)
                {
                    throw new Exception("短形選択中は設定できません。");
                }

                if (value < MinimumItemSize || MaximumItemSize < value)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _itemWidth = value;

                this.Invalidate();
            }
        }

        /// <summary>
        /// 項目高さ
        /// </summary>
        [Category("項目表示"), DefaultValue(MinimumItemSize)]
        public int ItemHeight
        {
            get
            {
                return _itemHeight;
            }
            set
            {
                if (_rectangleSelection.IsBegun)
                {
                    throw new Exception("短形選択中は設定できません。");
                }

                if (value < MinimumItemSize || MaximumItemSize < value)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _itemHeight = value;

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
                return _itemSpace;
            }
            set
            {
                if (_rectangleSelection.IsBegun)
                {
                    throw new Exception("短形選択中は設定できません。");
                }

                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _itemSpace = value;

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
                return _isLileList;
            }
            set
            {
                if (_rectangleSelection.IsBegun)
                {
                    throw new Exception("短形選択中は設定できません。");
                }

                _isLileList = value;

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
                return _isMultiSelect;
            }
            set
            {
                if (_rectangleSelection.IsBegun)
                {
                    throw new Exception("短形選択中は設定できません。");
                }

                _selectedItemIndexs.Clear();

                _isMultiSelect = value;
                _rectangleSelection.IsUse = value;

                this.Invalidate();
            }
        }

        /// <summary>
        /// 項目テキスト色
        /// </summary>
        [Category("項目描画")]
        public Color ItemTextColor
        {
            get
            {
                return _itemTextColor;
            }
        }

        /// <summary>
        /// 項目選択色
        /// </summary>
        [Category("項目描画")]
        public Color SelectedItemColor
        {
            get
            {
                return _selectedItemColor;
            }
        }

        /// <summary>
        /// 項目フォーカス色
        /// </summary>
        [Category("項目描画")]
        public Color FocusItemColor
        {
            get
            {
                return _focusItemColor;
            }
        }

        /// <summary>
        /// 項目マウスポイント色
        /// </summary>
        [Category("項目描画")]
        public Color MousePointItemColor
        {
            get
            {
                return _mousePointItemColor;
            }
        }

        /// <summary>
        /// 短形選択色
        /// </summary>
        [Category("項目描画")]
        public Color RectangleSelectionColor
        {
            get
            {
                return _rectangleSelectionColor;
            }
        }

        /// <summary>
        /// 項目テキストフォーマット
        /// </summary>
        [Category("項目描画")]
        public StringTrimming ItemTextTrimming
        {
            get
            {
                return _itemTextTrimming;
            }
            set
            {
                _itemTextTrimming = value;
            }
        }

        /// <summary>
        /// 項目テキストフォーマット
        /// </summary>
        [Category("項目描画")]
        public StringAlignment ItemTextAlignment
        {
            get
            {
                return _itemTextAlignment;
            }
            set
            {
                _itemTextAlignment = value;
            }
        }

        /// <summary>
        /// 項目テキストフォーマット
        /// </summary>
        [Category("項目描画")]
        public StringAlignment ItemTextLineAlignment
        {
            get
            {
                return _itemTextLineAlignment;
            }
            set
            {
                _itemTextLineAlignment = value;
            }
        }

        /// <summary>
        /// 項目テキストフォーマット
        /// </summary>
        [Category("項目描画")]
        public StringFormatFlags ItemTextFormatFlags
        {
            get
            {
                return _itemTextFormatFlags;
            }
            set
            {
                _itemTextFormatFlags = value;
            }
        }

        [Browsable(false)]
        public SolidBrush ItemTextBrush
        {
            get
            {
                if (_itemTextBrush == null)
                {
                    _itemTextBrush = new SolidBrush(_itemTextColor);
                }

                return _itemTextBrush;
            }
        }

        [Browsable(false)]
        public SolidBrush SelectedItemBrush
        {
            get
            {
                if (_selectedItemBrush == null)
                {
                    _selectedItemBrush = new SolidBrush(_selectedItemColor);
                }

                return _selectedItemBrush;
            }
        }

        [Browsable(false)]
        public Pen SelectedItemPen
        {
            get
            {
                if (_selectedItemPen == null)
                {
                    _selectedItemPen = new Pen(Color.FromArgb(
                        _selectedItemColor.A * 2,
                        _selectedItemColor.R,
                        _selectedItemColor.G,
                        _selectedItemColor.B));
                }

                return _selectedItemPen;
            }
        }

        [Browsable(false)]
        public SolidBrush FocusItemBrush
        {
            get
            {
                if (_foucusItemBrush == null)
                {
                    _foucusItemBrush = new SolidBrush(_focusItemColor);
                }

                return _foucusItemBrush;
            }
        }

        [Browsable(false)]
        public SolidBrush MousePointItemBrush
        {
            get
            {
                if (_mousePointItemBrush == null)
                {
                    _mousePointItemBrush = new SolidBrush(_mousePointItemColor);
                }

                return _mousePointItemBrush;
            }
        }

        [Browsable(false)]
        public StringFormat ItemTextFormat
        {
            get
            {
                if (_itemTextFormat == null)
                {
                    _itemTextFormat = new StringFormat();
                }

                _itemTextFormat.Trimming = _itemTextTrimming;
                _itemTextFormat.Alignment = _itemTextAlignment;
                _itemTextFormat.LineAlignment = _itemTextLineAlignment;
                _itemTextFormat.FormatFlags = _itemTextFormatFlags;

                return _itemTextFormat;
            }
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// 描画領域をリフレッシュします。
        /// </summary>
        public new void Refresh()
        {
            _drawParameter = new DrawParameter();
            this.Invalidate();
            this.Update();
        }

        /// <summary>
        /// 描画を停止します。
        /// </summary>
        public void BeginUpdate()
        {
            _isDraw = false;
        }

        /// <summary>
        /// 描画を開始します。
        /// </summary>
        public void EndUpdate()
        {
            _isDraw = true;
            this.Invalidate();
            this.Update();
        }

        /// <summary>
        /// 項目のサイズを設定します。
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetItemSize(int width, int height)
        {
            if (_rectangleSelection.IsBegun)
            {
                throw new Exception("短形選択中は設定できません。");
            }

            if (width < MinimumItemSize || MaximumItemSize < width)
            {
                throw new ArgumentOutOfRangeException("width");
            }

            if (height < MinimumItemSize || MaximumItemSize < height)
            {
                throw new ArgumentOutOfRangeException("height");
            }

            _itemWidth = width;
            _itemHeight = height;

            this.Invalidate();
        }

        /// <summary>
        /// 選択項目をクリアします。
        /// </summary>
        public void ClearSelectedItems()
        {
            _selectedItemIndexs.Clear();
            _foucusItemIndex = -1;
            _mousePointItemIndex = -1;
            this.Invalidate();
        }

        /// <summary>
        /// 指定した項目を選択します。
        /// </summary>
        /// <param name="itemIndex"></param>
        public void SelectItem(int itemIndex)
        {
            if (_rectangleSelection.IsBegun)
            {
                throw new Exception("短形選択中は設定できません。");
            }

            if (itemIndex < 0 || _itemCount - 1 < itemIndex)
            {
                throw new ArgumentOutOfRangeException("itemIndex");
            }

            _foucusItemIndex = itemIndex;

            _selectedItemIndexs.Clear();
            _selectedItemIndexs.Add(itemIndex);

            ensureVisible(itemIndex);

            this.Invalidate();

            OnSelectedItemChanged(new EventArgs());
        }

        /// <summary>
        /// 選択している項目のインデックスを取得します。
        /// </summary>
        /// <returns></returns>
        public IList<int> GetSelectedIndexs()
        {
            if (_rectangleSelection.IsBegun)
            {
                throw new Exception("短形選択中は取得できません。");
            }

            return _selectedItemIndexs.GetList();
        }

        /// <summary>
        /// クライアント座標から、項目のインデックスを取得します。
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <returns>座標上に項目が存在する場合は、項目のインデックス。存在しない場合は-1を返します。</returns>
        public int IndexFromPoint(int x, int y)
        {
            HitTestInfo ht = getHitTestFromDrawPoint(x, y);
            return ht.ItemIndex;
        }

        /// <summary>
        /// 項目を再描画します。
        /// </summary>
        /// <param name="itemIndex">項目インデックス</param>
        public void InvalidateFromItemIndex(int itemIndex)
        {
            if (itemIndex < 0 || _itemCount - 1 < itemIndex)
            {
                throw new ArgumentOutOfRangeException("itemIndex");
            }

            invalidateFromItemIndex(itemIndex);
        }

        /// <summary>
        /// マウスホイール処理を実行します。
        /// </summary>
        /// <param name="e"></param>
        public void MouseWheelProcess(MouseEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

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

            base.OnMouseWheel(e);
        }

        #endregion

        #region イベント発生メソッド

        protected virtual void OnDrawItem(DrawItemEventArgs e)
        {
            if (DrawItem != null)
            {
                DrawItem(this, e);
            }
        }

        protected virtual void OnDrawItemChanged(DrawItemChangedEventArgs e)
        {
            if (DrawItemChanged != null)
            {
                DrawItemChanged(this, e);
            }
        }

        protected virtual void OnSelectedItemChanged(EventArgs e)
        {
            if (SelectedItemChanged != null)
            {
                SelectedItemChanged(this, e);
            }
        }

        protected virtual void OnItemMouseClick(MouseEventArgs e)
        {
            if (ItemMouseClick != null)
            {
                ItemMouseClick(this, e);
            }
        }

        protected virtual void OnItemMouseDoubleClick(MouseEventArgs e)
        {
            if (ItemMouseDoubleClick != null)
            {
                ItemMouseDoubleClick(this, e);
            }
        }

        protected virtual void OnBackgroundMouseClick(MouseEventArgs e)
        {
            if (BackgroundMouseClick != null)
            {
                BackgroundMouseClick(this, e);
            }
        }

        protected virtual void OnItemExecute(EventArgs e)
        {
            if (ItemExecute != null)
            {
                ItemExecute(this, e);
            }
        }

        protected virtual void OnItemDelete(EventArgs e)
        {
            if (ItemDelete != null)
            {
                ItemDelete(this, e);
            }
        }

        protected virtual void OnItemCopy(EventArgs e)
        {
            if (ItemCopy != null)
            {
                ItemCopy(this, e);
            }
        }

        protected virtual void OnItemCut(EventArgs e)
        {
            if (ItemCut != null)
            {
                ItemCut(this, e);
            }
        }

        protected virtual void OnDragStart(EventArgs e)
        {
            if (DragStart != null)
            {
                DragStart(this, e);
            }
        }

        #endregion
    }
}
