using PicSum.Core.Base.Conf;
using PicSum.Core.Job.AsyncJob;
using PicSum.Job.Logics;
using PicSum.Job.Paramters;
using PicSum.Job.Results;
using SWF.Common;
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

            var mainResult = new ImageFileGetResult();
            var subResult = new ImageFileGetResult();
            var imageLogic = new ImageFileReadLogic(this);
            var thumbLogic = new ThumbnailGetLogic(this);
            var mainFilePath = parameter.FilePathList[parameter.CurrentIndex];

            try
            {
                this.CheckCancel();
                var mainImage = this.ReadImageFile(mainFilePath, imageLogic);
                if (parameter.ImageDisplayMode != ImageDisplayMode.Single
                    && mainImage != ImageUtil.EMPTY_IMAGE
                    && mainImage.Width < mainImage.Height)
                {
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

                        this.Callback(mainResult);

                        this.CheckCancel();
                        subResult.IsMain = false;
                        subResult.HasSub = true;
                        var subImage = this.ReadImageFile(subFilePath, imageLogic);
                        var isSubSuccess = subImage != ImageUtil.EMPTY_IMAGE;
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
                        this.Callback(subResult);
                    }
                    else
                    {
                        try
                        {
                            this.CheckCancel();
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

                        this.Callback(mainResult);
                    }
                }
                else
                {
                    try
                    {
                        this.CheckCancel();
                        var isMainSuccess = mainImage != ImageUtil.EMPTY_IMAGE;
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

                    this.Callback(mainResult);
                }
            }
            catch (JobCancelException)
            {
                ExeptionHandler(subResult);
                throw;
            }

            var cacheLogic = new ImageFileReadLogic(this);
            if (parameter.CacheList == null)
            {
                throw new NullReferenceException("パラメータにキャッシュするファイルのリストが設定されていません。");
            }
            
            foreach (var path in parameter.CacheList)
            {
                this.CheckCancel();

                try
                {
                    cacheLogic.Create(path);
                }
                catch (FileUtilException ex)
                {
                    this.WriteErrorLog(new JobException(this.ID, ex));
                }
                catch (ImageUtilException ex)
                {
                    this.WriteErrorLog(new JobException(this.ID, ex));
                }
            }
        }

        private Bitmap ReadImageFile(string filePath, ImageFileReadLogic logic)
        {
            try
            {
                return logic.Execute(filePath);
            }
            catch (FileUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.ID, ex));
                return ImageUtil.EMPTY_IMAGE;
            }
            catch (ImageUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.ID, ex));
                return ImageUtil.EMPTY_IMAGE;
            }
        }

        private Size GetImageSize(string filePath)
        {
            try
            {
                return ImageUtil.GetImageSizeFromCache(filePath);
            }
            catch (ImageUtilException ex)
            {
                this.WriteErrorLog(new JobException(this.ID, ex));
                return ImageUtil.EMPTY_SIZE;
            }
        }
    }
}
