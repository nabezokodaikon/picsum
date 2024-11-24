using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WinApi
{
    public static class ExplorerDesktopDragDrop
    {
        public static string GetExplorerPathAtCursor(int x, int y)
        {
            var windowHandle = WinApiMembers.WindowFromPoint(new WinApiMembers.POINT(x, y));
            if (IsDesktopWindow(windowHandle))
            {
                return GetDesktopPath();
            }
            else
            {
                return string.Empty;
            }
        }

        private static bool IsDesktopWindow(IntPtr hwnd)
        {
            var className = new StringBuilder(256);
            WinApiMembers.GetClassName(hwnd, className, className.Capacity);
            return className.ToString() == "SysListView32";
        }

        private static string GetDesktopPath()
        {
            if (WinApiMembers.SHGetKnownFolderPath(WinApiMembers.FOLDERID_Desktop, 0, IntPtr.Zero, out var pszPath) == 0)
            {
                try
                {
                    return Marshal.PtrToStringUni(pszPath);
                }
                finally
                {
                    Marshal.FreeCoTaskMem(pszPath);
                }
            }
            else
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
        }
    }
}
