using SWF.Core.ConsoleAccessor;

namespace SWF.Core.FileAccessor
{
    /// <summary>
    /// ファイルユーティリティ例外クラス。
    /// </summary>
    public sealed class FileUtilException
        : AppException
    {
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
