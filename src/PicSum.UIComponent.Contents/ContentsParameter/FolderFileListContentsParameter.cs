using System;
using SWF.UIComponent.TabOperation;
using PicSum.UIComponent.Contents.FileListContents;

namespace PicSum.UIComponent.Contents.ContentsParameter
{
    /// <summary>
    /// フォルダファイルリストコンテンツパラメータ
    /// </summary>
    public class FolderFileListContentsParameter : IContentsParameter
    {
        private string _folderPath;
        private string _selectedFilePath;

        public string FolderPath
        {
            get
            {
                return _folderPath;
            }
        }

        public string SelectedFilePath
        {
            get
            {
                return _selectedFilePath;
            }
        }

        public Action<bool> AfterLoadAction { get; private set; }

        public FolderFileListContentsParameter(string folderPath, string selectedFilePath, Action<bool> afterLoadAction)
        {
            if (folderPath == null)
            {
                throw new ArgumentNullException("folderPath");
            }

            if (selectedFilePath == null)
            {
                throw new ArgumentNullException("selectedFilePath");
            }

            _folderPath = folderPath;
            _selectedFilePath = selectedFilePath;
            this.AfterLoadAction = afterLoadAction ?? throw new ArgumentNullException(nameof(afterLoadAction));
        }

        public FolderFileListContentsParameter(string folderPath, Action<bool> afterLoadAction)
        {
            if (folderPath == null)
            {
                throw new ArgumentNullException("folderPath");
            }

            _folderPath = folderPath;
            _selectedFilePath = string.Empty;
            this.AfterLoadAction = afterLoadAction ?? throw new ArgumentNullException(nameof(afterLoadAction));
        }

        public ContentsPanel CreateContents()
        {
            return new FolderFileListContents(this);
        }
    }
}
