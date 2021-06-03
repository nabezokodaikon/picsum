using System.Collections.Generic;
using System.Threading;

namespace PicSum.Core.Data.DatabaseAccessor
{
    /// <summary>
    /// SQL管理
    /// </summary>
    public static class SqlManager
    {
        // 排他を制御するオブジェクト
        private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        // SQLフォルダ
        private static string _sqlFolder = string.Empty;

        // SQLディクショナリ
        private static readonly Dictionary<string, string> _sqlDictionary = new Dictionary<string, string>();

        /// <summary>
        /// SQLフォルダ
        /// </summary>
        public static string SqlFolder
        {
            get
            {
                return _sqlFolder;
            }
            set
            {
                _sqlFolder = value;
            }
        }

        /// <summary>
        /// SQL文字列を取得します。
        /// </summary>
        /// <param name="sqlFileName">データアクセサ名称</param>
        /// <returns>SQL文</returns>
        public static string GetSqlText(string sqlName)
        {
            _lock.EnterUpgradeableReadLock();

            try
            {
                if (!_sqlDictionary.ContainsKey(sqlName))
                {
                    _lock.EnterWriteLock();

                    try
                    {
                        string sql = SqlFileUtil.ReadSqlFile(_sqlFolder, sqlName);
                        _sqlDictionary.Add(sqlName, sql);
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }
                }

                return _sqlDictionary[sqlName];
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }
        }
    }
}
