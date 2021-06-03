using System;
using System.Collections.Generic;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// ファイルをタグで検索します。
    /// </summary>
    public class SearchFileByTagAsyncFacade
        : TwoWayFacadeBase<SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>>
    {
        public override void Execute(SingleValueEntity<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            SearchFileByTagAsyncLogic logic = new SearchFileByTagAsyncLogic(this);
            IList<string> fileList = logic.Execute(param.Value);

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
