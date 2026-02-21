using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    public static class TabSwitchResources
    {
        public static readonly Color TITLE_COLOR = Color.FromArgb(0, 64, 64, 64);
        public static readonly TextFormatFlags TITLE_FORMAT_FLAGS
            = TextFormatFlags.Left |
              TextFormatFlags.VerticalCenter |
              TextFormatFlags.SingleLine |
              TextFormatFlags.EndEllipsis;

        public static readonly SolidBrush TAB_ADD_BUTTON_MOUSE_POINT_BRUSH = new(Color.FromArgb(128, 255, 255, 255));
        public static readonly SolidBrush TAB_ADD_BUTTON_NORMAL_BRUSH = new(Color.FromArgb(0, 0, 0, 0));
        public static readonly Pen TAB_ADD_BUTTON_MOUSE_POINT_PEN = new(Color.Black, 2f);
        public static readonly Pen TAB_ADD_BUTTON_NORMAL_PEN = new(Color.LightGray, 2f);

        public static readonly SolidBrush PAGE_OUTLINE_BRUSH = new(Color.FromArgb(128, 128, 128));
        public static readonly SolidBrush PAGE_INNER_BRUSH = new(Color.FromArgb(250, 250, 250));

        public static readonly SolidBrush TAB_CLOSE_BUTTON_ACTIVE_BRUSH
            = new(Color.FromArgb(64, 0, 0, 0));

        public static readonly SolidBrush TAB_CLOSE_BUTTON_INACTIVE_BRUSH
            = new(Color.FromArgb(64, 0, 0, 0));

        public static readonly Pen TAB_CLOSE_BUTTON_SLASH_PEN
            = new(Color.Black, 2f);

        public static readonly SolidBrush TAB_ACTIVE_BRUSH
            = new(Color.FromArgb(250, 250, 250));

        public static readonly SolidBrush TAB_INACTIVE_BRUSH
            = new(Color.FromArgb(200, 200, 200));

        public static readonly SolidBrush TAB_MOUSE_POINT_BRUSH
            = new(Color.FromArgb(220, 220, 220));

        public static readonly Pen TAB_OUTLINE_PEN
            = new(Color.FromArgb(32, 32, 32), 0.1f);

        public static void Dispose()
        {
            TAB_ADD_BUTTON_MOUSE_POINT_BRUSH.Dispose();
            TAB_ADD_BUTTON_NORMAL_BRUSH.Dispose();
            TAB_ADD_BUTTON_MOUSE_POINT_PEN.Dispose();
            TAB_ADD_BUTTON_NORMAL_PEN.Dispose();
            PAGE_OUTLINE_BRUSH.Dispose();
            PAGE_INNER_BRUSH.Dispose();
            TAB_CLOSE_BUTTON_ACTIVE_BRUSH.Dispose();
            TAB_CLOSE_BUTTON_INACTIVE_BRUSH.Dispose();
            TAB_CLOSE_BUTTON_SLASH_PEN.Dispose();
            TAB_ACTIVE_BRUSH.Dispose();
            TAB_INACTIVE_BRUSH.Dispose();
            TAB_MOUSE_POINT_BRUSH.Dispose();
            TAB_OUTLINE_PEN.Dispose();
        }
    }
}
