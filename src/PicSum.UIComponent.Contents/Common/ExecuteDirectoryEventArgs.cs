using SWF.Core.Base;
using System;

namespace PicSum.UIComponent.Contents.Common
{
    /// <summary>
    /// フォルダ実行イベント引数クラス
    /// </summary>
    public sealed class ExecuteDirectoryEventArgs
        : EventArgs
    {
        public PageOpenMode OpenMode { get; private set; }
        public string DirectoryPath { get; private set; }

        public ExecuteDirectoryEventArgs(PageOpenMode openMode, string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            this.OpenMode = openMode;
            this.DirectoryPath = directoryPath;
        }
    }
}
