using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WinApi;
using static WinApi.WinApiMembers;

namespace SWF.Common
{
    public static class FileUtil
    {
        #region ファイル確認メソッド

        /// <summary>
        /// ファイル、フォルダの存在を確認します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>ファイルまたはフォルダが存在するならTrue。存在しなければFalse。</returns>
        public static bool IsExists(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            if (string.IsNullOrEmpty(filePath))
            {
                return true;
            }
            else
            {
                return WinApiMembers.PathFileExists(filePath) == 1;
            }
        }

        /// <summary>
        /// ファイルパスがドライブであるか確認します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>ファイルパスがドライブならTrue。ドライブでなければFalse。</returns>
        public static bool IsDrive(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }
            else
            {
                return (Path.GetDirectoryName(filePath) == null);
            }
        }

        /// <summary>
        /// ファイルパスがフォルダであるか確認します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>ファイルパスがフォルダならTrue。フォルダでなければFalse。</returns>
        public static bool IsDirectory(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            if (string.IsNullOrEmpty(filePath))
            {
                return true;
            }
            else
            {
                return (Directory.Exists(filePath));
            }
        }

        /// <summary>
        /// ファイルパスがファイルであるか確認します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>ファイルパスがファイルならTrue。ファイルでなければFalse。</returns>
        public static bool IsFile(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            return File.Exists(filePath);
        }

        /// <summary>
        /// 指定したディレクトリ内に、ファイルが存在するか確認します。
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool HasFile(string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            return Directory.EnumerateFiles(directoryPath).Any();
        }

