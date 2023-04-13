using System;

namespace SWF.Common
{
    /// <summary>
    /// ファイルユーティリティ例外クラス。
    /// </summary>
    public class FileUtilException
        : Exception
    {
        // TODO: 英語にする。
        public FileUtilException(Exception ex)
            : base("ファイル関連の例外が発生しました。", ex) { }

        public FileUtilException(string message)
            : base(message) { }

        public FileUtilException(string message, string filePath)
            : base(string.Format("[{0}] {1}", filePath, message)) { }
    }
}
