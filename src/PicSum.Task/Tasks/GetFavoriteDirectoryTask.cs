using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Logics;
using PicSum.Task.Entities;
using PicSum.Task.Paramters;
using SWF.Common;
using System;
using System.Runtime.Versioning;

namespace PicSum.Task.Tasks
{
    [SupportedOSPlatform("windows")]
    public sealed class GetFavoriteDirectoryTask
        : AbstractAsyncTask<GetFavoriteDirectoryParameter, ListResult<FileShallowInfoEntity>>
    {
        protected override void Execute(GetFavoriteDirectoryParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var logic = new GetFavoriteDirectoryLogic(this);
            var fileList = logic.Execute();

            var getInfoLogic = new GetFileShallowInfoLogic(this);
            var infoList = new ListResult<FileShallowInfoEntity>();
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

                try
                {
                    var info = getInfoLogic.Execute(file);
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