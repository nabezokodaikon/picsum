using System;
using System.Collections.Generic;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// ファイルを評価値で検索します。
    /// </summary>
    public class SearchFileByRatingAsyncFacade
        : TwoWayFacadeBase<SingleValueEntity<int>, ListEntity<FileShallowInfoEntity>>
    {
        public override void Execute(SingleValueEntity<int> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            SearchFileByRatingAsyncLogic logic = new SearchFileByRatingAsyncLogic(this);
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
