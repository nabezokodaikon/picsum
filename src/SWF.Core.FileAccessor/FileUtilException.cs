using SWF.Core.Base;
using System.Runtime.Versioning;

namespace SWF.Core.FileAccessor
{
    /// <summary>
    /// ファイルユーティリティ例外クラス。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
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
