using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Logics;
using PicSum.Task.Paramters;
using SWF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;

namespace PicSum.Task.Tasks
{
    /// <summary>
    /// 次のフォルダを取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class GetNextDirectoryTask
        : AbstractAsyncTask<GetNextContentsParameter<string>, ValueResult<string>>
    {
        protected override void Execute(GetNextContentsParameter<string> param)
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
                list = (new GetSubDirectorysLogic(this)).Execute(parentDirectory);
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

            var result = new ValueResult<string>();
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

            this.Callback(result);
        }
    }
}
