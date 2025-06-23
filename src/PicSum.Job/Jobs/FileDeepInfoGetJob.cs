using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Common;
using PicSum.Job.Entities;
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
        private void CallbackLodingInfo(FileDeepInfoGetParameter param)
        {
            if (param.FilePathList == null)
            {
                throw new NullReferenceException("ファイルパスリストがNULLです。");
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

                    this.CheckCancel();

                    this.Callback(result);
                }
                catch (JobCancelException)
                {
                    result.FileInfo.Thumbnail.ThumbnailImage?.Dispose();
                    throw;
                }
                catch (FileUtilException ex)
                {
                    throw new JobException(this.ID, ex);
                }
                catch (ImageUtilException ex)
                {
                    throw new JobException(this.ID, ex);
                }
            }
            else
            {
                var result = new FileDeepInfoGetResult
                {
                    FilePathList = param.FilePathList,
                    TagInfoList = new ListEntity<FileTagInfoEntity>(0)
                };

                this.Callback(result);
            }
        }

        private void CallbackInfo(FileDeepInfoGetParameter param)
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
                    return;
                }
                catch (ImageUtilException ex)
                {
                    result.FileInfo.Thumbnail.ThumbnailImage?.Dispose();
                    this.WriteErrorLog(new JobException(this.ID, ex));
                    this.Callback(FileDeepInfoGetResult.ERROR);
                    return;
                }
            }

            try
            {
                using (var con = Instance<IFileInfoDB>.Value.Connect())
                {
                    var tagsGetLogic = new FilesTagsGetLogic(this);
                    result.TagInfoList = tagsGetLogic.Execute(con, result.FilePathList);
                }

                this.CheckCancel();
            }
            catch (JobCancelException)
            {
                result.FileInfo.Thumbnail.ThumbnailImage?.Dispose();
                throw;
            }

            this.Callback(result);
        }

        protected override Task Execute(FileDeepInfoGetParameter param)
        {
            if (param.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(param));
            }

            this.CheckCancel();

            var filePath = param.FilePathList[0];
            if (Instance<IThumbnailCacher>.Value.GetCache(filePath) != null)
            {
                this.CallbackLodingInfo(param);
                this.CheckCancel();
            }

            this.CallbackInfo(param);
            this.CheckCancel();

            return Task.CompletedTask;
        }
    }
}
