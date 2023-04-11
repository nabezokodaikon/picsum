
using System;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// コンテンツパラメータインターフェース
    /// </summary>
    public interface IContentsParameter
    {
        string Key { get; }
        string ContentsSources { get; }
        string SourcesKey { get; }
        ContentsPanel CreateContents();
    }
}
