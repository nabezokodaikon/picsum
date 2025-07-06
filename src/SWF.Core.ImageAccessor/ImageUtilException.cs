using SWF.Core.Base;
using System.Runtime.Versioning;

namespace SWF.Core.ImageAccessor
{
    /// <summary>
    /// 画像ユーティリティ例外クラス。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ImageUtilException
        : AppException
    {
        public ImageUtilException(string message, Exception exception)
            : base(message, exception)
        {

        }

        public ImageUtilException(string message)
            : base(message)
        {

        }
    }
}
