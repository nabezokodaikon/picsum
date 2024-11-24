using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Windows.Forms;

namespace WinApi
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class DragDropUtil
    {
        public static readonly Cursor DRAG_CURSOR = new(CreateDragImage().GetHicon());

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

                Console.WriteLine(className.ToString());

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

        public static string GetExplorerTreePathAtCursor(int x, int y)
        {
            var hwnd = WinApiMembers.WindowFromPoint(new WinApiMembers.POINT(x, y));
            var itemHandle = WinApiMembers.SendMessage(hwnd, WinApiMembers.TVM_GETNEXTITEM, new IntPtr(WinApiMembers.TVGN_CARET), IntPtr.Zero);
            if (itemHandle == IntPtr.Zero)
            {
                return string.Empty;
            }

            return GetTreeViewFullPath(hwnd, itemHandle);
        }

        private static string GetShellFolderPath(IntPtr hwnd)
        {
            try
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"COM操作エラー: {ex.Message}");
            }

            return string.Empty;
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

        private static string GetTreeViewFullPath(IntPtr hwnd, IntPtr hItem)
        {
            var fullPath = new StringBuilder();
            var currentItem = hItem;

            while (currentItem != IntPtr.Zero)
            {
                var itemText = GetSelectedItemPath(hwnd, currentItem);

                if (!string.IsNullOrEmpty(itemText))
                {
                    if (fullPath.Length > 0)
                    {
                        fullPath.Insert(0, "\\");
                    }

                    fullPath.Insert(0, itemText);
                }

                currentItem = (IntPtr)WinApiMembers.SendMessage(
                    hwnd, WinApiMembers.TVM_GETNEXTITEM, new IntPtr(3), currentItem);
            }

            return fullPath.ToString();
        }

        private static string GetSelectedItemPath(IntPtr hwnd, IntPtr hItem)
        {
            if (hItem == IntPtr.Zero)
            {
                return string.Empty;
            }

            try
            {
                WinApiMembers.GetWindowThreadProcessId(hwnd, out var processId);
                var process = Process.GetProcessById((int)processId);

                const int MAX_STR = 260;
                var tvitem = new WinApiMembers.TVITEM();
                var tvitemSize = Marshal.SizeOf(tvitem);
                var dwSize = tvitemSize + MAX_STR;

                var pLocalShared = Marshal.AllocHGlobal(tvitemSize);
                try
                {
                    var pLocalShared_Buf = Marshal.AllocCoTaskMem(MAX_STR);
                    try
                    {
                        var pSysShared = WinApiMembers.VirtualAllocEx(
                            process.Handle, IntPtr.Zero, (uint)tvitemSize,
                            WinApiMembers.MEM_COMMIT | WinApiMembers.MEM_RESERVE, WinApiMembers.PAGE_READWRITE);
                        try
                        {
                            var pSysShared_Buf = WinApiMembers.VirtualAllocEx(
                                process.Handle, IntPtr.Zero, (uint)MAX_STR,
                                WinApiMembers.MEM_COMMIT | WinApiMembers.MEM_RESERVE, WinApiMembers.PAGE_READWRITE);

                            try
                            {
                                tvitem.mask = WinApiMembers.TVIF_TEXT;
                                tvitem.hItem = hItem;
                                tvitem.pszText = pSysShared_Buf;
                                tvitem.cchTextMax = MAX_STR;

                                Marshal.StructureToPtr(tvitem, pLocalShared, false);

                                uint retWrite = 0;
                                WinApiMembers.WriteProcessMemory(process.Handle, pSysShared, pLocalShared, tvitemSize, ref retWrite);

                                WinApiMembers.SendMessage(hwnd, WinApiMembers.TVM_GETITEM, IntPtr.Zero, pSysShared);

                                uint retRead = 0;
                                WinApiMembers.ReadProcessMemory(process.Handle, pSysShared_Buf, pLocalShared_Buf, MAX_STR, ref retRead);

                                return Marshal.PtrToStringAnsi(pLocalShared_Buf);
                            }
                            finally
                            {
                                WinApiMembers.VirtualFreeEx(process.Handle, pSysShared_Buf, (uint)dwSize, WinApiMembers.MEM_RELEASE);
                            }
                        }
                        finally
                        {
                            WinApiMembers.VirtualFreeEx(process.Handle, pSysShared, (uint)dwSize, WinApiMembers.MEM_RELEASE);
                        }
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(pLocalShared_Buf);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(pLocalShared);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting item path: {ex.Message}");
                return string.Empty;
            }
        }

        private static Bitmap CreateDragImage()
        {
            const int w = 64;
            const int h = 64;
            var bmp = new Bitmap(w, h);
            using (var g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(Brushes.White, 0, 0, w, h);
            }
            return bmp;
        }
    }
}
