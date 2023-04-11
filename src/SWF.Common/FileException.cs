using System;

namespace SWF.Common
{
    /// <summary>
    /// ファイル関連例外クラス。
    /// </summary>
    public class FileException
        : Exception
    {
        public FileException(string message)
            : base(message) { }

        public FileException(string message, string filePath)
            : base(string.Format("[{0}] {1}", filePath, message)) { }

        public FileException(Exception ex, string filePath)
            : base(string.Format("{0}の読込に失敗しました。", filePath), ex) { }
    }
}
