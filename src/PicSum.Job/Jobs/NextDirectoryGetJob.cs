using PicSum.Job.Parameters;
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

            var parentDirectory = FileUtil.GetParentDirectoryPath(param.CurrentParameter);
            var dirs = FileUtil.GetSubDirectoriesArray(parentDirectory, true);

            var index = Array.IndexOf(dirs, param.CurrentParameter);
            if (index < 0)
            {
                return;
            }

            var result = new ValueResult<string>();
            if (param.IsNext)
            {
                if (index + 1 > dirs.Length - 1)
                {
                    result.Value = dirs[0];
                }
                else
                {
                    result.Value = dirs[index + 1];
                }
            }
            else
            {
                if (index - 1 < 0)
                {
                    result.Value = dirs[^1];
                }
                else
                {
                    result.Value = dirs[index - 1];
                }
            }

            this.Callback(result);
        }
    }
}
