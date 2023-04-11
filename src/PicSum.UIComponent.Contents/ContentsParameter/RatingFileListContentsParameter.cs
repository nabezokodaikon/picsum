using System;
using PicSum.UIComponent.Contents.FileListContents;
using SWF.UIComponent.TabOperation;

namespace PicSum.UIComponent.Contents.ContentsParameter
{
    /// <summary>
    /// 評価値ファイルリストコンテンツパラメータ
    /// </summary>
    public class RatingFileListContentsParameter : IContentsParameter
    {
        public const string CONTENTS_SOURCES = "Rating";

        private int _ragingValue;
        private string _selectedFilePath;

        public string ContentsSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string Key { get; private set; }

        public int RagingValue
        {
            get
            {
                return _ragingValue;
            }
        }

        public string SelectedFilePath
        {
            get
            {
                return _selectedFilePath;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _selectedFilePath = value;
            }
        }

        public RatingFileListContentsParameter(int ragingValue)
        {
            if (ragingValue < 1)
            {
                throw new ArgumentOutOfRangeException("ragingValue");
            }

            this.ContentsSources = CONTENTS_SOURCES;
            this.SourcesKey = ragingValue.ToString();
            this.Key = string.Format("{0}ListContents:{1}", this.ContentsSources, this.SourcesKey);
            _ragingValue = ragingValue;
            _selectedFilePath = string.Empty;
        }

        public ContentsPanel CreateContents()
        {
            return new RatingFileListContents(this);
        }
    }
}
