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
            KeepListOperatingAsyncLogic logic = new KeepListOperatingAsyncLogic(this);
            IList<KeepFileEntity> keepFileList = logic.GetKeep();

            GetFileShallowInfoAsyncLogic getInfoLogic = new GetFileShallowInfoAsyncLogic(this);
            ListEntity<FileShallowInfoEntity> infoList = new ListEntity<FileShallowInfoEntity>();
            foreach (var keepFile in keepFileList)
            {
                CheckCancel();

                FileShallowInfoEntity info = getInfoLogic.Execute(keepFile.FilePath, keepFile.RegistrationDate);
                if (info != null)
                {
                    infoList.Add(info);
                }
            }

            OnCallback(infoList);
        }
    }
}
