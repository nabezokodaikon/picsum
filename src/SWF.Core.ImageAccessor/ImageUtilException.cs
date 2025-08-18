using SWF.Core.Base;

namespace SWF.Core.ImageAccessor
{
    /// <summary>
    /// 画像ユーティリティ例外クラス。
    /// </summary>

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
