using PicSum.UIComponent.Contents.FileList;
using SWF.UIComponent.TabOperation;

namespace PicSum.UIComponent.Contents.Parameter
{

    public sealed class HistoryFileListPageParameter
        : AbstractPageParameter
    {
        public const string PAGE_SOURCES = "History";

        public HistoryFileListPageParameter()
        {
            this.PageSources = HistoryFileListPageParameter.PAGE_SOURCES;
            this.SourcesKey = string.Empty;
            this.Key = $"{this.PageSources}ListPage";
            this.SelectedFilePath = string.Empty;
            this.SortInfo = null;
            this.VisibleBookmarkMenuItem = true;
        }

        public override PagePanel CreatePage()
        {
            return new HisotryFileListPage(this);
        }
    }
}
