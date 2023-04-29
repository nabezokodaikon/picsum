using PicSum.UIComponent.Contents.FileList;
using SWF.UIComponent.TabOperation;

namespace PicSum.UIComponent.Contents.Parameter
{
    public sealed class BookmarkFileListContentsParameter
        : IContentsParameter
    {
        public const string CONTENTS_SOURCES = "Bookmark";

        public string Key { get; private set; }
        public string ContentsSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string SelectedFilePath { get; set; }

        public BookmarkFileListContentsParameter()
        {
            this.ContentsSources = BookmarkFileListContentsParameter.CONTENTS_SOURCES;
            this.SourcesKey = string.Empty;
            this.Key = string.Format("{0}ListContents", this.ContentsSources);
            this.SelectedFilePath = string.Empty;
        }

        public ContentsPanel CreateContents()
        {
            return new BookmarkFileListContents(this);
        }
    }
}
