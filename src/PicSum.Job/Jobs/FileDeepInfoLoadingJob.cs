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
        protected override void Execute(FileDeepInfoGetParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            if (param.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(param));
            }

            this.Wait();

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
                    result.FileInfo = deepInfoGetLogic.Execute(filePath, param.ThumbnailSize, false);

                    this.CheckCancel();
                }
                catch (JobCancelException)
                {
                    result.FileInfo?.Thumbnail?.ThumbnailImage?.Dispose();
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

            result.TagInfoList = [];

            this.Callback(result);
        }

        private void Wait()
        {
            try
            {
                var sw = Stopwatch.StartNew();
                while (true)
                {
                    if (sw.ElapsedMilliseconds > 10)
                    {
                        return;
                    }

                    this.CheckCancel();

                    Thread.Sleep(1);
                }
            }
            catch (JobCancelException)
            {
                return;
            }
        }
    }
}
