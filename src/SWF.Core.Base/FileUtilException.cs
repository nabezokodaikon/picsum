namespace SWF.Core.Base
{
    /// <summary>
    /// ファイルユーティリティ例外クラス。
    /// </summary>
    public sealed class FileUtilException
        : Exception
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
