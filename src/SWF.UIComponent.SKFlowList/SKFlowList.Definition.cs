using SkiaSharp;
using SWF.Core.Base;
using SWF.UIComponent.FlowList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.SKFlowList
{
    /// <summary>
    /// フローリストコントロール
    /// </summary>
    public sealed partial class SKFlowList
    {
        public const int SCROLL_BAR_DEFAULT_WIDTH = 17;

        // 項目最小サイズ
        public const int MINIMUM_ITEM_SIZE = 16;

        private static readonly Size DRAG_SIZE = GetDragSize();

        private static Size GetDragSize()
        {
            var size = SystemInformation.DragSize.Width * 16;
            return new Size(size, size);
        }

        public readonly SKSamplingOptions TextSamplingOptions
            = new(SKFilterMode.Nearest);

        private static readonly SKColor DARK_BACKGROUND_COLOR = new(64, 68, 71);
        private static readonly SKColor DARK_SELECTED_FILL_COLOR = new(255, 255, 255, 64);
        private static readonly SKColor DARK_SELECTED_STROKE_COLOR = new(255, 255, 255, 64);
        private static readonly SKColor DARK_FOCUS_STROKE_COLOR = new(255, 255, 255, 64);
        private static readonly SKColor DARK_MOUSE_POINT_FILL_COLOR = new(255, 255, 255, 32);
        private static readonly SKColor DARK_RECTANGLE_SELECTION_FILL_COLOR = new(255, 255, 255, 64);
        private static readonly SKColor DARK_RECTANGLE_SELECTION_STROKE_COLOR = new(255, 255, 255, 64);

        private static readonly SKColor LIGHT_BACKGROUND_COLOR = new(255, 255, 255);

        private static readonly SKColor LIGHT_SELECTED_FILL_COLOR = new(
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B,
            (byte)(SystemColors.Highlight.A / 4));
        private static readonly SKColor LIGHT_SELECTED_STROKE_COLOR = new(
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B,
            (byte)(SystemColors.Highlight.A / 4));
        private static readonly SKColor LIGHT_FOCUS_FILL_COLOR = new(
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B,
            (byte)(SystemColors.Highlight.A / 8));
        private static readonly SKColor LIGHT_FOCUS_STROKE_COLOR = new(
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B,
            (byte)(SystemColors.Highlight.A / 8));
        private static readonly SKColor LIGHT_MOUSE_POINT_FILL_COLOR = new(
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B,
            (byte)(SystemColors.Highlight.A / 8));
        private static readonly SKColor LIGHT_RECTANGLE_SELECTION_FILL_COLOR = new(
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B,
            (byte)(SystemColors.Highlight.A / 4));
        private static readonly SKColor LIGHT_RECTANGLE_SELECTION_STROKE_COLOR = new(
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B,
            (byte)(SystemColors.Highlight.A / 4));

        public readonly SKPaint DarkBackgroundPaint = new()
        {
            BlendMode = SKBlendMode.Src,
            Color = DARK_BACKGROUND_COLOR,
            IsAntialias = false,
        };

        public readonly SKPaint DarkTextPaint = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = new(255, 255, 255, 255),
            IsAntialias = false,
        };

        public readonly SKPaint DarkSelectedFillPaint = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = DARK_SELECTED_FILL_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Fill,
        };

        public readonly SKPaint DarkMousePointFillPaint = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = DARK_MOUSE_POINT_FILL_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Fill,
        };

        private readonly SKPaint _darkSelectedStrokePaint = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = DARK_SELECTED_STROKE_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
        };

        private readonly SKPaint _darkRectangleSelectionFillPaint = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = DARK_RECTANGLE_SELECTION_FILL_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Fill,
        };

        private readonly SKPaint _darkRectangleSelectionStrokePaint = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = DARK_RECTANGLE_SELECTION_STROKE_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
        };

        private readonly SKPaint _darkFocusStrokePaint = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = DARK_FOCUS_STROKE_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
        };

        public readonly SKPaint LightBackgroundPaint = new()
        {
            BlendMode = SKBlendMode.Src,
            Color = LIGHT_BACKGROUND_COLOR,
            IsAntialias = false,
        };

        public readonly SKPaint LightTextPaint = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = new(0, 0, 0),
            IsAntialias = false,
        };

        public readonly SKPaint LightSelectedFillPaint = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = LIGHT_SELECTED_FILL_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Fill,
        };

        public readonly SKPaint LightMousePointFillPaint = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = LIGHT_MOUSE_POINT_FILL_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Fill,
        };

        private readonly SKPaint _lightSelectedStrokePaint = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = LIGHT_SELECTED_STROKE_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
        };

        private readonly SKPaint _lightRectangleSelectionFillPaint = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = LIGHT_RECTANGLE_SELECTION_FILL_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Fill,
        };

        private readonly SKPaint _lightRectangleSelectionStrokePaint = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = LIGHT_RECTANGLE_SELECTION_STROKE_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
        };

        private readonly SKPaint _lightFocusStrokePaint = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = LIGHT_FOCUS_FILL_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
        };

        private readonly Dictionary<float, SKPaint> _darkSelectedStrokePaintCache = [];
        private readonly Dictionary<float, SKPaint> _darkRectangleSelectionStrokePaintCache = [];
        private readonly Dictionary<float, SKPaint> _darkFocusStrokePaintCache = [];

        private readonly Dictionary<float, SKPaint> _lightSelectedStrokePaintCache = [];
        private readonly Dictionary<float, SKPaint> _lightRectangleSelectionStrokePaintCache = [];
        private readonly Dictionary<float, SKPaint> _lightFocusStrokePaintCache = [];

        public SKPaint GetDarkSelectedStrokePaint()
        {
            var scale = WindowUtil.GetCurrentWindowScale(this);
            if (this._darkSelectedStrokePaintCache.TryGetValue(scale, out var paint))
            {
                return paint;
            }
            else
            {
                var newPaint = new SKPaint
                {
                    BlendMode = this._darkSelectedStrokePaint.BlendMode,
                    Color = this._darkSelectedStrokePaint.Color,
                    IsAntialias = this._darkSelectedStrokePaint.IsAntialias,
                    StrokeWidth = this._darkSelectedStrokePaint.StrokeWidth * scale,
                    Style = this._darkSelectedStrokePaint.Style,
                };
                this._darkSelectedStrokePaintCache.Add(scale, newPaint);
                return newPaint;
            }
        }

        public SKPaint GetDarkRectangleSelectionStrokePatint()
        {
            var scale = WindowUtil.GetCurrentWindowScale(this);
            if (this._darkRectangleSelectionStrokePaintCache.TryGetValue(scale, out var paint))
            {
                return paint;
            }
            else
            {
                var newPaint = new SKPaint
                {
                    BlendMode = this._darkRectangleSelectionStrokePaint.BlendMode,
                    Color = this._darkRectangleSelectionStrokePaint.Color,
                    IsAntialias = this._darkRectangleSelectionStrokePaint.IsAntialias,
                    StrokeWidth = this._darkRectangleSelectionStrokePaint.StrokeWidth * scale,
                    Style = this._darkRectangleSelectionStrokePaint.Style,
                };
                this._darkRectangleSelectionStrokePaintCache.Add(scale, newPaint);
                return newPaint;
            }
        }

        public SKPaint GetDarkFocusStrokePaint()
        {
            var scale = WindowUtil.GetCurrentWindowScale(this);
            if (this._darkFocusStrokePaintCache.TryGetValue(scale, out var paint))
            {
                return paint;
            }
            else
            {
                var newPaint = new SKPaint
                {
                    BlendMode = this._darkFocusStrokePaint.BlendMode,
                    Color = this._darkFocusStrokePaint.Color,
                    IsAntialias = this._darkFocusStrokePaint.IsAntialias,
                    StrokeWidth = this._darkFocusStrokePaint.StrokeWidth * scale,
                    Style = this._darkFocusStrokePaint.Style,
                };
                this._darkFocusStrokePaintCache.Add(scale, newPaint);
                return newPaint;
            }
        }

        public SKPaint GetLightSelectedStrokePaint()
        {
            var scale = WindowUtil.GetCurrentWindowScale(this);
            if (this._lightSelectedStrokePaintCache.TryGetValue(scale, out var paint))
            {
                return paint;
            }
            else
            {
                var newPaint = new SKPaint
                {
                    BlendMode = this._lightSelectedStrokePaint.BlendMode,
                    Color = this._lightSelectedStrokePaint.Color,
                    IsAntialias = this._lightSelectedStrokePaint.IsAntialias,
                    StrokeWidth = this._lightSelectedStrokePaint.StrokeWidth * scale,
                    Style = this._lightSelectedStrokePaint.Style,
                };
                this._lightSelectedStrokePaintCache.Add(scale, newPaint);
                return newPaint;
            }
        }

        public SKPaint GetLightRectangleSelectionStrokePatint()
        {
            var scale = WindowUtil.GetCurrentWindowScale(this);
            if (this._lightRectangleSelectionStrokePaintCache.TryGetValue(scale, out var paint))
            {
                return paint;
            }
            else
            {
                var newPaint = new SKPaint
                {
                    BlendMode = this._lightRectangleSelectionStrokePaint.BlendMode,
                    Color = this._lightRectangleSelectionStrokePaint.Color,
                    IsAntialias = this._lightRectangleSelectionStrokePaint.IsAntialias,
                    StrokeWidth = this._lightRectangleSelectionStrokePaint.StrokeWidth * scale,
                    Style = this._lightRectangleSelectionStrokePaint.Style,
                };
                this._lightRectangleSelectionStrokePaintCache.Add(scale, newPaint);
                return newPaint;
            }
        }

        public SKPaint GetLightFocusStrokePaint()
        {
            var scale = WindowUtil.GetCurrentWindowScale(this);
            if (this._lightFocusStrokePaintCache.TryGetValue(scale, out var paint))
            {
                return paint;
            }
            else
            {
                var newPaint = new SKPaint
                {
                    BlendMode = this._lightFocusStrokePaint.BlendMode,
                    Color = this._lightFocusStrokePaint.Color,
                    IsAntialias = this._lightFocusStrokePaint.IsAntialias,
                    StrokeWidth = this._lightFocusStrokePaint.StrokeWidth * scale,
                    Style = this._lightFocusStrokePaint.Style,
                };
                this._lightFocusStrokePaintCache.Add(scale, newPaint);
                return newPaint;
            }
        }

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
