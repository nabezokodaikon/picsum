using System;

namespace PicSum.UIComponent.Common
{
    /// <summary>
    /// ファイル実行イベント引数クラス
    /// </summary>
    public sealed class ExecuteFileEventArgs
        : EventArgs
    {
        public string FilePath { get; private set; }

        public ExecuteFileEventArgs(string filePath)
        {
            this.FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }
    }
}
