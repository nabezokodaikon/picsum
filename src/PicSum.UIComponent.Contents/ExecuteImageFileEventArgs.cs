using System;
using System.Collections.Generic;
using PicSum.Core.Base.Conf;

namespace PicSum.UIComponent.Contents
{
    /// <summary>
    /// 画像ファイル実行イベント引数クラス
    /// </summary>
    public class ExecuteImageFileEventArgs : EventArgs
    {
        private ContentsOpenType _openType = ContentsOpenType.Default;
        private IList<string> _filePathList = null;
        private string _selectedFilePath = string.Empty;

        public ContentsOpenType OpenType
        {
            get
            {
                return _openType;
            }
        }

        public IList<string> FilePathList
        {
            get
            {
                return _filePathList;
            }
        }

        public string SelectedFilePath
        {
            get
            {
                return _selectedFilePath;
            }
        }

        public ExecuteImageFileEventArgs(ContentsOpenType openType, IList<string> filePathList, string selectedFilePath)
        {
            if (filePathList == null)
            {
                throw new ArgumentNullException("filePathList");
            }

            if (filePathList.Count == 0)
            {
                throw new ArgumentException("ファイルパスリストが0件です。", "filePathList");
            }

            if (selectedFilePath == null)
            {
                throw new ArgumentNullException("selectedFilePath");
            }

            if (!filePathList.Contains(selectedFilePath))
            {
                throw new ArgumentException("選択ファイルパスがファイルパスリスト内に存在しません。", "selectedFilePath");
            }

            _openType = openType;
            _filePathList = filePathList;
            _selectedFilePath = selectedFilePath;
        }

        public ExecuteImageFileEventArgs(ContentsOpenType openType, IList<string> filePathList)
        {
            if (filePathList == null)
            {
                throw new ArgumentNullException("filePathList");
            }

            if (filePathList.Count == 0)
            {
                throw new ArgumentException("ファイルパスリストが0件です。", "filePathList");
            }

            _openType = openType;
            _filePathList = filePathList;
            _selectedFilePath = filePathList[0];
        }
    }
}
