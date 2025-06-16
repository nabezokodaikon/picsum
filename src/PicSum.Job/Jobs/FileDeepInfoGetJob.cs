using PicSum.Job.Entities;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
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

            this.CheckCancel();

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

                    var ratingGetLogic = new FileRatingGetLogic(this);
                    fileInfo.Rating = ratingGetLogic.Execute(filePath);

                    result.FileInfo = fileInfo;

                    this.CheckCancel();
                }
                catch (JobCancelException)
                {
                    result.FileInfo.Thumbnail.ThumbnailImage?.Dispose();
                    throw;
                }
                catch (FileUtilException ex)
                {
                    result.FileInfo.Thumbnail.ThumbnailImage?.Dispose();
                    this.WriteErrorLog(new JobException(this.ID, ex));
                    this.Callback(FileDeepInfoGetResult.ERROR);
                    return Task.CompletedTask;
                }
                catch (ImageUtilException ex)
                {
                    result.FileInfo.Thumbnail.ThumbnailImage?.Dispose();
                    this.WriteErrorLog(new JobException(this.ID, ex));
                    this.Callback(FileDeepInfoGetResult.ERROR);
                    return Task.CompletedTask;
                }
            }

            if (param.FilePathList.Length <= 997)
            {
                try
                {
                    var tagsGetLogic = new FilesTagsGetLogic(this);
                    result.TagInfoList = tagsGetLogic.Execute(result.FilePathList);

                    this.CheckCancel();
                }
                catch (JobCancelException)
                {
                    result.FileInfo.Thumbnail.ThumbnailImage?.Dispose();
                    throw;
                }
            }
            else
            {
                result.TagInfoList = new ListEntity<FileTagInfoEntity>(0);
            }

            this.Callback(result);

            return Task.CompletedTask;
        }
    }
}
