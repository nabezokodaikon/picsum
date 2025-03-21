using SWF.Core.Base;

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
        SortInfo SortInfo { get; set; }
        public bool VisibleBookmarkMenuItem { get; }
        PagePanel CreatePage();
    }
}
