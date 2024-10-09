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
        private long isThreadCompleted = 0;

        private bool IsThreadCompleted
        {
            get
            {
                return Interlocked.Read(ref this.isThreadCompleted) == 1;
            }
            set
            {
                Interlocked.Exchange(ref this.isThreadCompleted, Convert.ToInt64(value));
            }
        }

        protected override void Execute(ImageFileReadParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            if (parameter.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(parameter));
            }

            this.CheckCancel();

            var mainFilePath = parameter.FilePathList[parameter.CurrentIndex];
            var mainSize = this.GetImageSize(mainFilePath);
            this.CheckCancel();

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
                this.CheckCancel();

                if (subSize != ImageUtil.EMPTY_SIZE
                    && subSize.Width < subSize.Height)
                {
                    this.Callback(this.CreateResult(
                        mainFilePath, true, true, parameter.ThumbnailSize, parameter.ImageSizeMode, mainSize));

                    this.CheckCancel();

                    this.Callback(this.CreateResult(
                        subFilePath, false, true, parameter.ThumbnailSize, parameter.ImageSizeMode, subSize));
                }
                else
                {
                    this.Callback(this.CreateResult(
                        mainFilePath, true, false, parameter.ThumbnailSize, parameter.ImageSizeMode, mainSize));
                }
            }
            else
            {
                this.Callback(this.CreateResult(
                    mainFilePath, true, false, parameter.ThumbnailSize, parameter.ImageSizeMode, mainSize));
            }
        }

        private ImageFileGetResult CreateResult(
            string filePath, bool isMain, bool hasSub, int thumbnailSize, ImageSizeMode imageSizeMode, Size imageSize)
        {
            CvImage? image = null;
            Bitmap? thumbnail = null;

            try
            {
                image = this.ReadImageFile(filePath);
                var isError = image == CvImage.EMPTY;
                this.CheckCancel();

                image.CreateMat();
                this.CheckCancel();

                thumbnail = (isError) ?
                        null :
                        ThumbnailGetLogic.CreateThumbnail(image, thumbnailSize, imageSizeMode);
                this.CheckCancel();

                return new ImageFileGetResult()
                {
                    IsMain = isMain,
                    HasSub = hasSub,
                    Image = new()
                    {
                        FilePath = filePath,
                        Thumbnail = thumbnail,
                        Image = image,
                        Size = imageSize,
                        IsEmpty = false,
                        IsError = isError,
                    },
                };
            }
            catch (JobCancelException)
            {
                image?.Dispose();
                thumbnail?.Dispose();
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

        private void Callback(
            string filePath, bool isMain, bool hasSub, int thumbnailSize, ImageSizeMode imageSizeMode, Size imageSize)
        {
            this.IsThreadCompleted = false;
            Exception? exception = null;
            var threadTime = Stopwatch.StartNew();

            var thread = Task.Run((() =>
            {
                try
                {
                    return this.CreateResult(
                        filePath, isMain, hasSub, thumbnailSize, imageSizeMode, imageSize);
                }
                catch (JobCancelException)
                {
                    return null;
                }
                catch (Exception ex)
                {
                    exception = ex;
                    return null;
                }
                finally
                {
                    this.IsThreadCompleted = true;
                }
            }));

            var isEmptyCallbacked = false;
            while (true)
            {
                if (!isEmptyCallbacked && threadTime.ElapsedMilliseconds >= 20)
                {
                    this.Callback(this.CreateEmptyResult(
                        filePath, isMain, hasSub, thumbnailSize, imageSizeMode, imageSize));
                    isEmptyCallbacked = true;
                    this.CheckCancel();
                }

                if (isEmptyCallbacked || this.IsThreadCompleted)
                {
                    var result = thread.Result;

                    if (exception != null)
                    {
                        throw exception;
                    }

                    if (result != null)
                    {
                        this.Callback(result);
                        return;
                    }
                    else
                    {
                        return;
                    }
                }

                Thread.Sleep(1);
            }
        }

        private ImageFileGetResult CreateEmptyResult(
            string filePath, bool isMain, bool hasSub, int thumbnailSize, ImageSizeMode imageSizeMode, Size imageSize)
        {
            var image = new CvImage(this.CreateEmptyImage(imageSize));
            var thumbnail = ThumbnailGetLogic.CreateThumbnail(image, thumbnailSize, imageSizeMode);

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
    }
}
