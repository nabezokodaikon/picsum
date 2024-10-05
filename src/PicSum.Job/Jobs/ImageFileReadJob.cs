using PicSum.Core.Base.Conf;
using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using System.Diagnostics;
using System.Drawing;

namespace PicSum.Job.Jobs
{
    public sealed class ImageFileReadJob
        : AbstractTwoWayJob<ImageFileReadParameter, ImageFileGetResult>
    {
        private static void ExeptionHandler(ImageFileGetResult result)
        {
            if (result.Image != null)
            {
                if (result.Image.Image != null)
                {
                    result.Image.Image.Dispose();
                    result.Image.Image = null;
                }

                if (result.Image.Thumbnail != null)
                {
                    result.Image.Thumbnail.Dispose();
                    result.Image.Thumbnail = null;
                }
            }
        }

        protected override void Execute(ImageFileReadParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            if (parameter.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(parameter));
            }

            var sw = Stopwatch.StartNew();
            Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageFileReadJob: Start");

            var mainResult = new ImageFileGetResult();
            var subResult = new ImageFileGetResult();
            var thumbLogic = new ThumbnailGetLogic(this);
            var mainFilePath = parameter.FilePathList[parameter.CurrentIndex];

            try
            {
                this.CheckCancel();
                var mainImage = this.ReadImageFile(mainFilePath);
                if (parameter.ImageDisplayMode != ImageDisplayMode.Single
                    && mainImage != CvImage.EMPTY
                    && mainImage.Width < mainImage.Height)
                {
                    this.CheckCancel();
                    var subtIndex = parameter.CurrentIndex + 1;
                    if (subtIndex > parameter.FilePathList.Count - 1)
                    {
                        subtIndex = 0;
                    }

                    var subFilePath = parameter.FilePathList[subtIndex];
                    var subSrcImageSize = this.GetImageSize(subFilePath);
                    if (subSrcImageSize.Width < subSrcImageSize.Height)
                    {
                        try
                        {
                            this.CheckCancel();
                            mainImage.CreateMat();
                            mainResult.IsMain = true;
                            mainResult.HasSub = true;
                            mainResult.Image = new()
                            {
                                FilePath = mainFilePath,
                                Thumbnail = thumbLogic.CreateThumbnail(mainImage, parameter.ThumbnailSize, parameter.ImageSizeMode),
                                Image = mainImage,
                                IsError = false,
                            };
                            this.CheckCancel();
                        }
                        catch (JobCancelException)
                        {
                            ExeptionHandler(mainResult);
                            throw;
                        }

                        sw.Stop();
                        Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageFileReadJob Main Callback: {sw.ElapsedMilliseconds} ms");
                        this.Callback(mainResult);

                        sw = Stopwatch.StartNew();
                        Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageFileReadJob Sub Read: Start");

                        this.CheckCancel();
                        subResult.IsMain = false;
                        subResult.HasSub = true;
                        var subImage = this.ReadImageFile(subFilePath);
                        subImage.CreateMat();
                        var isSubSuccess = subImage != CvImage.EMPTY;
                        subResult.Image = new()
                        {
                            FilePath = subFilePath,
                            Image = subImage,
                            Thumbnail = (isSubSuccess) ?
                                thumbLogic.CreateThumbnail(subImage, parameter.ThumbnailSize, parameter.ImageSizeMode) :
                                null,
                            IsError = !isSubSuccess,
                        };
                        this.CheckCancel();

                        sw.Stop();
                        Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageFileReadJob Sub Callback: {sw.ElapsedMilliseconds} ms");

                        this.Callback(subResult);
                    }
                    else
                    {
                        try
                        {
                            this.CheckCancel();
                            mainImage.CreateMat();
                            mainResult.IsMain = true;
                            mainResult.HasSub = false;
                            mainResult.Image = new()
                            {
                                FilePath = mainFilePath,
                                Thumbnail = thumbLogic.CreateThumbnail(mainImage, parameter.ThumbnailSize, parameter.ImageSizeMode),
                                Image = mainImage,
                                IsError = false,
                            };
                            this.CheckCancel();
                        }
                        catch (JobCancelException)
                        {
                            ExeptionHandler(mainResult);
                            throw;
                        }

                        sw.Stop();
                        Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageFileReadJob Main Callback: {sw.ElapsedMilliseconds} ms");
                        this.Callback(mainResult);
                    }
                }
                else
                {
                    try
                    {
                        this.CheckCancel();
                        var isMainSuccess = mainImage != CvImage.EMPTY;
                        mainImage.CreateMat();
                        mainResult.IsMain = true;
                        mainResult.HasSub = false;
                        mainResult.Image = new()
                        {
                            FilePath = mainFilePath,
                            Thumbnail = (isMainSuccess) ?
                                thumbLogic.CreateThumbnail(mainImage, parameter.ThumbnailSize, parameter.ImageSizeMode) :
                                null,
                            Image = mainImage,
                            IsError = !isMainSuccess,
                        };
                        this.CheckCancel();
                    }
                    catch (JobCancelException)
                    {
                        ExeptionHandler(mainResult);
                        throw;
                    }

                    sw.Stop();
                    Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageFileReadJob Main Callback: {sw.ElapsedMilliseconds} ms");
                    this.Callback(mainResult);
                }
            }
            catch (JobCancelException)
            {
                ExeptionHandler(subResult);
                throw;
            }
        }

        private CvImage ReadImageFile(string filePath)
        {
            try
            {
                return ImageFileCacheUtil.GetCvImage(filePath);
            }
            catch (FileUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.ID, ex));
                return CvImage.EMPTY;
            }
            catch (ImageUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.ID, ex));
                return CvImage.EMPTY;
            }
        }

        private Size GetImageSize(string filePath)
        {
            try
            {
                return ImageUtil.GetImageInfo(filePath).Size;
            }
            catch (FileUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.ID, ex));
                return ImageUtil.EMPTY_SIZE;
            }
            catch (ImageUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.ID, ex));
                return ImageUtil.EMPTY_SIZE;
            }
        }
    }
}
