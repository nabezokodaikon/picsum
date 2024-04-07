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
    public sealed class GetSubDirectoryTask
        : AbstractTwoWayTask<ValueParameter<string>, ListResult<FileShallowInfoEntity>>
    {
        protected override void Execute(ValueParameter<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var subDirectorys = (new GetSubDirectorysLogic(this)).Execute(param.Value);

            var logic = new GetFileShallowInfoLogic(this);
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
                    this.WriteErrorLog(ex);
                    continue;
                }                
            }

            this.Callback(result);
        }
    }
}
