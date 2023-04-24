using System;
using System.Collections.Generic;
using System.Threading;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// キープリスト操作非同期ロジック
    /// </summary>
    public class KeepListOperatingAsyncLogic : AsyncLogicBase
    {
        private static readonly List<KeepFileEntity> _keepList = new List<KeepFileEntity>();
        private static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        /// <summary>
        /// 静的リソースを解放します。
        /// </summary>
        public static void DisposeStaticResouces()
        {
            _lock.Dispose();
        }

        public KeepListOperatingAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public void AddKeep(IList<KeepFileEntity> keepFileList)
        {
            if (keepFileList == null)
            {
                throw new ArgumentNullException("keepFileList");
            }

            _lock.EnterWriteLock();

            try
            {
                foreach (var keepFile in keepFileList)
                {
                    _keepList.Remove(keepFile);
                    _keepList.Insert(0, keepFile);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void RemoveKeep(IList<string> keepFileList)
        {
            if (keepFileList == null)
            {
                throw new ArgumentNullException("keepFileList");
            }

            _lock.EnterWriteLock();

            try
            {
                foreach (var keepFile in keepFileList)
                {
                    _keepList.Remove(new KeepFileEntity(keepFile));
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public IList<KeepFileEntity> GetKeep()
        {
            _lock.EnterReadLock();

            try
            {
                return new List<KeepFileEntity>(_keepList.ToArray());
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }
}
