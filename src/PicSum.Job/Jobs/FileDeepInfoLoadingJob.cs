using PicSum.Job.Entities;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class FileDeepInfoLoadingJob
        : AbstractTwoWayJob<FileDeepInfoGetParameter, FileDeepInfoGetResult>
    {
        protected override async Task Execute(FileDeepInfoGetParameter param)
        {
            if (param.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(param));
            }

            await this.Wait();

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

        private async Task Wait()
        {
            try
            {
                var sw = Stopwatch.StartNew();
                while (true)
                {
                    if (sw.ElapsedMilliseconds > 1)
                    {
                        return;
                    }

                    this.CheckCancel();

                    await Task.Delay(1);
                }
            }
            catch (JobCancelException)
            {
                return;
            }
        }
    }
}
