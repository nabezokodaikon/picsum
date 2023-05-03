using System;
using System.Collections.Generic;

namespace PicSum.UIComponent.Common
{
    /// <summary>
    /// ファイルリスト実行イベント引数クラス
    /// </summary>
    public sealed class ExecuteFileListEventArgs
        : EventArgs
    {
        public IList<string> FilePathList { get; private set; }

        public ExecuteFileListEventArgs(IList<string> filePathList)
        {
            this.FilePathList = filePathList ?? throw new ArgumentNullException(nameof(filePathList));
        }
    }
}
