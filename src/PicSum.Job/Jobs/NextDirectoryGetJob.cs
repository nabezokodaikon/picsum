using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// 次のフォルダを取得します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class NextDirectoryGetJob
        : AbstractTwoWayJob<NextDirectoryGetParameter, ValueResult<string>>
    {
        protected override void Execute(NextDirectoryGetParameter param)
        {

            if (param.CurrentParameter == null)
            {
                throw new ArgumentException("カレントパラメータがNULLです。", nameof(param));
            }

            if (string.IsNullOrEmpty(param.CurrentParameter))
            {
                throw new ArgumentException("カレントパラメータが空文字です。", nameof(param));
            }

            string[] list;
            try
            {
                var parentDirectory = FileUtil.GetParentDirectoryPath(param.CurrentParameter);
                list = (new SubDirectoriesGetLogic(this)).Execute(parentDirectory);
            }
            catch (FileUtilException ex)
            {
                throw new JobException(this.ID, ex);
            }

            var index = Array.IndexOf(list, param.CurrentParameter);
            if (index < 0)
            {
                return;
            }

            var result = new ValueResult<string>();
            if (param.IsNext)
            {
                if (index + 1 > list.Length - 1)
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
                    result.Value = list[^1];
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
