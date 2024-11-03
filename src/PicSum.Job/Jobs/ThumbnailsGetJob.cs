using PicSum.DatabaseAccessor.Connection;
using PicSum.Job.Common;
using PicSum.Job.Entities;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.DatabaseAccessor;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// サムネイルを取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class ThumbnailsGetJob
        : AbstractTwoWayJob<ThumbnailsGetParameter, ThumbnailImageResult>
    {
        protected override void Execute(ThumbnailsGetParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            if (param.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(param));
            }

            using (var tran = Dao<ThumbnailDB>.Instance.BeginTransaction())
            {
                for (var index = param.FirstIndex; index <= param.LastIndex; index++)
                {
                    this.CheckCancel();

                    try
                    {
                        var bf = ThumbnailCacher.Instance.GetOrCreateCache(param.FilePathList[index], param.ThumbnailWidth, param.ThumbnailHeight);
                        if (bf == ThumbnailCacheEntity.EMPTY)
                        {
                            continue;
                        }

                        if (bf.ThumbnailBuffer == null)
                        {
                            throw new NullReferenceException("サムネイルのバッファがNullです。");
                        }

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
                    catch (FileUtilException ex)
                    {
                        this.WriteErrorLog(new JobException(this.ID, ex));
                        continue;
                    }
                    catch (ImageUtilException ex)
                    {
                        this.WriteErrorLog(new JobException(this.ID, ex));
                        continue;
                    }

                    Thread.Sleep(1);
                }

                tran.Commit();
            }
        }
    }
}
