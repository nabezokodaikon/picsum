using SWF.Core.Base;
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
        public static readonly Color LIGHT_ITEM_TEXT_COLOR = Color.FromArgb(255, 0, 0, 0);
        public static readonly Color DARK_ITEM_TEXT_COLOR = Color.FromArgb(255, 255, 255);

        public static readonly Color LIGHT_SELECTED_ITEM_COLOR = Color.FromArgb(
            SystemColors.Highlight.A / 4,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B);

        public static readonly Color LIGHT_FOCUS_ITEM_COLOR = Color.FromArgb(
            SystemColors.Highlight.A / 8,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B);

        public static readonly Color LIGHT_MOUSE_POINT_ITEM_COLOR = Color.FromArgb(
            SystemColors.Highlight.A / 8,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B);

        public static readonly Color LIGHT_RECTANGLE_SELECTION_COLOR = Color.FromArgb(
            SystemColors.Highlight.A / 4,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B);

        public static readonly Color DARK_SELECTED_ITEM_COLOR = Color.FromArgb(64, 255, 255, 255);
        public static readonly Color DARK_FOCUS_ITEM_COLOR = Color.FromArgb(32, 255, 255, 255);
        public static readonly Color DARK_MOUSE_POINT_ITEM_COLOR = Color.FromArgb(32, 255, 255, 255);
        public static readonly Color DARK_RECTANGLE_SELECTION_COLOR = Color.FromArgb(64, 255, 255, 255);

        public static readonly SolidBrush LIGHT_ITEM_TEXT_BRUSH = new(LIGHT_ITEM_TEXT_COLOR);
        public static readonly SolidBrush LIGHT_SELECTED_ITEM_BRUSH = new(LIGHT_SELECTED_ITEM_COLOR);
        public static readonly SolidBrush LIGHT_FOUCUS_ITEM_BRUSH = new(LIGHT_FOCUS_ITEM_COLOR);
        public static readonly SolidBrush LIGHT_MOUSE_POINT_ITEM_BRUSH = new(LIGHT_MOUSE_POINT_ITEM_COLOR);
        public static readonly SolidBrush LIGHT_RECTANGLE_SELECTION_BRUSH = new(LIGHT_RECTANGLE_SELECTION_COLOR);

        public static readonly SolidBrush DARK_ITEM_TEXT_BRUSH = new(DARK_ITEM_TEXT_COLOR);
        public static readonly SolidBrush DARK_SELECTED_ITEM_BRUSH = new(DARK_SELECTED_ITEM_COLOR);
        public static readonly SolidBrush DARK_FOUCUS_ITEM_BRUSH = new(DARK_FOCUS_ITEM_COLOR);
        public static readonly SolidBrush DARK_MOUSE_POINT_ITEM_BRUSH = new(DARK_MOUSE_POINT_ITEM_COLOR);
        public static readonly SolidBrush DARK_RECTANGLE_SELECTION_BRUSH = new(DARK_RECTANGLE_SELECTION_COLOR);

        private static readonly Pen LIGHT_SELECTED_ITEM_PEN = new(Color.FromArgb(
            255,
            LIGHT_SELECTED_ITEM_COLOR.R,
            LIGHT_SELECTED_ITEM_COLOR.G,
            LIGHT_SELECTED_ITEM_COLOR.B),
            1f);

        private static readonly Pen LIGHT_FOUCUS_ITEM_PEN = new(Color.FromArgb(
            255,
            LIGHT_FOCUS_ITEM_COLOR.R,
            LIGHT_FOCUS_ITEM_COLOR.G,
            LIGHT_FOCUS_ITEM_COLOR.B),
            1f);

        private static readonly Pen LIGHT_RECTANGLE_SELECTION_PEN = new(Color.FromArgb(
            LIGHT_RECTANGLE_SELECTION_COLOR.A,
            LIGHT_RECTANGLE_SELECTION_COLOR.R,
            LIGHT_RECTANGLE_SELECTION_COLOR.G,
            LIGHT_RECTANGLE_SELECTION_COLOR.B),
            1f);

        private static readonly Pen DARK_SELECTED_ITEM_PEN = new(Color.FromArgb(
            128,
            DARK_SELECTED_ITEM_COLOR.R,
            DARK_SELECTED_ITEM_COLOR.G,
            DARK_SELECTED_ITEM_COLOR.B),
            1f);

        private static readonly Pen DARK_FOUCUS_ITEM_PEN = new(Color.FromArgb(
            128,
            DARK_FOCUS_ITEM_COLOR.R,
            DARK_FOCUS_ITEM_COLOR.G,
            DARK_FOCUS_ITEM_COLOR.B),
            1f);

        private static readonly Pen DARK_RECTANGLE_SELECTION_PEN = new(Color.FromArgb(
            128,
            DARK_RECTANGLE_SELECTION_COLOR.R,
            DARK_RECTANGLE_SELECTION_COLOR.G,
            DARK_RECTANGLE_SELECTION_COLOR.B),
            1f);

        private static readonly Dictionary<float, Pen> LIGHT_SELECTED_ITEM_PEN_CACHE = [];
        private static readonly Dictionary<float, Pen> LIGHT_FOUCUS_ITEM_PEN_CACHE = [];
        private static readonly Dictionary<float, Pen> LIGHT_RECTANGLE_SELECTION_PEN_CACHE = [];

        private static readonly Dictionary<float, Pen> DARK_SELECTED_ITEM_PEN_CACHE = [];
        private static readonly Dictionary<float, Pen> DARK_FOUCUS_ITEM_PEN_CACHE = [];
        private static readonly Dictionary<float, Pen> DARK_RECTANGLE_SELECTION_PEN_CACHE = [];

        public static Pen GetLightSelectedItemPen(Control control)
        {
            var scale = WindowUtil.GetCurrentWindowScale(control);
            if (LIGHT_SELECTED_ITEM_PEN_CACHE.TryGetValue(scale, out var pen))
            {
                return pen;
            }
            else
            {
                var newPen = new Pen(
                    LIGHT_SELECTED_ITEM_PEN.Color,
                    LIGHT_SELECTED_ITEM_PEN.Width * scale);
                LIGHT_SELECTED_ITEM_PEN_CACHE.Add(scale, newPen);
                return newPen;
            }
        }

        public static Pen GetLightFoucusItemPen(Control control)
        {
            var scale = WindowUtil.GetCurrentWindowScale(control);
            if (LIGHT_FOUCUS_ITEM_PEN_CACHE.TryGetValue(scale, out var pen))
            {
                return pen;
            }
            else
            {
                var newPen = new Pen(
                    LIGHT_FOUCUS_ITEM_PEN.Color,
                    LIGHT_FOUCUS_ITEM_PEN.Width * scale);
                LIGHT_FOUCUS_ITEM_PEN_CACHE.Add(scale, newPen);
                return newPen;
            }
        }

        public Pen GetLightRectangleSelectionPen(Control control)
        {
            var scale = WindowUtil.GetCurrentWindowScale(control);
            if (LIGHT_RECTANGLE_SELECTION_PEN_CACHE.TryGetValue(scale, out var pen))
            {
                return pen;
            }
            else
            {
                var newPen = new Pen(
                    LIGHT_RECTANGLE_SELECTION_PEN.Color,
                    LIGHT_RECTANGLE_SELECTION_PEN.Width * scale);
                LIGHT_RECTANGLE_SELECTION_PEN_CACHE.Add(scale, newPen);
                return newPen;
            }
        }

        public static Pen GetDarkSelectedItemPen(Control control)
        {
            var scale = WindowUtil.GetCurrentWindowScale(control);
            if (DARK_SELECTED_ITEM_PEN_CACHE.TryGetValue(scale, out var pen))
            {
                return pen;
            }
            else
            {
                var newPen = new Pen(
                    DARK_SELECTED_ITEM_PEN.Color,
                    DARK_SELECTED_ITEM_PEN.Width * scale);
                DARK_SELECTED_ITEM_PEN_CACHE.Add(scale, newPen);
                return newPen;
            }
        }

        public static Pen GetDarkFoucusItemPen(Control control)
        {
            var scale = WindowUtil.GetCurrentWindowScale(control);
            if (DARK_FOUCUS_ITEM_PEN_CACHE.TryGetValue(scale, out var pen))
            {
                return pen;
            }
            else
            {
                var newPen = new Pen(
                    DARK_FOUCUS_ITEM_PEN.Color,
                    DARK_FOUCUS_ITEM_PEN.Width * scale);
                DARK_FOUCUS_ITEM_PEN_CACHE.Add(scale, newPen);
                return newPen;
            }
        }

        public Pen GetDarkRectangleSelectionPen(Control control)
        {
            var scale = WindowUtil.GetCurrentWindowScale(control);
            if (DARK_RECTANGLE_SELECTION_PEN_CACHE.TryGetValue(scale, out var pen))
            {
                return pen;
            }
            else
            {
                var newPen = new Pen(
                    DARK_RECTANGLE_SELECTION_PEN.Color,
                    DARK_RECTANGLE_SELECTION_PEN.Width * scale);
                DARK_RECTANGLE_SELECTION_PEN_CACHE.Add(scale, newPen);
                return newPen;
            }
        }

        private static readonly Size DRAG_SIZE = GetDragSize();

        private static Size GetDragSize()
        {
            var size = SystemInformation.DragSize.Width * 16;
            return new Size(size, size);
        }

        public const int SCROLL_BAR_DEFAULT_WIDTH = 17;

        // 項目最小サイズ
        public const int MINIMUM_ITEM_SIZE = 16;

        public event EventHandler<DrawItemEventArgs> DrawItem;
        public event EventHandler<DrawItemsEventArgs> DrawItems;
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

        private void OnDrawItem(DrawItemEventArgs e)
        {
            this.DrawItem?.Invoke(this, e);
        }

        private void OnDrawItems(DrawItemsEventArgs e)
        {
            this.DrawItems?.Invoke(this, e);
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
