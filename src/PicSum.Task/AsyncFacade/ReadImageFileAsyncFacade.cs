using System;
using System.Diagnostics;
using System.Drawing;
using PicSum.Core.Base.Conf;
using PicSum.Core.Data.FileAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using SWF.Common;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// 画像ファイルを読込みます。
    /// </summary>
    public class ReadImageFileAsyncFacade
        : TwoWayFacadeBase<ReadImageFileParameterEntity, ReadImageFileResultEntity>
    {
        public override void Execute(ReadImageFileParameterEntity param)
        {
            Console.WriteLine("----------------");
            Console.WriteLine("ReadImageFileAsyncFacade");
            var sw = Stopwatch.StartNew();            

            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            ReadImageFileResultEntity result = new ReadImageFileResultEntity();
            ReadImageFileAsyncLogic logic = new ReadImageFileAsyncLogic(this);

            string currentFilePath = param.FilePathList[param.CurrentIndex];

            try
            {
                using (var srcImg1 = ImageUtil.ReadImageFile(currentFilePath))
                {
                    CheckCancel();

                    if (param.ImageDisplayMode != ImageDisplayMode.Single &&
                        srcImg1.Width < srcImg1.Height)
                    {
                        int nextIndex = param.CurrentIndex + 1;
                        if (nextIndex > param.FilePathList.Count - 1)
                        {
                            nextIndex = 0;
                        }

                        string nextFilePath = param.FilePathList[nextIndex];
                        
                        {
                            CheckCancel();

                            var srcImg2Size = ImageUtil.GetImageSize(nextFilePath);
                            if (srcImg2Size.Width < srcImg2Size.Height)
                            {
                                using (var srcImg2 = ImageUtil.ReadImageFile(nextFilePath))
                                {
                                    Size drawSize = new Size((int)(param.DrawSize.Width / 2f), param.DrawSize.Height);

                                    result.Image1 = new ImageFileEntity();
                                    result.Image1.FilePath = currentFilePath;
                                    result.Image1.Image = logic.CreateImage(currentFilePath, srcImg1, param.ImageSizeMode, drawSize);
                                    CheckCancel();
                                    result.Image1.Thumbnail = logic.CreateThumbnail(result.Image1.Image, param.ThumbnailSize, param.ImageSizeMode);
                                    CheckCancel();

                                    result.Image2 = new ImageFileEntity();
                                    result.Image2.FilePath = nextFilePath;
                                    result.Image2.Image = logic.CreateImage(nextFilePath, srcImg2, param.ImageSizeMode, drawSize);
                                    CheckCancel();
                                    result.Image2.Thumbnail = logic.CreateThumbnail(result.Image2.Image, param.ThumbnailSize, param.ImageSizeMode);
                                    CheckCancel();
                                }
                            }
                            else
                            {
                                result.Image1 = new ImageFileEntity();
                                result.Image1.FilePath = currentFilePath;
                                result.Image1.Image = logic.CreateImage(currentFilePath, srcImg1, param.ImageSizeMode, param.DrawSize);
                                CheckCancel();
                                result.Image1.Thumbnail = logic.CreateThumbnail(result.Image1.Image, param.ThumbnailSize, param.ImageSizeMode);
                                CheckCancel();
                            }
                        }
                    }
                    else
                    {
                        result.Image1 = new ImageFileEntity();
                        result.Image1.FilePath = currentFilePath;
                        result.Image1.Image = logic.CreateImage(currentFilePath, srcImg1, param.ImageSizeMode, param.DrawSize);
                        CheckCancel();
                        result.Image1.Thumbnail = logic.CreateThumbnail(result.Image1.Image, param.ThumbnailSize, param.ImageSizeMode);
                        CheckCancel();
                    }
                }

                result.ReadImageFileException = null;
            }
            catch (ImageException ex)
            {
                exeptionHandler(result);
                result.ReadImageFileException = ex;
            }
            finally 
            {
                sw.Stop();
                Console.WriteLine("[{0}] ReadImageFileAsyncFacade", sw.ElapsedMilliseconds);
            }

            OnCallback(result);
        }

        private void exeptionHandler(ReadImageFileResultEntity result)
        {
            if (result.Image1 != null)
            {
                if (result.Image1.Image != null)
                {
                    result.Image1.Image.Dispose();
                    result.Image1.Image = null;
                }

                if (result.Image1.Thumbnail != null)
                {
                    result.Image1.Thumbnail.Dispose();
                    result.Image1.Thumbnail = null;
                }
            }

            if (result.Image2 != null)
            {
                if (result.Image2.Image != null)
                {
                    result.Image2.Image.Dispose();
                    result.Image2.Image = null;
                }

                if (result.Image2.Thumbnail != null)
                {
                    result.Image2.Thumbnail.Dispose();
                    result.Image2.Thumbnail = null;
                }
            }
        }
    }
}