using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ImageFileLoadingJob
        : AbstractTwoWayJob<ImageFileReadParameter, ImageFileReadResult>
    {
        protected override void Execute(ImageFileReadParameter parameter)
        {
            if (parameter.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(parameter));
            }

            //this.Wait();

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
                    if (!Instance<IImageFileCacher>.Value.Has(mainFilePath))
                    {
                        this.Callback(logic.CreateEmptyResult(
                            mainIndex, mainFilePath, true, true, mainSize));
                    }

                    if (!Instance<IImageFileCacher>.Value.Has(subFilePath))
                    {
                        this.Callback(logic.CreateEmptyResult(
                            subtIndex, subFilePath, false, true, subSize));
                    }
                }
                else
                {
                    if (!Instance<IImageFileCacher>.Value.Has(mainFilePath))
                    {
                        this.Callback(logic.CreateEmptyResult(
                            mainIndex, mainFilePath, true, false, mainSize));
                    }
                }
            }
            else
            {
                if (!Instance<IImageFileCacher>.Value.Has(mainFilePath))
                {
                    this.Callback(logic.CreateEmptyResult(
                        mainIndex, mainFilePath, true, false, mainSize));
                }
            }
        }

        private void Wait()
        {
            try
            {
                var sw = Stopwatch.StartNew();
                while (true)
                {
                    if (sw.ElapsedMilliseconds > 100)
                    {
                        return;
                    }

                    this.CheckCancel();

                    Thread.Sleep(1);
                }
            }
            catch (JobCancelException)
            {
                return;
            }
        }
    }
}
