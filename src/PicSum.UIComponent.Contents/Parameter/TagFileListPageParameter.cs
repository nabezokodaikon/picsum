using PicSum.UIComponent.Contents.FileList;
using SWF.UIComponent.TabOperation;
using System;
using System.Runtime.Versioning;

namespace PicSum.UIComponent.Contents.Parameter
{
    /// <summary>
    /// タグファイルリストコンテンツパラメータ
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class TagFileListPageParameter
        : IPageParameter
    {
        public const string PAGE_SOURCES = "Tag";

        public string PageSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string Key { get; private set; }
        public String Tag { get; private set; }
        public string SelectedFilePath { get; set; }

        public TagFileListPageParameter(string tag)
        {
            this.PageSources = TagFileListPageParameter.PAGE_SOURCES;
            this.SourcesKey = tag;
            this.Key = string.Format("{0}ListPage:{1}", this.PageSources, this.SourcesKey);
            this.Tag = tag ?? throw new ArgumentNullException(nameof(tag));
            this.SelectedFilePath = string.Empty;
        }

        public PagePanel CreatePage()
        {
            return new TagFileListPage(this);
        }
    }
}
