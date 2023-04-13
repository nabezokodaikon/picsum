using System;
using PicSum.Core.Base.Conf;

namespace PicSum.UIComponent.Contents
{
    /// <summary>
    /// フォルダ実行イベント引数クラス
    /// </summary>
    public class ExecuteDirectoryEventArgs : EventArgs
    {
        private ContentsOpenType _openType = ContentsOpenType.Default;
        private string _directoryPath = string.Empty;

        public ContentsOpenType OpenType
        {
            get
            {
                return _openType;
            }
        }

        public string DirectoryPath
        {
            get
            {
                return _directoryPath;
            }
        }

        public ExecuteDirectoryEventArgs(ContentsOpenType openType, string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException("directoryPath");
            }

            _openType = openType;
            _directoryPath = directoryPath;
        }
    }
}
