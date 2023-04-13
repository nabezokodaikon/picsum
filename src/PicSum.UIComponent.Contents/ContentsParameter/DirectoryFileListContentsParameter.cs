using System;
using SWF.UIComponent.TabOperation;
using PicSum.UIComponent.Contents.FileListContents;

namespace PicSum.UIComponent.Contents.ContentsParameter
{
    /// <summary>
    /// フォルダファイルリストコンテンツパラメータ
    /// </summary>
    public class DirectoryFileListContentsParameter : IContentsParameter
    {
        public const string CONTENTS_SOURCES = "Directory";

        private string _directoryPath;
        private string _selectedFilePath;

        public string ContentsSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string Key { get; private set; }


        public string DirectoryPath
        {
            get
            {
                return _directoryPath;
            }
        }

        public string SelectedFilePath
        {
            get
            {
                return _selectedFilePath;
            }
        }

        public DirectoryFileListContentsParameter(string directoryPath, string selectedFilePath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException("directoryPath");
            }

            if (selectedFilePath == null)
            {
                throw new ArgumentNullException("selectedFilePath");
            }

            this.ContentsSources = CONTENTS_SOURCES;
            this.SourcesKey = directoryPath;
            this.Key = string.Format("{0}ListContents:{1}", this.ContentsSources, this.SourcesKey);
            _directoryPath = directoryPath;
            _selectedFilePath = selectedFilePath;
        }

        public DirectoryFileListContentsParameter(string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException("directoryPath");
            }

            this.ContentsSources = CONTENTS_SOURCES;
            this.SourcesKey = directoryPath;
            this.Key = string.Format("{0}ListContents:{1}", this.ContentsSources, this.SourcesKey);
            _directoryPath = directoryPath;
            _selectedFilePath = string.Empty;
        }

        public ContentsPanel CreateContents()
        {
            return new DirectoryFileListContents(this);
        }
    }
}
