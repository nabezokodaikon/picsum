using PicSum.Job.Entities;
using PicSum.Job.Logics;
using SWF.Core.FileAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// サブフォルダ取得非同期ジョブ
    /// </summary>
    /// <remarks>フォルダパスが空文字の場合、ドライブリストを取得します。</remarks>

    public sealed class SubDirectoriesGetJob
        : AbstractTwoWayJob<ValueParameter<string>, ListResult<FileShallowInfoEntity>>
    {
        protected override ValueTask Execute(ValueParameter<string> param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            if (string.IsNullOrEmpty(param.Value))
            {
                throw new ArgumentNullException(param.Value, nameof(param.Value));
            }

            var subDirectorys = FileUtil.GetSubDirectoriesArray(param.Value, true);
            var logic = new FileShallowInfoGetLogic(this);
            var result = new ListResult<FileShallowInfoEntity>(subDirectorys.Length);
            foreach (var subDirectory in subDirectorys)
            {
                this.ThrowIfJobCancellationRequested();

                try
                {
                    var info = logic.Get(subDirectory, false);
                    if (info != FileShallowInfoEntity.EMPTY)
                    {
                        result.Add(info);
                    }
                }
                catch (FileUtilException ex)
                {
                    this.WriteErrorLog(ex);
                    continue;
                }
            }

            this.Callback(result);

            return ValueTask.CompletedTask;
        }
    }
}
