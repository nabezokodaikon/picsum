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
        protected override void Execute(ImageFileReadParameter parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            if (parameter.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(parameter));
            }

            this.CheckCancel();

            var logic = new ImageFileReadLogic(this);

            var mainFilePath = parameter.FilePathList[parameter.CurrentIndex];
            var mainSize = logic.GetImageSize(mainFilePath);
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
                var subSize = logic.GetImageSize(subFilePath);
                this.CheckCancel();

                if (subFilePath != mainFilePath
                    && subSize != ImageUtil.EMPTY_SIZE
                    && subSize.Width < subSize.Height)
                {
                    this.Callback(logic.CreateResult(
                        mainFilePath, true, true, parameter.ThumbnailSize, parameter.ImageSizeMode, mainSize));

                    this.CheckCancel();

                    this.Callback(logic.CreateResult(
                        subFilePath, false, true, parameter.ThumbnailSize, parameter.ImageSizeMode, subSize));
                }
                else
                {
                    this.Callback(logic.CreateResult(
                        mainFilePath, true, false, parameter.ThumbnailSize, parameter.ImageSizeMode, mainSize));
                }
            }
            else
            {
                this.Callback(logic.CreateResult(
                    mainFilePath, true, false, parameter.ThumbnailSize, parameter.ImageSizeMode, mainSize));
            }
        }
    }
}
