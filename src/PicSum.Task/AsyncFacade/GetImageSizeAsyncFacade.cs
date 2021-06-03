using System;
using System.Drawing;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using PicSum.Core.Data.FileAccessor;
using SWF.Common;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// 画像の大きさを取得します。
    /// </summary>
    public class GetImageSizeAsyncFacade
        : TwoWayFacadeBase<ListEntity<string>, ImageSizeEntity>
    {
        public override void Execute(ListEntity<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            GetImageSizeAsyncLogic logic = new GetImageSizeAsyncLogic(this);

            foreach (string filePath in param)
            {
                ImageSizeEntity result = new ImageSizeEntity();
                result.FilePath = filePath;

                try
                {
                    result.ImageSize = logic.Execute(filePath);
                }
                catch (ImageException)
                {
                    break;
                }

                CheckCancel();
                OnCallback(result);
            }
        }
    }
}
