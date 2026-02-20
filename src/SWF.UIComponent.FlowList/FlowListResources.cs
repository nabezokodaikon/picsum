using SWF.Core.Base;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.FlowList
{
    public static class FlowListResources
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
            LIGHT_SELECTED_ITEM_COLOR.A,
            LIGHT_SELECTED_ITEM_COLOR.R,
            LIGHT_SELECTED_ITEM_COLOR.G,
            LIGHT_SELECTED_ITEM_COLOR.B),
            2f);

        private static readonly Pen LIGHT_FOUCUS_ITEM_PEN = new(Color.FromArgb(
            LIGHT_FOCUS_ITEM_COLOR.A,
            LIGHT_FOCUS_ITEM_COLOR.R,
            LIGHT_FOCUS_ITEM_COLOR.G,
            LIGHT_FOCUS_ITEM_COLOR.B),
            2f);

        private static readonly Pen LIGHT_RECTANGLE_SELECTION_PEN = new(Color.FromArgb(
            LIGHT_RECTANGLE_SELECTION_COLOR.A,
            LIGHT_RECTANGLE_SELECTION_COLOR.R,
            LIGHT_RECTANGLE_SELECTION_COLOR.G,
            LIGHT_RECTANGLE_SELECTION_COLOR.B),
            2f);

        private static readonly Pen DARK_SELECTED_ITEM_PEN = new(Color.FromArgb(
            DARK_SELECTED_ITEM_COLOR.A,
            DARK_SELECTED_ITEM_COLOR.R,
            DARK_SELECTED_ITEM_COLOR.G,
            DARK_SELECTED_ITEM_COLOR.B),
            2f);

        private static readonly Pen DARK_FOUCUS_ITEM_PEN = new(Color.FromArgb(
            DARK_FOCUS_ITEM_COLOR.A,
            DARK_FOCUS_ITEM_COLOR.R,
            DARK_FOCUS_ITEM_COLOR.G,
            DARK_FOCUS_ITEM_COLOR.B),
            2f);

        private static readonly Pen DARK_RECTANGLE_SELECTION_PEN = new(Color.FromArgb(
            DARK_RECTANGLE_SELECTION_COLOR.A,
            DARK_RECTANGLE_SELECTION_COLOR.R,
            DARK_RECTANGLE_SELECTION_COLOR.G,
            DARK_RECTANGLE_SELECTION_COLOR.B),
            2f);

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

        public static Pen GetLightRectangleSelectionPen(Control control)
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

        public static Pen GetDarkRectangleSelectionPen(Control control)
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

        public static void Dispose()
        {
            LIGHT_ITEM_TEXT_BRUSH.Dispose();
            LIGHT_SELECTED_ITEM_BRUSH.Dispose();
            LIGHT_FOUCUS_ITEM_BRUSH.Dispose();
            LIGHT_MOUSE_POINT_ITEM_BRUSH.Dispose();
            LIGHT_RECTANGLE_SELECTION_BRUSH.Dispose();

            DARK_ITEM_TEXT_BRUSH.Dispose();
            DARK_SELECTED_ITEM_BRUSH.Dispose();
            DARK_FOUCUS_ITEM_BRUSH.Dispose();
            DARK_MOUSE_POINT_ITEM_BRUSH.Dispose();
            DARK_RECTANGLE_SELECTION_BRUSH.Dispose();

            LIGHT_SELECTED_ITEM_PEN.Dispose();
            LIGHT_FOUCUS_ITEM_PEN.Dispose();
            LIGHT_RECTANGLE_SELECTION_PEN.Dispose();

            DARK_SELECTED_ITEM_PEN.Dispose();
            DARK_FOUCUS_ITEM_PEN.Dispose();
            DARK_RECTANGLE_SELECTION_PEN.Dispose();

            foreach (var item in LIGHT_SELECTED_ITEM_PEN_CACHE)
            {
                item.Value.Dispose();
            }

            foreach (var item in LIGHT_FOUCUS_ITEM_PEN_CACHE)
            {
                item.Value.Dispose();
            }

            foreach (var item in LIGHT_RECTANGLE_SELECTION_PEN_CACHE)
            {
                item.Value.Dispose();
            }

            foreach (var item in DARK_SELECTED_ITEM_PEN_CACHE)
            {
                item.Value.Dispose();
            }

            foreach (var item in DARK_FOUCUS_ITEM_PEN_CACHE)
            {
                item.Value.Dispose();
            }

            foreach (var item in DARK_RECTANGLE_SELECTION_PEN_CACHE)
            {
                item.Value.Dispose();
            }

            LIGHT_SELECTED_ITEM_PEN_CACHE.Clear();
            LIGHT_FOUCUS_ITEM_PEN_CACHE.Clear();
            LIGHT_RECTANGLE_SELECTION_PEN_CACHE.Clear();

            DARK_SELECTED_ITEM_PEN_CACHE.Clear();
            DARK_FOUCUS_ITEM_PEN_CACHE.Clear();
            DARK_RECTANGLE_SELECTION_PEN_CACHE.Clear();
        }
    }
}
