using PicSum.Job.Common;
using PicSum.Job.Entities;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// サムネイルを取得します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ThumbnailsGetJob
        : AbstractTwoWayJob<ThumbnailsGetParameter, ThumbnailImageResult>
    {
        protected override void Execute(ThumbnailsGetParameter param)
        {
            if (param.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(param));
            }

            var array = param.FilePathList
                .Skip(param.FirstIndex)
                .Take(param.LastIndex - param.FirstIndex + 1)
                .ToArray();

            Parallel.ForEach(
                array,
                new ParallelOptions { MaxDegreeOfParallelism = Math.Min(array.Length, 4) },
                filePath =>
                {
                    try
                    {
                        this.CheckCancel();

                        var bf = Instance<IThumbnailCacher>.Value.GetOrCreateCache(
                            filePath, param.ThumbnailWidth, param.ThumbnailHeight);
                        if (bf != ThumbnailCacheEntity.EMPTY
                            && bf.ThumbnailBuffer != null)
                        {
                            var img = new ThumbnailImageResult
                            {
                                FilePath = bf.FilePath,
                                ThumbnailImage = ThumbnailUtil.ToImage(bf.ThumbnailBuffer),
                                ThumbnailWidth = bf.ThumbnailWidth,
                                ThumbnailHeight = bf.ThumbnailHeight,
                                SourceWidth = bf.SourceWidth,
                                SourceHeight = bf.SourceHeight,
                                FileUpdatedate = bf.FileUpdatedate
                            };

                            this.Callback(img);
                        }
                    }
                    catch (FileUtilException ex)
                    {
                        this.WriteErrorLog(new JobException(this.ID, ex));
                    }
                    catch (ImageUtilException ex)
                    {
                        this.WriteErrorLog(new JobException(this.ID, ex));
                    }
                    catch (JobCancelException)
                    {
                        return;
                    }

                    Thread.Sleep(10);
                }
            );
        }
    }
}
