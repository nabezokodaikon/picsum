using System;
using System.Collections.Generic;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// ファイルを評価値で検索します。
    /// </summary>
    public class GetFileByRatingAsyncFacade
        : TwoWayFacadeBase<SingleValueEntity<int>, ListEntity<FileShallowInfoEntity>>
    {
        public override void Execute(SingleValueEntity<int> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            GetFileByRatingAsyncLogic logic = new GetFileByRatingAsyncLogic(this);
            IList<FileByRatingDto> fileList = logic.Execute(param.Value);

            GetFileShallowInfoAsyncLogic getInfoLogic = new GetFileShallowInfoAsyncLogic(this);
            ListEntity<FileShallowInfoEntity> infoList = new ListEntity<FileShallowInfoEntity>();
            foreach (FileByRatingDto dto in fileList)
            {
                CheckCancel();

                FileShallowInfoEntity info = getInfoLogic.Execute(dto.FilePath, dto.RegistrationDate);
                if (info != null)
                {
                    infoList.Add(info);
                }
            }

            OnCallback(infoList);
        }
    }
}
