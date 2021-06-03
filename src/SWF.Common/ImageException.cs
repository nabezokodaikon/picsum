using System;

namespace SWF.Common
{
    /// <summary>
    /// 画像ファイル読込失敗例外
    /// </summary>
    public class ImageException : Exception
    {
        public ImageException(string filePath)
            : base(string.Format("{0}の読込に失敗しました。", filePath)) { }
    }
}
