using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SWF.Common
{
    /// <summary>
    /// ファイルアイコンキャッシュクラス
    /// </summary>
    public static class FileIconCash
    {
        private static readonly ReaderWriterLockSlim _smallIconCashLock = new ReaderWriterLockSlim();
        private static readonly ReaderWriterLockSlim _largeIconCashLock = new ReaderWriterLockSlim();
        private static Dictionary<string, Image> _smallIconCash = null;
        private static Dictionary<string, Image> _largeIconCash = null;
        private static Image _smallMyComputerIcon = null;
        private static Image _largeMyComputerIcon = null;
        private static Image _smallFolderIcon = null;
        private static Image _largeFolderIcon = null;

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

        public static Image SmallFolderIcon
        {
            get
            {
                return _smallFolderIcon;
            }
        }

        public static Image LargeFolderIcon
        {
            get
            {
                return _largeFolderIcon;
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

            return FileUtil.GetExtraLargeIconByFilePath(filePath);
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
                if (_smallIconCash.ContainsKey(ex))
                {
                    return _smallIconCash[ex];
                }
                else
                {
                    _smallIconCashLock.EnterWriteLock();

                    try
                    {
                        Image icon = FileUtil.GetSmallIconByExtension(ex);
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
                if (_largeIconCash.ContainsKey(ex))
                {
                    return _largeIconCash[ex];
                }
                else
                {
                    _largeIconCashLock.EnterWriteLock();

                    try
                    {
                        Image icon = FileUtil.GetLargeIconByExtension(ex);
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

        public static void Init()
        {
            _smallIconCash = new Dictionary<string, Image>();
            _largeIconCash = new Dictionary<string, Image>();
            _smallMyComputerIcon = FileUtil.GetSmallSystemIcon(WinApi.WinApiMembers.ShellSpecialFolder.CSIDL_DRIVES);
            _largeMyComputerIcon = FileUtil.GetLargeSystemIcon(WinApi.WinApiMembers.ShellSpecialFolder.CSIDL_DRIVES);
            _smallFolderIcon = FileUtil.GetSmallIconByFilePath(FileUtil.GetParentFolderPath(Assembly.GetExecutingAssembly().Location));
            _largeFolderIcon = FileUtil.GetExtraLargeIconByFilePath(FileUtil.GetParentFolderPath(Assembly.GetExecutingAssembly().Location));
        }

        /// <summary>
        /// 静的リソースを解放します。
        /// </summary>
        public static void DisposeStaticResouces()
        {
            _smallIconCashLock.Dispose();
            _largeIconCashLock.Dispose();
            _smallMyComputerIcon.Dispose();
            _largeMyComputerIcon.Dispose();
            _smallFolderIcon.Dispose();
            _largeFolderIcon.Dispose();
        }
    }
}
