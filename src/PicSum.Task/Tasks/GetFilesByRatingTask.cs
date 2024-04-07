using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Logics;
using PicSum.Task.Entities;
using SWF.Common;
using System;
using System.Runtime.Versioning;

namespace PicSum.Task.Tasks
{
    /// <summary>
    /// ファイルを評価値で検索します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class GetFilesByRatingTask
        : AbstractTwoWayTask<ValueParameter<int>, ListResult<FileShallowInfoEntity>>
    {
        protected override void Execute(ValueParameter<int> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var logic = new GetFileByRatingLogic(this);
            var fileList = logic.Execute(param.Value);

            var getInfoLogic = new GetFileShallowInfoLogic(this);
            var infoList = new ListResult<FileShallowInfoEntity>();
            foreach (var dto in fileList)
            {
                this.CheckCancel();

                try
                {
                    var info = getInfoLogic.Execute(dto.FilePath, dto.RegistrationDate);
                    if (info != null)
                    {
                        infoList.Add(info);
                    }
                }
                catch (FileUtilException ex)
                {
                    this.WriteErrorLog(ex);
                    continue;
                }
            }

            this.Callback(infoList);
        }
    }
}
