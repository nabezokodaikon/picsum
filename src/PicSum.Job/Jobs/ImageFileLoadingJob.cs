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
    public sealed class ImageFileLoadingJob
        : AbstractTwoWayJob<ImageFileReadParameter, ImageFileReadResult>
    {
        protected override async ValueTask Execute(ImageFileReadParameter parameter)
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

            var mainFilePath = parameter.FilePathList[mainIndex];
            var mainSize = logic.GetImageSize(mainFilePath);
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
                if (subFilePath != mainFilePath
                    && subSize != ImageUtil.EMPTY_SIZE
                    && subSize.Width <= subSize.Height)
                {
                    await this.Delay();

                    this.Callback(logic.CreateLoadingResult(
                        mainIndex, mainFilePath, true, true, mainSize, parameter.ZoomValue));

                    await this.Delay();

                    this.Callback(logic.CreateLoadingResult(
                        subtIndex, subFilePath, false, true, subSize, parameter.ZoomValue));
                }
                else
                {
                    await this.Delay();

                    this.Callback(logic.CreateLoadingResult(
                        mainIndex, mainFilePath, true, false, mainSize, parameter.ZoomValue));
                }
            }
            else
            {
                await this.Delay();

                this.Callback(logic.CreateLoadingResult(
                    mainIndex, mainFilePath, true, false, mainSize, parameter.ZoomValue));
            }
        }

        private async ValueTask Delay()
        {
            await Task.Delay(10, this.CancellationToken);
        }
    }
}
