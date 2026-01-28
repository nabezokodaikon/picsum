using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{

    public sealed class ImageFileCacheJob
        : AbstractOneWayJob<ImageFileCacheParameter>
    {
        protected override async ValueTask Execute(ImageFileCacheParameter parameter)
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

            string[] targetFiles = [
                parameter.Files[parameter.CurrentIndex],
                .. nextFiles,
                .. previewFiles];
            if (targetFiles.Length < 1)
            {
                return;
            }

            using (var cts = new CancellationTokenSource())
            {
                try
                {
                    await Parallel.ForEachAsync(
                        targetFiles,
                        new ParallelOptions
                        {
                            CancellationToken = cts.Token,
                            MaxDegreeOfParallelism = Math.Min(targetFiles.Length, AppConstants.MAX_DEGREE_OF_PARALLELISM_LIGHT),
                        },
                        async (file, token) =>
                        {
                            try
                            {
                                token.ThrowIfCancellationRequested();

                                if (this.IsJobCancel)
                                {
                                    await cts.CancelAsync().False();
                                    return;
                                }

                                await Instance<IImageFileCacher>.Value.Create(file).False();
                                var size = await Instance<IImageFileCacher>.Value.GetSize(file).False();
                                if (size != ImageUtil.EMPTY_SIZE)
                                {
                                    await Instance<IImageFileSizeCacher>.Value.Set(file, size).False();
                                }
                                else
                                {
                                    await Instance<IImageFileSizeCacher>.Value.Create(file).False();
                                }
                            }
                            catch (Exception ex) when (
                                ex is FileUtilException ||
                                ex is ImageUtilException)
                            {
                                this.WriteErrorLog(ex);
                            }
                            catch (ObjectDisposedException)
                            {
                                await cts.CancelAsync().False();
                                return;
                            }
                        }
                    ).False();
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
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
