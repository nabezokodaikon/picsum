using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    [SupportedOSPlatform("windows")]
    public static class ScreenUtil
    {
        public static Rectangle GetTopRect(Size rectSize)
        {
            var screenRect = Screen.GetWorkingArea(Cursor.Position);
            var x = (int)(screenRect.Left + (screenRect.Width - rectSize.Width) / 2f);
            var y = screenRect.Top;
            return new Rectangle(x, y, rectSize.Width, rectSize.Height);
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
