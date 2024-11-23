using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace WinApi
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class DragDropUtil
    {
        public static string GetExplorerPathAtCursor(int x, int y)
        {
            try
            {
                // カーソル位置のウィンドウハンドルを取得
                var windowHandle = WinApiMembers.WindowFromPoint(new WinApiMembers.POINT(x, y));
                if (IsDesktopWindow(windowHandle))
                {
                    return GetDesktopPath();
                }

                // ルートウィンドウを取得
                var rootWindow = WinApiMembers.GetAncestor(windowHandle, WinApiMembers.GA_ROOT);

                // ウィンドウクラス名を確認
                var className = new StringBuilder(256);
                WinApiMembers.GetClassName(rootWindow, className, className.Capacity);

                if (className.ToString() == "CabinetWClass") // エクスプローラーウィンドウの場合
                {
                    // Shell APIを使用してパスを取得
                    return GetShellFolderPath(rootWindow);
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"エラー: {ex.Message}");
                return string.Empty;
            }
        }

        private static string GetShellFolderPath(IntPtr hwnd)
        {
            try
            {
                // ShellWindowsオブジェクトを取得
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
                    // エクスプローラーウィンドウを検索
                    foreach (dynamic window in windows)
                    {
                        try
                        {
                            if (window != null && window.HWND == (int)hwnd)
                            {
                                // 現在のパスを取得
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"COM操作エラー: {ex.Message}");
            }

            return string.Empty;
        }

        public static bool IsDesktopWindow(IntPtr hwnd)
        {
            var className = new StringBuilder(256);
            WinApiMembers.GetClassName(hwnd, className, className.Capacity);
            //return className.ToString() == "Progman" || className.ToString() == "WorkerW";
            return className.ToString() == "SysListView32";
        }

        public static string GetDesktopPath()
        {
            IntPtr pszPath;
            if (WinApiMembers.SHGetKnownFolderPath(WinApiMembers.FOLDERID_Desktop, 0, IntPtr.Zero, out pszPath) == 0)
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
