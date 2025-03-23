using SWF.Core.Base;
using System.Runtime.InteropServices;
using WinApi;

namespace SWF.Core.FileAccessor
{
    internal static class FileIconUtil
    {
        public static Bitmap GetSystemIcon(WinApiMembers.ShellSpecialFolder specialFolder, WinApiMembers.ShellFileInfoFlags shil)
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
                    using (var icon = Icon.FromHandle(sh.hIcon))
                    {
                        return icon.ToBitmap();
                    }
                }
                else
                {
                    throw new NullReferenceException("システムアイコンを取得できませんでした。");
                }
            }
            finally
            {
                WinApiMembers.DestroyIcon(sh.hIcon);
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

            var sh = new WinApiMembers.SHFILEINFOW();
            var hSuccess = WinApiMembers.SHGetFileInfoW(filePath, 0, ref sh, (uint)Marshal.SizeOf(sh),
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_ICON |
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_SMALLICON);
            try
            {
                if (hSuccess != IntPtr.Zero)
                {
                    using (var icon = Icon.FromHandle(sh.hIcon))
                    {
                        return icon.ToBitmap();
                    }
                }
                else
                {
                    return ResourceFiles.ExtraLargeEmptyIcon.Value;
                }
            }
            finally
            {
                WinApiMembers.DestroyIcon(sh.hIcon);
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

            var shinfo = new WinApiMembers.SHFILEINFO();
            var hSuccess = WinApiMembers.SHGetFileInfo(
                filePath,
                0,
                ref shinfo,
                (int)Marshal.SizeOf(typeof(WinApiMembers.SHFILEINFO)),
                (int)WinApiMembers.ShellFileInfoFlags.SHGFI_SYSICONINDEX);
            if (hSuccess == IntPtr.Zero)
            {
                return ResourceFiles.ExtraLargeEmptyIcon.Value;
            }

            var result = WinApiMembers.SHGetImageList(
                shil,
                WinApiMembers.IID_IImageList,
                out IntPtr pimgList);

            try
            {
                if (result != WinApiMembers.S_OK)
                {
                    return ResourceFiles.ExtraLargeEmptyIcon.Value;
                }

                var hicon = WinApiMembers.ImageList_GetIcon(pimgList, shinfo.iIcon, 0);
                if (hicon == IntPtr.Zero)
                {
                    return ResourceFiles.ExtraLargeEmptyIcon.Value;
                }

                try
                {
                    using (var icon = Icon.FromHandle(hicon))
                    {
                        return icon.ToBitmap();
                    }
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
        }
    }
}
