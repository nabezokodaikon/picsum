using PicSum.UIComponent.Contents.FileList;
using SWF.UIComponent.TabOperation;
using System;
using System.Runtime.Versioning;

namespace PicSum.UIComponent.Contents.Parameter
{
    /// <summary>
    /// 評価値ファイルリストコンテンツパラメータ
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class RatingFileListPageParameter
        : AbstractPageParameter
    {
        public const string PAGE_SOURCES = "Rating";

        public int RatingValue { get; private set; }

        public RatingFileListPageParameter(int ratingValue)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(ratingValue, 1, nameof(ratingValue));

            this.PageSources = PAGE_SOURCES;
            this.SourcesKey = ratingValue.ToString();
            this.Key = $"{this.PageSources}ListPage: {this.SourcesKey}";
            this.RatingValue = ratingValue;
            this.SelectedFilePath = string.Empty;
            this.SortInfo = null;
            this.VisibleBookmarkMenuItem = false;
        }

        public override PagePanel CreatePage()
        {
            return new RatingFileListPage(this);
        }
    }
}
