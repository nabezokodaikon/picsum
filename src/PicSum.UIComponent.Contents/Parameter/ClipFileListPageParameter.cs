using PicSum.UIComponent.Contents.FileList;
using SWF.Core.Base;
using SWF.UIComponent.TabOperation;
using System.Runtime.Versioning;

namespace PicSum.UIComponent.Contents.Parameter
{
    /// <summary>
    /// 評価値ファイルリストコンテンツパラメータ
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ClipFileListPageParameter
        : IPageParameter
    {
        public const string PAGE_SOURCES = "Clip";

        public string PageSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string Key { get; private set; }
        public string SelectedFilePath { get; set; }
        public SortInfo SortInfo { get; set; }

        public ClipFileListPageParameter()
        {
            this.PageSources = PAGE_SOURCES;
            this.SourcesKey = string.Empty;
            this.Key = $"{this.PageSources}ListPage";
            this.SelectedFilePath = string.Empty;
            this.SortInfo = null;
        }

        public PagePanel CreatePage()
        {
            return new ClipFileListPage(this);
        }
    }
}
