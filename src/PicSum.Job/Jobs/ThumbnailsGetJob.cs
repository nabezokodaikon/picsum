using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Job.AsyncJob;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Job.Entities;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Common;
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

            using (var tran = DatabaseManager<ThumbnailConnection>.BeginTransaction())
            {
                var getLogic = new ThumbnailGetLogic(this);

                for (var index = param.FirstIndex; index <= param.LastIndex; index++)
                {
                    this.CheckCancel();

                    try
                    {
                        var bf = getLogic.Execute(param.FilePathList[index], param.ThumbnailWidth, param.ThumbnailHeight);
                        if (bf == ThumbnailBufferEntity.EMPTY)
                        {
                            continue;
                        }

                        var img = new ThumbnailImageResult
                        {
                            FilePath = bf.FilePath,
                            ThumbnailImage = ImageUtil.ToImage(bf.ThumbnailBuffer),
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
                }

                tran.Commit();
            }
        }
    }
}
