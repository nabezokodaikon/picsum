using System.Collections.Generic;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// キープされているファイルを取得します。
    /// </summary>
    public class GetKeepAsyncFacade
        : TwoWayFacadeBase<ListEntity<FileShallowInfoEntity>>
    {
        public override void Execute()
        {
            OperatingKeepListAsyncLogic logic = new OperatingKeepListAsyncLogic(this);
            IList<string> fileList = logic.GetKeep();

            GetFileShallowInfoAsyncLogic getInfoLogic = new GetFileShallowInfoAsyncLogic(this);
            ListEntity<FileShallowInfoEntity> infoList = new ListEntity<FileShallowInfoEntity>();
            foreach (string file in fileList)
            {
                CheckCancel();

                FileShallowInfoEntity info = getInfoLogic.Execute(file);
                if (info != null)
                {
                    infoList.Add(info);
                }
            }

            OnCallback(infoList);
        }
    }
}
