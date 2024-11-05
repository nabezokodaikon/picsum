using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class ScreenUtil
    {
        public static Rectangle GetTopRect(Size rectSize)
        {
            var screenRect = Screen.GetWorkingArea(Cursor.Position);
            var x = (int)(screenRect.Left + (screenRect.Width - rectSize.Width) / 2f);
            var y = screenRect.Top;
            return new Rectangle(x, y, rectSize.Width, rectSize.Height);
        }

        public static Rectangle GetLeftBorderRect()
        {
            var screenRect = Screen.GetWorkingArea(Cursor.Position);
            var x = screenRect.Left;
            var y = screenRect.Top;
            return new Rectangle(x, y, 48, screenRect.Height);
        }

        public static Rectangle GetRightBorderRect()
        {
            var screenRect = Screen.GetWorkingArea(Cursor.Position);
            var x = screenRect.Right - 48;
            var y = screenRect.Top;
            return new Rectangle(x, y, 48, screenRect.Height);
        }

        public static Rectangle GetLeftRect(Size rectSize)
        {
            var screenRect = Screen.GetWorkingArea(Cursor.Position);
            var x = screenRect.Left;
            var y = (int)(screenRect.Y + (screenRect.Height - rectSize.Height) / 2f);
            return new Rectangle(x, y, rectSize.Width, rectSize.Height);
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
