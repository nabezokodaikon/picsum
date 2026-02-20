using System.Runtime.InteropServices;
using WinApi;

namespace SWF.Core.ResourceAccessor
{
    internal static class FileIconUtil
    {
        public static Bitmap GetMyComputerJumboIcon(bool isJumbo)
        {
            while (true)
            {
                var pidl = IntPtr.Zero;
                try
                {
                    // 1. マイコンピュータのPIDLを取得
                    if (WinApiMembers.SHGetSpecialFolderLocation(IntPtr.Zero, WinApiMembers.CSIDL_DRIVES, out pidl) != 0)
                    {
                        Task.Delay(500).GetAwaiter().GetResult();
                        continue;
                    }

                    // 2. PIDLからシステムイメージリスト内でのインデックスを取得
                    var shfi = new WinApiMembers.SHFILEINFO();
                    var flags = WinApiMembers.SHGFI_PIDL | WinApiMembers.SHGFI_SYSICONINDEX;
                    WinApiMembers.SHGetFileInfo(pidl, 0, ref shfi, (int)Marshal.SizeOf(shfi), flags);

                    // 3. IImageListを取得
                    var iidImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
                    if (WinApiMembers.SHGetImageList(isJumbo ? WinApiMembers.SHIL_JUMBO : WinApiMembers.SHIL_EXTRALARGE, ref iidImageList, out var iml) == 0)
                    {
                        var hIcon = IntPtr.Zero;
                        // 4. インデックスを指定してアイコンを抽出 (1 = ILD_TRANSPARENT)
                        var hr = iml.GetIcon(shfi.iIcon, 1, out hIcon);

                        if (hr == 0 && hIcon != IntPtr.Zero)
                        {
                            using (var icon = Icon.FromHandle(hIcon))
                            {
                                // 2. ToBitmap() を使用（これで多くの場合、透過が維持されます）
                                var bmp = icon.ToBitmap();

                                // 元のアイコンハンドルを解放
                                WinApiMembers.DestroyIcon(hIcon);

                                return bmp;
                            }
                        }
                        else
                        {
                            Task.Delay(500).GetAwaiter().GetResult();
                            continue;
                        }
                    }
                    else
                    {
                        Task.Delay(500).GetAwaiter().GetResult();
                        continue;
                    }
                }
                finally
                {
                    if (pidl != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(pidl);
                    }
                }
            }
        }

        public static Bitmap GetSystemIcon(WinApiMembers.ShellSpecialFolder specialFolder, WinApiMembers.ShellFileInfoFlags shil)
        {
            while (true)
            {
                _ = WinApiMembers.SHGetSpecialFolderLocation(IntPtr.Zero, specialFolder, out var idHandle);
                var sh = new WinApiMembers.SHFILEINFOW();
                var hSuccess = WinApiMembers.SHGetFileInfoW(idHandle, 0, ref sh, (uint)Marshal.SizeOf(sh),
                                                               WinApiMembers.ShellFileInfoFlags.SHGFI_ICON |
                                                               WinApiMembers.ShellFileInfoFlags.SHGFI_PIDL |
                                                               shil);
                try
                {
                    if (hSuccess != IntPtr.Zero)
                    {
#pragma warning disable CA1031
                        try
                        {
                            using (var icon = Icon.FromHandle(sh.hIcon))
                            {
                                return icon.ToBitmap();
                            }
                        }
                        catch (Exception) { }
                    }
#pragma warning restore CA1031
                }
                finally
                {
                    WinApiMembers.DestroyIcon(sh.hIcon);
                }

                Task.Delay(500).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// ファイルパスを指定して小アイコンを取得します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns></returns>
        public static Bitmap GetSmallIconByFilePath(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            while (true)
            {
                var sh = new WinApiMembers.SHFILEINFOW();
                var hSuccess = WinApiMembers.SHGetFileInfoW(filePath, 0, ref sh, (uint)Marshal.SizeOf(sh),
                                                               WinApiMembers.ShellFileInfoFlags.SHGFI_ICON |
                                                               WinApiMembers.ShellFileInfoFlags.SHGFI_SMALLICON);
                try
                {
                    if (hSuccess != IntPtr.Zero)
                    {
#pragma warning disable CA1031
                        try
                        {
                            using (var icon = Icon.FromHandle(sh.hIcon))
                            {
                                return icon.ToBitmap();
                            }
                        }
                        catch (Exception) { }
#pragma warning restore CA1031
                    }
                }
                finally
                {
                    WinApiMembers.DestroyIcon(sh.hIcon);
                }

                Task.Delay(500).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// ファイルパスを指定してEXTRALARGEアイコンを取得します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns></returns>
        public static Bitmap GetLargeIconByFilePath(string filePath, WinApiMembers.SHIL shil)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            while (true)
            {
                var shinfo = new WinApiMembers.SHFILEINFO();
                var hSuccess = WinApiMembers.SHGetFileInfo(
                    filePath,
                    0,
                    ref shinfo,
                    (int)Marshal.SizeOf<WinApiMembers.SHFILEINFO>(),
                    (int)WinApiMembers.ShellFileInfoFlags.SHGFI_SYSICONINDEX);
                if (hSuccess == IntPtr.Zero)
                {
                    return ResourceFiles.EmptyIcon.Value;
                }

                var result = WinApiMembers.SHGetImageList(
                    shil,
                    WinApiMembers.IID_IImageList,
                    out IntPtr pimgList);

                try
                {
                    if (result != WinApiMembers.S_OK)
                    {
                        return ResourceFiles.EmptyIcon.Value;
                    }

                    var hicon = WinApiMembers.ImageList_GetIcon(pimgList, shinfo.iIcon, 0);
                    if (hicon == IntPtr.Zero)
                    {
                        return ResourceFiles.EmptyIcon.Value;
                    }

                    try
                    {
#pragma warning disable CA1031
                        try
                        {
                            using (var icon = Icon.FromHandle(hicon))
                            {
                                return icon.ToBitmap();
                            }
                        }
                        catch (Exception) { }
#pragma warning restore CA1031
                    }
                    finally
                    {
                        WinApiMembers.DestroyIcon(hicon);
                    }
                }
                finally
                {
                    WinApiMembers.ImageList_Destroy(pimgList);
                }

                Task.Delay(500).GetAwaiter().GetResult();
            }
        }
    }
}
