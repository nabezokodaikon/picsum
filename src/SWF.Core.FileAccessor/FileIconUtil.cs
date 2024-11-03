using System.Runtime.InteropServices;
using WinApi;

namespace SWF.Core.FileAccessor
{
    internal static class FileIconUtil
    {
        /// <summary>
        /// 小システムアイコンを取得します。
        /// </summary>
        /// <param name="spesialDirectory">システムアイコンの種類</param>
        /// <returns></returns>
        public static Bitmap GetSmallSystemIcon(WinApiMembers.ShellSpecialFolder specialFolder)
        {
            _ = WinApiMembers.SHGetSpecialFolderLocation(IntPtr.Zero, specialFolder, out var idHandle);
            var sh = new WinApiMembers.SHFILEINFOW();
            var hSuccess = WinApiMembers.SHGetFileInfoW(idHandle, 0, ref sh, (uint)Marshal.SizeOf(sh),
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_ICON |
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_PIDL |
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_SMALLICON);
            try
            {
                if (!hSuccess.Equals(IntPtr.Zero))
                {
                    using (var icon = Icon.FromHandle(sh.hIcon))
                    {
                        return icon.ToBitmap();
                    }
                }
                else
                {
                    throw new NullReferenceException("小システムアイコンを取得できませんでした。");
                }
            }
            finally
            {
                WinApiMembers.DestroyIcon(sh.hIcon);
            }
        }

        /// <summary>
        /// 大システムアイコンを取得します。
        /// </summary>
        /// <param name="spesialDirectory">システムアイコンの種類</param>
        /// <returns></returns>
        public static Bitmap GetLargeSystemIcon(WinApiMembers.ShellSpecialFolder specialFolder)
        {
            _ = WinApiMembers.SHGetSpecialFolderLocation(IntPtr.Zero, specialFolder, out var idHandle);
            var sh = new WinApiMembers.SHFILEINFOW();
            var hSuccess = WinApiMembers.SHGetFileInfoW(idHandle, 0, ref sh, (uint)Marshal.SizeOf(sh),
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_ICON |
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_PIDL |
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_LARGEICON);
            try
            {
                if (!hSuccess.Equals(IntPtr.Zero))
                {
                    using (var icon = Icon.FromHandle(sh.hIcon))
                    {
                        return icon.ToBitmap();
                    }
                }
                else
                {
                    throw new NullReferenceException("大システムアイコンを取得できませんでした。");
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
                if (!hSuccess.Equals(IntPtr.Zero))
                {
                    using (var icon = Icon.FromHandle(sh.hIcon))
                    {
                        return icon.ToBitmap();
                    }
                }
                else
                {
                    throw new NullReferenceException("SMALLICONアイコンを取得できませんでした。");
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
        public static Bitmap GetExtraLargeIconByFilePath(string filePath, WinApiMembers.SHIL shil)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var shinfo = new WinApiMembers.SHFILEINFO();
            var hSuccess = WinApiMembers.SHGetFileInfo(
                filePath,
                0,
                ref shinfo,
                (int)Marshal.SizeOf(typeof(WinApiMembers.SHFILEINFO)),
                (int)WinApiMembers.ShellFileInfoFlags.SHGFI_SYSICONINDEX);
            if (hSuccess.Equals(IntPtr.Zero))
            {
                throw new NullReferenceException("EXTRALARGEアイコンを取得できませんでした。");
            }

            var result = WinApiMembers.SHGetImageList(
                shil,
                WinApiMembers.IID_IImageList,
                out IntPtr pimgList);

            try
            {
                if (result != WinApiMembers.S_OK)
                {
                    throw new NullReferenceException("EXTRALARGEアイコンを取得できませんでした。");
                }

                var hicon = WinApiMembers.ImageList_GetIcon(pimgList, shinfo.iIcon, 0);
                if (hicon.Equals(IntPtr.Zero))
                {
                    throw new NullReferenceException("EXTRALARGEアイコンを取得できませんでした。");
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
