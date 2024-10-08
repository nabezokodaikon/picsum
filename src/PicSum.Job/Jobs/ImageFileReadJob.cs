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

        private long isReadCompleted = 0;

        private bool IsReadCompleted
        {
            get
            {
                return Interlocked.Read(ref this.isReadCompleted) == 1;
            }
            set
            {
                Interlocked.Exchange(ref this.isReadCompleted, Convert.ToInt64(value));
            }
        }

        protected override void Execute(ImageFileReadParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            if (parameter.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(parameter));
            }

            var mainFilePath = parameter.FilePathList[parameter.CurrentIndex];
            var mainSize = this.GetImageSize(mainFilePath);

            if (parameter.ImageDisplayMode != ImageDisplayMode.Single
                && mainSize != ImageUtil.EMPTY_SIZE
                && mainSize.Width < mainSize.Height)
            {
                var subtIndex = parameter.CurrentIndex + 1;
                if (subtIndex > parameter.FilePathList.Count - 1)
                {
                    subtIndex = 0;
                }

                var subFilePath = parameter.FilePathList[subtIndex];
                var subSize = this.GetImageSize(subFilePath);

                if (subSize != ImageUtil.EMPTY_SIZE
                    && subSize.Width < subSize.Height)
                {
                    this.Callback(
                        mainFilePath, true, true, parameter.ThumbnailSize, parameter.ImageSizeMode, mainSize);

                    this.CheckCancel();

                    this.Callback(
                        subFilePath, false, true, parameter.ThumbnailSize, parameter.ImageSizeMode, subSize);
                }
                else
                {
                    this.Callback(
                        mainFilePath, true, false, parameter.ThumbnailSize, parameter.ImageSizeMode, mainSize);
                }
            }
            else
            {
                this.Callback(
                    mainFilePath, true, false, parameter.ThumbnailSize, parameter.ImageSizeMode, mainSize);
            }
        }

        private void Callback(
            string filePath, bool isMain, bool hasSub, int thumbnailSize, ImageSizeMode imageSizeMode, Size imageSize)
        {
            var whileSw = Stopwatch.StartNew();
            Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageFileReadJob.Callback Start IsMain: {isMain}, HasSub: {hasSub}");

            this.IsReadCompleted = false;
            ImageFileGetResult? result = null;
            Exception? exception = null;
            var readTime = Stopwatch.StartNew();

            var thread = new Thread(() =>
            {
                var sw = Stopwatch.StartNew();
                try
                {
                    result = this.CreateResult(
                        filePath, isMain, hasSub, thumbnailSize, imageSizeMode, imageSize);
                }
                catch (JobCancelException)
                {
                    result = null;
                }
                catch (Exception ex)
                {
                    exception = ex;
                    result = null;
                }
                finally
                {
                    Console.WriteLine($"[{Thread.CurrentThread.Name}]: {sw.ElapsedMilliseconds} ms");
                    this.IsReadCompleted = true;
                }
            });
            thread.Name = $"{Thread.CurrentThread.Name} Thread IsMain: {isMain}, HasSub: {hasSub}";
            thread.Start();

            while (true)
            {
                if (readTime.ElapsedMilliseconds > 20)
                {
                    var emptyResult = this.CreateEmptyResult(
                        filePath, isMain, hasSub, thumbnailSize, imageSizeMode, imageSize);
                    this.Callback(emptyResult);
                    thread.Join();
                }

                if (this.IsReadCompleted)
                {
                    thread.Join();

                    if (result != null)
                    {
                        this.Callback(result);
                        break;
                    }
                    else if (exception != null)
                    {
                        throw exception;
                    }
                    else if (result == null)
                    {
                        break;
                    }
                }

                Thread.Sleep(1);
            }

            whileSw.Stop();
            Console.WriteLine($"[{Thread.CurrentThread.Name}] ImageFileReadJob.Callback IsMain: {isMain}, HasSub: {hasSub} {whileSw.ElapsedMilliseconds} ms");
        }

        private ImageFileGetResult CreateEmptyResult(
            string filePath, bool isMain, bool hasSub, int thumbnailSize, ImageSizeMode imageSizeMode, Size imageSize)
        {
            var image = new CvImage(this.CreateEmptyImage(imageSize));
            var thumbLogic = new ThumbnailGetLogic(this);
            var thumbnail = thumbLogic.CreateThumbnail(image, thumbnailSize, imageSizeMode);

            return new()
            {
                IsMain = isMain,
                HasSub = hasSub,
                Image = new()
                {
                    FilePath = filePath,
                    Thumbnail = thumbnail,
                    Image = image,
                    Size = imageSize,
                    IsEmpty = true,
                    IsError = false,
                }
            };
        }

        private Bitmap CreateEmptyImage(Size imageSize)
        {
            var bmp = new Bitmap(imageSize.Width, imageSize.Height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(Brushes.Gray, new Rectangle(0, 0, bmp.Width, bmp.Height));
            }
            return bmp;
        }

        private ImageFileGetResult CreateResult(
            string filePath, bool isMain, bool hasSub, int thumbnailSize, ImageSizeMode imageSizeMode, Size imageSize)
        {
            var result = new ImageFileGetResult();
            result.IsMain = isMain;
            result.HasSub = hasSub;

            try
            {
                var image = this.ReadImageFile(filePath);
                var isError = image == CvImage.EMPTY;
                this.CheckCancel();

                image.CreateMat();
                this.CheckCancel();

                var thumbLogic = new ThumbnailGetLogic(this);
                var thumbnail = (isError) ?
                        null :
                        thumbLogic.CreateThumbnail(image, thumbnailSize, imageSizeMode);
                this.CheckCancel();

                result.Image = new()
                {
                    FilePath = filePath,
                    Thumbnail = thumbnail,
                    Image = image,
                    Size = imageSize,
                    IsEmpty = false,
                    IsError = isError,
                };

                return result;
            }
            catch (JobCancelException)
            {
                ExeptionHandler(result);
                throw;
            }
            catch (Exception)
            {
                ExeptionHandler(result);
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
