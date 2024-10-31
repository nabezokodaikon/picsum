using SWF.Core.Base;
using System.Reflection;
using System.Runtime.Versioning;
using WinApi;
using static WinApi.WinApiMembers;

namespace SWF.Core.FileAccessor
{
    /// <summary>
    /// ファイルアイコンキャッシュクラス
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class FileIconCash
    {
        private static readonly object SMALL_ICON_CASH_LOCK = new();
        private static readonly object LARGE_ICON_CASH_LOCK = new();
        private static readonly object JUMBO_ICON_CASH_LOCK = new();
        private static readonly Dictionary<string, Bitmap> SMALL_ICON_CASH = [];
        private static readonly Dictionary<string, Bitmap> LARGE_ICON_CASH = [];
        private static readonly Dictionary<string, Bitmap> JUMBO_ICON_CASH = [];
        private static readonly Bitmap OTHER_FILE_ICON = FileIconCash.GetOtherFileIcon();
        private static readonly Bitmap SMALL_PC_ICON =
            FileUtil.GetSmallSystemIcon(WinApiMembers.ShellSpecialFolder.CSIDL_DRIVES);
        private static readonly Bitmap LARGE_PC_ICON =
            FileUtil.GetLargeSystemIcon(WinApiMembers.ShellSpecialFolder.CSIDL_DRIVES);
        private static readonly Bitmap SMALL_DIRECTORY_ICON =
            FileUtil.GetSmallIconByFilePath(FileUtil.GetParentDirectoryPath(Assembly.GetExecutingAssembly().Location));
        private static readonly Bitmap LARGE_DIRECTORY_ICON =
            FileUtil.GetExtraLargeIconByFilePath(FileUtil.GetParentDirectoryPath(Assembly.GetExecutingAssembly().Location), SHIL.SHIL_EXTRALARGE);
        private static readonly Bitmap JUMBO_DIRECTORY_ICON =
            FileUtil.GetExtraLargeIconByFilePath(FileUtil.GetParentDirectoryPath(Assembly.GetExecutingAssembly().Location), SHIL.SHIL_JUMBO);

        public static Image SmallMyComputerIcon
        {
            get
            {
                return SMALL_PC_ICON;
            }
        }

        public static Image LargeMyComputerIcon
        {
            get
            {
                return LARGE_PC_ICON;
            }
        }

        public static Image SmallDirectoryIcon
        {
            get
            {
                return SMALL_DIRECTORY_ICON;
            }
        }

        public static Image LargeDirectoryIcon
        {
            get
            {
                return LARGE_DIRECTORY_ICON;
            }
        }

        public static Image JumboDirectoryIcon
        {
            get
            {
                return JUMBO_DIRECTORY_ICON;
            }
        }

        public static Bitmap GetOtherFileIcon()
        {
            var filePath = ResourceFiles.EmptyIcon;
            return (Bitmap)FileUtil.GetExtraLargeIconByFilePath(filePath, SHIL.SHIL_EXTRALARGE);
        }

        public static Image GetSmallDriveIcon(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return FileUtil.GetSmallIconByFilePath(filePath);
        }

        public static Image GetLargeDriveIcon(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return FileUtil.GetExtraLargeIconByFilePath(filePath, SHIL.SHIL_JUMBO);
        }

        public static Image GetSmallFileIcon(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (!FileUtil.IsImageFile(filePath))
            {
                return OTHER_FILE_ICON;
            }

            var ex = FileUtil.GetExtension(filePath);

            lock (SMALL_ICON_CASH_LOCK)
            {
                if (SMALL_ICON_CASH.TryGetValue(ex, out var cashIcon))
                {
                    return cashIcon;
                }
                else
                {
                    var icon = FileUtil.GetSmallIconByFilePath(filePath);
                    SMALL_ICON_CASH.Add(ex, icon);
                    return icon;
                }
            }
        }

        public static Image GetLargeFileIcon(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (!FileUtil.IsImageFile(filePath))
            {
                return OTHER_FILE_ICON;
            }

            var ex = FileUtil.GetExtension(filePath);

            lock (LARGE_ICON_CASH_LOCK)
            {
                if (LARGE_ICON_CASH.TryGetValue(ex, out var cashIcon))
                {
                    return cashIcon;
                }
                else
                {
                    var icon = FileUtil.GetExtraLargeIconByFilePath(filePath, SHIL.SHIL_EXTRALARGE);
                    LARGE_ICON_CASH.Add(ex, icon);
                    return icon;
                }
            }
        }

        public static Bitmap GetJumboFileIcon(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (!FileUtil.IsImageFile(filePath))
            {
                return OTHER_FILE_ICON;
            }

            var ex = FileUtil.GetExtension(filePath);

            lock (JUMBO_ICON_CASH_LOCK)
            {
                if (JUMBO_ICON_CASH.TryGetValue(ex, out var cashIcon))
                {
                    return cashIcon;
                }
                else
                {
                    var icon = FileUtil.GetExtraLargeIconByFilePath(filePath, SHIL.SHIL_JUMBO);
                    JUMBO_ICON_CASH.Add(ex, icon);
                    return icon;
                }
            }
        }

        /// <summary>
        /// 静的リソースを解放します。
        /// </summary>
        public static void DisposeStaticResources()
        {
            SMALL_PC_ICON.Dispose();
            LARGE_PC_ICON.Dispose();
            SMALL_DIRECTORY_ICON.Dispose();
            LARGE_DIRECTORY_ICON.Dispose();
            JUMBO_DIRECTORY_ICON.Dispose();
        }
    }
}
