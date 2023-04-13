using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// フォルダの表示履歴を取得します。
    /// </summary>
    public class GetDirectoryViewHistoryAsyncFacade
        : TwoWayFacadeBase<ListEntity<FileShallowInfoEntity>>
    {
        public override void Execute()
        {
            GetFileShallowInfoAsyncLogic logic = new GetFileShallowInfoAsyncLogic(this);
            ListEntity<FileShallowInfoEntity> result = new ListEntity<FileShallowInfoEntity>();            
            foreach (string directoryPath in (new GetDirectoryViewHistoryAsyncLogic(this)).Execute())
            {
                CheckCancel();
                result.Add(logic.Execute(directoryPath));
            }

            OnCallback(result);
        }
    }
}
