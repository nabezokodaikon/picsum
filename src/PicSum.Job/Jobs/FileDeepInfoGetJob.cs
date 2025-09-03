using PicSum.DatabaseAccessor.Connection;
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
    /// <summary>
    /// ファイルの深い情報取得非同期ジョブ
    /// </summary>

    public sealed class FileDeepInfoGetJob
        : AbstractTwoWayJob<FileDeepInfoGetParameter, FileDeepInfoGetResult>
    {
        protected override async ValueTask Execute(FileDeepInfoGetParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            if (param.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(param));
            }

            var result = await this.CreateCallbackResult(param).WithConfig();

            this.ThrowIfJobCancellationRequested();

            this.Callback(result);
        }

        private async ValueTask<FileDeepInfoGetResult> CreateCallbackResult(FileDeepInfoGetParameter param)
        {
            if (param.FilePathList == null)
            {
                throw new NotSupportedException("ファイルパスリストがNULLです。");
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
                    fileInfo = await deepInfoGetLogic.Get(filePath, param.ThumbnailSize, true).WithConfig();

                    this.ThrowIfJobCancellationRequested();

                    await using (var con = await Instance<IFileInfoDao>.Value.Connect().WithConfig())
                    {
                        var ratingGetLogic = new FileRatingGetLogic(this);
                        fileInfo.Rating = await ratingGetLogic.Execute(con, filePath).WithConfig();
                    }

                    this.ThrowIfJobCancellationRequested();

                    result.FileInfo = fileInfo;
                }
                catch (JobCancelException)
                {
                    fileInfo?.Thumbnail?.ThumbnailImage?.Dispose();
                    throw;
                }
                catch (Exception ex) when (
                    ex is FileUtilException ||
                    ex is ImageUtilException)
                {
                    fileInfo?.Thumbnail?.ThumbnailImage?.Dispose();
                    this.WriteErrorLog(ex);
                    return FileDeepInfoGetResult.ERROR;
                }
            }

            await using (var con = await Instance<IFileInfoDao>.Value.Connect().WithConfig())
            {
                var tagsGetLogic = new FilesTagsGetLogic(this);
                result.TagInfoList = await tagsGetLogic.Execute(con, result.FilePathList).WithConfig();
            }

            return result;
        }
    }
}
