using PicSum.Job.Entities;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{

    public sealed class FileDeepInfoLoadingJob
        : AbstractTwoWayJob<FileDeepInfoGetParameter, FileDeepInfoGetResult>
    {
        protected override async ValueTask Execute(FileDeepInfoGetParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            if (param.FilePathList == null)
            {
                throw new NotSupportedException("ファイルパスリストがNULLです。");
            }

            await Task.Delay(10, this.CancellationToken).WithConfig();

            var result = this.CreateCallbackResult(param);
            this.Callback(result);
        }

        private FileDeepInfoGetResult CreateCallbackResult(FileDeepInfoGetParameter param)
        {
            if (param.FilePathList == null)
            {
                throw new NotSupportedException("ファイルパスリストがNULLです。");
            }

            if (param.FilePathList.Length == 1)
            {
                var result = new FileDeepInfoGetResult
                {
                    FilePathList = param.FilePathList,
                };

                try
                {
                    var filePath = param.FilePathList[0];
                    var deepInfoGetLogic = new FileDeepInfoGetLogic(this);
                    result.FileInfo = deepInfoGetLogic.Get(filePath, param.ThumbnailSize, false);
                    return result;
                }
                catch (Exception ex) when (
                    ex is JobCancelException ||
                    ex is FileUtilException ||
                    ex is ImageUtilException)
                {
                    result.FileInfo?.Thumbnail?.ThumbnailImage?.Dispose();
                    throw;
                }
            }
            else
            {
                return new FileDeepInfoGetResult
                {
                    FilePathList = param.FilePathList,
                    TagInfoList = new ListEntity<FileTagInfoEntity>(0)
                };
            }
        }
    }
}
