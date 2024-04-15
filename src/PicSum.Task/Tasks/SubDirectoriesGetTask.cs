using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Logics;
using PicSum.Task.Entities;
using SWF.Common;
using System;
using System.Runtime.Versioning;

namespace PicSum.Task.Tasks
{
    /// <summary>
    /// サブフォルダ取得非同期タスク
    /// </summary>
    /// <remarks>フォルダパスが空文字の場合、ドライブリストを取得します。</remarks>
    [SupportedOSPlatform("windows")]
    public sealed class SubDirectoriesGetTask
        : AbstractTwoWayTask<ValueParameter<string>, ListResult<FileShallowInfoEntity>>
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
                    this.WriteErrorLog(new TaskException(this.ID, ex));
                    continue;
                }                
            }

            this.Callback(result);
        }
    }
}
