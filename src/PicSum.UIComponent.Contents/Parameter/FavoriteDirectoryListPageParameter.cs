using PicSum.UIComponent.Contents.FileList;
using SWF.UIComponent.TabOperation;
using System.Runtime.Versioning;

namespace PicSum.UIComponent.Contents.Parameter
{
    [SupportedOSPlatform("windows")]
    public sealed class FavoriteDirectoryListPageParameter
        : IPageParameter
    {
        public const string PAGE_SOURCES = "Favorite";

        public string PageSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string Key { get; private set; }
        public string SelectedFilePath { get; set; }

        public FavoriteDirectoryListPageParameter()
        {
            this.PageSources = FavoriteDirectoryListPageParameter.PAGE_SOURCES;
            this.SourcesKey = string.Empty;
            this.Key = string.Format("{0}ListPage", this.PageSources);
            this.SelectedFilePath = string.Empty;
        }

        public PagePanel CreatePage()
        {
            // ディレクトリのみ表示のため、画像ビューアへの遷移は有り得ない。
            return new FavoriteDirectoryListPage(this);
        }
    }
}
