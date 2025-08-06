using PicSum.UIComponent.Contents.FileList;
using SWF.UIComponent.TabOperation;
using System;
using System.Runtime.Versioning;

namespace PicSum.UIComponent.Contents.Parameter
{
    /// <summary>
    /// タグファイルリストコンテンツパラメータ
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class TagFileListPageParameter
        : AbstractPageParameter
    {
        public const string PAGE_SOURCES = "Tag";

        public String Tag { get; private set; }

        public TagFileListPageParameter(string tag)
        {
            this.PageSources = TagFileListPageParameter.PAGE_SOURCES;
            this.SourcesKey = tag;
            this.Key = $"{this.PageSources}ListPage: {this.SourcesKey}";
            this.Tag = tag ?? throw new ArgumentNullException(nameof(tag));
            this.SelectedFilePath = string.Empty;
            this.SortInfo = null;
            this.VisibleBookmarkMenuItem = false;
        }

        public override PagePanel CreatePage()
        {
            return new TagFileListPage(this);
        }
    }
}
