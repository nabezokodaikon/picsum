using SWF.Core.Base;

namespace SWF.Core.ImageAccessor
{
    /// <summary>
    /// 画像ユーティリティ例外クラス。
    /// </summary>

    public sealed class ImageUtilException
        : AppException
    {
        public ImageUtilException(
            string message, string filePath, Exception exception)
            : base($"{message}'${filePath}'", exception)
        {

        }

        public ImageUtilException(
            string message, string filePath)
            : base($"{message}'${filePath}'")
        {

        }

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
