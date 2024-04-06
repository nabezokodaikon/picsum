using PicSum.Core.Task.AsyncTask;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Logics;
using PicSum.Task.Entities;
using SWF.Common;
using System.Runtime.Versioning;

namespace PicSum.Task.Tasks
{
    [SupportedOSPlatform("windows")]
    public sealed class GetBookmarkTask
        : AbstractAsyncTask<EmptyParameter, ListResult<FileShallowInfoEntity>>
    {
        protected override void Execute(EmptyParameter parameter)
        {
            var getBookmarkLogic = new GetBookmarkListLogic(this);
            var dtoList = getBookmarkLogic.Execute();

            var getInfoLogic = new GetFileShallowInfoLogic(this);
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
