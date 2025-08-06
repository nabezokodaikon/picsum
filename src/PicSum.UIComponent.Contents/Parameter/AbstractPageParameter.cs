using SWF.Core.Base;
using SWF.UIComponent.FlowList;
using SWF.UIComponent.TabOperation;

namespace PicSum.UIComponent.Contents.Parameter
{
    public abstract class AbstractPageParameter
        : IPageParameter
    {
        public string Key { get; protected set; }
        public string PageSources { get; protected set; }
        public string SourcesKey { get; protected set; }

        public string SelectedFilePath { get; set; }
        public ScrollParameter ScrollInfo { get; set; }
        public SortParameter SortInfo { get; set; }
        public bool VisibleBookmarkMenuItem { get; protected set; }

        public abstract PagePanel CreatePage();
    }
}
