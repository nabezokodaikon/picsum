using SWF.Core.Base;
using SWF.Core.StringAccessor;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using WinApi;
using ZLinq;

namespace SWF.Core.FileAccessor
{

    public static class FileUtil
    {
        public const string ROOT_DIRECTORY_NAME = "PC";
        public const string ROOT_DIRECTORY_TYPE_NAME = "System root";
        public const string ROOT_DIRECTORY_PATH = "36f780fdbda5b2b2ce85c9ebb57086d1880ae757";

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
                        .Any(d => StringUtil.CompareFilePath(d.Name, filePath));
                }
                catch (Exception ex) when (
                    ex is IOException ||
                    ex is UnauthorizedAccessException)
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
                return Directory.Exists(path);
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
                return File.Exists(path);
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
                if (FileUtil.IsSystemRoot(filePath))
                {
                    return true;
                }

                try
                {
                    if (IsExistsDrive(filePath))
                    {
                        var info = new DirectoryInfo(filePath);
                        var _ = info.GetAccessControl();
                        return true;
                    }

                    var attributes = WinApiMembers.GetFileAttributes(filePath);
                    if (attributes == WinApiMembers.INVALID_FILE_ATTRIBUTES)
                    {
                        var error = Marshal.GetLastWin32Error();
                        return error switch
                        {
                            // ファイルまたはパスが存在しない
                            WinApiMembers.ERROR_FILE_NOT_FOUND or
                            WinApiMembers.ERROR_PATH_NOT_FOUND => false,
                            // アクセスが拒否された
                            WinApiMembers.ERROR_ACCESS_DENIED => false,
                            // その他のエラー
                            _ => false,
                        };
                    }

                    return (attributes & WinApiMembers.FILE_ATTRIBUTE_HIDDEN) == 0;
                }
                catch (Exception ex) when (
                    ex is ArgumentNullException ||
                    ex is InvalidOperationException ||
                    ex is SecurityException ||
                    ex is ArgumentException ||
                    ex is UnauthorizedAccessException ||
                    ex is PathTooLongException ||
                    ex is NotSupportedException)
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

