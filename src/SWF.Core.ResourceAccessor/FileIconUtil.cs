using System.Runtime.InteropServices;
using WinApi;

namespace SWF.Core.ResourceAccessor
{

    internal static class FileIconUtil
    {
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

                Task.WaitAll(Task.Delay(500));
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

                Task.WaitAll(Task.Delay(500));
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

                Task.WaitAll(Task.Delay(500));
            }
        }
    }
}
