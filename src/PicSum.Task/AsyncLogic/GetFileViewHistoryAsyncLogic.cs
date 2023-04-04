using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// ファイル表示履歴を取得します。
    /// </summary>
    public class GetFileViewHistoryAsyncLogic : AsyncLogicBase
    {
        private readonly static List<DateTime> ViewDateList = new List<DateTime>();
        private readonly static ReaderWriterLockSlim ViewDateListLock = new ReaderWriterLockSlim();

        /// <summary>
        /// 静的リソースを解放します。
        /// </summary>
        public static void DisposeStaticResouces()
        {
            ViewDateListLock.Dispose();
        }

        private List<DateTime> viewDateList
        {
            get
            {
                return ViewDateList;
            }
        }

        private ReaderWriterLockSlim viewDateListLock
        {
            get
            {
                return ViewDateListLock;
            }
        }

        public GetFileViewHistoryAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public IList<DateTime> Execute()
        {
            viewDateListLock.EnterUpgradeableReadLock();

            try
            {
                if (viewDateList.Count > 0 && hasToDay(viewDateList, DateTime.Now))
                {
                    return viewDateList;
                }
                else
                {
                    viewDateListLock.EnterWriteLock();

                    try
                    {
                        viewDateList.Clear();
                        viewDateList.AddRange(getViewDateList());
                        return viewDateList;
                    }
                    finally
                    {
                        viewDateListLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                viewDateListLock.ExitUpgradeableReadLock();
            }
        }

        private List<DateTime> getViewDateList()
        {
            ReadFileViewHistorySql sql = new ReadFileViewHistorySql();
            IList<SingleValueDto<DateTime>> dtoList = DatabaseManager<FileInfoConnection>.ReadList<SingleValueDto<DateTime>>(sql);

            List<DateTime> list = new List<DateTime>();
            foreach (SingleValueDto<DateTime> dto in dtoList)
            {
                CheckCancel();
                list.Add(dto.Value);
            }

            DateTime now = DateTime.Now;
            if (!hasToDay(list, now))
            {
                list.Add(now);
            }

            return list;
        }

        private bool hasToDay(IList<DateTime> list, DateTime now)
        {
            string nowString = now.ToString("yyyyMMdd");
            DateTime result = list.SingleOrDefault(d => d.ToString("yyyyMMdd").Equals(nowString, StringComparison.Ordinal));
            return !result.Equals(DateTime.MinValue);
        }
    }
}
