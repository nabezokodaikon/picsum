using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using System;
using System.Runtime.Versioning;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// ファイルを評価値で検索します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class GetFilesByRatingAsyncFacade
        : TwoWayFacadeBase<SingleValueEntity<int>, ListEntity<FileShallowInfoEntity>>
    {
        public override void Execute(SingleValueEntity<int> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var logic = new GetFileByRatingAsyncLogic(this);
            var fileList = logic.Execute(param.Value);

            var getInfoLogic = new GetFileShallowInfoAsyncLogic(this);
            var infoList = new ListEntity<FileShallowInfoEntity>();
            foreach (var dto in fileList)
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
