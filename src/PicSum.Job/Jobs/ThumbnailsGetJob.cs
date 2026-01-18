using PicSum.Job.Common;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Drawing;
using ZLinq;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// サムネイルを取得します。
    /// </summary>

    public sealed class ThumbnailsGetJob
        : AbstractTwoWayJob<ThumbnailsGetParameter, ThumbnailImageResult>
    {
        private const int MAX_DEGREE_OF_PARALLELISM = 4;

        protected override async ValueTask Execute(ThumbnailsGetParameter param)
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
                    await Parallel.ForEachAsync(
                        filePathList,
                        new ParallelOptions
                        {
                            CancellationToken = cts.Token,
                            MaxDegreeOfParallelism = MAX_DEGREE_OF_PARALLELISM,
                        },
                        async (filePath, token) =>
                        {
                            token.ThrowIfCancellationRequested();

                            if (this.IsJobCancel)
                            {
                                await cts.CancelAsync().False();
                                return;
                            }

                            try
                            {
                                var bf = await Instance<IThumbnailCacher>.Value.GetOrCreateCache(
                                    filePath, param.ThumbnailWidth, param.ThumbnailHeight).False();
                                if (param.IsExecuteCallback
                                    && !bf.IsEmpry
                                    && bf.ThumbnailBuffer != null)
                                {
                                    await Instance<IImageFileSizeCacher>.Value.Set(
                                        bf.FilePath,
                                        new Size(bf.SourceWidth, bf.SourceHeight),
                                        bf.FileUpdateDate).False();

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
    }
}
