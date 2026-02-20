using SkiaSharp;
using SWF.Core.Base;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.SKFlowList
{
    public static class SKFlowListResouces
    {
        public static readonly SKSamplingOptions TextSamplingOptions
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

        public static readonly SKPaint DARK_BACKGROUND_PAINT = new()
        {
            BlendMode = SKBlendMode.Src,
            Color = DARK_BACKGROUND_COLOR,
            IsAntialias = false,
        };

        public static readonly SKPaint DARK_TEXT_PAINT = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = new(255, 255, 255, 255),
            IsAntialias = false,
        };

        public static readonly SKPaint DARK_SELECTED_FILL_PAINT = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = DARK_SELECTED_FILL_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Fill,
        };

        public static readonly SKPaint DARK_MOUSE_POINT_FILL_PAINT = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = DARK_MOUSE_POINT_FILL_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Fill,
        };

        public static readonly SKPaint DARK_SELECTED_STROKE_PAINT = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = DARK_SELECTED_STROKE_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
        };

        public static readonly SKPaint DARK_RECTANGLE_SELECTION_FILL_PAINT = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = DARK_RECTANGLE_SELECTION_FILL_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Fill,
        };

        private static readonly SKPaint DARK_RECTANGLE_SELECTION_STROKE_PAINT = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = DARK_RECTANGLE_SELECTION_STROKE_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
        };

        private static readonly SKPaint DARK_FOCUS_STROKE_PAINT = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = DARK_FOCUS_STROKE_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
        };

        public static readonly SKPaint LIGHT_BACKGROUND_PAINT = new()
        {
            BlendMode = SKBlendMode.Src,
            Color = LIGHT_BACKGROUND_COLOR,
            IsAntialias = false,
        };

        public static readonly SKPaint LIGHT_TEXT_PAINT = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = new(0, 0, 0),
            IsAntialias = false,
        };

        public static readonly SKPaint LIGHT_SELECTED_FILL_PAINT = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = LIGHT_SELECTED_FILL_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Fill,
        };

        public static readonly SKPaint LIGHT_MOUSE_POINT_FILL_PAINT = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = LIGHT_MOUSE_POINT_FILL_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Fill,
        };

        private static readonly SKPaint LIGHT_SELECTED_STROKE_PAINT = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = LIGHT_SELECTED_STROKE_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
        };

        public static readonly SKPaint LIGHT_RECTANGLE_SELECTION_FILL_PAINT = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = LIGHT_RECTANGLE_SELECTION_FILL_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Fill,
        };

        public static readonly SKPaint LIGHT_RECTANGLE_SELECTION_STROKE_PAINT = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = LIGHT_RECTANGLE_SELECTION_STROKE_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
        };

        public static readonly SKPaint LIGHT_FOCUS_STROKE_PAINT = new()
        {
            BlendMode = SKBlendMode.SrcOver,
            Color = LIGHT_FOCUS_STROKE_COLOR,
            IsAntialias = false,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
        };

        private static readonly Dictionary<float, SKPaint> _darkSelectedStrokePaintCache = [];
        private static readonly Dictionary<float, SKPaint> _darkRectangleSelectionStrokePaintCache = [];
        private static readonly Dictionary<float, SKPaint> _darkFocusStrokePaintCache = [];

        private static readonly Dictionary<float, SKPaint> _lightSelectedStrokePaintCache = [];
        private static readonly Dictionary<float, SKPaint> _lightRectangleSelectionStrokePaintCache = [];
        private static readonly Dictionary<float, SKPaint> _lightFocusStrokePaintCache = [];

        public static SKPaint GetDarkSelectedStrokePaint(Control control)
        {
            var scale = WindowUtil.GetCurrentWindowScale(control);
            if (_darkSelectedStrokePaintCache.TryGetValue(scale, out var paint))
            {
                return paint;
            }
            else
            {
                var newPaint = new SKPaint
                {
                    BlendMode = DARK_SELECTED_STROKE_PAINT.BlendMode,
                    Color = DARK_SELECTED_STROKE_PAINT.Color,
                    IsAntialias = DARK_SELECTED_STROKE_PAINT.IsAntialias,
                    StrokeWidth = DARK_SELECTED_STROKE_PAINT.StrokeWidth * scale,
                    Style = DARK_SELECTED_STROKE_PAINT.Style,
                };
                _darkSelectedStrokePaintCache.Add(scale, newPaint);
                return newPaint;
            }
        }

        public static SKPaint GetDarkRectangleSelectionStrokePatint(Control control)
        {
            var scale = WindowUtil.GetCurrentWindowScale(control);
            if (_darkRectangleSelectionStrokePaintCache.TryGetValue(scale, out var paint))
            {
                return paint;
            }
            else
            {
                var newPaint = new SKPaint
                {
                    BlendMode = DARK_RECTANGLE_SELECTION_STROKE_PAINT.BlendMode,
                    Color = DARK_RECTANGLE_SELECTION_STROKE_PAINT.Color,
                    IsAntialias = DARK_RECTANGLE_SELECTION_STROKE_PAINT.IsAntialias,
                    StrokeWidth = DARK_RECTANGLE_SELECTION_STROKE_PAINT.StrokeWidth * scale,
                    Style = DARK_RECTANGLE_SELECTION_STROKE_PAINT.Style,
                };
                _darkRectangleSelectionStrokePaintCache.Add(scale, newPaint);
                return newPaint;
            }
        }

        public static SKPaint GetDarkFocusStrokePaint(Control control)
        {
            var scale = WindowUtil.GetCurrentWindowScale(control);
            if (_darkFocusStrokePaintCache.TryGetValue(scale, out var paint))
            {
                return paint;
            }
            else
            {
                var newPaint = new SKPaint
                {
                    BlendMode = DARK_FOCUS_STROKE_PAINT.BlendMode,
                    Color = DARK_FOCUS_STROKE_PAINT.Color,
                    IsAntialias = DARK_FOCUS_STROKE_PAINT.IsAntialias,
                    StrokeWidth = DARK_FOCUS_STROKE_PAINT.StrokeWidth * scale,
                    Style = DARK_FOCUS_STROKE_PAINT.Style,
                };
                _darkFocusStrokePaintCache.Add(scale, newPaint);
                return newPaint;
            }
        }

        public static SKPaint GetLightSelectedStrokePaint(Control control)
        {
            var scale = WindowUtil.GetCurrentWindowScale(control);
            if (_lightSelectedStrokePaintCache.TryGetValue(scale, out var paint))
            {
                return paint;
            }
            else
            {
                var newPaint = new SKPaint
                {
                    BlendMode = LIGHT_SELECTED_STROKE_PAINT.BlendMode,
                    Color = LIGHT_SELECTED_STROKE_PAINT.Color,
                    IsAntialias = LIGHT_SELECTED_STROKE_PAINT.IsAntialias,
                    StrokeWidth = LIGHT_SELECTED_STROKE_PAINT.StrokeWidth * scale,
                    Style = LIGHT_SELECTED_STROKE_PAINT.Style,
                };
                _lightSelectedStrokePaintCache.Add(scale, newPaint);
                return newPaint;
            }
        }

        public static SKPaint GetLightRectangleSelectionStrokePatint(Control control)
        {
            var scale = WindowUtil.GetCurrentWindowScale(control);
            if (_lightRectangleSelectionStrokePaintCache.TryGetValue(scale, out var paint))
            {
                return paint;
            }
            else
            {
                var newPaint = new SKPaint
                {
                    BlendMode = LIGHT_RECTANGLE_SELECTION_STROKE_PAINT.BlendMode,
                    Color = LIGHT_RECTANGLE_SELECTION_STROKE_PAINT.Color,
                    IsAntialias = LIGHT_RECTANGLE_SELECTION_STROKE_PAINT.IsAntialias,
                    StrokeWidth = LIGHT_RECTANGLE_SELECTION_STROKE_PAINT.StrokeWidth * scale,
                    Style = LIGHT_RECTANGLE_SELECTION_STROKE_PAINT.Style,
                };
                _lightRectangleSelectionStrokePaintCache.Add(scale, newPaint);
                return newPaint;
            }
        }

        public static SKPaint GetLightFocusStrokePaint(Control control)
        {
            var scale = WindowUtil.GetCurrentWindowScale(control);
            if (_lightFocusStrokePaintCache.TryGetValue(scale, out var paint))
            {
                return paint;
            }
            else
            {
                var newPaint = new SKPaint
                {
                    BlendMode = LIGHT_FOCUS_STROKE_PAINT.BlendMode,
                    Color = LIGHT_FOCUS_STROKE_PAINT.Color,
                    IsAntialias = LIGHT_FOCUS_STROKE_PAINT.IsAntialias,
                    StrokeWidth = LIGHT_FOCUS_STROKE_PAINT.StrokeWidth * scale,
                    Style = LIGHT_FOCUS_STROKE_PAINT.Style,
                };
                _lightFocusStrokePaintCache.Add(scale, newPaint);
                return newPaint;
            }
        }
    }
}
