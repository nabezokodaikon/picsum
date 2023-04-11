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
        public const string CONTENTS_SOURCES = "Directory";

        private string _folderPath;
        private string _selectedFilePath;

        public string ContentsSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string Key { get; private set; }


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

            this.ContentsSources = CONTENTS_SOURCES;
            this.SourcesKey = folderPath;
            this.Key = string.Format("{0}ListContents:{1}", this.ContentsSources, this.SourcesKey);
            _folderPath = folderPath;
            _selectedFilePath = selectedFilePath;
        }

        public FolderFileListContentsParameter(string folderPath)
        {
            if (folderPath == null)
            {
                throw new ArgumentNullException("folderPath");
            }

            this.ContentsSources = CONTENTS_SOURCES;
            this.SourcesKey = folderPath;
            this.Key = string.Format("{0}ListContents:{1}", this.ContentsSources, this.SourcesKey);
            _folderPath = folderPath;
            _selectedFilePath = string.Empty;
        }

        public ContentsPanel CreateContents()
        {
            return new FolderFileListContents(this);
        }
    }
}
