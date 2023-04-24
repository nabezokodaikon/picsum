using System;
using System.Collections.Generic;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;
using SWF.Common;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// 次のフォルダを取得します。
    /// </summary>
    public class GetNextDirectoryAsyncFacade
        : TwoWayFacadeBase<GetNextContentsParameter<string>, SingleValueEntity<string>>
    {
        public override void Execute(GetNextContentsParameter<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            if (param.CurrentParameter == null)
            {
                throw new ArgumentException("カレントパラメータがNULLです。", "param");
            }

            if (string.IsNullOrEmpty(param.CurrentParameter.Value))
            {
                throw new ArgumentException("カレントパラメータが空文字です。", "param");
            }

            List<string> list = null;
            string parentDirectory = FileUtil.GetParentDirectoryPath(param.CurrentParameter.Value);
            if (string.IsNullOrEmpty(parentDirectory))
            {
                list = new List<string>((new GetDrivesAsyncLogic(this)).Execute());
            }
            else
            {
                list = new List<string>((new GetSubDirectorysAsyncLogic(this)).Execute(parentDirectory));
            }

            list.Sort((x, y) => x.CompareTo(y));
            int index = list.IndexOf(param.CurrentParameter.Value);
            if (index < 0)
            {
                return;
            }

            SingleValueEntity<string> result = new SingleValueEntity<string>();
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
