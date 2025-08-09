using PicSum.DatabaseAccessor.Connection;
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
        protected override async ValueTask Execute(FileDeepInfoGetParameter param)
        {
            if (param.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(param));
            }

            var result = await this.CreateCallbackResult(param);

            this.ThrowIfJobCancellationRequested();

            this.Callback(result);
        }

        private async ValueTask<FileDeepInfoGetResult> CreateCallbackResult(FileDeepInfoGetParameter param)
        {
            if (param.FilePathList == null)
            {
                throw new NullReferenceException("ファイルパスリストがNULLです。");
            }

            var result = new FileDeepInfoGetResult
            {
                FilePathList = param.FilePathList,
            };

            FileDeepInfoEntity? fileInfo = null;

            if (param.FilePathList.Length == 1)
            {
                try
                {
                    var deepInfoGetLogic = new FileDeepInfoGetLogic(this);
                    var filePath = param.FilePathList[0];
                    fileInfo = deepInfoGetLogic.Get(filePath, param.ThumbnailSize, true);

                    this.ThrowIfJobCancellationRequested();

                    await using (var con = await Instance<IFileInfoDB>.Value.Connect())
                    {
                        var ratingGetLogic = new FileRatingGetLogic(this);
                        fileInfo.Rating = ratingGetLogic.Execute(con, filePath);
                    }

                    this.ThrowIfJobCancellationRequested();

                    result.FileInfo = fileInfo;
                }
                catch (JobCancelException)
                {
                    fileInfo?.Thumbnail?.ThumbnailImage?.Dispose();
                    throw;
                }
                catch (FileUtilException ex)
                {
                    fileInfo?.Thumbnail?.ThumbnailImage?.Dispose();
                    this.WriteErrorLog(ex);
                    return FileDeepInfoGetResult.ERROR;
                }
                catch (ImageUtilException ex)
                {
                    fileInfo?.Thumbnail?.ThumbnailImage?.Dispose();
                    this.WriteErrorLog(ex);
                    return FileDeepInfoGetResult.ERROR;
                }
            }

            await using (var con = await Instance<IFileInfoDB>.Value.Connect())
            {
                var tagsGetLogic = new FilesTagsGetLogic(this);
                result.TagInfoList = tagsGetLogic.Execute(con, result.FilePathList);
            }

            return result;
        }
    }
}
