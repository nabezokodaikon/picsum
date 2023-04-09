using System;
using PicSum.UIComponent.Contents.FileListContents;
using SWF.UIComponent.TabOperation;

namespace PicSum.UIComponent.Contents.ContentsParameter
{
    /// <summary>
    /// タグファイルリストコンテンツパラメータ
    /// </summary>
    public class TagFileListContentsParameter : IContentsParameter
    {
        private string _tag;
        private string _selectedFilePath;

        public String Tag
        {
            get
            {
                return _tag;
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

        public TagFileListContentsParameter(string tag, Action<bool> afterLoadAction)
        {
            if (tag == null)
            {
                throw new ArgumentNullException("tag");
            }

            _tag = tag;
            _selectedFilePath = string.Empty;
            this.AfterLoadAction = afterLoadAction ?? throw new ArgumentNullException(nameof(afterLoadAction));
        }

        public ContentsPanel CreateContents()
        {
            return new TagFileListContents(this);
        }
    }
}
