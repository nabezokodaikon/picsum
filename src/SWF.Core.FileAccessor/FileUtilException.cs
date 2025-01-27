using SWF.Core.Base;

namespace SWF.Core.FileAccessor
{
    /// <summary>
    /// ファイルユーティリティ例外クラス。
    /// </summary>
    public sealed class FileUtilException
        : SWFException
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