            if (FileUtil.IsSystemRoot(filePath))
            {
                return FileUtil.ROOT_DIRECTORY_NAME;
            }
            else if (IsExistsDrive(filePath))
            {
                try
                {
                    var driveInfo = DriveInfo.GetDrives()
                        .FirstOrDefault(di => di.Name == filePath || di.Name == $"{filePath}");
                    if (driveInfo == null)
                    {
                        throw new FileUtilException($"ファイル名を取得できませんでした。", filePath);
                    }

                    return $"{driveInfo.VolumeLabel}({ToRemoveLastPathSeparate(filePath)})";
                }
                catch (Exception ex) when (
                    ex is IOException ||
                    ex is UnauthorizedAccessException)
                {
                    throw new FileUtilException($"ファイル名を取得できませんでした。", filePath, ex);
                }
            }
            else
            {
                try
                {
                    var name = Path.GetFileName(filePath);
                    if (string.IsNullOrEmpty(name))
                    {
                        throw new FileUtilException($"ファイル名を取得できませんでした。", filePath);
                    }

                    return name;
                }
                catch (Exception ex) when (
                    ex is ArgumentException)
                {
                    throw new FileUtilException($"ファイル名を取得できませんでした。", filePath, ex);
                }
            }
        }

        /// <summary>
        /// 親フォルダパスを取得します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>親フォルダパス</returns>
        public static string GetParentDirectoryPath(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (IsExistsDrive(filePath))
            {
                return FileUtil.ROOT_DIRECTORY_PATH;
            }
            else
            {
                try
                {
                    var parent = Path.GetDirectoryName(filePath);
                    if (string.IsNullOrEmpty(parent))
                    {
                        throw new FileUtilException($"親ディレクトリパスを取得できませんでした。", filePath);
                    }

                    return parent;
                }
                catch (Exception ex) when (
                    ex is ArgumentException ||
                    ex is PathTooLongException)
                {
                    throw new FileUtilException($"親ディレクトリパスを取得できませんでした。", filePath, ex);
                }
            }
        }

        /// <summary>
        /// ファイル種類名称を取得します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>ファイル種類名称</returns>
        public static string GetTypeName(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (FileUtil.IsSystemRoot(filePath))
            {
                return FileUtil.ROOT_DIRECTORY_TYPE_NAME;
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

        public static DateTime GetCreateDate(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (FileUtil.IsSystemRoot(filePath))
            {
                return DateTimeExtensions.EMPTY;
            }

            try
            {
                return File.GetCreationTime(filePath);
            }
            catch (Exception ex) when (
                ex is UnauthorizedAccessException ||
                ex is PathTooLongException ||
                ex is NotSupportedException ||
                ex is ArgumentNullException ||
                ex is ArgumentException)
            {
                throw new FileUtilException($"作成日時を取得できませんでした。", filePath, ex);
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

            if (FileUtil.IsSystemRoot(filePath))
            {
                return DateTimeExtensions.EMPTY;
            }

            try
            {
                return File.GetLastWriteTime(filePath);
            }
            catch (Exception ex) when (
                ex is UnauthorizedAccessException ||
                ex is PathTooLongException ||
                ex is NotSupportedException ||
                ex is ArgumentNullException ||
                ex is ArgumentException)
            {
                throw new FileUtilException($"更新日時を取得できませんでした。", filePath, ex);
            }
        }

        /// <summary>
        /// ファイルサイズを取得します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static long GetFileSize(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            try
            {
                var fi = new FileInfo(filePath);
                return fi.Length;
            }
            catch (Exception ex) when (
                ex is SecurityException ||
                ex is UnauthorizedAccessException ||
                ex is PathTooLongException ||
                ex is NotSupportedException ||
                ex is ArgumentNullException ||
                ex is ArgumentException)
            {
                throw new FileUtilException($"ファイルサイズを取得できませんでした。", filePath, ex);
            }
        }

        public static (DateTime CreateDate, DateTime UpdateDate, long FileSize) GetFileInfo(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            try
            {
                var fi = new FileInfo(filePath);
                return (fi.CreationTime, fi.LastWriteTime, fi.Length);
            }
            catch (Exception ex) when (
                ex is SecurityException ||
                ex is UnauthorizedAccessException ||
                ex is PathTooLongException ||
                ex is NotSupportedException ||
                ex is ArgumentNullException ||
                ex is ArgumentException)
            {
                throw new FileUtilException($"ファイル情報を取得できませんでした。", filePath, ex);
            }
        }

        public static (DateTime CreateDate, DateTime UpdateDate) GetDirectoryInfo(string filePath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            try
            {
                var fi = new DirectoryInfo(filePath);
                return (fi.CreationTime, fi.LastWriteTime);
            }
            catch (Exception ex) when (
                ex is SecurityException ||
                ex is UnauthorizedAccessException ||
                ex is PathTooLongException ||
                ex is NotSupportedException ||
                ex is ArgumentNullException ||
                ex is ArgumentException)
            {
                throw new FileUtilException($"ディレクトリ情報を取得できませんでした。", filePath, ex);
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

            if (!CanAccess(directoryPath))
            {
                return [];
            }

            try
            {
                return Directory
                    .EnumerateFiles(directoryPath)
                    .AsEnumerable()
                    .Where(static _ => CanAccess(_));
            }
            catch (Exception ex) when (
                ex is ArgumentNullException ||
                ex is ArgumentException ||
                ex is DirectoryNotFoundException ||
                ex is PathTooLongException ||
                ex is IOException ||
                ex is SecurityException ||
                ex is UnauthorizedAccessException)
            {
                return [];
            }
        }

        public static bool HasSubDirectory(string directoryPath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            if (FileUtil.IsSystemRoot(directoryPath))
            {
                return true;
            }
            else
            {
                if (!CanAccess(directoryPath))
                {
                    return false;
                }

                try
                {
                    var root = new DirectoryInfo(directoryPath);

                    return root
                        .Children()
                        .OfType<DirectoryInfo>()
                        .Where(static dir => CanAccess(dir.FullName))
                        .Any();
                }
                catch (Exception ex) when (
                    ex is ArgumentNullException ||
                    ex is SecurityException ||
                    ex is ArgumentException ||
                    ex is PathTooLongException)
                {
                    return false;
                }
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

            if (FileUtil.IsSystemRoot(directoryPath))
            {
                if (isSort)
                {
                    return [.. GetDrives()
                        .AsEnumerable()
                        .OrderBy(static drive => drive, NaturalStringComparer.WINDOWS)];
                }
                else
                {
                    return [.. GetDrives()];
                }
            }
            else
            {
                if (!CanAccess(directoryPath))
                {
                    return [];
                }

                try
                {
                    var root = new DirectoryInfo(directoryPath);

                    if (isSort)
                    {
                        return root
                            .Children()
                            .OfType<DirectoryInfo>()
                            .Where(static dir => CanAccess(dir.FullName))
                            .OrderBy(static dir => dir.FullName, NaturalStringComparer.WINDOWS)
                            .Select(static dir => dir.FullName)
                            .ToArray();
                    }
                    else
                    {
                        return root
                            .Children()
                            .OfType<DirectoryInfo>()
                            .Where(static dir => CanAccess(dir.FullName))
                            .Select(static dir => dir.FullName)
                            .ToArray();
                    }
                }
                catch (Exception ex) when (
                    ex is ArgumentNullException ||
                    ex is SecurityException ||
                    ex is ArgumentException ||
                    ex is PathTooLongException)
                {
                    return [];
                }
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

            using (TimeMeasuring.Run(true, "FileUtil.GetFileSystemEntriesArray"))
            {
                if (FileUtil.IsSystemRoot(directoryPath))
                {
                    return [.. GetDrives()];
                }
                else
                {
                    try
                    {
                        if (!CanAccess(directoryPath))
                        {
                            return [];
                        }

                        var root = new DirectoryInfo(directoryPath);

                        return root
                            .Children()
                            .Where(static dir => CanAccess(dir.FullName))
                            .Select(static dir => dir.FullName)
                            .ToArray();
                    }
                    catch (Exception ex) when (
                        ex is ArgumentNullException ||
                        ex is SecurityException ||
                        ex is ArgumentException ||
                        ex is PathTooLongException)
                    {
                        return [];
                    }
                }
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
                return $"{fileSize} B";
            }
            else if (fileSize < Math.Pow(1024f, 2))
            {
                return $"{Math.Round(fileSize / 1024f, 0)} KB";
            }
            else if (fileSize < Math.Pow(1024f, 3))
            {
                return $"{Math.Round(fileSize / Math.Pow(1024f, 2), 2)} MB";
            }
            else if (fileSize < Math.Pow(1024f, 4))
            {
                return $"{Math.Round(fileSize / Math.Pow(1024f, 3), 2)} GB";
            }
            else
            {
                return $"{Math.Round(fileSize / Math.Pow(1024f, 4), 2)} TB";
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
            catch (Exception ex) when (
                ex is Win32Exception ||
                ex is ObjectDisposedException ||
                ex is InvalidOperationException ||
                ex is PlatformNotSupportedException)
            {
                throw new FileUtilException($"ファイルを実行できませんでした。", filePath, ex);
            }
        }

        public static void OpenExplorer(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (FileUtil.IsSystemRoot(filePath))
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
                    throw new FileUtilException($"PIDLの取得に失敗しました。", dirPath);
                }

                // ファイルのPIDLを取得
                hr = WinApiMembers.SHParseDisplayName(filePath, IntPtr.Zero, out pidlFile, 0, out psfgaoOut);

                if (hr != 0)
                {
                    WinApiMembers.CoTaskMemFree(pidlFolder);
                    throw new FileUtilException("PIDLの取得に失敗しました。", filePath);
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
            if (StringUtil.CompareFilePath(lastChar, @"\"))
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
                    .Select(static drive => ToRemoveLastPathSeparate(@$"{drive.Name}\"));
                return drives;
            }
            catch (Exception ex) when (
                ex is IOException ||
                ex is UnauthorizedAccessException)
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

            return new string(buffer[index..]);
        }
    }
}
