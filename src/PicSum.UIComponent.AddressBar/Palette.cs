using System.Drawing;
using System.Runtime.Versioning;

namespace PicSum.UIComponent.AddressBar
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal static class Palette
    {
        private static readonly Color TEXT_COLOR = Color.FromArgb(
            SystemColors.ControlText.A,
            SystemColors.ControlText.R,
            SystemColors.ControlText.G,
            SystemColors.ControlText.B);

        private static readonly Color MOUSE_POINT_COLOR = Color.FromArgb(
            SystemColors.Highlight.A / 8,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B);

        private static readonly Color SELECTED_COLOR = Color.FromArgb(
            SystemColors.Highlight.A / 8,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B);

        public static readonly Color OUTLINE_COLOR = SystemColors.ControlDark;
        public static readonly Color INNER_COLOR = Color.White;

        public static readonly Font TEXT_FONT = new("Yu Gothic UI", 10F);
        public static readonly SolidBrush TEXT_BRUSH = new(TEXT_COLOR);
        public static readonly SolidBrush MOUSE_POINT_BRUSH = new(MOUSE_POINT_COLOR);
        public static readonly Pen MOUSE_POINT_PEN = new(MOUSE_POINT_COLOR);
        public static readonly SolidBrush MOUSE_DOWN_BRUSH = new(SELECTED_COLOR);
        public static readonly SolidBrush OUT_LINE_BRUSH = new(OUTLINE_COLOR);
        public static readonly SolidBrush INNER_BRUSH = new(INNER_COLOR);
    }
}
