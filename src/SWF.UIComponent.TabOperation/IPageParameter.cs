using SWF.Core.Base;
using System.Drawing;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// コンテンツパラメータインターフェース
    /// </summary>
    public interface IPageParameter
    {
        string Key { get; }
        string PageSources { get; }
        string SourcesKey { get; }
        string SelectedFilePath { get; set; }
        int ScrollValue { get; set; }
        Size FlowListSize { get; set; }
        Size ItemSize { get; set; }
        SortInfo SortInfo { get; set; }
        public bool VisibleBookmarkMenuItem { get; }
        PagePanel CreatePage();
    }
}
