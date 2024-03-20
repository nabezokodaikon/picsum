using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using WinApi;
using static WinApi.WinApiMembers;

namespace SWF.Common
{
    [SupportedOSPlatform("windows")]
    public static class FileUtil
    {
        private const string ROOT_DIRECTORY_NAME = "PC";
        private const string ROOT_DIRECTORY_TYPE_NAME = "System root";

        public const string ROOT_DIRECTORY_PATH =
            "1435810adf6f3080e21df9c3b666c7887883da42ad582d911a81931c38e720da1235036c60c69389e8c4fcc26be0c796626ef8ed3296bd9c65445ff12168fb22";

        /// <summary>
        /// ファイル、フォルダの存在を確認します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>ファイルまたはフォルダが存在するならTrue。存在しなければFalse。</returns>
        public static bool IsExists(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (FileUtil.IsSystemRoot(filePath))
            {
                return true;
            }
            else
            {
                return WinApiMembers.PathFileExists(filePath) == 1;
            }
        }

        /// <summary>
        /// ファイルパスがシステムルートであるか確認します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsSystemRoot(string filePath)
        {
            return filePath.Trim() == FileUtil.ROOT_DIRECTORY_PATH;
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
                throw new ArgumentNullException(nameof(filePath));
            }

            if (FileUtil.IsSystemRoot(filePath))
            {
                return false;
            }
            else
            {
                try
                {
                    return Directory.Exists(filePath)
                        && Path.GetDirectoryName(filePath) == null;
                }
                catch (PathTooLongException)
                {
                    return false;
                }
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
                throw new ArgumentNullException(nameof(filePath));
            }

            if (FileUtil.IsSystemRoot(filePath))
            {
                return true;
            }
            else
            {
                return Directory.Exists(filePath);
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
                throw new ArgumentNullException(nameof(filePath));
            }

            return File.Exists(filePath);
        }

