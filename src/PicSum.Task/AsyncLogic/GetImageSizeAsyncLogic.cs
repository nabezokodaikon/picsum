using System;
using System.Drawing;
using PicSum.Core.Data.FileAccessor;
using PicSum.Core.Task.AsyncTask;
using SWF.Common;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// 画像の大きさを取得します。
    /// </summary>
    internal class GetImageSizeAsyncLogic : AsyncLogicBase
    {
        public GetImageSizeAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public Size Execute(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            return ImageUtil.GetImageSize(filePath);
        }
    }
}
