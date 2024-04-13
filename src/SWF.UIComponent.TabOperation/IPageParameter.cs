using SWF.Common;

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
        SortInfo SortInfo { get; set; }
        PagePanel CreatePage();
    }
}
