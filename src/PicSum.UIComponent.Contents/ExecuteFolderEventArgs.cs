using System;
using PicSum.Core.Base.Conf;

namespace PicSum.UIComponent.Contents
{
    /// <summary>
    /// フォルダ実行イベント引数クラス
    /// </summary>
    public class ExecuteFolderEventArgs : EventArgs
    {
        private ContentsOpenType _openType = ContentsOpenType.Default;
        private string _folderPath = string.Empty;

        public ContentsOpenType OpenType
        {
            get
            {
                return _openType;
            }
        }

        public string FolderPath
        {
            get
            {
                return _folderPath;
            }
        }

        public ExecuteFolderEventArgs(ContentsOpenType openType, string folderPath)
        {
            if (folderPath == null)
            {
                throw new ArgumentNullException("folderPath");
            }

            _openType = openType;
            _folderPath = folderPath;
        }
    }
}
