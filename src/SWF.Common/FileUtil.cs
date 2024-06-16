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
using System.Text;
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
        public static readonly DateTime ROOT_DIRECTORY_DATETIME = DateTime.MinValue;

        /// <summary>
        /// ファイル、フォルダの存在を確認します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>ファイルまたはフォルダが存在するならTrue。存在しなければFalse。</returns>
        public static bool IsExists(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (IsSystemRoot(filePath))
            {
                return true;
            }
            else
            {
                var utf8Bytes = Encoding.UTF8.GetBytes(filePath);
                var utf8FilePath = Encoding.UTF8.GetString(utf8Bytes);
                return File.Exists(utf8FilePath) || Directory.Exists(utf8FilePath);
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
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (IsSystemRoot(filePath))
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
                catch (PathTooLongException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
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
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (IsSystemRoot(filePath))
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
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return File.Exists(filePath);
        }

        /// <summary>
        /// 指定したファイルが画像ファイルであるか確認します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsImageFile(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

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
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

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
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var ex = FileUtil.GetExtension(filePath);
            return (ex == ImageUtil.AVIF_FILE_EXTENSION);
        }

        public static bool IsHEICFile(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var ex = FileUtil.GetExtension(filePath);
            return (ex == ImageUtil.HEIC_FILE_EXTENSION);
        }

        /// <summary>
        /// 指定したディレクトリ内に、画像ファイルが存在するか確認します。
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool HasImageFile(string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            foreach (var ex in ImageUtil.IMAGE_FILE_EXTENSION_LIST)
            {
                try
                {
                    var result = Directory.EnumerateFiles(directoryPath, $"*{ex}").Any();
                    if (result)
                    {
                        return result;
                    }
                }
                catch (DirectoryNotFoundException exception)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(directoryPath), exception);
                }
                catch (PathTooLongException exception)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(directoryPath), exception);
                }
                catch (IOException exception)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(directoryPath), exception);
                }
                catch (UnauthorizedAccessException exception)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(directoryPath), exception);
                }
                catch (SecurityException exception)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(directoryPath), exception);
                }
            }

            return false;
        }

        /// <summary>
        /// 指定したディレクトリ内に、画像ファイルが複数存在するか確認します。
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool HasImageFiles(string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            foreach (var ex in ImageUtil.IMAGE_FILE_EXTENSION_LIST)
            {
                try
                {
                    var result = Directory.EnumerateFiles(directoryPath, $"*{ex}").Count() > 2;
                    if (result)
                    {
                        return result;
                    }
                }
                catch (DirectoryNotFoundException exception)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(directoryPath), exception);
                }
                catch (PathTooLongException exception)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(directoryPath), exception);
                }
                catch (IOException exception)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(directoryPath), exception);
                }
                catch (UnauthorizedAccessException exception)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(directoryPath), exception);
                }
                catch (SecurityException exception)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(directoryPath), exception);
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
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (IsSystemRoot(filePath))
            {
                return true;
            }
            else if (IsDrive(filePath))
            {
                return true;
            }
            else if (IsExists(filePath))
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
                catch (PathTooLongException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
                }
                catch (NotSupportedException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
                }
                catch (FileNotFoundException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
                }
                catch (DirectoryNotFoundException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
                }
                catch (IOException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
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
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (FileUtil.IsSystemRoot(filePath))
            {
                return FileUtil.ROOT_DIRECTORY_NAME;
            }
            else if (FileUtil.IsDrive(filePath))
            {
                try
                {
                    var driveInfo = DriveInfo.GetDrives()
                        .FirstOrDefault(di => di.Name == filePath || di.Name == $"{filePath}");
                    if (driveInfo != null)
                    {
                        return $"{driveInfo.VolumeLabel}({FileUtil.ToRemoveLastPathSeparate(filePath)})";
                    }
                    else
                    {
                        return FileUtil.ROOT_DIRECTORY_NAME;
                    }
                }
                catch (IOException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
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
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (FileUtil.IsSystemRoot(filePath))
            {
                throw new ArgumentException("システムルートが指定されました。", nameof(filePath));
            }

            if (FileUtil.IsDrive(filePath))
            {
                return FileUtil.ROOT_DIRECTORY_PATH;
            }
            else
            {
                try
                {
                    return Path.GetDirectoryName(filePath);
                }
                catch (PathTooLongException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
                }
                catch (IOException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
                }

            }
        }

        /// <summary>
        /// ファイルの拡張子を取得します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>ファイルの拡張子をピリオド + 大文字(.XXX)で返します。</returns>
        public static string GetExtension(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return Path.GetExtension(filePath).ToUpper();
        }

        /// <summary>
        /// ファイル種類名称を取得します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>ファイル種類名称</returns>
        public static string GetTypeName(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

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
        public static DateTime GetUpdateDate(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (FileUtil.IsSystemRoot(filePath))
            {
                return ROOT_DIRECTORY_DATETIME;
            }

            try
            {
                return File.GetLastWriteTime(filePath);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (PathTooLongException ex)
            {
                throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (NotSupportedException ex)
            {
                throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
        }

        /// <summary>
        /// ファイルサイズを取得します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static long GetFileSize(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            try
            {
                var fi = new FileInfo(filePath);
                return fi.Length;
            }
            catch (SecurityException ex)
            {
                throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (PathTooLongException ex)
            {
                throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (NotSupportedException ex)
            {
                throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
        }

        /// <summary>
        /// 指定したディレクトリ内の最初の画像ファイルを取得します。
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static string GetFirstImageFilePath(string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

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
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            if (FileUtil.CanAccess(directoryPath))
            {
                try
                {
                    return Directory
                        .GetFiles(directoryPath)
                        .Where(file => CanAccess(file))
                        .ToList();
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(directoryPath), ex);
                }
                catch (PathTooLongException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(directoryPath), ex);
                }
                catch (DirectoryNotFoundException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(directoryPath), ex);
                }
                catch (IOException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(directoryPath), ex);
                }
            }
            else
            {
                return [];
            }
        }

        /// <summary>
        /// フォルダ内のフォルダを取得します。
        /// </summary>
        /// <param name="directoryPath">フォルダパス</param>
        /// <returns></returns>
        public static string[] GetSubDirectorys(string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

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
                catch (UnauthorizedAccessException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(directoryPath), ex);
                }
                catch (PathTooLongException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(directoryPath), ex);
                }
                catch (DirectoryNotFoundException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(directoryPath), ex);
                }
                catch (IOException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(directoryPath), ex);
                }
            }
            else
            {
                return [];
            }
        }

        /// <summary>
        /// フォルダ内のファイルとフォルダを取得します。
        /// </summary>
        /// <param name="directoryPath">フォルダパス</param>
        /// <returns></returns>
        public static IList<string> GetFilesAndSubDirectorys(string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

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
                catch (UnauthorizedAccessException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(directoryPath), ex);
                }
                catch (PathTooLongException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(directoryPath), ex);
                }
                catch (DirectoryNotFoundException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(directoryPath), ex);
                }
                catch (IOException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(directoryPath), ex);
                }
            }
            else
            {
                return [];
            }
        }

        /// <summary>
        /// ファイルサイズを文字列に変換します。
        /// </summary>
        /// <param name="fileSize">ファイルサイズ</param>
        /// <returns>ファイルサイズの文字列</returns>
        public static string ToSizeUnitString(long fileSize)
        {
            if (fileSize < 0)
            {
                ArgumentOutOfRangeException.ThrowIfNegative(fileSize, nameof(fileSize));
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
        public static Bitmap GetSmallSystemIcon(WinApiMembers.ShellSpecialFolder spesialFolder)
        {
            _ = WinApiMembers.SHGetSpecialFolderLocation(IntPtr.Zero, spesialFolder, out var idHandle);
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
        public static Bitmap GetLargeSystemIcon(WinApiMembers.ShellSpecialFolder spesialFolder)
        {
            _ = WinApiMembers.SHGetSpecialFolderLocation(IntPtr.Zero, spesialFolder, out var idHandle);
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
        public static Bitmap GetExtraLargeIconByFilePath(string filePath, SHIL shil)
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
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            try
            {
                using (var p = new Process())
                {
                    p.StartInfo.UseShellExecute = true;
                    p.StartInfo.FileName = filePath;
                    p.Start();
                }
            }
            catch (Win32Exception ex)
            {
                throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (ObjectDisposedException ex)
            {
                throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (FileNotFoundException ex)
            {
                throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
        }

        public static void OpenExplorer(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            try
            {
                if (FileUtil.IsSystemRoot(filePath))
                {
                    Process.Start("EXPLORER.EXE", "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}");
                }
                else
                {
                    Process.Start("EXPLORER.EXE", $"{filePath}");
                }
            }
            catch (Win32Exception ex)
            {
                throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (ObjectDisposedException ex)
            {
                throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (FileNotFoundException ex)
            {
                throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
        }

        public static void OpenExplorerSelect(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            try
            {
                Process.Start("EXPLORER.EXE", $"/select,{filePath}");
            }
            catch (Win32Exception ex)
            {
                throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (ObjectDisposedException ex)
            {
                throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (FileNotFoundException ex)
            {
                throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
        }

        /// <summary>
        /// エクスポート時のダイアログのフィルター文字列を取得します。
        /// </summary>
        /// <param name="filePath">エクスポートするファイルパス。</param>
        /// <returns></returns>
        public static string GetExportFilterText(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var ex = FileUtil.GetExtension(filePath)[1..];
            return $"{ex.ToUpper()} Files (*.{ex.ToLower()})|*.{ex.ToLower()}|All Files (*.*)|*.*";
        }

        /// <summary>
        /// エクスポート可能なファイルパスを取得します。
        /// </summary>
        /// <param name="exportDirectoryPath">エクスポートディレクトリパス</param>
        /// <param name="srcFilePath">エクスポートファイル名</param>
        /// <returns></returns>
        public static string GetExportFileName(string exportDirectoryPath, string srcFilePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(exportDirectoryPath, nameof(exportDirectoryPath));
            ArgumentException.ThrowIfNullOrEmpty(srcFilePath, nameof(srcFilePath));

            var ex = FileUtil.GetExtension(srcFilePath).ToLower();
            var name = FileUtil.GetFileName(srcFilePath);
            name = name[..^ex.Length];

            var count = 0;

            do
            {
                string destFilePath;
                if (count == 0)
                {
                    destFilePath = @$"{exportDirectoryPath}\{name}{ex}";
                }
                else
                {
                    destFilePath = @$"{exportDirectoryPath}\{name} ({count + 1}){ex}";
                }

                if (!FileUtil.IsExists(destFilePath))
                {
                    return FileUtil.GetFileName(destFilePath);
                }

                count++;

            } while (true);
        }

        // ファイルパスの末尾が"\"の場合取り除きます。
        private static string ToRemoveLastPathSeparate(string filePath)
        {
            var length = filePath.Length;
            var lastChar = filePath.Substring(length - 1, 1);
            if (lastChar.Equals(@"\", StringComparison.Ordinal))
            {
                return filePath[..(length - 1)];
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
                    .Select(drive => FileUtil.ToRemoveLastPathSeparate(@$"{drive.Name}\"))
                    .ToArray();
                return drives;
            }
            catch (IOException ex)
            {
                throw new FileUtilException("ドライブリストの取得に失敗しました。", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new FileUtilException("ドライブリストの取得に失敗しました。", ex);
            }
        }

        private static string CreateFileAccessErrorMessage(string path)
        {
            return $"'{path}'を開けませんでした。";
        }
    }
}
