using SWF.Core.Base;
using SWF.Core.Resource;
using System.Runtime.Versioning;
using WinApi;

namespace SWF.Core.ImageAccessor
{
    /// <summary>
    /// ファイルアイコンキャッシュクラス
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class FileIconCacher
        : IFileIconCacher
    {
        private bool disposed = false;
        private readonly Lock SMALL_ICON_CASH_LOCK = new();
        private readonly Lock EXTRALARGE_ICON_CASH_LOCK = new();
        private readonly Lock JUMBO_ICON_CASH_LOCK = new();
        private readonly Dictionary<string, Bitmap> SMALL_ICON_CASH = [];
        private readonly Dictionary<string, Bitmap> EXTRALARGE_ICON_CASH = [];
        private readonly Dictionary<string, Bitmap> JUMBO_ICON_CASH = [];
        private readonly Bitmap EMPTY_FILE_ICON = (Bitmap)ResourceFiles.EmptyIcon.Value;
        private readonly Bitmap SMALL_PC_ICON =
            FileIconUtil.GetSystemIcon(WinApiMembers.ShellSpecialFolder.CSIDL_DRIVES, WinApiMembers.ShellFileInfoFlags.SHGFI_SMALLICON);
        private readonly Bitmap LARGE_PC_ICON =
            FileIconUtil.GetSystemIcon(WinApiMembers.ShellSpecialFolder.CSIDL_DRIVES, WinApiMembers.ShellFileInfoFlags.SHGFI_LARGEICON);
        private readonly Bitmap SMALL_DIRECTORY_ICON =
            FileIconUtil.GetSmallIconByFilePath(AppConstants.APPLICATION_DIRECTORY.Value);
        private readonly Bitmap EXTRALARGE_DIRECTORY_ICON =
            FileIconUtil.GetLargeIconByFilePath(AppConstants.APPLICATION_DIRECTORY.Value, WinApiMembers.SHIL.SHIL_EXTRALARGE);
        private readonly Bitmap JUMBO_DIRECTORY_ICON =
            FileIconUtil.GetLargeIconByFilePath(AppConstants.APPLICATION_DIRECTORY.Value, WinApiMembers.SHIL.SHIL_JUMBO);

        public Image SmallPCIcon
        {
            get
            {
                return this.SMALL_PC_ICON;
            }
        }

        public Image LargePCIcon
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

        public Image ExtraLargeDirectoryIcon
        {
            get
            {
                return this.EXTRALARGE_DIRECTORY_ICON;
            }
        }

        public Image JumboDirectoryIcon
        {
            get
            {
                return this.JUMBO_DIRECTORY_ICON;
            }
        }

        public FileIconCacher()
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
                this.EXTRALARGE_DIRECTORY_ICON.Dispose();
                this.JUMBO_DIRECTORY_ICON.Dispose();

                foreach (var cache in this.SMALL_ICON_CASH)
                {
                    cache.Value.Dispose();
                }

                foreach (var cache in this.EXTRALARGE_ICON_CASH)
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

        public Image GetExtraLargeDriveIcon(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return FileIconUtil.GetLargeIconByFilePath(filePath, WinApiMembers.SHIL.SHIL_EXTRALARGE);
        }

        public Image GetJumboDriveIcon(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            return FileIconUtil.GetLargeIconByFilePath(filePath, WinApiMembers.SHIL.SHIL_JUMBO);
        }

        public Image GetSmallFileIcon(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (!ImageUtil.IsImageFile(filePath))
            {
                return this.EMPTY_FILE_ICON;
            }

            var ex = FileUtil.GetExtensionFastStack(filePath);

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

        public Image GetExtraLargeFileIcon(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (!ImageUtil.IsImageFile(filePath))
            {
                return this.EMPTY_FILE_ICON;
            }

            var ex = FileUtil.GetExtensionFastStack(filePath);

            lock (this.EXTRALARGE_ICON_CASH_LOCK)
            {
                if (this.EXTRALARGE_ICON_CASH.TryGetValue(ex, out var cashIcon))
                {
                    return cashIcon;
                }
                else
                {
                    var icon = FileIconUtil.GetLargeIconByFilePath(filePath, WinApiMembers.SHIL.SHIL_EXTRALARGE);
                    this.EXTRALARGE_ICON_CASH.Add(ex, icon);
                    return icon;
                }
            }
        }

        public Bitmap GetJumboFileIcon(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (!ImageUtil.IsImageFile(filePath))
            {
                return this.EMPTY_FILE_ICON;
            }

            var ex = FileUtil.GetExtensionFastStack(filePath);

            lock (this.JUMBO_ICON_CASH_LOCK)
            {
                if (this.JUMBO_ICON_CASH.TryGetValue(ex, out var cashIcon))
                {
                    return cashIcon;
                }
                else
                {
                    var icon = FileIconUtil.GetLargeIconByFilePath(filePath, WinApiMembers.SHIL.SHIL_JUMBO);
                    this.JUMBO_ICON_CASH.Add(ex, icon);
                    return icon;
                }
            }
        }
    }
}
