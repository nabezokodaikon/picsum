using PicSum.UIComponent.Contents.FileList;
using SWF.Core.Base;
using SWF.UIComponent.TabOperation;
using System.Runtime.Versioning;

namespace PicSum.UIComponent.Contents.Parameter
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class BookmarkFileListPageParameter
        : IPageParameter
    {
        public const string PAGE_SOURCES = "Bookmark";

        public string Key { get; private set; }
        public string PageSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string SelectedFilePath { get; set; }
        public SortInfo SortInfo { get; set; }
        public bool VisibleBookmarkMenuItem { get; private set; }
        public bool VisibleClipMenuItem { get; private set; }

        public BookmarkFileListPageParameter()
        {
            this.PageSources = BookmarkFileListPageParameter.PAGE_SOURCES;
            this.SourcesKey = string.Empty;
            this.Key = $"{this.PageSources}ListPage";
            this.SelectedFilePath = string.Empty;
            this.SortInfo = null;
            this.VisibleBookmarkMenuItem = false;
            this.VisibleClipMenuItem = true;
        }

        public PagePanel CreatePage()
        {
            return new BookmarkFileListPage(this);
        }
    }
}
