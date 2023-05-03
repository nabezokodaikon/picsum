using PicSum.Core.Base.Conf;
using System;
using System.Collections.Generic;

namespace PicSum.UIComponent.Contents.Common
{
    /// <summary>
    /// 画像ファイル実行イベント引数クラス
    /// </summary>
    public sealed class ExecuteImageFileEventArgs 
        : EventArgs
    {
        public ContentsOpenType OpenType { get; private set; }
        public IList<string> FilePathList { get; private set; }
        public string SelectedFilePath { get; private set; }

        public ExecuteImageFileEventArgs(ContentsOpenType openType, IList<string> filePathList, string selectedFilePath)
        {
            if (filePathList == null)
            {
                throw new ArgumentNullException(nameof(filePathList));
            }

            if (filePathList.Count == 0)
            {
                throw new ArgumentException("ファイルパスリストが0件です。", nameof(filePathList));
            }

            if (selectedFilePath == null)
            {
                throw new ArgumentNullException("selectedFilePath");
            }

            if (!filePathList.Contains(selectedFilePath))
            {
                throw new ArgumentException("選択ファイルパスがファイルパスリスト内に存在しません。", nameof(selectedFilePath));
            }

            this.OpenType = openType;
            this.FilePathList = filePathList;
            this.SelectedFilePath = selectedFilePath;
        }
    }
}
