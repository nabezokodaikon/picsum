using PicSum.Job.Common;
using PicSum.Job.Entities;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Drawing;
using System.Runtime.Versioning;
using ZLinq;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// サムネイルを取得します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ThumbnailsGetJob
        : AbstractTwoWayJob<ThumbnailsGetParameter, ThumbnailImageResult>
    {
        private static readonly ParallelOptions _parallelOptions = new()
        {
            MaxDegreeOfParallelism = 4,
        };

        protected override Task Execute(ThumbnailsGetParameter param)
        {
            if (param.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(param));
            }

            var filePathList = param.FilePathList
                .AsValueEnumerable()
                .Skip(param.FirstIndex)
                .Take(param.LastIndex - param.FirstIndex + 1)
                .ToArray();

            Parallel.ForEach(
                filePathList,
                _parallelOptions,
                filePath =>
                {
                    try
                    {
                        this.ThrowIfJobCancellationRequested();

                        var bf = Instance<IThumbnailCacher>.Value.GetOrCreateCache(
                            filePath, param.ThumbnailWidth, param.ThumbnailHeight);
                        if (param.IsExecuteCallback
                            && bf != ThumbnailCacheEntity.EMPTY
                            && bf.ThumbnailBuffer != null)
                        {
                            Instance<IImageFileSizeCacher>.Value.Set(
                                bf.FilePath,
                                new Size(bf.SourceWidth, bf.SourceHeight),
                                bf.FileUpdatedate);

                            var img = new ThumbnailImageResult
                            {
                                FilePath = bf.FilePath,
                                ThumbnailImage = new CvImage(
                                    bf.FilePath,
                                    ThumbnailUtil.ToImage(bf.ThumbnailBuffer)),
                                ThumbnailWidth = bf.ThumbnailWidth,
                                ThumbnailHeight = bf.ThumbnailHeight,
                                SourceWidth = bf.SourceWidth,
                                SourceHeight = bf.SourceHeight,
                                FileUpdatedate = bf.FileUpdatedate
                            };

                            this.Callback(img);
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
    }
}
