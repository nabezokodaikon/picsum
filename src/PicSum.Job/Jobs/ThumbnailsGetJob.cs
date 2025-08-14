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
        private const int MAX_DEGREE_OF_PARALLELISM = 4;

        protected override ValueTask Execute(ThumbnailsGetParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            if (param.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(param));
            }

            var filePathList = param.FilePathList
                .AsValueEnumerable()
                .Skip(param.FirstIndex)
                .Take(param.LastIndex - param.FirstIndex + 1)
                .ToArray();

            using (var cts = new CancellationTokenSource())
            {
                try
                {
                    Parallel.ForEach(
                        filePathList,
                        new ParallelOptions
                        {
                            CancellationToken = cts.Token,
                            MaxDegreeOfParallelism = MAX_DEGREE_OF_PARALLELISM,
                        },
                        filePath =>
                        {
                            try
                            {
                                if (this.IsJobCancel)
                                {
                                    cts.Cancel();
                                    cts.Token.ThrowIfCancellationRequested();
                                }

                                var bf = Instance<IThumbnailCacher>.Value.GetOrCreateCache(
                                    filePath, param.ThumbnailWidth, param.ThumbnailHeight);
                                if (param.IsExecuteCallback
                                    && bf != ThumbnailCacheEntity.EMPTY
                                    && bf.ThumbnailBuffer != null)
                                {
                                    Instance<IImageFileSizeCacher>.Value.Set(
                                        bf.FilePath,
                                        new Size(bf.SourceWidth, bf.SourceHeight),
                                        bf.FileUpdateDate);

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
                                        FileUpdateDate = bf.FileUpdateDate
                                    };

                                    this.Callback(img);
                                }
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
                }
                catch (OperationCanceledException) { }
            }

            return ValueTask.CompletedTask;
        }
    }
}
