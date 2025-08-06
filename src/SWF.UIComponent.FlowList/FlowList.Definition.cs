using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.FlowList
{
    /// <summary>
    /// フローリストコントロール
    /// </summary>
    public sealed partial class FlowList
    {
        public const int SCROLL_BAR_DEFAULT_WIDTH = 17;

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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanKeyDown { get; set; } = true;

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

        /// <summary>
        /// 項目テキスト色
        /// </summary>
        [Category("項目描画")]
        public Color ItemTextColor
        {
            get
            {
                return ITEM_TEXT_COLOR;
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
                return SELECTED_ITEM_COLOR;
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
                return FOCUS_ITEM_COLOR;
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
                return MOUSE_POINT_ITEM_COLOR;
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
                return RECTANGLE_SELECTION_COLOR;
            }
        }

        /// <summary>
        /// 項目テキストフォーマット
        /// </summary>
        [Category("項目描画")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StringTrimming ItemTextTrimming
        {
            get
            {
                return this._itemTextTrimming;
            }
            set
            {
                this._itemTextTrimming = value;
            }
        }

        /// <summary>
        /// 項目テキストフォーマット
        /// </summary>
        [Category("項目描画")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StringAlignment ItemTextAlignment
        {
            get
            {
                return this._itemTextAlignment;
            }
            set
            {
                this._itemTextAlignment = value;
            }
        }

        /// <summary>
        /// 項目テキストフォーマット
        /// </summary>
        [Category("項目描画")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StringAlignment ItemTextLineAlignment
        {
            get
            {
                return this._itemTextLineAlignment;
            }
            set
            {
                this._itemTextLineAlignment = value;
            }
        }

        /// <summary>
        /// 項目テキストフォーマット
        /// </summary>
        [Category("項目描画")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StringFormatFlags ItemTextFormatFlags
        {
            get
            {
                return this._itemTextFormatFlags;
            }
            set
            {
                this._itemTextFormatFlags = value;
            }
        }

        [Browsable(false)]
        public SolidBrush ItemTextBrush
        {
            get
            {
                return ITEM_TEXT_BRUSH;
            }
        }

        [Browsable(false)]
        public SolidBrush SelectedItemBrush
        {
            get
            {
                return SELECTED_ITEM_BRUSH;
            }
        }

        [Browsable(false)]
        public SolidBrush FocusItemBrush
        {
            get
            {
                return FOUCUS_ITEM_BRUSH;
            }
        }

        [Browsable(false)]
        public SolidBrush MousePointItemBrush
        {
            get
            {
                return MOUSE_POINT_ITEM_BRUSH;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StringFormat ItemTextFormat
        {
            get
            {
                this._itemTextFormat ??= new()
                {
                    Trimming = this._itemTextTrimming,
                    Alignment = this._itemTextAlignment,
                    LineAlignment = this._itemTextLineAlignment,
                    FormatFlags = this._itemTextFormatFlags
                };
                return this._itemTextFormat;
            }
            set
            {
                this._itemTextFormat = value;
            }
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

            this.EnsureVisible(itemIndex);

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
                this.EnsureVisible(itemIndex);
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
            this.Invalidate(rect);
        }

        private void OnDrawItem(DrawItemEventArgs e)
        {
            this.DrawItem?.Invoke(this, e);
        }

        private void OnDrawItemChanged(DrawItemChangedEventArgs e)
        {
            this.DrawItemChanged?.Invoke(this, e);
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
