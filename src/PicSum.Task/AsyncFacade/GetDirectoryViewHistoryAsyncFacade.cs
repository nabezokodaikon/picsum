using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using System.Runtime.Versioning;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// フォルダの表示履歴を取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class GetDirectoryViewHistoryAsyncFacade
        : TwoWayFacadeBase<ListEntity<FileShallowInfoEntity>>
    {
        public override void Execute()
        {
            var logic = new GetFileShallowInfoAsyncLogic(this);
            var result = new ListEntity<FileShallowInfoEntity>();
            foreach (var directoryPath in (new GetDirectoryViewHistoryAsyncLogic(this)).Execute())
            {
                this.CheckCancel();
                result.Add(logic.Execute(directoryPath));
            }

            this.OnCallback(result);
        }
    }
}
