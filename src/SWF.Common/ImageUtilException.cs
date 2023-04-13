using System;

namespace SWF.Common
{
    /// <summary>
    /// 画像ユーティリティ例外クラス。
    /// </summary>
    public class ImageUtilException
        : Exception
    {
        // TODO: 英語にする。
        public ImageUtilException(Exception ex)
            : base("画像関連の例外が発生しました。", ex) { }

        public ImageUtilException(string message)
            : base(message) { }

        public ImageUtilException(string message, string filePath)
            : base(string.Format("[{0}] {1}", filePath, message)) { }
    }
}
