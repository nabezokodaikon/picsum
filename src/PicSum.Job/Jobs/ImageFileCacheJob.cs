using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ImageFileCacheJob
        : AbstractOneWayJob<ImageFileCacheParameter>
    {
        private static readonly ParallelOptions _parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = 4,
        };

        protected override Task Execute(ImageFileCacheParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            var nextFiles = new List<string>(parameter.NextCount);
            var nextIndex = this.GetNextIndex(parameter.CurrentIndex, parameter.Files);
            if (parameter.NextCount > 0)
            {
                nextFiles.Add(parameter.Files[nextIndex]);
            }
            while (nextFiles.Count < parameter.NextCount)
            {
                nextIndex = this.GetNextIndex(nextIndex, parameter.Files);
                nextFiles.Add(parameter.Files[nextIndex]);
            }

            var previewFiles = new List<string>(parameter.PreviewCount);
            var previewIndex = this.GetPreviewIndex(parameter.CurrentIndex, parameter.Files);
            if (parameter.PreviewCount > 0)
            {
                previewFiles.Add(parameter.Files[previewIndex]);
            }
            while (previewFiles.Count < parameter.PreviewCount)
            {
                previewIndex = this.GetPreviewIndex(previewIndex, parameter.Files);
                previewFiles.Add(parameter.Files[previewIndex]);
            }

            Parallel.ForEach(
                [parameter.Files[parameter.CurrentIndex], .. nextFiles, .. previewFiles],
                _parallelOptions,
                filePath =>
                {
                    try
                    {
                        this.ThrowIfJobCancellationRequested();

                        Instance<IImageFileCacher>.Value.Create(filePath);
                        var size = Instance<IImageFileCacher>.Value.GetSize(filePath);
                        if (size != ImageUtil.EMPTY_SIZE)
                        {
                            Instance<IImageFileSizeCacher>.Value.Set(filePath, size);
                        }
                        else
                        {
                            Instance<IImageFileSizeCacher>.Value.Create(filePath);
                        }
                    }
                    catch (JobCancelException)
                    {
                        return;
                    }
                    catch (FileUtilException ex)
                    {
                        this.WriteErrorLog(ex);
                    }
                    catch (ImageUtilException ex)
                    {
                        this.WriteErrorLog(ex);
                    }
                }
            );

            return Task.CompletedTask;
        }

        private int GetNextIndex(int currentIndex, string[] files)
        {
            var nextIndex = currentIndex + 1;
            if (nextIndex >= files.Length)
            {
                return 0;
            }
            else
            {
                return nextIndex;
            }
        }

        private int GetPreviewIndex(int currentIndex, string[] files)
        {
            var previewIndex = currentIndex - 1;
            if (previewIndex < 0)
            {
                return files.Length - 1;
            }
            else
            {
                return previewIndex;
            }
        }
    }
}
