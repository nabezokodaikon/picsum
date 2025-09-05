using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{

    public sealed class ImageFileReadJob
        : AbstractTwoWayJob<ImageFileReadParameter, ImageFileReadResult>
    {
        protected override async ValueTask Execute(ImageFileReadParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            if (parameter.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(parameter));
            }

            var logic = new ImageFileReadLogic(this);

            var mainIndex =
                parameter.IsNext == null ?
                parameter.CurrentIndex
                : parameter.IsNext == true ?
                    await logic.GetNextIndex(parameter).False() :
                    await logic.GetPreviewIndex(parameter).False();
            this.ThrowIfJobCancellationRequested();

            var mainFilePath = parameter.FilePathList[mainIndex];
            var mainSize = await logic.GetImageSize(mainFilePath).False();
            this.ThrowIfJobCancellationRequested();

            if (parameter.DisplayMode != ImageDisplayMode.Single
                && mainSize != ImageUtil.EMPTY_SIZE
                && mainSize.Width <= mainSize.Height)
            {
                var subtIndex = mainIndex + 1;
                if (subtIndex > parameter.FilePathList.Length - 1)
                {
                    subtIndex = 0;
                }

                var subFilePath = parameter.FilePathList[subtIndex];
                var subSize = await logic.GetImageSize(subFilePath).False();
                this.ThrowIfJobCancellationRequested();

                if (subFilePath != mainFilePath
                    && subSize != ImageUtil.EMPTY_SIZE
                    && subSize.Width <= subSize.Height)
                {
                    this.Callback(await logic.CreateResult(
                        mainIndex, mainFilePath, true, true, parameter.ZoomValue, parameter.ThumbnailSize, parameter.SizeMode).False());

                    this.ThrowIfJobCancellationRequested();

                    this.Callback(await logic.CreateResult(
                        subtIndex, subFilePath, false, true, parameter.ZoomValue, parameter.ThumbnailSize, parameter.SizeMode).False());
                }
                else
                {
                    this.Callback(await logic.CreateResult(
                        mainIndex, mainFilePath, true, false, parameter.ZoomValue, parameter.ThumbnailSize, parameter.SizeMode).False());
                }
            }
            else
            {
                this.Callback(await logic.CreateResult(
                    mainIndex, mainFilePath, true, false, parameter.ZoomValue, parameter.ThumbnailSize, parameter.SizeMode).False());
            }
        }
    }
}
