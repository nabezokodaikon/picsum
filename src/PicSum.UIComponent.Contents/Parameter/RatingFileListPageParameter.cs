using PicSum.UIComponent.Contents.FileList;
using SWF.Common;
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
        public int RatingValue { get; private set; }
        public string SelectedFilePath { get; set; }
        public SortInfo SortInfo { get; set; }

        public RatingFileListPageParameter(int ratingValue)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(ratingValue, 1, nameof(ratingValue));

            this.PageSources = PAGE_SOURCES;
            this.SourcesKey = ratingValue.ToString();
            this.Key = $"{this.PageSources}ListPage: {this.SourcesKey}";
            this.RatingValue = ratingValue;
            this.SelectedFilePath = string.Empty;
            this.SortInfo = null;
        }

        public PagePanel CreatePage()
        {
            return new RatingFileListPage(this);
        }
    }
}
