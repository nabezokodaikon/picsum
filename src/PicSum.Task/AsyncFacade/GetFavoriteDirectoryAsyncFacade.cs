using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;
using SWF.Common;
using System;

namespace PicSum.Task.AsyncFacade
{
    public sealed class GetFavoriteDirectoryAsyncFacade
        : TwoWayFacadeBase<GetFavoriteFolderParameter, ListEntity<FileShallowInfoEntity>>
    {
        public override void Execute(GetFavoriteFolderParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var logic = new GetFavoriteDirectoryAsyncLogic(this);
            var fileList = logic.Execute();

            var getInfoLogic = new GetFileShallowInfoAsyncLogic(this);
            var infoList = new ListEntity<FileShallowInfoEntity>();
            foreach (var file in fileList)
            {
                this.CheckCancel();

                if (infoList.Count >= param.Count)
                {
                    break;
                }

                if (param.IsOnlyDirectory &&
                    (string.IsNullOrEmpty(file) || FileUtil.IsDrive(file)))
                {
                    continue;
                }

                var info = getInfoLogic.Execute(file);
                if (info != null)
                {
                    infoList.Add(info);
                }
            }

            this.OnCallback(infoList);
        }
    }
}
