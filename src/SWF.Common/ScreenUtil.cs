using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    public static class ScreenUtil
    {
        public static Rectangle GetTopRect(Size rectSize)
        {
            Rectangle screenRect = Screen.GetWorkingArea(Cursor.Position);
            int x = (int)(screenRect.Left + (screenRect.Width - rectSize.Width) / 2f);
            int y = screenRect.Top;
            return new Rectangle(x, y, rectSize.Width, rectSize.Height);
        }

        public static Rectangle GetLeftRect(Size rectSize)
        {
            Rectangle screenRect = Screen.GetWorkingArea(Cursor.Position);
            int x = screenRect.Left;
            int y = (int)(screenRect.Y + (screenRect.Height - rectSize.Height) / 2f);
            return new Rectangle(x, y, rectSize.Width, rectSize.Height);
        }

        public static Rectangle GetRightRect(Size rectSize)
        {
            Rectangle screenRect = Screen.GetWorkingArea(Cursor.Position);
            int x = screenRect.Right - rectSize.Width;
            int y = (int)(screenRect.Y + (screenRect.Height - rectSize.Height) / 2f);
            return new Rectangle(x, y, rectSize.Width, rectSize.Height);
        }

        public static Rectangle GetBottomRect(Size rectSize)
        {
            Rectangle screenRect = Screen.GetWorkingArea(Cursor.Position);
            int x = (int)(screenRect.X + (screenRect.Width - rectSize.Width) / 2f);
            int y = screenRect.Bottom - rectSize.Height;
            return new Rectangle(x, y, rectSize.Width, rectSize.Height);
        }
    }
}
