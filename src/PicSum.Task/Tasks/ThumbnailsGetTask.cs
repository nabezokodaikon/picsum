using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.Logics;
using PicSum.Task.Paramters;
using PicSum.Task.Results;
using SWF.Common;
using System;
using System.Runtime.Versioning;

namespace PicSum.Task.Tasks
{
    /// <summary>
    /// サムネイルを取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class ThumbnailsGetTask
        : AbstractTwoWayTask<ThumbnailsGetParameter, ThumbnailImageResult>
    {
        protected override void Execute(ThumbnailsGetParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
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
                        if (bf == null)
                        {
                            continue;
                        }

                        var img = new ThumbnailImageResult();
                        img.FilePath = bf.FilePath;
                        img.ThumbnailImage = ImageUtil.ToImage(bf.ThumbnailBuffer);
                        img.ThumbnailWidth = bf.ThumbnailWidth;
                        img.ThumbnailHeight = bf.ThumbnailHeight;
                        img.SourceWidth = bf.SourceWidth;
                        img.SourceHeight = bf.SourceHeight;
                        img.FileUpdatedate = bf.FileUpdatedate;
                        this.Callback(img);
                    }
                    catch (FileUtilException ex)
                    {
                        this.WriteErrorLog(new TaskException(this.ID, ex));
                        continue;
                    }
                    catch (ImageUtilException ex)
                    {
                        this.WriteErrorLog(new TaskException(this.ID, ex));
                        continue;
                    }
                }

                tran.Commit();
            }
        }
    }
}
