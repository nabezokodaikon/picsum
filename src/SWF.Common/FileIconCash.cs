using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static WinApi.WinApiMembers;

namespace SWF.Common
{
    /// <summary>
    /// ファイルアイコンキャッシュクラス
    /// </summary>
    public static class FileIconCash
    {
        private static readonly ReaderWriterLockSlim _smallIconCashLock = new ReaderWriterLockSlim();
        private static readonly ReaderWriterLockSlim _largeIconCashLock = new ReaderWriterLockSlim();
        private static readonly ReaderWriterLockSlim _jumboIconCashLock = new ReaderWriterLockSlim();
        private static Dictionary<string, Image> _smallIconCash = null;
        private static Dictionary<string, Image> _largeIconCash = null;
        private static Dictionary<string, Image> _jumboIconCash = null;
        private static Image _smallMyComputerIcon = null;
        private static Image _largeMyComputerIcon = null;
        private static Image _smallDirectoryIcon = null;
        private static Image _largeDirectoryIcon = null;
        private static Image _jumboDirectoryIcon = null;

        public static Image SmallMyComputerIcon
        {
            get
            {
                return _smallMyComputerIcon;
            }
        }

        public static Image LargeMyComputerIcon
        {
            get
            {
                return _largeMyComputerIcon;
            }
        }

        public static Image SmallDirectoryIcon
        {
            get
            {
                return _smallDirectoryIcon;
            }
        }

        public static Image LargeDirectoryIcon
        {
            get
            {
                return _largeDirectoryIcon;
            }
        }

        public static Image JumboDirectoryIcon
        {
            get
            {
                return _jumboDirectoryIcon;
            }
        }

        public static Image GetSmallDriveIcon(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            return FileUtil.GetSmallIconByFilePath(filePath);
        }

        public static Image GetLargeDriveIcon(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            return FileUtil.GetExtraLargeIconByFilePath(filePath, SHIL.SHIL_JUMBO);
        }

        public static Image GetSmallFileIcon(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            string ex = FileUtil.GetExtension(filePath);

            _smallIconCashLock.EnterUpgradeableReadLock();

            try
            {
                Image cashIcon = null;
                if (_smallIconCash.TryGetValue(ex, out cashIcon))
                {
                    return cashIcon;
                }
                else
                {
                    _smallIconCashLock.EnterWriteLock();

                    try
                    {
                        Image icon = FileUtil.GetSmallIconByFilePath(filePath);
                        _smallIconCash.Add(ex, icon);
                        return icon;
                    }
                    finally
                    {
                        _smallIconCashLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _smallIconCashLock.ExitUpgradeableReadLock();
            }
        }

        public static Image GetLargeFileIcon(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            string ex = FileUtil.GetExtension(filePath);

            _largeIconCashLock.EnterUpgradeableReadLock();

            try
            {
                Image cashIcon = null;
                if (_largeIconCash.TryGetValue(ex, out cashIcon))
                {
                    return cashIcon;
                }
                else
                {
                    _largeIconCashLock.EnterWriteLock();

                    try
                    {
                        Image icon = FileUtil.GetExtraLargeIconByFilePath(filePath, SHIL.SHIL_EXTRALARGE);
                        _largeIconCash.Add(ex, icon);
                        return icon;
                    }
                    finally
                    {
                        _largeIconCashLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _largeIconCashLock.ExitUpgradeableReadLock();
            }
        }

        public static Image GetJumboFileIcon(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var ex = FileUtil.GetExtension(filePath);

            _jumboIconCashLock.EnterUpgradeableReadLock();

            try
            {
                Image cashIcon = null;
                if (_jumboIconCash.TryGetValue(ex, out cashIcon))
                {
                    return cashIcon;
                }
                else
                {
                    _jumboIconCashLock.EnterWriteLock();

                    try
                    {
                        var icon = FileUtil.GetExtraLargeIconByFilePath(filePath, SHIL.SHIL_JUMBO);
                        _jumboIconCash.Add(ex, icon);
                        return icon;
                    }
                    finally
                    {
                        _jumboIconCashLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _jumboIconCashLock.ExitUpgradeableReadLock();
            }
        }

        public static void Init()
        {
            _smallIconCash = new Dictionary<string, Image>();
            _largeIconCash = new Dictionary<string, Image>();
            _jumboIconCash = new Dictionary<string, Image>();
            _smallMyComputerIcon = FileUtil.GetSmallSystemIcon(WinApi.WinApiMembers.ShellSpecialFolder.CSIDL_DRIVES);
            _largeMyComputerIcon = FileUtil.GetLargeSystemIcon(WinApi.WinApiMembers.ShellSpecialFolder.CSIDL_DRIVES);
            _smallDirectoryIcon = FileUtil.GetSmallIconByFilePath(FileUtil.GetParentDirectoryPath(Assembly.GetExecutingAssembly().Location));
            _largeDirectoryIcon = FileUtil.GetExtraLargeIconByFilePath(FileUtil.GetParentDirectoryPath(Assembly.GetExecutingAssembly().Location), SHIL.SHIL_EXTRALARGE);
            _jumboDirectoryIcon = FileUtil.GetExtraLargeIconByFilePath(FileUtil.GetParentDirectoryPath(Assembly.GetExecutingAssembly().Location), SHIL.SHIL_JUMBO);
        }

        /// <summary>
        /// 静的リソースを解放します。
        /// </summary>
        public static void DisposeStaticResouces()
        {
            _smallIconCashLock.Dispose();
            _largeIconCashLock.Dispose();
            _jumboIconCashLock.Dispose();
            _smallMyComputerIcon.Dispose();
            _largeMyComputerIcon.Dispose();
            _smallDirectoryIcon.Dispose();
            _largeDirectoryIcon.Dispose();
            _jumboDirectoryIcon.Dispose();
        }
    }
}
