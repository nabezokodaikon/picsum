using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using System.Runtime.Versioning;
using WinApi;

namespace SWF.Core.ResourceAccessor
{
    /// <summary>
    /// ファイルアイコンキャッシュクラス
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class FileIconCacher
        : IFileIconCacher
    {
        private bool _disposed = false;
        private readonly Lock _smallIconCashLock = new();
        private readonly Lock _extralargeIconCashLock = new();
        private readonly Lock _jumboIconCashLock = new();
        private readonly Dictionary<string, Bitmap> _smallIconCash = [];
        private readonly Dictionary<string, Bitmap> _extralargeIconCash = [];
        private readonly Dictionary<string, Bitmap> _jumboIconCash = [];
        private readonly Lazy<Bitmap> _emptyFileIcon = new(
            () => (Bitmap)ResourceFiles.EmptyIcon.Value,
            LazyThreadSafetyMode.ExecutionAndPublication);
        private readonly Lazy<Bitmap> _smallPcIcon = new(
            () => FileIconUtil.GetSystemIcon(WinApiMembers.ShellSpecialFolder.CSIDL_DRIVES, WinApiMembers.ShellFileInfoFlags.SHGFI_SMALLICON),
            LazyThreadSafetyMode.ExecutionAndPublication);
        private readonly Lazy<Bitmap> _largePcIcon = new(
            () => FileIconUtil.GetSystemIcon(WinApiMembers.ShellSpecialFolder.CSIDL_DRIVES, WinApiMembers.ShellFileInfoFlags.SHGFI_LARGEICON),
            LazyThreadSafetyMode.ExecutionAndPublication);
        private readonly Lazy<Bitmap> _smallDirectoryIcon = new(
            () => FileIconUtil.GetSmallIconByFilePath(AppFiles.APPLICATION_DIRECTORY.Value),
            LazyThreadSafetyMode.ExecutionAndPublication);
        private readonly Lazy<Bitmap> _extralargeDirectoryIcon = new(
            () => FileIconUtil.GetLargeIconByFilePath(AppFiles.APPLICATION_DIRECTORY.Value, WinApiMembers.SHIL.SHIL_EXTRALARGE),
            LazyThreadSafetyMode.ExecutionAndPublication);
        private readonly Lazy<Bitmap> _jumboDirectoryIcon = new(
            () => FileIconUtil.GetLargeIconByFilePath(AppFiles.APPLICATION_DIRECTORY.Value, WinApiMembers.SHIL.SHIL_JUMBO),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public Image SmallPCIcon
        {
            get
            {
                return this._smallPcIcon.Value;
            }
        }

        public Image LargePCIcon
        {
            get
            {
                return this._largePcIcon.Value;
            }
        }

        public Image SmallDirectoryIcon
        {
            get
            {
                return this._smallDirectoryIcon.Value;
            }
        }

        public Image ExtraLargeDirectoryIcon
        {
            get
            {
                return this._extralargeDirectoryIcon.Value;
            }
        }

        public Image JumboDirectoryIcon
        {
            get
            {
                return this._jumboDirectoryIcon.Value;
            }
        }

        public FileIconCacher()
        {

        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this._smallPcIcon.Value.Dispose();
                this._largePcIcon.Value.Dispose();
                this._smallDirectoryIcon.Value.Dispose();
                this._extralargeDirectoryIcon.Value.Dispose();
                this._jumboDirectoryIcon.Value.Dispose();

                foreach (var cache in this._smallIconCash)
                {
                    cache.Value.Dispose();
                }

                foreach (var cache in this._extralargeIconCash)
                {
                    cache.Value.Dispose();
                }

                foreach (var cache in this._jumboIconCash)
                {
                    cache.Value.Dispose();
                }
            }

            this._disposed = true;
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
                return this._emptyFileIcon.Value;
            }

            var ex = FileUtil.GetExtensionFastStack(filePath);

            lock (this._smallIconCashLock)
            {
                if (this._smallIconCash.TryGetValue(ex, out var cashIcon))
                {
                    return cashIcon;
                }
                else
                {
                    var icon = FileIconUtil.GetSmallIconByFilePath(filePath);
                    this._smallIconCash.Add(ex, icon);
                    return icon;
                }
            }
        }

        public Image GetExtraLargeFileIcon(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (!ImageUtil.IsImageFile(filePath))
            {
                return this._emptyFileIcon.Value;
            }

            var ex = FileUtil.GetExtensionFastStack(filePath);

            lock (this._extralargeIconCashLock)
            {
                if (this._extralargeIconCash.TryGetValue(ex, out var cashIcon))
                {
                    return cashIcon;
                }
                else
                {
                    var icon = FileIconUtil.GetLargeIconByFilePath(filePath, WinApiMembers.SHIL.SHIL_EXTRALARGE);
                    this._extralargeIconCash.Add(ex, icon);
                    return icon;
                }
            }
        }

        public Bitmap GetJumboFileIcon(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            if (!ImageUtil.IsImageFile(filePath))
            {
                return this._emptyFileIcon.Value;
            }

            var ex = FileUtil.GetExtensionFastStack(filePath);

            lock (this._jumboIconCashLock)
            {
                if (this._jumboIconCash.TryGetValue(ex, out var cashIcon))
                {
                    return cashIcon;
                }
                else
                {
                    var icon = FileIconUtil.GetLargeIconByFilePath(filePath, WinApiMembers.SHIL.SHIL_JUMBO);
                    this._jumboIconCash.Add(ex, icon);
                    return icon;
                }
            }
        }
    }
}
