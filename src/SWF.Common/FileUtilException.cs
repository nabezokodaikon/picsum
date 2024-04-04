using System;

namespace SWF.Common
{
    /// <summary>
    /// ファイルユーティリティ例外クラス。
    /// </summary>
    public class FileUtilException
        : Exception
    {
        public FileUtilException(string filePath, Exception exception)
            : base($"'{filePath}'を読み込めませんでした。", exception)
        {

        }

        public FileUtilException(Exception exception)
            : base(exception.Message, exception)
        {

        }

        public FileUtilException(string filePath)
            : base($"'{filePath}'を読み込めませんでした。")
        {

        }
    }
}
