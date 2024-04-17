using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Entities;
using PicSum.Job.Logics;
using SWF.Common;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// サブフォルダ取得非同期ジョブ
    /// </summary>
    /// <remarks>フォルダパスが空文字の場合、ドライブリストを取得します。</remarks>
    [SupportedOSPlatform("windows")]
    public sealed class SubDirectoriesGetJob
        : AbstractTwoWayJob<ValueParameter<string>, ListResult<FileShallowInfoEntity>>
    {
        protected override void Execute(ValueParameter<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var subDirectorys = (new SubDirectoriesGetLogic(this)).Execute(param.Value);

            var logic = new FileShallowInfoGetLogic(this);
            var result = new ListResult<FileShallowInfoEntity>();
            foreach (var subDirectory in subDirectorys)
            {
                this.CheckCancel();

                try
                {
                    result.Add(logic.Execute(subDirectory));
                }
                catch (FileUtilException ex)
                {
                    this.WriteErrorLog(new JobException(this.ID, ex));
                    continue;
                }
            }

            this.Callback(result);
        }
    }
}
