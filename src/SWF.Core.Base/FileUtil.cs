using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using WinApi;
using ZLinq;

namespace SWF.Core.Base
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class FileUtil
    {
        public const string ROOT_DIRECTORY_NAME = "PC";
        public const string ROOT_DIRECTORY_TYPE_NAME = "System root";
        public const string ROOT_DIRECTORY_PATH = "36f780fdbda5b2b2ce85c9ebb57086d1880ae757";

        public static readonly DateTime ROOT_DIRECTORY_DATETIME = DateTime.MinValue;
        public static readonly DateTime EMPTY_DATETIME = DateTime.MinValue;

        public static bool IsExistsFileOrDirectory(string path)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(path, nameof(path));

            using (TimeMeasuring.Run(false, "FileUtil.IsExistsFileOrDirectory"))
            {
                var handle = WinApiMembers.FindFirstFile(path, out var findData);
                if (handle == WinApiMembers.INVALID_HANDLE_VALUE)
                {
                    return false;
                }

                WinApiMembers.FindClose(handle);
                return true;
            }
        }

        /// <summary>
        /// ファイルパスがシステムルートであるか確認します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsSystemRoot(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return filePath == ROOT_DIRECTORY_PATH;
        }

        /// <summary>
        /// ファイルパスがドライブであるか確認します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>ファイルパスがドライブならTrue。ドライブでなければFalse。</returns>
        public static bool IsExistsDrive(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            using (TimeMeasuring.Run(false, "FileUtil.IsDrive"))
            {
                try
                {
                    return DriveInfo
                        .GetDrives()
                        .Any(d => StringUtil.Compare(d.Name, filePath));
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
        }

        /// <summary>
        /// ファイルパスがフォルダであるか確認します。
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <returns>ファイルパスがフォルダならTrue。フォルダでなければFalse。</returns>
        public static bool IsExistsDirectory(string path)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(path, nameof(path));

            using (TimeMeasuring.Run(false, "FileUtil.IsExistsDirectory"))
            {
                var handle = WinApiMembers.FindFirstFile(path, out var findData);
                if (handle == WinApiMembers.INVALID_HANDLE_VALUE)
                {
                    return false;
                }

                var isDirectory = (findData.dwFileAttributes & WinApiMembers.FILE_ATTRIBUTE_DIRECTORY) != 0;
                WinApiMembers.FindClose(handle);
                return isDirectory;
            }
        }

        /// <summary>
        /// ファイルパスがファイルであるか確認します。
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <returns>ファイルパスがファイルならTrue。ファイルでなければFalse。</returns>
        public static bool IsExistsFile(string path)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(path, nameof(path));

            using (TimeMeasuring.Run(false, "FileUtil.IsExistsFile"))
            {
                var handle = WinApiMembers.FindFirstFile(path, out var findData);
                if (handle == WinApiMembers.INVALID_HANDLE_VALUE)
                {
                    return false;
                }

                var isFile = (findData.dwFileAttributes & WinApiMembers.FILE_ATTRIBUTE_DIRECTORY) == 0;
                WinApiMembers.FindClose(handle);
                return isFile;
            }
        }

        /// <summary>
        /// ファイルにアクセス可能か確認します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>隠しファイル、システムファイルならFalse。それ以外ならTrue。</returns>
        public static bool CanAccess(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            using (TimeMeasuring.Run(false, "FileUtil.CanAccess"))
            {
                if (IsSystemRoot(filePath))
                {
                    return true;
                }

                try
                {
                    if (IsExistsFile(filePath))
                    {
                        var info = new FileInfo(filePath);
                        var _ = info.GetAccessControl();
                        var attr = File.GetAttributes(filePath);
                        return (attr & FileAttributes.Hidden) == 0;
                    }
                    else if (IsExistsDirectory(filePath))
                    {
                        var info = new DirectoryInfo(filePath);
                        var _ = info.GetAccessControl();
                        var attr = File.GetAttributes(filePath);
                        return (attr & FileAttributes.Hidden) == 0;
                    }
                    else if (IsExistsDrive(filePath))
                    {
                        var info = new DirectoryInfo(filePath);
                        var _ = info.GetAccessControl();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (ArgumentNullException)
                {
                    return false;
                }
                catch (SecurityException)
                {
                    return false;
                }
                catch (ArgumentException)
                {
                    return false;
                }
                catch (UnauthorizedAccessException)
                {
                    return false;
                }
                catch (PathTooLongException)
                {
                    return false;
                }
                catch (NotSupportedException)
                {
                    return false;
                }
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
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (IsExistsFileOrDirectory(filePath))
            {
                try
                {
                    return Path.GetFileName(filePath);
                }
                catch (ArgumentException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
                }
            }
            else if (IsSystemRoot(filePath))
            {
                return ROOT_DIRECTORY_NAME;
            }
            else if (IsExistsDrive(filePath))
            {
                try
                {
                    var driveInfo = DriveInfo.GetDrives()
                        .FirstOrDefault(di => di.Name == filePath || di.Name == $"{filePath}");
                    if (driveInfo == null)
                    {
                        throw new FileUtilException(CreateFileAccessErrorMessage(filePath));
                    }

                    return $"{driveInfo.VolumeLabel}({ToRemoveLastPathSeparate(filePath)})";
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

            throw new ArgumentException("不明なパスが指定されました。", nameof(filePath));
        }

        /// <summary>
        /// 親フォルダパスを取得します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>親フォルダパス</returns>
        public static string GetParentDirectoryPath(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (IsSystemRoot(filePath))
            {
                throw new ArgumentException("システムルートが指定されました。", nameof(filePath));
            }

            if (IsExistsFileOrDirectory(filePath))
            {
                try
                {
                    var parent = Path.GetDirectoryName(filePath);
                    return parent ?? string.Empty;
                }
                catch (ArgumentException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
                }
                catch (PathTooLongException ex)
                {
                    throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
                }
            }
            else if (IsExistsDrive(filePath))
            {
                return ROOT_DIRECTORY_PATH;
            }

            throw new ArgumentException("不明なパスが指定されました。", nameof(filePath));
        }

        /// <summary>
        /// ファイル種類名称を取得します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>ファイル種類名称</returns>
        public static string GetTypeName(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (IsSystemRoot(filePath))
            {
                return ROOT_DIRECTORY_TYPE_NAME;
            }
            else
            {
                var sh = new WinApiMembers.SHFILEINFOW();
                var hSuccess = WinApiMembers.SHGetFileInfoW(
                    filePath, 0, ref sh, (uint)Marshal.SizeOf(sh), WinApiMembers.ShellFileInfoFlags.SHGFI_TYPENAME);
                if (hSuccess != IntPtr.Zero)
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
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (IsSystemRoot(filePath))
            {
                return ROOT_DIRECTORY_DATETIME;
            }

            try
            {
                if (IsExistsFile(filePath))
                {
                    return File.GetLastWriteTime(filePath);
                }
                else if (IsExistsDirectory(filePath) || IsExistsDrive(filePath))
                {
                    return Directory.GetLastWriteTime(filePath);
                }
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
            catch (ArgumentNullException ex)
            {
                throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (ArgumentException ex)
            {
                throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }

            throw new ArgumentException("不明なパスが指定されました。", nameof(filePath));
        }

        /// <summary>
        /// ファイルサイズを取得します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static long GetFileSize(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (!IsExistsFile(filePath))
            {
                throw new ArgumentException("ファイル以外が指定されました。", nameof(filePath));
            }

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
            catch (ArgumentNullException ex)
            {
                throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (ArgumentException ex)
            {
                throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
        }

        /// <summary>
        /// フォルダ内のファイルを取得します。
        /// </summary>
        /// <param name="directoryPath">フォルダパス</param>
        /// <returns></returns>
        public static IEnumerable<string> GetFiles(string directoryPath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            if (IsExistsDirectory(directoryPath) || IsExistsDrive(directoryPath))
            {
                try
                {
                    return Directory
                        .EnumerateFiles(directoryPath)
                        .Where(CanAccess);
                }
                catch (ArgumentNullException)
                {
                    return [];
                }
                catch (ArgumentException)
                {
                    return [];
                }
                catch (DirectoryNotFoundException)
                {
                    return [];
                }
                catch (PathTooLongException)
                {
                    return [];
                }
                catch (IOException)
                {
                    return [];
                }
                catch (SecurityException)
                {
                    return [];
                }
                catch (UnauthorizedAccessException)
                {
                    return [];
                }
            }
            else
            {
                throw new ArgumentException("ディレクトリまたはドライブ以外が指定されました。", nameof(directoryPath));
            }
        }

        public static bool HasSubDirectory(string directoryPath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            if (IsSystemRoot(directoryPath))
            {
                return true;
            }
            else if (IsExistsDirectory(directoryPath) || IsExistsDrive(directoryPath))
            {
                try
                {
                    var root = new DirectoryInfo(directoryPath);

                    return root
                        .Children()
                        .OfType<DirectoryInfo>()
                        .Where(dir => CanAccess(dir.FullName))
                        .Any();
                }
                catch (ArgumentNullException)
                {
                    return false;
                }
                catch (SecurityException)
                {
                    return false;
                }
                catch (ArgumentException)
                {
                    return false;
                }
                catch (PathTooLongException)
                {
                    return false;
                }
            }
            else
            {
                throw new ArgumentException("ファイルが指定されました。", nameof(directoryPath));
            }
        }

        /// <summary>
        /// フォルダ内のフォルダを取得します。
        /// </summary>
        /// <param name="directoryPath">フォルダパス</param>
        /// <returns></returns>
        public static string[] GetSubDirectoriesArray(string directoryPath, bool isSort)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            if (IsSystemRoot(directoryPath))
            {
                if (isSort)
                {
                    return [.. GetDrives().OrderBy(drive => drive, NaturalStringComparer.Windows)];
                }
                else
                {
                    return [.. GetDrives()];
                }
            }
            else if (IsExistsDirectory(directoryPath) || IsExistsDrive(directoryPath))
            {
                try
                {
                    if (!CanAccess(directoryPath))
                    {
                        return [];
                    }

                    var root = new DirectoryInfo(directoryPath);

                    if (isSort)
                    {
                        return root
                            .Children()
                            .OfType<DirectoryInfo>()
                            .Select(dir => dir.FullName)
                            .Where(CanAccess)
                            .OrderBy(dir => dir, NaturalStringComparer.Windows)
                            .ToArray();
                    }
                    else
                    {
                        return root
                            .Children()
                            .OfType<DirectoryInfo>()
                            .Select(dir => dir.FullName)
                            .Where(CanAccess)
                            .ToArray();
                    }
                }
                catch (ArgumentNullException)
                {
                    return [];
                }
                catch (SecurityException)
                {
                    return [];
                }
                catch (ArgumentException)
                {
                    return [];
                }
                catch (PathTooLongException)
                {
                    return [];
                }
            }
            else
            {
                throw new ArgumentException("ファイルが指定されました。", nameof(directoryPath));
            }
        }

        /// <summary>
        /// フォルダ内のファイルとフォルダを取得します。
        /// </summary>
        /// <param name="directoryPath">フォルダパス</param>
        /// <returns></returns>
        public static string[] GetFileSystemEntriesArray(string directoryPath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            if (IsSystemRoot(directoryPath))
            {
                return [.. GetDrives()];
            }
            else if (IsExistsDirectory(directoryPath) || IsExistsDrive(directoryPath))
            {
                try
                {
                    var root = new DirectoryInfo(directoryPath);

                    return root
                        .Children()
                        .Select(dir => dir.FullName)
                        .Where(CanAccess)
                        .ToArray();
                }
                catch (ArgumentNullException)
                {
                    return [];
                }
                catch (SecurityException)
                {
                    return [];
                }
                catch (ArgumentException)
                {
                    return [];
                }
                catch (PathTooLongException)
                {
                    return [];
                }
            }
            else
            {
                throw new ArgumentException("ファイルが指定されました。", nameof(directoryPath));
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
                return $"{fileSize.ToString()} B";
            }
            else if (fileSize < Math.Pow(1024f, 2))
            {
                return $"{Math.Round(fileSize / 1024f, 0).ToString()} KB";
            }
            else if (fileSize < Math.Pow(1024f, 3))
            {
                return $"{Math.Round(fileSize / Math.Pow(1024f, 2), 2).ToString()} MB";
            }
            else if (fileSize < Math.Pow(1024f, 4))
            {
                return $"{Math.Round(fileSize / Math.Pow(1024f, 3), 2).ToString()} GB";
            }
            else
            {
                return $"{Math.Round(fileSize / Math.Pow(1024f, 4), 2).ToString()} TB";
            }
        }

        public static void SelectApplication(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var hresult = WinApiMembers.OpenFileWith(IntPtr.Zero, filePath);
            switch (hresult)
            {
                case 0:
                    return;
                case -2147023673:
                    return;
                default:
                    throw new Win32Exception(hresult);
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
            catch (InvalidOperationException ex)
            {
                throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
            catch (PlatformNotSupportedException ex)
            {
                throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
        }

        public static void OpenExplorer(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (IsSystemRoot(filePath))
            {
                var _ = WinApiMembers.ShellExecute(
                    IntPtr.Zero, "open", "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}", null, null, 1);
            }
            else
            {
                var _ = WinApiMembers.ShellExecute(
                    IntPtr.Zero, "open", $"\"{filePath}\"", null, null, 1);
            }
        }

        public static void OpenExplorerSelect(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var pidlFolder = IntPtr.Zero;
            var pidlFile = IntPtr.Zero;

            try
            {
                var dirPath = GetParentDirectoryPath(filePath);

                // フォルダのPIDLを取得
                var hr = WinApiMembers.SHParseDisplayName(dirPath, IntPtr.Zero, out pidlFolder, 0, out var psfgaoOut);

                if (hr != 0)
                {
                    throw new FileUtilException($"'{dirPath}'のPIDLの取得に失敗しました。");
                }

                // ファイルのPIDLを取得
                hr = WinApiMembers.SHParseDisplayName(filePath, IntPtr.Zero, out pidlFile, 0, out psfgaoOut);

                if (hr != 0)
                {
                    WinApiMembers.CoTaskMemFree(pidlFolder);
                    throw new FileUtilException($"'{filePath}'のPIDLの取得に失敗しました。");
                }

                // 特定のファイルを選択した状態でフォルダを開く
                IntPtr[] fileArray = [pidlFile];
                hr = WinApiMembers.SHOpenFolderAndSelectItems(pidlFolder, (uint)fileArray.Length, fileArray, 0);

                if (hr != 0)
                {
                    var fileName = GetFileName(filePath);
                    throw new FileUtilException($"'{dirPath}'を開いて'{fileName}'を選択する処理に失敗しました。");
                }
            }
            finally
            {
                // PIDLを解放
                WinApiMembers.CoTaskMemFree(pidlFolder);
                WinApiMembers.CoTaskMemFree(pidlFile);
            }
        }

        // ファイルパスの末尾が"\"の場合取り除きます。
        private static string ToRemoveLastPathSeparate(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var length = filePath.Length;
            var lastChar = filePath.Substring(length - 1, 1);
            if (StringUtil.Compare(lastChar, @"\"))
            {
                return filePath[..(length - 1)];
            }
            else
            {
                return filePath;
            }
        }

        // ドライブリストを取得します。
        private static IEnumerable<string> GetDrives()
        {
            try
            {
                var drives = DriveInfo.GetDrives()
                    .Select(drive => ToRemoveLastPathSeparate(@$"{drive.Name}\"));
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

        private static string GetExtensionFast(string path)
        {
            ArgumentException.ThrowIfNullOrEmpty(path, nameof(path));

            var index = path.LastIndexOf('.');
            if (index <= path.LastIndexOf('\\') || index <= path.LastIndexOf('/'))
            {
                return string.Empty;
            }

            return path[index..];
        }

        public static unsafe string GetExtensionFastStack(string path)
        {
            ArgumentException.ThrowIfNullOrEmpty(path, nameof(path));

            if (path.Length > 256)
            {
                return GetExtensionFast(path);
            }

            Span<char> buffer = stackalloc char[path.Length];
            path.AsSpan().CopyTo(buffer);

            var index = buffer.LastIndexOf('.');
            if (index <= buffer.LastIndexOf('\\') || index <= buffer.LastIndexOf('/'))
            {
                return string.Empty;
            }

            return new string(buffer.Slice(index));
        }

        private static string CreateFileAccessErrorMessage(string path)
        {
            ArgumentException.ThrowIfNullOrEmpty(path, nameof(path));

            return $"'{path}'のアクセスに失敗しました。";
        }
    }
}
