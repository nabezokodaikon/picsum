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
    public sealed partial class FlowList
    {

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

        public bool CanKeyDown { get; set; } = true;

        /// <summary>
        /// スクロールバーの幅
        /// </summary>
        public int ScrollBarWidth
        {
            get
            {
                return this.scrollBar.Width;
            }
        }

        /// <summary>
        /// スクロールバー表示フラグ
        /// </summary>
        public bool IsScrollBarVisible
        {
            get
            {
                return this.scrollBar.Visible;
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
                return this.itemCount;
            }
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 0, nameof(value));

                if (this.rectangleSelection.IsBegun)
                {
                    this.rectangleSelection.EndSelection();
                }

                this.selectedItemIndexs.Clear();

                this.mousePointItemIndex = -1;

                if (this.foucusItemIndex > value - 1)
                {
                    this.foucusItemIndex = -1;
                }

                this.itemCount = value;

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
                return this.itemWidth;
            }
            set
            {
                if (this.rectangleSelection.IsBegun)
                {
                    throw new InvalidOperationException("短形選択中は設定できません。");
                }

                if (value < MINIMUM_ITEM_SIZE || MAXIMUM_ITEM_SIZE < value)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                this.itemWidth = value;

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
                return this.itemHeight;
            }
            set
            {
                if (this.rectangleSelection.IsBegun)
                {
                    throw new InvalidOperationException("短形選択中は設定できません。");
                }

                if (value < MINIMUM_ITEM_SIZE || MAXIMUM_ITEM_SIZE < value)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                this.itemHeight = value;

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
                return this.itemSpace;
            }
            set
            {
                if (this.rectangleSelection.IsBegun)
                {
                    throw new InvalidOperationException("短形選択中は設定できません。");
                }

                ArgumentOutOfRangeException.ThrowIfLessThan(value, 0, nameof(value));

                this.itemSpace = value;
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
                return this.isLileList;
            }
            set
            {
                if (this.rectangleSelection.IsBegun)
                {
                    throw new InvalidOperationException("短形選択中は設定できません。");
                }

                this.isLileList = value;
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
                return this.isMultiSelect;
            }
            set
            {
                if (this.rectangleSelection.IsBegun)
                {
                    throw new InvalidOperationException("短形選択中は設定できません。");
                }

                this.selectedItemIndexs.Clear();
                this.isMultiSelect = value;
                this.rectangleSelection.IsUse = value;
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
                return this.itemTextColor;
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
                return this.selectedItemColor;
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
                return this.focusItemColor;
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
                return this.mousePointItemColor;
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
                return this.rectangleSelectionColor;
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
                return this.itemTextTrimming;
            }
            set
            {
                this.itemTextTrimming = value;
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
                return this.itemTextAlignment;
            }
            set
            {
                this.itemTextAlignment = value;
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
                return this.itemTextLineAlignment;
            }
            set
            {
                this.itemTextLineAlignment = value;
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
                return this.itemTextFormatFlags;
            }
            set
            {
                this.itemTextFormatFlags = value;
            }
        }

        [Browsable(false)]
        public SolidBrush ItemTextBrush
        {
            get
            {
                this.itemTextBrush ??= new SolidBrush(this.itemTextColor);
                return this.itemTextBrush;
            }
        }

        [Browsable(false)]
        public SolidBrush SelectedItemBrush
        {
            get
            {
                this.selectedItemBrush ??= new SolidBrush(this.selectedItemColor);
                return this.selectedItemBrush;
            }
        }

        [Browsable(false)]
        public Pen SelectedItemPen
        {
            get
            {
                this.selectedItemPen ??= new Pen(Color.FromArgb(
                    255,
                    this.selectedItemColor.R,
                    this.selectedItemColor.G,
                    this.selectedItemColor.B),
                    2);
                return this.selectedItemPen;
            }
        }

        [Browsable(false)]
        public SolidBrush FocusItemBrush
        {
            get
            {
                this.foucusItemBrush ??= new SolidBrush(this.focusItemColor);
                return this.foucusItemBrush;
            }
        }

        [Browsable(false)]
        public SolidBrush MousePointItemBrush
        {
            get
            {
                this.mousePointItemBrush ??= new SolidBrush(this.mousePointItemColor);
                return this.mousePointItemBrush;
            }
        }

        [Browsable(false)]
        public StringFormat ItemTextFormat
        {
            get
            {
                this.itemTextFormat ??= new()
                {
                    Trimming = this.itemTextTrimming,
                    Alignment = this.itemTextAlignment,
                    LineAlignment = this.itemTextLineAlignment,
                    FormatFlags = this.itemTextFormatFlags
                };
                return this.itemTextFormat;
            }
        }

        /// <summary>
        /// 描画領域をリフレッシュします。
        /// </summary>
        public new void Refresh()
        {
            this.drawParameter = new DrawParameter();
            this.Invalidate();
        }

        /// <summary>
        /// 描画を停止します。
        /// </summary>
        public void BeginUpdate()
        {
            this.isDraw = false;
        }

        /// <summary>
        /// 描画を開始します。
        /// </summary>
        public void EndUpdate()
        {
            this.isDraw = true;
            this.Invalidate();
        }

        /// <summary>
        /// 項目のサイズを設定します。
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetItemSize(int width, int height)
        {
            if (this.rectangleSelection.IsBegun)
            {
                throw new InvalidOperationException("短形選択中は設定できません。");
            }

            if (width < MINIMUM_ITEM_SIZE || MAXIMUM_ITEM_SIZE < width)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }

            if (height < MINIMUM_ITEM_SIZE || MAXIMUM_ITEM_SIZE < height)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            this.itemWidth = width;
            this.itemHeight = height;

            this.Invalidate();
        }

        /// <summary>
        /// 選択項目をクリアします。
        /// </summary>
        public void ClearSelectedItems()
        {
            this.selectedItemIndexs.Clear();
            this.foucusItemIndex = -1;
            this.mousePointItemIndex = -1;
            this.Invalidate();
        }

        /// <summary>
        /// 指定した項目を選択します。
        /// </summary>
        /// <param name="itemIndex"></param>
        public void SelectItem(int itemIndex)
        {
            if (this.rectangleSelection.IsBegun)
            {
                throw new InvalidOperationException("短形選択中は設定できません。");
            }

            if (itemIndex < 0 || this.itemCount - 1 < itemIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(itemIndex));
            }

            this.foucusItemIndex = itemIndex;

            this.selectedItemIndexs.Clear();
            this.selectedItemIndexs.Add(itemIndex);

            this.EnsureVisible(itemIndex);

            this.Invalidate();

            this.OnSelectedItemChanged(EventArgs.Empty);
        }

        /// <summary>
        /// 選択している項目のインデックスを取得します。
        /// </summary>
        /// <returns></returns>
        public IList<int> GetSelectedIndexs()
        {
            if (this.rectangleSelection.IsBegun)
            {
                throw new InvalidOperationException("短形選択中は取得できません。");
            }

            return this.selectedItemIndexs.GetList();
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
            if (itemIndex < 0 || this.itemCount - 1 < itemIndex)
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
