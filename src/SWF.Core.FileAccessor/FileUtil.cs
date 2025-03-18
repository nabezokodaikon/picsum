using SWF.Core.Base;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Text;
using WinApi;

namespace SWF.Core.FileAccessor
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class FileUtil
    {
        public enum ImageFileFormat
        {
            Avif,
            Bitmap,
            Heif,
            Icon,
            Jpeg,
            Png,
            Svg,
            Webp,
        }

        private const string ROOT_DIRECTORY_NAME = "PC";
        private const string ROOT_DIRECTORY_TYPE_NAME = "System root";

        internal const string AVIF_FILE_EXTENSION = ".AVIF";
        internal const string BMP_FILE_EXTENSION = ".BMP";
        internal const string GIF_FILE_EXTENSION = ".GIF";
        internal const string ICON_FILE_EXTENSION = ".ICO";
        internal const string JPEG_FILE_EXTENSION = ".JPEG";
        internal const string JPG_FILE_EXTENSION = ".JPG";
        internal const string HEIC_FILE_EXTENSION = ".HEIC";
        internal const string HEIF_FILE_EXTENSION = ".HEIF";
        internal const string PNG_FILE_EXTENSION = ".PNG";
        internal const string SVG_FILE_EXTENSION = ".SVG";
        internal const string WEBP_FILE_EXTENSION = ".WEBP";

        internal static readonly string[] IMAGE_FILE_EXTENSION_LIST = GetImageFileExtensionList();

        public const string ROOT_DIRECTORY_PATH =
            "1435810adf6f3080e21df9c3b666c7887883da42ad582d911a81931c38e720da1235036c60c69389e8c4fcc26be0c796626ef8ed3296bd9c65445ff12168fb22";
        public static readonly DateTime ROOT_DIRECTORY_DATETIME = DateTime.MinValue;
        public static readonly DateTime EMPTY_DATETIME = DateTime.MinValue;

        /// <summary>
        /// ファイル、フォルダの存在を確認します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>ファイルまたはフォルダが存在するならTrue。存在しなければFalse。</returns>
        public static bool IsExists(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

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
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            return filePath.Trim() == ROOT_DIRECTORY_PATH;
        }

        /// <summary>
        /// ファイルパスがドライブであるか確認します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>ファイルパスがドライブならTrue。ドライブでなければFalse。</returns>
        public static bool IsDrive(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

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
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

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
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
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
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            var ex = GetExtension(filePath);
            return IMAGE_FILE_EXTENSION_LIST.Contains(ex);
        }

        public static bool IsSvgFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            var ex = GetExtension(filePath);
            return (ex == SVG_FILE_EXTENSION);
        }

        public static bool IsIconFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            var ex = GetExtension(filePath);
            return (ex == ICON_FILE_EXTENSION);
        }

        public static bool IsBmpFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            var ex = GetExtension(filePath);
            return (ex == BMP_FILE_EXTENSION);
        }

        public static bool IsJpegFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            var ex = GetExtension(filePath);
            return (ex == JPG_FILE_EXTENSION || ex == JPEG_FILE_EXTENSION);
        }

        public static bool IsPngFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            var ex = GetExtension(filePath);
            return (ex == PNG_FILE_EXTENSION || ex == PNG_FILE_EXTENSION);
        }

        public static bool IsGifFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            var ex = GetExtension(filePath);
            return (ex == GIF_FILE_EXTENSION);
        }

        /// <summary>
        /// 指定したファイルがWEBPファイルであるか確認します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsWebpFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            var ex = GetExtension(filePath);
            return (ex == WEBP_FILE_EXTENSION);
        }

        /// <summary>
        /// 指定したファイルがAVIFファイルであるか確認します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsAvifFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            var ex = GetExtension(filePath);
            return (ex == AVIF_FILE_EXTENSION);
        }

        public static bool IsHeifFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            var ex = GetExtension(filePath);
            return (ex == HEIC_FILE_EXTENSION || ex == HEIF_FILE_EXTENSION);
        }

        /// <summary>
        /// ファイルにアクセス可能か確認します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>隠しファイル、システムファイルならFalse。それ以外ならTrue。</returns>
        public static bool CanAccess(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }
            else if (IsSystemRoot(filePath))
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
            if (string.IsNullOrEmpty(filePath))
            {
                return string.Empty;
            }

            if (IsSystemRoot(filePath))
            {
                return ROOT_DIRECTORY_NAME;
            }
            else if (IsDrive(filePath))
            {
                try
                {
                    var driveInfo = DriveInfo.GetDrives()
                        .FirstOrDefault(di => di.Name == filePath || di.Name == $"{filePath}");
                    if (driveInfo != null)
                    {
                        return $"{driveInfo.VolumeLabel}({ToRemoveLastPathSeparate(filePath)})";
                    }
                    else
                    {
                        return ROOT_DIRECTORY_NAME;
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

        public static string GetFileNameWithoutExtension(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return string.Empty;
            }

            if (IsSystemRoot(filePath))
            {
                return ROOT_DIRECTORY_NAME;
            }
            else if (IsDrive(filePath))
            {
                try
                {
                    var driveInfo = DriveInfo.GetDrives()
                        .FirstOrDefault(di => di.Name == filePath || di.Name == $"{filePath}");
                    if (driveInfo != null)
                    {
                        return $"{driveInfo.VolumeLabel}({ToRemoveLastPathSeparate(filePath)})";
                    }
                    else
                    {
                        return ROOT_DIRECTORY_NAME;
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
                return Path.GetFileNameWithoutExtension(filePath);
            }
        }

        /// <summary>
        /// 親フォルダパスを取得します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>親フォルダパス</returns>
        public static string GetParentDirectoryPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return string.Empty;
            }

            if (IsSystemRoot(filePath))
            {
                throw new ArgumentException("システムルートが指定されました。", nameof(filePath));
            }

            if (IsDrive(filePath))
            {
                return ROOT_DIRECTORY_PATH;
            }
            else
            {
                try
                {
                    var parent = Path.GetDirectoryName(filePath);
                    return parent ?? string.Empty;
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
            if (string.IsNullOrEmpty(filePath))
            {
                return string.Empty;
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
            if (string.IsNullOrEmpty(filePath))
            {
                return string.Empty;
            }

            if (IsSystemRoot(filePath))
            {
                return ROOT_DIRECTORY_TYPE_NAME;
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
            if (string.IsNullOrEmpty(filePath))
            {
                return EMPTY_DATETIME;
            }

            if (IsSystemRoot(filePath))
            {
                return ROOT_DIRECTORY_DATETIME;
            }

            try
            {
                if (CanAccess(filePath))
                {
                    return File.GetLastWriteTime(filePath);
                }
                else
                {
                    return EMPTY_DATETIME;
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
        }

        /// <summary>
        /// ファイルサイズを取得します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static long GetFileSize(string filePath)
        {
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
        public static string? GetFirstImageFilePath(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                return string.Empty;
            }

            var imageFilePath = GetFiles(directoryPath)
                .OrderBy(_ => _, NaturalStringComparer.Windows)
                .FirstOrDefault(IsImageFile);

            if (string.IsNullOrEmpty(imageFilePath))
            {
                return string.Empty;
            }

            return imageFilePath;
        }

        /// <summary>
        /// フォルダ内のファイルを取得します。
        /// </summary>
        /// <param name="directoryPath">フォルダパス</param>
        /// <returns></returns>
        public static IEnumerable<string> GetFiles(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                return [];
            }

            if (CanAccess(directoryPath))
            {
                try
                {
                    return Directory
                        .EnumerateFiles(directoryPath)
                        .Where(CanAccess);
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
        public static IEnumerable<string> GetSubDirectories(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                return [];
            }

            if (IsSystemRoot(directoryPath))
            {
                return GetDriveList();
            }

            if (CanAccess(directoryPath))
            {
                try
                {
                    return Directory
                        .EnumerateDirectories(directoryPath)
                        .Where(CanAccess);
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
        public static IEnumerable<string> GetFileSystemEntries(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                return [];
            }

            if (IsSystemRoot(directoryPath))
            {
                return GetDriveList();
            }

            if (CanAccess(directoryPath))
            {
                try
                {
                    return Directory
                        .EnumerateFileSystemEntries(directoryPath)
                        .Where(CanAccess);
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
            else if (fileSize < Math.Pow(1024f, 2))
            {
                return Math.Round(fileSize / 1024f, 0).ToString() + " KB";
            }
            else if (fileSize < Math.Pow(1024f, 3))
            {
                return Math.Round(fileSize / Math.Pow(1024f, 2), 2).ToString() + " MB";
            }
            else if (fileSize < Math.Pow(1024f, 4))
            {
                return Math.Round(fileSize / Math.Pow(1024f, 3), 2).ToString() + " GB";
            }
            else if (fileSize < Math.Pow(1024f, 5))
            {
                return Math.Round(fileSize / Math.Pow(1024f, 4), 2).ToString() + " TB";
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(fileSize));
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
            catch (FileNotFoundException ex)
            {
                throw new FileUtilException(CreateFileAccessErrorMessage(filePath), ex);
            }
        }

        public static void OpenExplorer(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (IsSystemRoot(filePath))
            {
                var _ = WinApiMembers.ShellExecute(IntPtr.Zero, "open", "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}", null, null, 1);
            }
            else
            {
                var _ = WinApiMembers.ShellExecute(IntPtr.Zero, "open", $"\"{filePath}\"", null, null, 1);
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

        public static string GetImageFileExtension(ImageFileFormat imageFileFormat)
        {
            return imageFileFormat switch
            {
                ImageFileFormat.Avif => AVIF_FILE_EXTENSION,
                ImageFileFormat.Bitmap => BMP_FILE_EXTENSION,
                ImageFileFormat.Heif => HEIF_FILE_EXTENSION,
                ImageFileFormat.Icon => ICON_FILE_EXTENSION,
                ImageFileFormat.Jpeg => JPG_FILE_EXTENSION,
                ImageFileFormat.Png => PNG_FILE_EXTENSION,
                ImageFileFormat.Svg => SVG_FILE_EXTENSION,
                ImageFileFormat.Webp => WEBP_FILE_EXTENSION,
                _ => throw new NotImplementedException("未定義の画像ファイルフォーマットです。")
            };
        }

        public static string GetConvertFilterText(string filePath, ImageFileFormat imageFileFormat)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var extensionText = GetImageFileExtension(imageFileFormat)[1..];
            return $"{extensionText.ToUpper()} Files (*.{extensionText.ToLower()})|*.{extensionText.ToLower()}";
        }

        // ファイルパスの末尾が"\"の場合取り除きます。
        private static string ToRemoveLastPathSeparate(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return string.Empty;
            }

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
        private static IEnumerable<string> GetDriveList()
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

        private static string CreateFileAccessErrorMessage(string path)
        {
            return $"'{path}'を開けませんでした。";
        }

        /// <summary>
        /// 画像ファイルの拡張子リストを取得します。
        /// </summary>
        /// <remarks>リスト内の各項目には、ピリオド + 英大文字 * n の文字列(.XXX)が格納されます。</remarks>
        /// <returns></returns>
        private static string[] GetImageFileExtensionList()
        {
            return
            [
                AVIF_FILE_EXTENSION,
                BMP_FILE_EXTENSION,
                GIF_FILE_EXTENSION,
                ICON_FILE_EXTENSION,
                JPEG_FILE_EXTENSION,
                JPG_FILE_EXTENSION,
                HEIC_FILE_EXTENSION,
                HEIF_FILE_EXTENSION,
                PNG_FILE_EXTENSION,
                SVG_FILE_EXTENSION,
                WEBP_FILE_EXTENSION
            ];
        }
    }
}
