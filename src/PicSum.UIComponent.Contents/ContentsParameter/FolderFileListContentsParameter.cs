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

        public FolderFileListContentsParameter(string folderPath, string selectedFilePath)
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
        }

        public FolderFileListContentsParameter(string folderPath)
        {
            if (folderPath == null)
            {
                throw new ArgumentNullException("folderPath");
            }

            _folderPath = folderPath;
            _selectedFilePath = string.Empty;
        }

        public ContentsPanel CreateContents()
        {
            return new FolderFileListContents(this);
        }
    }
}
