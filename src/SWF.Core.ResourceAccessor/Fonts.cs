using System.Runtime.Versioning;

namespace SWF.Core.ResourceAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class Fonts
    {
        public static readonly Font UI_FONT_09 = new("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Pixel);
        public static readonly Font UI_FONT_10 = new("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Pixel);
        public static readonly Font UI_FONT_12 = new("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Pixel);
        public static readonly Font UI_FONT_14_REGULAR = new("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
        public static readonly Font UI_FONT_14_BOLD = new("Yu Gothic UI", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
        public static readonly Font UI_FONT_18 = new("Yu Gothic UI", 18F, FontStyle.Regular, GraphicsUnit.Pixel);
        public static readonly Font UI_FONT_22 = new("Yu Gothic UI", 22F, FontStyle.Regular, GraphicsUnit.Pixel);
    }
}
