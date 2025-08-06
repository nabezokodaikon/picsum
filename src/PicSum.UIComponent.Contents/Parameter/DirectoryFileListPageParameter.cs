using PicSum.UIComponent.Contents.FileList;
using SWF.Core.Base;
using SWF.UIComponent.TabOperation;
using System;
using System.Drawing;
using System.Runtime.Versioning;

namespace PicSum.UIComponent.Contents.Parameter
{
    /// <summary>
    /// フォルダファイルリストコンテンツパラメータ
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class DirectoryFileListPageParameter
        : IPageParameter
    {
        public const string PAGE_SOURCES = "Directory";

        public string PageSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string Key { get; private set; }
        public string DirectoryPath { get; private set; }
        public string SelectedFilePath { get; set; }
        public int ScrollValue { get; set; }
        public Size FlowListSize { get; set; }
        public Size ItemSize { get; set; }
        public SortInfo SortInfo { get; set; }
        public bool VisibleBookmarkMenuItem { get; private set; }

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

        public PagePanel CreatePage()
        {
            return new DirectoryFileListPage(this);
        }
    }
}
