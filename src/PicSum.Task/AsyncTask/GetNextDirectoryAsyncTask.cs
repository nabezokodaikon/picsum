using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;
using SWF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;

namespace PicSum.Task.AsyncTask
{
    /// <summary>
    /// 次のフォルダを取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class GetNextDirectoryAsyncTask
        : TwoWayTaskBase<GetNextContentsParameter<string>, SingleValueEntity<string>>
    {
        public override void Execute(GetNextContentsParameter<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            if (param.CurrentParameter == null)
            {
                throw new ArgumentException("カレントパラメータがNULLです。", nameof(param));
            }

            if (string.IsNullOrEmpty(param.CurrentParameter.Value))
            {
                throw new ArgumentException("カレントパラメータが空文字です。", nameof(param));
            }

            IList<string> list;
            try
            {
                var parentDirectory = FileUtil.GetParentDirectoryPath(param.CurrentParameter.Value);
                list = (new GetSubDirectorysAsyncLogic(this)).Execute(parentDirectory);
            }
            catch (FileUtilException)
            {
                return;
            }

            list.OrderBy(f => f).ToList();
            var index = list.IndexOf(param.CurrentParameter.Value);
            if (index < 0)
            {
                return;
            }

            var result = new SingleValueEntity<string>();
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

            this.OnCallback(result);
        }
    }
}
