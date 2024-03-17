using PicSum.UIComponent.Contents.FileList;
using SWF.UIComponent.TabOperation;
using System.Runtime.Versioning;

namespace PicSum.UIComponent.Contents.Parameter
{
    [SupportedOSPlatform("windows")]
    public sealed class FavoriteDirectoryListContentsParameter
        : IContentsParameter
    {
        public const string CONTENTS_SOURCES = "Favorite";

        public string ContentsSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string Key { get; private set; }
        public string SelectedFilePath { get; set; }

        public FavoriteDirectoryListContentsParameter()
        {
            this.ContentsSources = FavoriteDirectoryListContentsParameter.CONTENTS_SOURCES;
            this.SourcesKey = string.Empty;
            this.Key = string.Format("{0}ListContents", this.ContentsSources);
            this.SelectedFilePath = string.Empty;
        }

        public ContentsPanel CreateContents()
        {
            // ディレクトリのみ表示のため、画像ビューアへの遷移は有り得ない。
            return new FavoriteDirectoryListContents(this);
        }
    }
}
