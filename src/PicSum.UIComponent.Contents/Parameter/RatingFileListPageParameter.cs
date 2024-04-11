using PicSum.UIComponent.Contents.FileList;
using SWF.UIComponent.TabOperation;
using System;
using System.Runtime.Versioning;

namespace PicSum.UIComponent.Contents.Parameter
{
    /// <summary>
    /// 評価値ファイルリストコンテンツパラメータ
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class RatingFileListPageParameter
        : IPageParameter
    {
        public const string PAGE_SOURCES = "Rating";

        public string PageSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string Key { get; private set; }
        public int RagingValue { get; private set; }
        public string SelectedFilePath { get; set; }

        public RatingFileListPageParameter(int ragingValue)
        {
            if (ragingValue < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(ragingValue));
            }

            this.PageSources = PAGE_SOURCES;
            this.SourcesKey = ragingValue.ToString();
            this.Key = string.Format("{0}ListPage:{1}", this.PageSources, this.SourcesKey);
            this.RagingValue = ragingValue;
            this.SelectedFilePath = string.Empty;
        }

        public PagePanel CreatePage()
        {
            return new RatingFileListPage(this);
        }
    }
}
