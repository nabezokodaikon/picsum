using System;
using System.Collections.Generic;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// ファイルをタグで検索します。
    /// </summary>
    public class GetFileByTagAsyncFacade
        : TwoWayFacadeBase<SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>>
    {
        public override void Execute(SingleValueEntity<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            GetFileByTagAsyncLogic logic = new GetFileByTagAsyncLogic(this);
            IList<FileByTagDto> dtoList = logic.Execute(param.Value);

            GetFileShallowInfoAsyncLogic getInfoLogic = new GetFileShallowInfoAsyncLogic(this);
            ListEntity<FileShallowInfoEntity> infoList = new ListEntity<FileShallowInfoEntity>();
            foreach (FileByTagDto dto in dtoList)
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
