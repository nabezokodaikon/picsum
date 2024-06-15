using PicSum.Core.Base.Conf;
using PicSum.Core.Job.AsyncJob;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;

namespace PicSum.Job.Logics
{
    /// <summary>
    /// 画像ファイル読込ロジック
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class ImageFileReadLogic(AbstractAsyncJob job)
        : AbstractAsyncLogic(job)
    {

    }
}