        /// <summary>
        /// 指定したディレクトリ内に、画像ファイルが存在するか確認します。
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool HasImageFile(string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            foreach (var ex in ImageUtil.ImageFileExtensionList)
            {
                var result = Directory.EnumerateFiles(directoryPath, string.Format("*{0}", ex)).Any();
                if (result)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// ファイルにアクセス可能か確認します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>隠しファイル、システムファイルならFalse。それ以外ならTrue。</returns>
        public static bool CanAccess(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            if (string.IsNullOrEmpty(filePath))
            {
                return true;
            }
            else if (IsExists(filePath))
            {
                if (IsDrive(filePath))
                {
                    return true;
                }
                else
                {
                    try
                    {
                        FileAttributes fa = File.GetAttributes(filePath);
                        if ((fa & FileAttributes.Hidden) == FileAttributes.Hidden ||
                            (fa & (FileAttributes.Hidden | FileAttributes.System)) == (FileAttributes.Hidden | FileAttributes.System))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region ファイル情報取得メソッド

        /// <summary>
        /// ファイル名を取得します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <remarks>ドライブの場合、ボリュームラベルを返します。</remarks>
        /// <returns>ファイルパス</returns>
        public static string GetFileName(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            if (string.IsNullOrEmpty(filePath))
            {
                return "PC";
            }
            else if (IsDrive(filePath))
            {
                DriveInfo driveInfo = DriveInfo.GetDrives().FirstOrDefault(di => di.Name == filePath ||
                                                                           di.Name == filePath + "\\");
                if (driveInfo == null)
                {
                    throw new NullReferenceException("ドライブが存在しません。");
                }

                if (string.IsNullOrEmpty(driveInfo.VolumeLabel))
                {
                    return toRemoveLastPathSeparate(filePath);
                }
                else
                {
                    return string.Format("{0}({1})", driveInfo.VolumeLabel, toRemoveLastPathSeparate(filePath));
                }
            }
            else
            {
                return Path.GetFileName(filePath);
            }
        }

        /// <summary>
        /// 親フォルダパスを取得します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>親フォルダパス</returns>
        public static string GetParentDirectoryPath(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("システムルートが指定されました。", "filePath");
            }

            return Path.GetDirectoryName(filePath);
        }

        /// <summary>
        /// ファイルの拡張子を取得します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>ファイルの拡張子をピリオド + 大文字(.XXX)で返します。</returns>
        public static string GetExtension(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            return Path.GetExtension(filePath).ToUpper();
        }

        /// <summary>
        /// ファイル種類名称を取得します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>ファイル種類名称</returns>
        public static string GetTypeName(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            if (string.IsNullOrEmpty(filePath))
            {
                return "システムルート";
            }
            else
            {
                WinApiMembers.SHFILEINFOW sh = new WinApiMembers.SHFILEINFOW();
                IntPtr hSuccess = WinApiMembers.SHGetFileInfoW(filePath, 0, ref sh, (uint)Marshal.SizeOf(sh), WinApiMembers.ShellFileInfoFlags.SHGFI_TYPENAME);
                if (!hSuccess.Equals(IntPtr.Zero))
                {
                    return sh.szTypeName;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// ファイルの更新日時を取得します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>ファイル更新日時</returns>
        public static DateTime GetUpdateDate(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            return File.GetLastWriteTime(filePath);
        }

        /// <summary>
        /// ファイルの作成日時を取得します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>ファイル作成日時</returns>
        public static DateTime GetCreateDate(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            return File.GetCreationTime(filePath);
        }

        /// <summary>
        /// ファイルサイズを取得します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static long GetFileSize(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            FileInfo fi = new FileInfo(filePath);
            return fi.Length;
        }

        /// <summary>
        /// 短いファイルパスを取得します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetShortPathName(string filePath)
        {
            StringBuilder sb = new StringBuilder(1024);
            uint ret = WinApiMembers.GetShortPathName(filePath, sb, (uint)sb.Capacity);
            if (ret == 0)
            {
                throw new Exception("短いファイル名の取得に失敗しました。");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 指定したディレクトリ内の最初の画像ファイルを取得します。
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static string GetFirstImageFilePath(string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            return FileUtil.GetFiles(directoryPath)
                .OrderBy(file => file)
                .FirstOrDefault(file => ImageUtil.ImageFileExtensionList.Contains(FileUtil.GetExtension(file)));
        }

        #endregion

        #region ファイルリスト取得メソッド

        /// <summary>
        /// ドライブリストを取得します。
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetDriveList()
        {
            var drives = from drive in DriveInfo.GetDrives()
                         select toRemoveLastPathSeparate(drive.Name + "\\");
            return drives.ToList();
        }

        /// <summary>
        /// フォルダ内のファイルを取得します。
        /// </summary>
        /// <param name="directoryPath">フォルダパス</param>
        /// <returns></returns>
        public static IList<string> GetFiles(string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException("directoryPath");
            }

            if (CanAccess(directoryPath))
            {
                try
                {
                    return Directory
                        .GetFiles(directoryPath)
                        .Where(file => CanAccess(file))
                        .ToList();
                }
                catch (UnauthorizedAccessException)
                {
                    return new string[] { };
                }
            }
            else
            {
                return new string[] { };
            }
        }

        /// <summary>
        /// フォルダ内のフォルダを取得します。
        /// </summary>
        /// <param name="directoryPath">フォルダパス</param>
        /// <returns></returns>
        public static IList<string> GetSubDirectorys(string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException("directoryPath");
            }

            if (CanAccess(directoryPath))
            {
                try
                {
                    return Directory
                        .GetDirectories(directoryPath)
                        .Where(file => CanAccess(file))
                        .ToList();
                }
                catch (UnauthorizedAccessException)
                {
                    return new string[] { };
                }
            }
            else
            {
                return new string[] { };
            }
        }

        /// <summary>
        /// フォルダ内のファイルとフォルダを取得します。
        /// </summary>
        /// <param name="directoryPath">フォルダパス</param>
        /// <returns></returns>
        public static IList<string> GetFilesAndSubDirectorys(string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException("directoryPath");
            }

            if (CanAccess(directoryPath))
            {
                try
                {
                    return Directory
                        .GetFileSystemEntries(directoryPath)
                        .Where(file => CanAccess(file))
                        .ToList();
                }
                catch (UnauthorizedAccessException)
                {
                    return new string[] { };
                }
            }
            else
            {
                return new string[] { };
            }
        }

        #endregion

        #region 変換メソッド

        /// <summary>
        /// ファイルサイズを文字列に変換します。
        /// </summary>
        /// <param name="fileSize">ファイルサイズ</param>
        /// <returns>ファイルサイズの文字列</returns>
        public static string ToSizeString(long fileSize)
        {
            if (fileSize < 0)
            {
                throw new ArgumentOutOfRangeException("fileSize");
            }

            if (fileSize < 1024)
            {
                return fileSize.ToString() + " B";
            }
            else if (fileSize < 1048576)
            {
                return (fileSize / 1024).ToString() + " KB";
            }
            else if (fileSize < 1073741824)
            {
                return (fileSize / 1048576).ToString() + " MB";
            }
            else if (fileSize < 1099511627776)
            {
                return (fileSize / 1073741824).ToString() + " GB";
            }
            else if (fileSize < 1125899906842624)
            {
                return (fileSize / 1099511627776).ToString() + "TB";
            }
            else
            {
                throw new ArgumentOutOfRangeException("fileSize");
            }
        }

        #endregion

        #region アイコン取得メソッド

        /// <summary>
        /// 小システムアイコンを取得します。
        /// </summary>
        /// <param name="spesialDirectory">システムアイコンの種類</param>
        /// <returns></returns>
        public static Image GetSmallSystemIcon(WinApiMembers.ShellSpecialFolder spesialFolder)
        {
            IntPtr idHandle = IntPtr.Zero;
            WinApiMembers.SHGetSpecialFolderLocation(IntPtr.Zero, spesialFolder, out idHandle);
            WinApiMembers.SHFILEINFOW sh = new WinApiMembers.SHFILEINFOW();
            IntPtr hSuccess = WinApiMembers.SHGetFileInfoW(idHandle, 0, ref sh, (uint)Marshal.SizeOf(sh),
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_ICON |
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_PIDL |
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_SMALLICON);
            if (!hSuccess.Equals(IntPtr.Zero))
            {
                Image icon = Icon.FromHandle(sh.hIcon).ToBitmap();
                WinApiMembers.DestroyIcon(sh.hIcon);
                return icon;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 大システムアイコンを取得します。
        /// </summary>
        /// <param name="spesialDirectory">システムアイコンの種類</param>
        /// <returns></returns>
        public static Image GetLargeSystemIcon(WinApiMembers.ShellSpecialFolder spesialFolder)
        {
            IntPtr idHandle = IntPtr.Zero;
            WinApiMembers.SHGetSpecialFolderLocation(IntPtr.Zero, spesialFolder, out idHandle);
            WinApiMembers.SHFILEINFOW sh = new WinApiMembers.SHFILEINFOW();
            IntPtr hSuccess = WinApiMembers.SHGetFileInfoW(idHandle, 0, ref sh, (uint)Marshal.SizeOf(sh),
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_ICON |
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_PIDL |
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_LARGEICON);
            if (!hSuccess.Equals(IntPtr.Zero))
            {
                Image icon = Icon.FromHandle(sh.hIcon).ToBitmap();
                WinApiMembers.DestroyIcon(sh.hIcon);
                return icon;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// ファイルパスを指定して小アイコンを取得します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns></returns>
        public static Image GetSmallIconByFilePath(string filePath)
        {
            WinApiMembers.SHFILEINFOW sh = new WinApiMembers.SHFILEINFOW();
            IntPtr hSuccess = WinApiMembers.SHGetFileInfoW(filePath, 0, ref sh, (uint)Marshal.SizeOf(sh),
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_ICON |
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_SMALLICON);
            if (!hSuccess.Equals(IntPtr.Zero))
            {
                Image icon = Icon.FromHandle(sh.hIcon).ToBitmap();
                WinApiMembers.DestroyIcon(sh.hIcon);
                return icon;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// ファイルパスを指定して大アイコンを取得します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns></returns>
        public static Image GetLargeIconByFilePath(string filePath)
        {
            WinApiMembers.SHFILEINFOW sh = new WinApiMembers.SHFILEINFOW();
            IntPtr hSuccess = WinApiMembers.SHGetFileInfoW(filePath, 0, ref sh, (uint)Marshal.SizeOf(sh),
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_ICON |
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_LARGEICON);
            if (!hSuccess.Equals(IntPtr.Zero))
            {
                Image icon = Icon.FromHandle(sh.hIcon).ToBitmap();
                WinApiMembers.DestroyIcon(sh.hIcon);
                return icon;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// ファイルパスを指定してEXTRALARGEアイコンを取得します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns></returns>
        public static Image GetExtraLargeIconByFilePath(string filePath, SHIL shil)
        {
            var shinfo = new WinApiMembers.SHFILEINFO();
            var hSuccess = WinApiMembers.SHGetFileInfo(
                filePath,
                0,
                ref shinfo,
                (int)Marshal.SizeOf(typeof(WinApiMembers.SHFILEINFO)),
                (int)WinApiMembers.ShellFileInfoFlags.SHGFI_SYSICONINDEX);
            if (hSuccess.Equals(IntPtr.Zero))
            {
                return null;
            }

            int result;

            var pimgList = IntPtr.Zero;
            result = WinApiMembers.SHGetImageList(
                shil,
                WinApiMembers.IID_IImageList,
                out pimgList);
            if (result != WinApiMembers.S_OK)
            {
                return null;
            }

            try
            {
                var hicon = WinApiMembers.ImageList_GetIcon(pimgList, shinfo.iIcon, 0);
                if (hicon.Equals(IntPtr.Zero))
                {
                    return null;
                }

                try
                {
                    var icon = Icon.FromHandle(hicon).ToBitmap();
                    return icon;
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

        /// <summary>
        /// 拡張子を指定して小アイコンを取得します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns></returns>
        public static Image GetSmallIconByExtension(string ex)
        {
            WinApiMembers.SHFILEINFOW sh = new WinApiMembers.SHFILEINFOW();
            IntPtr hSuccess = WinApiMembers.SHGetFileInfoW(string.Format(@"*{0}", ex), 0, ref sh, (uint)Marshal.SizeOf(sh),
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_USEFILEATTRIBUTES |
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_ICON |
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_SMALLICON);
            if (!hSuccess.Equals(IntPtr.Zero))
            {
                Image icon = Icon.FromHandle(sh.hIcon).ToBitmap();
                WinApiMembers.DestroyIcon(sh.hIcon);
                return icon;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 拡張子を指定して大アイコンを取得します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns></returns>
        public static Image GetLargeIconByExtension(string ex)
        {
            WinApiMembers.SHFILEINFOW sh = new WinApiMembers.SHFILEINFOW();
            IntPtr hSuccess = WinApiMembers.SHGetFileInfoW(string.Format(@"*{0}", ex), 0, ref sh, (uint)Marshal.SizeOf(sh),
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_USEFILEATTRIBUTES |
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_ICON |
                                                           WinApiMembers.ShellFileInfoFlags.SHGFI_LARGEICON);
            if (!hSuccess.Equals(IntPtr.Zero))
            {
                Image icon = Icon.FromHandle(sh.hIcon).ToBitmap();
                WinApiMembers.DestroyIcon(sh.hIcon);
                return icon;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 指定された実行可能ファイル、
        /// ダイナミックリンクライブラリ（DLL）、
        /// アイコンファイルのいずれかから、小さいアイコンを取得します。
        /// </summary>
        /// <param name="path">抽出するファイルパス</param>
        /// <param name="iconIndex">アイコンのインデックス</param>
        /// <returns></returns>
        public static Image GetSmallIconFromFile(string path, int iconIndex)
        {
            IntPtr largeIconHandle = IntPtr.Zero;
            IntPtr smallIconHandle = IntPtr.Zero;
            WinApiMembers.ExtractIconEx(path, iconIndex, out largeIconHandle, out smallIconHandle, 1);
            Image icon = Icon.FromHandle(smallIconHandle).ToBitmap();
            WinApiMembers.DestroyIcon(largeIconHandle);
            WinApiMembers.DestroyIcon(smallIconHandle);
            return icon;
        }

        /// <summary>
        /// 指定された実行可能ファイル、
        /// ダイナミックリンクライブラリ（DLL）、
        /// アイコンファイルのいずれかから、大きいアイコンを取得します。
        /// </summary>
        /// <param name="path">抽出するファイルパス</param>
        /// <param name="iconIndex">アイコンのインデックス</param>
        /// <returns></returns>
        public static Image GetLargeIconFromFile(string path, int iconIndex)
        {
            IntPtr largeIconHandle = IntPtr.Zero;
            IntPtr smallIconHandle = IntPtr.Zero;
            WinApiMembers.ExtractIconEx(path, iconIndex, out largeIconHandle, out smallIconHandle, 1);
            Image icon = Icon.FromHandle(largeIconHandle).ToBitmap();
            WinApiMembers.DestroyIcon(largeIconHandle);
            WinApiMembers.DestroyIcon(smallIconHandle);
            return icon;
        }

        #endregion

        #region プロセス関連メソッド

        public static void OpenFile(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            try
            {
                Process.Start(filePath);
            }
            catch (Win32Exception)
            {
                return;
            }
        }

        public static void OpenMyComputer()
        {
            Process.Start("EXPLORER.EXE", "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}");
        }

        public static void OpenExplorer(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            Process.Start("EXPLORER.EXE", filePath);
        }

        public static void OpenExplorerSelect(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            Process.Start("EXPLORER.EXE", string.Format(@"/select,{0}", filePath));
        }

        #endregion

        // ファイルパスの末尾が"\"の場合取り除きます。
        private static string toRemoveLastPathSeparate(string filePath)
        {
            int length = filePath.Length;
            string lastChar = filePath.Substring(length - 1, 1);
            if (lastChar.Equals("\\", StringComparison.Ordinal))
            {
                return filePath.Substring(0, length - 1);
            }
            else
            {
                return filePath;
            }
        }
    }
}
