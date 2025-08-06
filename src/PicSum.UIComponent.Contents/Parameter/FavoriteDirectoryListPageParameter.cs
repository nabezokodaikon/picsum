using PicSum.UIComponent.Contents.FileList;
using SWF.UIComponent.TabOperation;
using System.Runtime.Versioning;

namespace PicSum.UIComponent.Contents.Parameter
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class FavoriteDirectoryListPageParameter
        : AbstractPageParameter
    {
        public const string PAGE_SOURCES = "Favorite";

        public FavoriteDirectoryListPageParameter()
        {
            this.PageSources = FavoriteDirectoryListPageParameter.PAGE_SOURCES;
            this.SourcesKey = string.Empty;
            this.Key = $"{this.PageSources}ListPage";
            this.SelectedFilePath = string.Empty;
            this.SortInfo = null;
            this.VisibleBookmarkMenuItem = false;
        }

        public override PagePanel CreatePage()
        {
            // ディレクトリのみ表示のため、画像ビューアへの遷移は有り得ない。
            return new FavoriteDirectoryListPage(this);
        }
    }
}
