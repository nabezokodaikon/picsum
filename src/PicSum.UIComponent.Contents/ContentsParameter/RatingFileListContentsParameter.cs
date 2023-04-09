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
        private int _ragingValue;
        private string _selectedFilePath;

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

        public Action<bool> AfterLoadAction { get; private set; }

        public RatingFileListContentsParameter(int ragingValue, Action<bool> afterLoadAction)
        {
            if (ragingValue < 1)
            {
                throw new ArgumentOutOfRangeException("ragingValue");
            }

            _ragingValue = ragingValue;
            _selectedFilePath = string.Empty;
            this.AfterLoadAction = afterLoadAction ?? throw new ArgumentNullException(nameof(afterLoadAction));
        }

        public ContentsPanel CreateContents()
        {
            return new RatingFileListContents(this);
        }
    }
}
