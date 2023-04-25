using PicSum.UIComponent.Contents.FileListContents;
using SWF.UIComponent.TabOperation;

namespace PicSum.UIComponent.Contents.ContentsParameter
{
    /// <summary>
    /// クリップボードファイルリストコンテンツパラメータ
    /// </summary>
    public sealed class KeepFileListContentsParameter
        : IContentsParameter
    {
        public const string CONTENTS_SOURCES = "Keep";

        public string ContentsSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string Key { get; private set; }
        public string SelectedFilePath { get; set; }

        public KeepFileListContentsParameter()
        {
            this.ContentsSources = KeepFileListContentsParameter.CONTENTS_SOURCES;
            this.SourcesKey = string.Empty;
            this.Key = string.Format("{0}ListContents", this.ContentsSources);
            this.SelectedFilePath = string.Empty;
        }

        public ContentsPanel CreateContents()
        {
            return new KeepFileListContents(this);
        }
    }
}
