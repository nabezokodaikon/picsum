using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using System;
using System.Runtime.Versioning;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// ファイルをタグで検索します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class GetFilesByTagAsyncFacade
        : TwoWayFacadeBase<SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>>
    {
        public override void Execute(SingleValueEntity<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var logic = new GetFileByTagAsyncLogic(this);
            var dtoList = logic.Execute(param.Value);

            var getInfoLogic = new GetFileShallowInfoAsyncLogic(this);
            var infoList = new ListEntity<FileShallowInfoEntity>();
            foreach (var dto in dtoList)
            {
                this.CheckCancel();

                var info = getInfoLogic.Execute(dto.FilePath, dto.RegistrationDate);
                if (info != null)
                {
                    infoList.Add(info);
                }
            }

            this.OnCallback(infoList);
        }
    }
}
