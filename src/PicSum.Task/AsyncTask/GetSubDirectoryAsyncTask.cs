using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using SWF.Common;
using System;
using System.Runtime.Versioning;

namespace PicSum.Task.AsyncTask
{
    /// <summary>
    /// サブフォルダ取得非同期タスク
    /// </summary>
    /// <remarks>フォルダパスが空文字の場合、ドライブリストを取得します。</remarks>
    [SupportedOSPlatform("windows")]
    public sealed class GetSubDirectoryAsyncTask
        : AbstractAsyncTask<ValueParameter<string>, ListResult<FileShallowInfoEntity>>
    {
        protected override void Execute(ValueParameter<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var subDirectorys = (new GetSubDirectorysAsyncLogic(this)).Execute(param.Value);

            var logic = new GetFileShallowInfoAsyncLogic(this);
            var result = new ListResult<FileShallowInfoEntity>();
            foreach (var subDirectory in subDirectorys)
            {
                this.CheckCancel();

                try
                {
                    result.Add(logic.Execute(subDirectory));
                }
                catch (FileUtilException)
                {
                    continue;
                }                
            }

            this.Callback(result);
        }
    }
}
