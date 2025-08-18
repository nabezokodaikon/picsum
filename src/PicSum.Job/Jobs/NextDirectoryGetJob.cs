using PicSum.Job.Parameters;
using SWF.Core.FileAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// 次のフォルダを取得します。
    /// </summary>

    public sealed class NextDirectoryGetJob
        : AbstractTwoWayJob<NextDirectoryGetParameter, ValueResult<string>>
    {
        protected override ValueTask Execute(NextDirectoryGetParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

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
                return ValueTask.CompletedTask;
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

            return ValueTask.CompletedTask;
        }
    }
}
