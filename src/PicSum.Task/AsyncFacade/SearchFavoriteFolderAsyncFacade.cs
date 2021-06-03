using System;
using System.Collections.Generic;
using PicSum.Core.Data.FileAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    public class SearchFavoriteFolderAsyncFacade
        : TwoWayFacadeBase<SearchFavoriteFolderParameterEntity, ListEntity<FileShallowInfoEntity>>
    {
        public override void Execute(SearchFavoriteFolderParameterEntity param)
        {
            if (param == null)
            {
                throw new ArgumentNullException();
            }

            SearchFavoriteFolderAsyncLogic logic = new SearchFavoriteFolderAsyncLogic(this);
            IList<string> fileList = logic.Execute();

            GetFileShallowInfoAsyncLogic getInfoLogic = new GetFileShallowInfoAsyncLogic(this);
            ListEntity<FileShallowInfoEntity> infoList = new ListEntity<FileShallowInfoEntity>();
            foreach (string file in fileList)
            {
                CheckCancel();

                if (infoList.Count >= param.Count)
                {
                    break;
                }

                if (param.IsOnlyFolder &&
                    (string.IsNullOrEmpty(file) || FileUtil.IsDrive(file)))
                {
                    continue;
                }

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
