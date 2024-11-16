using System;

namespace PicSum.UIComponent.Contents.ContextMenu
{
    /// <summary>
    /// ファイルリスト実行イベント引数クラス
    /// </summary>
    public sealed class ExecuteFileListEventArgs
        : EventArgs
    {
        public string[] FilePathList { get; private set; }

        public ExecuteFileListEventArgs(string[] filePathList)
        {
            this.FilePathList = filePathList ?? throw new ArgumentNullException(nameof(filePathList));
        }
    }
}
