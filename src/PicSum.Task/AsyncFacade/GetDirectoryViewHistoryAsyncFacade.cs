using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// フォルダの表示履歴を取得します。
    /// </summary>
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
