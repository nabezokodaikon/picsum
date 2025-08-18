using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{

    public static class ScreenUtil
    {
        private const int BORDER_WIDTH = 48;

        public static Rectangle GetTopRect(Size rectSize)
        {
            var screenRect = Screen.GetWorkingArea(Cursor.Position);
            var x = (int)(screenRect.Left + (screenRect.Width - rectSize.Width) / 2f);
            var y = screenRect.Top;
            return new Rectangle(x, y, rectSize.Width, rectSize.Height);
        }

        public static Rectangle GetLeftBorderRect(float scale)
        {
            var screenRect = Screen.GetWorkingArea(Cursor.Position);
            var x = screenRect.Left;
            var y = screenRect.Top;
            return new Rectangle(x, y, (int)(BORDER_WIDTH * scale), screenRect.Height);
        }

        public static Rectangle GetRightBorderRect(float scale)
        {
            var screenRect = Screen.GetWorkingArea(Cursor.Position);
            var borderWidth = (int)(BORDER_WIDTH * scale);
            var x = screenRect.Right - borderWidth;
            var y = screenRect.Top;
            return new Rectangle(x, y, borderWidth, screenRect.Height);
        }

        public static Rectangle GetRightRect(Size rectSize)
        {
            var screenRect = Screen.GetWorkingArea(Cursor.Position);
            var x = screenRect.Right - rectSize.Width;
            var y = (int)(screenRect.Y + (screenRect.Height - rectSize.Height) / 2f);
            return new Rectangle(x, y, rectSize.Width, rectSize.Height);
        }

        public static Rectangle GetBottomRect(Size rectSize)
        {
            var screenRect = Screen.GetWorkingArea(Cursor.Position);
            var x = (int)(screenRect.X + (screenRect.Width - rectSize.Width) / 2f);
            var y = screenRect.Bottom - rectSize.Height;
            return new Rectangle(x, y, rectSize.Width, rectSize.Height);
        }
    }
}
