using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Threading;
using WinApi;
using static WinApi.WinApiMembers;

namespace SWF.Common
{
    /// <summary>
    /// ファイルアイコンキャッシュクラス
    /// </summary>
    public static class FileIconCash
    {
        private static readonly ReaderWriterLockSlim SMALL_ICON_CASH_LOCK = new ReaderWriterLockSlim();
        private static readonly ReaderWriterLockSlim LARGE_ICON_CASH_LOCK = new ReaderWriterLockSlim();
        private static readonly ReaderWriterLockSlim JUMBO_ICON_CASH_LOCK = new ReaderWriterLockSlim();
        private static readonly Dictionary<string, Image> SMALL_ICON_CASH = new Dictionary<string, Image>();
        private static readonly Dictionary<string, Image> LARGE_ICON_CASH = new Dictionary<string, Image>();
        private static readonly Dictionary<string, Image> JUMBO_ICON_CASH = new Dictionary<string, Image>();
        private static readonly Image SMALL_PC_ICON =
            FileUtil.GetSmallSystemIcon(WinApiMembers.ShellSpecialFolder.CSIDL_DRIVES);
        private static readonly Image LARGE_PC_ICON =
            FileUtil.GetLargeSystemIcon(WinApiMembers.ShellSpecialFolder.CSIDL_DRIVES);
        private static readonly Image SMALL_DIRECTORY_ICON =
            FileUtil.GetSmallIconByFilePath(FileUtil.GetParentDirectoryPath(Assembly.GetExecutingAssembly().Location));
        private static readonly Image LARGE_DIRECTORY_ICON =
            FileUtil.GetExtraLargeIconByFilePath(FileUtil.GetParentDirectoryPath(Assembly.GetExecutingAssembly().Location), SHIL.SHIL_EXTRALARGE);
        private static readonly Image JUMBO_DIRECTORY_ICON =
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

        public static Image GetSmallDriveIcon(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            return FileUtil.GetSmallIconByFilePath(filePath);
        }

        public static Image GetLargeDriveIcon(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            return FileUtil.GetExtraLargeIconByFilePath(filePath, SHIL.SHIL_JUMBO);
        }

        public static Image GetSmallFileIcon(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var ex = FileUtil.GetExtension(filePath);

            SMALL_ICON_CASH_LOCK.EnterUpgradeableReadLock();

            try
            {
                if (SMALL_ICON_CASH.TryGetValue(ex, out var cashIcon))
                {
                    return cashIcon;
                }
                else
                {
                    SMALL_ICON_CASH_LOCK.EnterWriteLock();

                    try
                    {
                        var icon = FileUtil.GetSmallIconByFilePath(filePath);
                        SMALL_ICON_CASH.Add(ex, icon);
                        return icon;
                    }
                    finally
                    {
                        SMALL_ICON_CASH_LOCK.ExitWriteLock();
                    }
                }
            }
            finally
            {
                SMALL_ICON_CASH_LOCK.ExitUpgradeableReadLock();
            }
        }

        public static Image GetLargeFileIcon(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var ex = FileUtil.GetExtension(filePath);

            LARGE_ICON_CASH_LOCK.EnterUpgradeableReadLock();

            try
            {
                if (LARGE_ICON_CASH.TryGetValue(ex, out var cashIcon))
                {
                    return cashIcon;
                }
                else
                {
                    LARGE_ICON_CASH_LOCK.EnterWriteLock();

                    try
                    {
                        var icon = FileUtil.GetExtraLargeIconByFilePath(filePath, SHIL.SHIL_EXTRALARGE);
                        LARGE_ICON_CASH.Add(ex, icon);
                        return icon;
                    }
                    finally
                    {
                        LARGE_ICON_CASH_LOCK.ExitWriteLock();
                    }
                }
            }
            finally
            {
                LARGE_ICON_CASH_LOCK.ExitUpgradeableReadLock();
            }
        }

        public static Image GetJumboFileIcon(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var ex = FileUtil.GetExtension(filePath);

            JUMBO_ICON_CASH_LOCK.EnterUpgradeableReadLock();

            try
            {
                if (JUMBO_ICON_CASH.TryGetValue(ex, out var cashIcon))
                {
                    return cashIcon;
                }
                else
                {
                    JUMBO_ICON_CASH_LOCK.EnterWriteLock();

                    try
                    {
                        var icon = FileUtil.GetExtraLargeIconByFilePath(filePath, SHIL.SHIL_JUMBO);
                        JUMBO_ICON_CASH.Add(ex, icon);
                        return icon;
                    }
                    finally
                    {
                        JUMBO_ICON_CASH_LOCK.ExitWriteLock();
                    }
                }
            }
            finally
            {
                JUMBO_ICON_CASH_LOCK.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// 静的リソースを解放します。
        /// </summary>
        public static void DisposeStaticResouces()
        {
            SMALL_ICON_CASH_LOCK.Dispose();
            LARGE_ICON_CASH_LOCK.Dispose();
            JUMBO_ICON_CASH_LOCK.Dispose();
            SMALL_PC_ICON.Dispose();
            LARGE_PC_ICON.Dispose();
            SMALL_DIRECTORY_ICON.Dispose();
            LARGE_DIRECTORY_ICON.Dispose();
            JUMBO_DIRECTORY_ICON.Dispose();
        }
    }
}
