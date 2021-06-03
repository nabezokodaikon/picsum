using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// フォルダの表示履歴を取得します。
    /// </summary>
    public class GetFolderViewHistoryAsyncFacade
        : TwoWayFacadeBase<ListEntity<FileShallowInfoEntity>>
    {
        public override void Execute()
        {
            GetFileShallowInfoAsyncLogic logic = new GetFileShallowInfoAsyncLogic(this);
            ListEntity<FileShallowInfoEntity> result = new ListEntity<FileShallowInfoEntity>();            
            foreach (string folderPath in (new GetFolderViewHistoryAsyncLogic(this)).Execute())
            {
                CheckCancel();
                result.Add(logic.Execute(folderPath));
            }

            OnCallback(result);
        }
    }
}
