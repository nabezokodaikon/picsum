using PicSum.Core.Base.Conf;
using System;

namespace PicSum.UIComponent.Contents
{
    /// <summary>
    /// フォルダ実行イベント引数クラス
    /// </summary>
    public sealed class ExecuteDirectoryEventArgs 
        : EventArgs
    {
        public ContentsOpenType OpenType { get; private set; }
        public string DirectoryPath { get; private set; }

        public ExecuteDirectoryEventArgs(ContentsOpenType openType, string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException("directoryPath");
            }

            this.OpenType = openType;
            this.DirectoryPath = directoryPath;
        }
    }
}
