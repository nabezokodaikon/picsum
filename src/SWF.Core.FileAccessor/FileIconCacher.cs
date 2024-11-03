using SWF.Core.Base;
using System.Reflection;
using System.Runtime.Versioning;
using WinApi;

namespace SWF.Core.FileAccessor
{
    /// <summary>
    /// ファイルアイコンキャッシュクラス
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed partial class FileIconCacher
        : IDisposable
    {
        public readonly static FileIconCacher Instance = new();

        private bool disposed = false;
        private readonly object SMALL_ICON_CASH_LOCK = new();
        private readonly object LARGE_ICON_CASH_LOCK = new();
        private readonly object JUMBO_ICON_CASH_LOCK = new();
        private readonly Dictionary<string, Bitmap> SMALL_ICON_CASH = [];
        private readonly Dictionary<string, Bitmap> LARGE_ICON_CASH = [];
        private readonly Dictionary<string, Bitmap> JUMBO_ICON_CASH = [];
        private readonly Bitmap OTHER_FILE_ICON = (Bitmap)ResourceFiles.EmptyIcon.Value;
        private readonly Bitmap SMALL_PC_ICON =
            FileIconUtil.GetSmallSystemIcon(WinApiMembers.ShellSpecialFolder.CSIDL_DRIVES);
        private readonly Bitmap LARGE_PC_ICON =
            FileIconUtil.GetLargeSystemIcon(WinApiMembers.ShellSpecialFolder.CSIDL_DRIVES);
        private readonly Bitmap SMALL_DIRECTORY_ICON =
            FileIconUtil.GetSmallIconByFilePath(FileUtil.GetParentDirectoryPath(Assembly.GetExecutingAssembly().Location));
        private readonly Bitmap LARGE_DIRECTORY_ICON =
            FileIconUtil.GetExtraLargeIconByFilePath(FileUtil.GetParentDirectoryPath(Assembly.GetExecutingAssembly().Location), WinApiMembers.SHIL.SHIL_EXTRALARGE);
        private readonly Bitmap JUMBO_DIRECTORY_ICON =
            FileIconUtil.GetExtraLargeIconByFilePath(FileUtil.GetParentDirectoryPath(Assembly.GetExecutingAssembly().Location), WinApiMembers.SHIL.SHIL_JUMBO);

        public Image SmallMyComputerIcon
        {
            get
            {
                return this.SMALL_PC_ICON;
            }
        }

        public Image LargeMyComputerIcon
        {
            get
            {
                return this.LARGE_PC_ICON;
            }
        }

        public Image SmallDirectoryIcon
        {
            get
            {
                return this.SMALL_DIRECTORY_ICON;
            }
        }

        public Image LargeDirectoryIcon
        {
            get
            {
                return this.LARGE_DIRECTORY_ICON;
            }
        }

        public Image JumboDirectoryIcon
        {
            get
            {
                return this.JUMBO_DIRECTORY_ICON;
            }
        }

        private FileIconCacher()
        {

        }

        ~FileIconCacher()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.SMALL_PC_ICON.Dispose();
                this.LARGE_PC_ICON.Dispose();
                this.SMALL_DIRECTORY_ICON.Dispose();
                this.LARGE_DIRECTORY_ICON.Dispose();
                this.JUMBO_DIRECTORY_ICON.Dispose();

                foreach (var cache in this.SMALL_ICON_CASH)
                {
                    cache.Value.Dispose();
                }

                foreach (var cache in this.LARGE_ICON_CASH)
                {
                    cache.Value.Dispose();
                }

                foreach (var cache in this.JUMBO_ICON_CASH)
                {
                    cache.Value.Dispose();
                }
            }

            this.disposed = true;
        }

        public Image GetSmallDriveIcon(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return FileIconUtil.GetSmallIconByFilePath(filePath);
        }

        public Image GetLargeDriveIcon(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return FileIconUtil.GetExtraLargeIconByFilePath(filePath, WinApiMembers.SHIL.SHIL_JUMBO);
        }

        public Image GetSmallFileIcon(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (!FileUtil.IsImageFile(filePath))
            {
                return this.OTHER_FILE_ICON;
            }

            var ex = FileUtil.GetExtension(filePath);

            lock (this.SMALL_ICON_CASH_LOCK)
            {
                if (this.SMALL_ICON_CASH.TryGetValue(ex, out var cashIcon))
                {
                    return cashIcon;
                }
                else
                {
                    var icon = FileIconUtil.GetSmallIconByFilePath(filePath);
                    this.SMALL_ICON_CASH.Add(ex, icon);
                    return icon;
                }
            }
        }

        public Image GetLargeFileIcon(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (!FileUtil.IsImageFile(filePath))
            {
                return this.OTHER_FILE_ICON;
            }

            var ex = FileUtil.GetExtension(filePath);

            lock (this.LARGE_ICON_CASH_LOCK)
            {
                if (this.LARGE_ICON_CASH.TryGetValue(ex, out var cashIcon))
                {
                    return cashIcon;
                }
                else
                {
                    var icon = FileIconUtil.GetExtraLargeIconByFilePath(filePath, WinApiMembers.SHIL.SHIL_EXTRALARGE);
                    this.LARGE_ICON_CASH.Add(ex, icon);
                    return icon;
                }
            }
        }

        public Bitmap GetJumboFileIcon(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (!FileUtil.IsImageFile(filePath))
            {
                return this.OTHER_FILE_ICON;
            }

            var ex = FileUtil.GetExtension(filePath);

            lock (this.JUMBO_ICON_CASH_LOCK)
            {
                if (this.JUMBO_ICON_CASH.TryGetValue(ex, out var cashIcon))
                {
                    return cashIcon;
                }
                else
                {
                    var icon = FileIconUtil.GetExtraLargeIconByFilePath(filePath, WinApiMembers.SHIL.SHIL_JUMBO);
                    this.JUMBO_ICON_CASH.Add(ex, icon);
                    return icon;
                }
            }
        }
    }
}
