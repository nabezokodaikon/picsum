using PicSum.UIComponent.Contents.FileList;
using SWF.Core.Base;
using SWF.UIComponent.TabOperation;
using System.Drawing;
using System.Runtime.Versioning;

namespace PicSum.UIComponent.Contents.Parameter
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class FavoriteDirectoryListPageParameter
        : IPageParameter
    {
        public const string PAGE_SOURCES = "Favorite";

        public string PageSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string Key { get; private set; }
        public string SelectedFilePath { get; set; }
        public int ScrollValue { get; set; }
        public Size FlowListSize { get; set; }
        public Size ItemSize { get; set; }
        public SortInfo SortInfo { get; set; }
        public bool VisibleBookmarkMenuItem { get; private set; }

        public FavoriteDirectoryListPageParameter()
        {
            this.PageSources = FavoriteDirectoryListPageParameter.PAGE_SOURCES;
            this.SourcesKey = string.Empty;
            this.Key = $"{this.PageSources}ListPage";
            this.SelectedFilePath = string.Empty;
            this.SortInfo = null;
            this.VisibleBookmarkMenuItem = false;
        }

        public PagePanel CreatePage()
        {
            // ディレクトリのみ表示のため、画像ビューアへの遷移は有り得ない。
            return new FavoriteDirectoryListPage(this);
        }
    }
}
