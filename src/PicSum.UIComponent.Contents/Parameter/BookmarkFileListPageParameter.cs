using PicSum.UIComponent.Contents.FileList;
using SWF.UIComponent.TabOperation;
using System.Runtime.Versioning;

namespace PicSum.UIComponent.Contents.Parameter
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class BookmarkFileListPageParameter
        : AbstractPageParameter
    {
        public const string PAGE_SOURCES = "Bookmark";

        public BookmarkFileListPageParameter()
        {
            this.PageSources = BookmarkFileListPageParameter.PAGE_SOURCES;
            this.SourcesKey = string.Empty;
            this.Key = $"{this.PageSources}ListPage";
            this.SelectedFilePath = string.Empty;
            this.SortInfo = null;
            this.VisibleBookmarkMenuItem = true;
        }

        public override PagePanel CreatePage()
        {
            return new BookmarkFileListPage(this);
        }
    }
}
