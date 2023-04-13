using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using SWF.Common;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// サムネイルを取得します。
    /// </summary>
    public class GetThumbnailsAsyncFacade
        : TwoWayFacadeBase<GetThumbnailParameterEntity, ThumbnailImageEntity>
    {
        public override void Execute(GetThumbnailParameterEntity param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            using (Transaction tran = DatabaseManager<ThumbnailConnection>.BeginTransaction())
            {
                GetThumbnailAsyncLogic getLogic = new GetThumbnailAsyncLogic(this);

                for (int index = param.FirstIndex; index <= param.LastIndex; index++)
                {
                    CheckCancel();

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
                    catch (ImageUtilException)
                    {
                        continue;
                    }

                    if (bf != null)
                    {
                        ThumbnailImageEntity img = new ThumbnailImageEntity();
                        img.FilePath = bf.FilePath;
                        img.ThumbnailImage = ImageUtil.ToImage(bf.ThumbnailBuffer);
                        img.ThumbnailWidth = bf.ThumbnailWidth;
                        img.ThumbnailHeight = bf.ThumbnailHeight;
                        img.SourceWidth = bf.SourceWidth;
                        img.SourceHeight = bf.SourceHeight;
                        img.FileUpdatedate = bf.FileUpdatedate;
                        OnCallback(img);
                    }
                }

                tran.Commit();
            }
        }
    }
}