        /// <summary>
        /// 指定したファイルが画像ファイルであるか確認します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsImageFile(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var ex = FileUtil.GetExtension(filePath);
            return ImageUtil.IMAGE_FILE_EXTENSION_LIST.Contains(ex);
        }

        /// <summary>
        /// 指定したファイルがWEBPファイルであるか確認します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsWEBPFile(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var ex = FileUtil.GetExtension(filePath);
            return (ex == ImageUtil.WEBP_FILE_EXTENSION);
        }

        /// <summary>
        /// 指定したファイルがAVIFファイルであるか確認します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsAVIFFile(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var ex = FileUtil.GetExtension(filePath);
            return (ex == ImageUtil.AVIF_FILE_EXTENSION);
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

            if (string.IsNullOrEmpty(directoryPath))
            {
                throw new ArgumentException(nameof(directoryPath));
            }

            foreach (var ex in ImageUtil.IMAGE_FILE_EXTENSION_LIST)
            {
                try
                {
                    var result = Directory.EnumerateFiles(directoryPath, string.Format("*{0}", ex)).Any();
                    if (result)
                    {
                        return result;
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    return false;
                }
                catch (PathTooLongException)
                {
                    return false;
                }
                catch (IOException)
                {
                    return false;
                }
                catch (UnauthorizedAccessException)
                {
                    return false;
                }
                catch (SecurityException)
                {
                    return false;
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
                throw new ArgumentNullException(nameof(filePath));
            }

            if (FileUtil.IsSystemRoot(filePath))
            {
                return true;
            }
            else if (FileUtil.IsDrive(filePath))
            {
                return true;
            }
            else if (FileUtil.IsExists(filePath))
            {
                try
                {
                    var fa = File.GetAttributes(filePath);
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
                catch (PathTooLongException)
                {
                    return false;
                }
                catch (NotSupportedException)
                {
                    return false;
                }
                catch (FileNotFoundException)
                {
                    return false;
                }
                catch (DirectoryNotFoundException)
                {
                    return false;
                }
                catch (IOException)
                {
                    return false;
                }
                catch (UnauthorizedAccessException)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

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
                throw new ArgumentNullException(nameof(filePath));
            }

            if (FileUtil.IsSystemRoot(filePath))
            {
                return FileUtil.ROOT_DIRECTORY_NAME;
            }
            else if (FileUtil.IsDrive(filePath))
            {
                try
                {
                    var driveInfo = DriveInfo.GetDrives()
                        .FirstOrDefault(di => di.Name == filePath || di.Name == string.Format(@"{0}\", filePath));
                    if (driveInfo != null)
                    {
                        return string.Format("{0}({1})", driveInfo.VolumeLabel, FileUtil.ToRemoveLastPathSeparate(filePath));
                    }
                    else
                    {
                        return FileUtil.ROOT_DIRECTORY_NAME;
                    }
                }
                catch (IOException)
                {
                    // ドライブ情報の取得に失敗した場合、ルートディレクトリと判断します。
                    return FileUtil.ROOT_DIRECTORY_NAME;
                }
                catch (UnauthorizedAccessException)
                {
                    // ドライブ情報の取得に失敗した場合、ルートディレクトリと判断します。
                    return FileUtil.ROOT_DIRECTORY_NAME;
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
                throw new ArgumentNullException(nameof(filePath));
            }

            if (FileUtil.IsSystemRoot(filePath))
            {
                throw new ArgumentException("システムルートが指定されました。", nameof(filePath));
            }

            try
            {
                if (FileUtil.IsDrive(filePath))
                {
                    return FileUtil.ROOT_DIRECTORY_PATH;
                }
                else
                {
                    return Path.GetDirectoryName(filePath);
                }
            }
            catch (IOException)
            {
                throw new ArgumentException("不明なファイルパスが指定されました。", nameof(filePath));
            }
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
                throw new ArgumentNullException(nameof(filePath));
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
                throw new ArgumentNullException(nameof(filePath));
            }

            if (FileUtil.IsSystemRoot(filePath))
            {
                return FileUtil.ROOT_DIRECTORY_TYPE_NAME;
            }
            else
            {
                var sh = new WinApiMembers.SHFILEINFOW();
                var hSuccess = WinApiMembers.SHGetFileInfoW(filePath, 0, ref sh, (uint)Marshal.SizeOf(sh), WinApiMembers.ShellFileInfoFlags.SHGFI_TYPENAME);
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
        public static DateTime? GetUpdateDate(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (FileUtil.IsSystemRoot(filePath))
            {
                return null;
            }

            try
            {
                return File.GetLastWriteTime(filePath);
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (PathTooLongException)
            {
                throw;
            }
            catch (NotSupportedException)
            {
                throw;
            }
        }

        /// <summary>
        /// ファイルの作成日時を取得します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>ファイル作成日時</returns>
        public static DateTime? GetCreateDate(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (FileUtil.IsSystemRoot(filePath))
            {
                return null;
            }

            try
            {
                return File.GetCreationTime(filePath);
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (PathTooLongException)
            {
                throw;
            }
            catch (NotSupportedException)
            {
                throw;
            }
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
                throw new ArgumentNullException(nameof(filePath));
            }

            try
            {
                var fi = new FileInfo(filePath);
                return fi.Length;
            }
            catch (SecurityException)
            {
                throw;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (PathTooLongException)
            {
                throw;
            }
            catch (NotSupportedException)
            {
                throw;
            }
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
                .FirstOrDefault(file => FileUtil.IsImageFile(file));
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
                throw new ArgumentNullException(nameof(directoryPath));
            }

            if (FileUtil.CanAccess(directoryPath))
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
                catch (PathTooLongException)
                {
                    return new string[] { };
                }
                catch (DirectoryNotFoundException)
                {
                    return new string[] { };
                }
                catch (IOException)
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
        public static string[] GetSubDirectorys(string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            if (FileUtil.IsSystemRoot(directoryPath))
            {
                return FileUtil.GetDriveList();
            }

            if (FileUtil.CanAccess(directoryPath))
            {
                try
                {
                    return Directory
                        .GetDirectories(directoryPath)
                        .Where(file => CanAccess(file))
                        .ToArray();
                }
                catch (UnauthorizedAccessException)
                {
                    return new string[] { };
                }
                catch (PathTooLongException)
                {
                    return new string[] { };
                }
                catch (DirectoryNotFoundException)
                {
                    return new string[] { };
                }
                catch (IOException)
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
                throw new ArgumentNullException(nameof(directoryPath));
            }

            if (FileUtil.IsSystemRoot(directoryPath))
            {
                return FileUtil.GetDriveList();
            }

            if (FileUtil.CanAccess(directoryPath))
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
                catch (PathTooLongException)
                {
                    return new string[] { };
                }
                catch (DirectoryNotFoundException)
                {
                    return new string[] { };
                }
                catch (IOException)
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
        /// ファイルサイズを文字列に変換します。
        /// </summary>
        /// <param name="fileSize">ファイルサイズ</param>
        /// <returns>ファイルサイズの文字列</returns>
        public static string ToSizeString(long fileSize)
        {
            if (fileSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(fileSize));
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
                throw new ArgumentOutOfRangeException(nameof(fileSize));
            }
        }

        /// <summary>
        /// 小システムアイコンを取得します。
        /// </summary>
        /// <param name="spesialDirectory">システムアイコンの種類</param>
        /// <returns></returns>
        public static Image GetSmallSystemIcon(WinApiMembers.ShellSpecialFolder spesialFolder)
        {
            WinApiMembers.SHGetSpecialFolderLocation(IntPtr.Zero, spesialFolder, out var idHandle);
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
                    return null;
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
        public static Image GetLargeSystemIcon(WinApiMembers.ShellSpecialFolder spesialFolder)
        {
            WinApiMembers.SHGetSpecialFolderLocation(IntPtr.Zero, spesialFolder, out var idHandle);
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
                    return null;
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
        public static Image GetSmallIconByFilePath(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

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
                    return null;
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
        public static Image GetExtraLargeIconByFilePath(string filePath, SHIL shil)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

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

            var result = WinApiMembers.SHGetImageList(
                shil,
                WinApiMembers.IID_IImageList,
                out IntPtr pimgList);

            try
            {
                if (result != WinApiMembers.S_OK)
                {
                    return null;
                }

                var hicon = WinApiMembers.ImageList_GetIcon(pimgList, shinfo.iIcon, 0);
                if (hicon.Equals(IntPtr.Zero))
                {
                    return null;
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

        public static void OpenFile(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            try
            {
                using (var p = new Process())
                {
                    p.StartInfo.UseShellExecute = true;
                    p.StartInfo.FileName = filePath;
                    p.Start();
                }
            }
            catch (Win32Exception)
            {
                return;
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (FileNotFoundException)
            {
                return;
            }
        }

        public static void OpenExplorer(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            try
            {
                if (FileUtil.IsSystemRoot(filePath))
                {
                    Process.Start("EXPLORER.EXE", "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}");
                }
                else
                {
                    Process.Start("EXPLORER.EXE", filePath);
                }
            }
            catch (Win32Exception)
            {
                return;
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (FileNotFoundException)
            {
                return;
            }
        }

        public static void OpenExplorerSelect(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            try
            {
                Process.Start("EXPLORER.EXE", string.Format(@"/select,{0}", filePath));
            }
            catch (Win32Exception)
            {
                return;
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (FileNotFoundException)
            {
                return;
            }
        }

        // ファイルパスの末尾が"\"の場合取り除きます。
        private static string ToRemoveLastPathSeparate(string filePath)
        {
            var length = filePath.Length;
            var lastChar = filePath.Substring(length - 1, 1);
            if (lastChar.Equals(@"\", StringComparison.Ordinal))
            {
                return filePath.Substring(0, length - 1);
            }
            else
            {
                return filePath;
            }
        }

        // ドライブリストを取得します。
        private static string[] GetDriveList()
        {
            try
            {
                var drives = DriveInfo.GetDrives()
                    .Select(drive => FileUtil.ToRemoveLastPathSeparate(string.Format(@"{0}\", drive.Name)))
                    .ToArray();
                return drives;
            }
            catch (IOException)
            {
                return new string[] { };
            }
            catch (UnauthorizedAccessException)
            {
                return new string[] { };
            }
        }
    }
}
