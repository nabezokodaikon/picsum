using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// 次のフォルダを取得します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class NextDirectoryGetJob
        : AbstractTwoWayJob<NextDirectoryGetParameter<string>, ValueResult<string>>
    {
        protected override void Execute(NextDirectoryGetParameter<string> param)
        {

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
                list = (new SubDirectoriesGetLogic(this)).Execute(parentDirectory);
            }
            catch (FileUtilException ex)
            {
                throw new JobException(this.ID, ex);
            }

            var sortedList = list.OrderBy(f => f, NaturalStringComparer.Windows).ToList();
            var index = sortedList.IndexOf(param.CurrentParameter.Value);
            if (index < 0)
            {
                return;
            }

            var result = new ValueResult<string>();
            if (param.IsNext)
            {
                if (index + 1 > sortedList.Count - 1)
                {
                    result.Value = sortedList[0];
                }
                else
                {
                    result.Value = sortedList[index + 1];
                }
            }
            else
            {
                if (index - 1 < 0)
                {
                    result.Value = sortedList[^1];
                }
                else
                {
                    result.Value = sortedList[index - 1];
                }
            }

            this.Callback(result);
        }
    }
}
