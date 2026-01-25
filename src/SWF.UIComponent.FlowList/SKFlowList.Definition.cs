using SkiaSharp;
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
    public sealed partial class SKFlowList
    {
        public static readonly SKPaint IMAGE_PAINT = new()
        {

        };

        public static readonly SKColor BACK_COLOR = new(64, 68, 71);
        public static readonly SKColor ITEM_TEXT_COLOR = new(255, 255, 255, 255);
        public static readonly SKColor SELECTED_FILL_COLOR = new(255, 255, 255, 64);
        public static readonly SKColor SELECTED_STROKE_COLOR = new(255, 255, 255, 64);
        public static readonly SKColor FOCUS_FILL_COLOR = new(255, 255, 255, 32);
        public static readonly SKColor MOUSE_POINT_FILL_COLOR = new(255, 255, 255, 32);
        public static readonly SKColor RECTANGLE_SELECTION_FILL_COLOR = new(255, 255, 255, 64);
        public static readonly SKColor RECTANGLE_SELECTION_STROKE_COLOR = new(255, 255, 255, 64);

        public static readonly SKPaint TEXT_PAINT = new()
        {
            Color = ITEM_TEXT_COLOR,
            IsAntialias = true,
        };

        public static readonly SKPaint SELECTED_FILL_PAINT = new()
        {
            Color = SELECTED_FILL_COLOR,
            Style = SKPaintStyle.Fill,
        };

        public static readonly SKPaint SELECTED_STROKE_PAINT = new()
        {
            Color = SELECTED_STROKE_COLOR,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
        };

        public static readonly SKPaint FOCUS_FILL_PAINT = new()
        {
            Color = FOCUS_FILL_COLOR,
            Style = SKPaintStyle.Fill,
        };

        public static readonly SKPaint MOUSE_POINT_FILL_PAINT = new()
        {
            Color = MOUSE_POINT_FILL_COLOR,
            Style = SKPaintStyle.Fill,
        };

        public static readonly SKPaint RECTANGLE_SELECTION_FILL_PAINT = new()
        {
            Color = RECTANGLE_SELECTION_FILL_COLOR,
            Style = SKPaintStyle.Fill,
        };

        public static readonly SKPaint RECTANGLE_SELECTION_STROKE_PAINT = new()
        {
            Color = RECTANGLE_SELECTION_STROKE_COLOR,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
        };

        private static readonly Dictionary<float, SKPaint> SELECTED_STROKE_PAINT_CACHE = [];
        private static readonly Dictionary<float, SKPaint> RECTANGLE_SELECTION_STROKE_PAINT_CACHE = [];

        public static SKPaint GetSelectedStrokePaint(Control control)
        {
            var scale = WindowUtil.GetCurrentWindowScale(control);
            if (SELECTED_STROKE_PAINT_CACHE.TryGetValue(scale, out var paint))
            {
                return paint;
            }
            else
            {
                var newPaint = new SKPaint
                {
                    Color = SELECTED_STROKE_PAINT.Color,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = SELECTED_STROKE_PAINT.StrokeWidth,
                };
                SELECTED_STROKE_PAINT_CACHE.Add(scale, newPaint);
                return newPaint;
            }
        }

        public static SKPaint GetRectangleSelectionStrokePatint(Control control)
        {
            var scale = WindowUtil.GetCurrentWindowScale(control);
            if (RECTANGLE_SELECTION_STROKE_PAINT_CACHE.TryGetValue(scale, out var paint))
            {
                return paint;
            }
            else
            {
                var newPaint = new SKPaint
                {
                    Color = RECTANGLE_SELECTION_STROKE_PAINT.Color,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = RECTANGLE_SELECTION_STROKE_PAINT.StrokeWidth,
                };
                RECTANGLE_SELECTION_STROKE_PAINT_CACHE.Add(scale, newPaint);
                return newPaint;
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
        public event EventHandler<SKDrawItemsEventArgs> SKDrawItems;
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

        private void OnDrawItemChanged(DrawItemChangedEventArgs e)
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
