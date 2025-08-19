using SWF.Core.Base;

namespace SWF.Core.FileAccessor
{
    /// <summary>
    /// ファイルユーティリティ例外クラス。
    /// </summary>

    public sealed class FileUtilException
        : AppException
    {
        public FileUtilException(
            string message, string filePath, Exception exception)
            : base($"{message}'${filePath}'", exception)
        {

        }

        public FileUtilException(
            string message, string filePath)
            : base($"{message}'${filePath}'")
        {

        }

        public FileUtilException(string message, Exception exception)
            : base(message, exception)
        {

        }

        public FileUtilException(string message)
            : base(message)
        {

        }
    }
}
