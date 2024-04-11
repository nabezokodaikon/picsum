using PicSum.UIComponent.Contents.FileList;
using SWF.UIComponent.TabOperation;
using System;
using System.Runtime.Versioning;

namespace PicSum.UIComponent.Contents.Parameter
{
    /// <summary>
    /// フォルダファイルリストコンテンツパラメータ
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class DirectoryFileListPageParameter
        : IPageParameter
    {
        public const string PAGE_SOURCES = "Directory";

        public string PageSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string Key { get; private set; }
        public string DirectoryPath { get; private set; }
        public string SelectedFilePath { get; private set; }

        public DirectoryFileListPageParameter(string directoryPath, string selectedFilePath)
        {
            this.PageSources = DirectoryFileListPageParameter.PAGE_SOURCES;
            this.SourcesKey = directoryPath;
            this.Key = string.Format("{0}ListPage:{1}", this.PageSources, this.SourcesKey);
            this.DirectoryPath = directoryPath ?? throw new ArgumentNullException(nameof(directoryPath));
            this.SelectedFilePath = selectedFilePath;
        }

        public DirectoryFileListPageParameter(string directoryPath)
        {
            this.PageSources = DirectoryFileListPageParameter.PAGE_SOURCES;
            this.SourcesKey = directoryPath;
            this.Key = string.Format("{0}ListPage:{1}", this.PageSources, this.SourcesKey);
            this.DirectoryPath = directoryPath ?? throw new ArgumentNullException(nameof(directoryPath));
            this.SelectedFilePath = string.Empty;
        }

        public PagePanel CreatePage()
        {
            return new DirectoryFileListPage(this);
        }
    }
}
