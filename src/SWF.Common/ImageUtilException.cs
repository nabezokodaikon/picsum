using System;

namespace SWF.Common
{
    /// <summary>
    /// 画像ユーティリティ例外クラス。
    /// </summary>
    public class ImageUtilException
        : Exception
    {
        public ImageUtilException(string filePath, Exception exception)
            : base($"'{filePath}'を読み込めませんでした。", exception)
        {

        }

        public ImageUtilException(Exception exception)
            : base(exception.Message, exception)
        {

        }
    }
}
