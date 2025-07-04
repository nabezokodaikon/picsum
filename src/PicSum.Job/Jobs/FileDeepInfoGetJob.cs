using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// ファイルの深い情報取得非同期ジョブ
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class FileDeepInfoGetJob
        : AbstractTwoWayJob<FileDeepInfoGetParameter, FileDeepInfoGetResult>
    {
        protected override Task Execute(FileDeepInfoGetParameter param)
        {
            if (param.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(param));
            }

            var result = this.CreateCallbackResult(param);

            this.CheckCancel();

            this.Callback(result);

            return Task.CompletedTask;
        }

        private FileDeepInfoGetResult CreateCallbackResult(FileDeepInfoGetParameter param)
        {
            if (param.FilePathList == null)
            {
                throw new NullReferenceException("ファイルパスリストがNULLです。");
            }

            var result = new FileDeepInfoGetResult
            {
                FilePathList = param.FilePathList,
            };

            if (param.FilePathList.Length == 1)
            {
                try
                {
                    var deepInfoGetLogic = new FileDeepInfoGetLogic(this);
                    var filePath = param.FilePathList[0];
                    var fileInfo = deepInfoGetLogic.Get(filePath, param.ThumbnailSize, true);

                    this.CheckCancel();

                    using (var con = Instance<IFileInfoDB>.Value.Connect())
                    {
                        var ratingGetLogic = new FileRatingGetLogic(this);
                        fileInfo.Rating = ratingGetLogic.Execute(con, filePath);
                    }

                    this.CheckCancel();

                    result.FileInfo = fileInfo;
                }
                catch (JobCancelException)
                {
                    result.FileInfo?.Thumbnail?.ThumbnailImage?.Dispose();
                    throw;
                }
                catch (FileUtilException ex)
                {
                    result.FileInfo?.Thumbnail?.ThumbnailImage?.Dispose();
                    this.WriteErrorLog(new JobException(this.ID, ex));
                    return FileDeepInfoGetResult.ERROR;
                }
                catch (ImageUtilException ex)
                {
                    result.FileInfo?.Thumbnail?.ThumbnailImage?.Dispose();
                    this.WriteErrorLog(new JobException(this.ID, ex));
                    return FileDeepInfoGetResult.ERROR;
                }
            }

            using (var con = Instance<IFileInfoDB>.Value.Connect())
            {
                var tagsGetLogic = new FilesTagsGetLogic(this);
                result.TagInfoList = tagsGetLogic.Execute(con, result.FilePathList);
            }

            return result;
        }
    }
}
