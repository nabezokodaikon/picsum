using PicSum.Job.Common;
using PicSum.Job.Parameters;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// サムネイルを取得します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ThumbnailsGetJob
        : AbstractTwoWayJob<ThumbnailsGetParameter, ThumbnailImageResult>
    {
        protected override void Execute(ThumbnailsGetParameter param)
        {
            if (param.FilePathList == null)
            {
                throw new ArgumentException("ファイルパスリストがNULLです。", nameof(param));
            }

            Instance<IThumbnailCacheThreads>.Value.DoCache(param, this.Callback);
        }
    }
}
