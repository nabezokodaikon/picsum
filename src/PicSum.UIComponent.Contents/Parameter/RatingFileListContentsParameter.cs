using PicSum.UIComponent.Contents.FileList;
using SWF.UIComponent.TabOperation;
using System;

namespace PicSum.UIComponent.Contents.Parameter
{
    /// <summary>
    /// 評価値ファイルリストコンテンツパラメータ
    /// </summary>
    public sealed class RatingFileListContentsParameter
        : IContentsParameter
    {
        public const string CONTENTS_SOURCES = "Rating";

        public string ContentsSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string Key { get; private set; }
        public int RagingValue { get; private set; }
        public string SelectedFilePath { get; set; }

        public RatingFileListContentsParameter(int ragingValue)
        {
            if (ragingValue < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(ragingValue));
            }

            this.ContentsSources = CONTENTS_SOURCES;
            this.SourcesKey = ragingValue.ToString();
            this.Key = string.Format("{0}ListContents:{1}", this.ContentsSources, this.SourcesKey);
            this.RagingValue = ragingValue;
            this.SelectedFilePath = string.Empty;
        }

        public ContentsPanel CreateContents()
        {
            return new RatingFileListContents(this);
        }
    }
}
