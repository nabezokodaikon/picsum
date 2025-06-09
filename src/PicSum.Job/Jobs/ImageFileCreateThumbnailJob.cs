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
    public sealed class ImageFileCreateThumbnailJob
        : AbstractTwoWayJob<ImageFileReadParameter, ImageFileReadResult>
    {
        protected override void Execute(ImageFileReadParameter parameter)
        {
            if (parameter.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(parameter));
            }

            this.CheckCancel();

            var logic = new ImageFileReadLogic(this);

            var mainIndex =
                parameter.IsNext == null ?
                parameter.CurrentIndex
                : parameter.IsNext == true ?
                    logic.GetNextIndex(parameter) :
                    logic.GetPreviewIndex(parameter);
            this.CheckCancel();

            var mainFilePath = parameter.FilePathList[mainIndex];
            var mainSize = logic.GetImageSize(mainFilePath);
            var mainThumbnailScale = logic.GetThumbnailScale(parameter.ThumbnailSize, mainSize);
            this.CheckCancel();

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
                this.CheckCancel();

                if (subFilePath != mainFilePath
                    && subSize != ImageUtil.EMPTY_SIZE
                    && subSize.Width <= subSize.Height)
                {
                    this.Callback(logic.CreateThumbnailResult(
                        mainIndex, mainFilePath, true, true, mainThumbnailScale));

                    this.CheckCancel();

                    var subThumbnailScale = logic.GetThumbnailScale(parameter.ThumbnailSize, subSize);
                    this.Callback(logic.CreateThumbnailResult(
                        subtIndex, subFilePath, false, true, subThumbnailScale));
                }
                else
                {
                    this.Callback(logic.CreateThumbnailResult(
                        mainIndex, mainFilePath, true, false, mainThumbnailScale));
                }
            }
            else
            {
                this.Callback(logic.CreateThumbnailResult(
                    mainIndex, mainFilePath, true, false, mainThumbnailScale));
            }
        }
    }
}
