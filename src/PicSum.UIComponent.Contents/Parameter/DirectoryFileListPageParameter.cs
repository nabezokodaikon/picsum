using PicSum.UIComponent.Contents.FileList;
using SWF.UIComponent.TabOperation;
using System;

namespace PicSum.UIComponent.Contents.Parameter
{
    /// <summary>
    /// フォルダファイルリストコンテンツパラメータ
    /// </summary>

    public sealed class DirectoryFileListPageParameter
        : AbstractPageParameter
    {
        public const string PAGE_SOURCES = "Directory";

        public string DirectoryPath { get; private set; }

        public DirectoryFileListPageParameter(string directoryPath, string selectedFilePath)
        {
            this.PageSources = DirectoryFileListPageParameter.PAGE_SOURCES;
            this.SourcesKey = directoryPath;
            this.Key = $"{this.PageSources}ListPage: {this.SourcesKey}";
            this.DirectoryPath = directoryPath ?? throw new ArgumentNullException(nameof(directoryPath));
            this.SelectedFilePath = selectedFilePath;
            this.SortInfo = null;
            this.VisibleBookmarkMenuItem = true;
        }

        public DirectoryFileListPageParameter(string directoryPath)
        {
            this.PageSources = DirectoryFileListPageParameter.PAGE_SOURCES;
            this.SourcesKey = directoryPath;
            this.Key = $"{this.PageSources}ListPage: {this.SourcesKey}";
            this.DirectoryPath = directoryPath ?? throw new ArgumentNullException(nameof(directoryPath));
            this.SelectedFilePath = string.Empty;
            this.SortInfo = null;
            this.VisibleBookmarkMenuItem = true;
        }

        public override PagePanel CreatePage()
        {
            return new DirectoryFileListPage(this);
        }
    }
}
