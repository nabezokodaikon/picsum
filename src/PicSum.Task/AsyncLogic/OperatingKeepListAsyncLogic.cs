using System;
using System.Collections.Generic;
using System.Threading;
using PicSum.Core.Task.AsyncTask;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// キープリスト操作非同期ロジック
    /// </summary>
    internal class OperatingKeepListAsyncLogic : AsyncLogicBase
    {
        private static readonly List<string> _keepList = new List<string>();
        private static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public OperatingKeepListAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public void AddKeep(IList<string> filePathList)
        {
            if (filePathList == null)
            {
                throw new ArgumentNullException("filePathList");
            }

            _lock.EnterWriteLock();

            try
            {
                foreach (string filePath in filePathList)
                {
                    _keepList.Remove(filePath);
                    _keepList.Insert(0, filePath);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void RemoveKeep(IList<string> filePathList)
        {
            if (filePathList == null)
            {
                throw new ArgumentNullException("filePathList");
            }

            _lock.EnterWriteLock();

            try
            {
                foreach (string filePath in filePathList)
                {
                    _keepList.Remove(filePath);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public IList<string> GetKeep()
        {
            _lock.EnterReadLock();

            try
            {
                return new List<string>(_keepList.ToArray());
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }
}
