using System;
using System.Collections.Generic;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using SWF.Common;

namespace PicSum.Task.AsyncFacade
{
    public class GetFavoriteDirectoryAsyncFacade
        : TwoWayFacadeBase<SearchFavoriteDirectoryParameterEntity, ListEntity<FileShallowInfoEntity>>
    {
        public override void Execute(SearchFavoriteDirectoryParameterEntity param)
        {
            if (param == null)
            {
                throw new ArgumentNullException();
            }

            GetFavoriteDirectoryAsyncLogic logic = new GetFavoriteDirectoryAsyncLogic(this);
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

                if (param.IsOnlyDirectory &&
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
