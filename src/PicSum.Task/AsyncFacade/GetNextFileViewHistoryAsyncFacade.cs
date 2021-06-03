using System;
using System.Collections.Generic;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    public class GetNextFileViewHistoryAsyncFacade
           : TwoWayFacadeBase<GetNextContentsParameterEntity<DateTime>, SingleValueEntity<DateTime>>
    {
        public override void Execute(GetNextContentsParameterEntity<DateTime> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            if (param.CurrentParameter == null)
            {
                throw new ArgumentException("カレントパラメータがNULLです。", "param");
            }

            GetFileViewHistoryAsyncLogic logic = new GetFileViewHistoryAsyncLogic(this);
            List<DateTime> list = new List<DateTime>(logic.Execute());

            list.Sort((x, y) => x.CompareTo(y));
            int index = list.IndexOf(param.CurrentParameter.Value);
            if (index < 0)
            {
                return;
            }

            SingleValueEntity<DateTime> result = new SingleValueEntity<DateTime>();
            if (param.IsNext)
            {
                if (index + 1 > list.Count - 1)
                {
                    result.Value = list[0];
                }
                else
                {
                    result.Value = list[index + 1];
                }
            }
            else
            {
                if (index - 1 < 0)
                {
                    result.Value = list[list.Count - 1];
                }
                else
                {
                    result.Value = list[index - 1];
                }
            }

            OnCallback(result);
        }
    }
}
