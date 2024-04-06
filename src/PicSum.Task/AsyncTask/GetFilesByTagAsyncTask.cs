using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using SWF.Common;
using System;
using System.Runtime.Versioning;

namespace PicSum.Task.AsyncTask
{
    /// <summary>
    /// ファイルをタグで検索します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class GetFilesByTagAsyncTask
        : AbstractAsyncTask<ValueParameter<string>, ListResult<FileShallowInfoEntity>>
    {
        protected override void Execute(ValueParameter<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var logic = new GetFileByTagAsyncLogic(this);
            var dtoList = logic.Execute(param.Value);

            var getInfoLogic = new GetFileShallowInfoAsyncLogic(this);
            var infoList = new ListResult<FileShallowInfoEntity>();
            foreach (var dto in dtoList)
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
                catch (FileUtilException)
                {
                    continue;
                }
            }

            this.Callback(infoList);
        }
    }
}
