using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ImageFileReadJob
        : AbstractTwoWayJob<ImageFileReadParameter, ImageFileReadResult>
    {
        protected override Task Execute(ImageFileReadParameter parameter)
        {
            if (parameter.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(parameter));
            }

            var logic = new ImageFileReadLogic(this);

            var mainIndex =
                parameter.IsNext == null ?
                parameter.CurrentIndex
                : parameter.IsNext == true ?
                    logic.GetNextIndex(parameter) :
                    logic.GetPreviewIndex(parameter);
            this.ThrowIfJobCancellationRequested();

            var mainFilePath = parameter.FilePathList[mainIndex];
            var mainSize = logic.GetImageSize(mainFilePath);
            this.ThrowIfJobCancellationRequested();

            if (parameter.ImageDisplayMode != ImageDisplayMode.Single
                && mainSize != ImageUtil.EMPTY_SIZE
                && mainSize.Width <= mainSize.Height)
            {
                var subtIndex = mainIndex + 1;
                if (subtIndex > parameter.FilePathList.Length - 1)
                {
                    subtIndex = 0;
                }

                var subFilePath = parameter.FilePathList[subtIndex];
                var subSize = logic.GetImageSize(subFilePath);
                this.ThrowIfJobCancellationRequested();

                if (subFilePath != mainFilePath
                    && subSize != ImageUtil.EMPTY_SIZE
                    && subSize.Width <= subSize.Height)
                {
                    this.Callback(logic.CreateResult(
                        mainIndex, mainFilePath, true, true, parameter.ZoomValue, parameter.ThumbnailSize, parameter.ImageSizeMode));

                    this.ThrowIfJobCancellationRequested();

                    this.Callback(logic.CreateResult(
                        subtIndex, subFilePath, false, true, parameter.ZoomValue, parameter.ThumbnailSize, parameter.ImageSizeMode));
                }
                else
                {
                    this.Callback(logic.CreateResult(
                        mainIndex, mainFilePath, true, false, parameter.ZoomValue, parameter.ThumbnailSize, parameter.ImageSizeMode));
                }
            }
            else
            {
                this.Callback(logic.CreateResult(
                    mainIndex, mainFilePath, true, false, parameter.ZoomValue, parameter.ThumbnailSize, parameter.ImageSizeMode));
            }

            return Task.CompletedTask;
        }
    }
}
