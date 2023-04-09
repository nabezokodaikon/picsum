
using System;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// コンテンツパラメータインターフェース
    /// </summary>
    public interface IContentsParameter
    {
        ContentsPanel CreateContents();
        Action<bool> AfterLoadAction { get; }
    }
}
