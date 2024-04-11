using PicSum.Core.Base.Conf;
using System;

namespace PicSum.UIComponent.Contents.Common
{
    /// <summary>
    /// フォルダ実行イベント引数クラス
    /// </summary>
    public sealed class ExecuteDirectoryEventArgs
        : EventArgs
    {
        public PageOpenType OpenType { get; private set; }
        public string DirectoryPath { get; private set; }

        public ExecuteDirectoryEventArgs(PageOpenType openType, string directoryPath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            this.OpenType = openType;
            this.DirectoryPath = directoryPath;
        }
    }
}
