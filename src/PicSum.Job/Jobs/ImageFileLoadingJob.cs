using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{

    public sealed class ImageFileLoadingJob
        : AbstractTwoWayJob<ImageFileReadParameter, ImageFileReadResult>
    {
        private const int MILLISECONDS_DELAY = 10;

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
                    await logic.GetNextIndex(parameter).WithConfig() :
                    await logic.GetPreviewIndex(parameter).WithConfig();

            var mainFilePath = parameter.FilePathList[mainIndex];
            var mainSize = await logic.GetImageSize(mainFilePath).WithConfig();
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
                var subSize = await logic.GetImageSize(subFilePath).WithConfig();
                if (subFilePath != mainFilePath
                    && subSize != ImageUtil.EMPTY_SIZE
                    && subSize.Width <= subSize.Height)
                {
                    await Task.Delay(MILLISECONDS_DELAY, this.CancellationToken).WithConfig();

                    this.Callback(logic.CreateLoadingResult(
                        mainIndex, mainFilePath, true, true, mainSize, parameter.ZoomValue));

                    await Task.Delay(MILLISECONDS_DELAY, this.CancellationToken).WithConfig();

                    this.Callback(logic.CreateLoadingResult(
                        subtIndex, subFilePath, false, true, subSize, parameter.ZoomValue));
                }
                else
                {
                    await Task.Delay(MILLISECONDS_DELAY, this.CancellationToken).WithConfig();

                    this.Callback(logic.CreateLoadingResult(
                        mainIndex, mainFilePath, true, false, mainSize, parameter.ZoomValue));
                }
            }
            else
            {
                await Task.Delay(MILLISECONDS_DELAY, this.CancellationToken).WithConfig();

                this.Callback(logic.CreateLoadingResult(
                    mainIndex, mainFilePath, true, false, mainSize, parameter.ZoomValue));
            }
        }
    }
}
