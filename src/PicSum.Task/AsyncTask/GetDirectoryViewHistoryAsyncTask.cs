using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using SWF.Common;
using System.Runtime.Versioning;

namespace PicSum.Task.AsyncTask
{
    /// <summary>
    /// フォルダの表示履歴を取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class GetDirectoryViewHistoryAsyncTask
        : AbstractAsyncTask<EmptyParameter, ListResult<FileShallowInfoEntity>>
    {
        protected override void Execute(EmptyParameter parameter)
        {
            var logic = new GetFileShallowInfoAsyncLogic(this);
            var result = new ListResult<FileShallowInfoEntity>();

            foreach (var directoryPath in (new GetDirectoryViewHistoryAsyncLogic(this)).Execute())
            {
                this.CheckCancel();

                try
                {
                    result.Add(logic.Execute(directoryPath));
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
