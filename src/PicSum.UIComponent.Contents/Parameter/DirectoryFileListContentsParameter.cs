using PicSum.UIComponent.Contents.FileList;
using SWF.UIComponent.TabOperation;
using System;

namespace PicSum.UIComponent.Contents.Parameter
{
    /// <summary>
    /// フォルダファイルリストコンテンツパラメータ
    /// </summary>
    public sealed class DirectoryFileListContentsParameter
        : IContentsParameter
    {
        public const string CONTENTS_SOURCES = "Directory";

        public string ContentsSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string Key { get; private set; }
        public string DirectoryPath { get; private set; }
        public string SelectedFilePath { get; private set; }

        public DirectoryFileListContentsParameter(string directoryPath, string selectedFilePath)
        {
            this.ContentsSources = DirectoryFileListContentsParameter.CONTENTS_SOURCES;
            this.SourcesKey = directoryPath;
            this.Key = string.Format("{0}ListContents:{1}", this.ContentsSources, this.SourcesKey);
            this.DirectoryPath = directoryPath ?? throw new ArgumentNullException(nameof(directoryPath));
            this.SelectedFilePath = selectedFilePath;
        }

        public DirectoryFileListContentsParameter(string directoryPath)
        {
            this.ContentsSources = DirectoryFileListContentsParameter.CONTENTS_SOURCES;
            this.SourcesKey = directoryPath;
            this.Key = string.Format("{0}ListContents:{1}", this.ContentsSources, this.SourcesKey);
            this.DirectoryPath = directoryPath ?? throw new ArgumentNullException(nameof(directoryPath));
            this.SelectedFilePath = string.Empty;
        }

        public ContentsPanel CreateContents()
        {
            return new DirectoryFileListContents(this);
        }
    }
}
