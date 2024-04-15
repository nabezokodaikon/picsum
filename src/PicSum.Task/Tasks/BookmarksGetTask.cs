using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Entities;
using PicSum.Task.Logics;
using SWF.Common;
using System.Runtime.Versioning;

namespace PicSum.Task.Tasks
{
    [SupportedOSPlatform("windows")]
    public sealed class BookmarksGetTask
        : AbstractTwoWayTask<ListResult<FileShallowInfoEntity>>
    {
        protected override void Execute()
        {
            var getBookmarkLogic = new BookmarksGetLogic(this);
            var dtoList = getBookmarkLogic.Execute();

            var getInfoLogic = new FileShallowInfoGetLogic(this);
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
                catch (FileUtilException ex)
                {
                    this.WriteErrorLog(new TaskException(this.ID, ex));
                    continue;
                }
            }

            this.Callback(infoList);
        }
    }
}
