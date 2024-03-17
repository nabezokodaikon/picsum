using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;
using SWF.Common;
using System;
using System.IO;
using System.Runtime.Versioning;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// サムネイルを取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class GetThumbnailsAsyncFacade
        : TwoWayFacadeBase<GetThumbnailParameter, ThumbnailImageEntity>
    {
        public override void Execute(GetThumbnailParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            using (var tran = DatabaseManager<ThumbnailConnection>.BeginTransaction())
            {
                var getLogic = new GetThumbnailAsyncLogic(this);

                for (var index = param.FirstIndex; index <= param.LastIndex; index++)
                {
                    this.CheckCancel();

                    ThumbnailBufferEntity bf = null;

                    try
                    {
                        bf = getLogic.Execute(param.FilePathList[index], param.ThumbnailWidth, param.ThumbnailHeight);
                    }
                    catch (FileNotFoundException)
                    {
                        continue;
                    }
                    catch (DirectoryNotFoundException)
                    {
                        continue;
                    }
                    catch (DriveNotFoundException)
                    {
                        continue;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        continue;
                    }
                    catch (ImageUtilException)
                    {
                        continue;
                    }

                    if (bf != null)
                    {
                        var img = new ThumbnailImageEntity();
                        img.FilePath = bf.FilePath;
                        img.ThumbnailImage = ImageUtil.ToImage(bf.ThumbnailBuffer);
                        img.ThumbnailWidth = bf.ThumbnailWidth;
                        img.ThumbnailHeight = bf.ThumbnailHeight;
                        img.SourceWidth = bf.SourceWidth;
                        img.SourceHeight = bf.SourceHeight;
                        img.FileUpdatedate = bf.FileUpdatedate;
                        this.OnCallback(img);
                    }
                }

                tran.Commit();
            }
        }
    }
}
