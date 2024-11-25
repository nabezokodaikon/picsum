using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using WinApi;

namespace SWF.Core.FileAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal static class ExplorerWindowDragDrop
    {
        public static string GetExplorerPathAtCursor(int x, int y)
        {
            try
            {
                var windowHandle = WinApiMembers.WindowFromPoint(new WinApiMembers.POINT(x, y));
                var rootWindow = WinApiMembers.GetAncestor(windowHandle, WinApiMembers.GA_ROOT);
                var className = new StringBuilder(256);
                WinApiMembers.GetClassName(rootWindow, className, className.Capacity);
                if (className.ToString() == "CabinetWClass")
                {
                    return GetShellFolderPath(rootWindow);
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                throw new FileUtilException(
                    "エクスプローラーのウィンドウからディレクトリパスを取得できませんでした。", ex);
            }
        }

        private static string GetShellFolderPath(IntPtr hwnd)
        {
            var shellWindowType = Type.GetTypeFromProgID("Shell.Application");
            if (shellWindowType == null)
            {
                return string.Empty;
            }

            dynamic shell = Activator.CreateInstance(shellWindowType);
            if (shell == null)
            {
                return string.Empty;
            }

            dynamic windows = shell.Windows();

            try
            {
                foreach (dynamic window in windows)
                {
                    try
                    {
                        if (window != null && window.HWND == (int)hwnd)
                        {
                            var path = window.Document.Folder.Self.Path;
                            Marshal.ReleaseComObject(window);
                            return path;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            finally
            {
                Marshal.ReleaseComObject(windows);
                Marshal.ReleaseComObject(shell);
            }

            return string.Empty;
        }
    }
}
