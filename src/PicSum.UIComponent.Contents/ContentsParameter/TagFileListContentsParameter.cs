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
        public const string CONTENTS_SOURCES = "Tag";

        private string _tag;
        private string _selectedFilePath;

        public string ContentsSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string Key { get; private set; }

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

        public TagFileListContentsParameter(string tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException("tag");
            }

            this.ContentsSources = CONTENTS_SOURCES;
            this.SourcesKey = tag;
            this.Key = string.Format("{0}ListContents:{1}", this.ContentsSources, this.SourcesKey);
            _tag = tag;
            _selectedFilePath = string.Empty;
        }

        public ContentsPanel CreateContents()
        {
            return new TagFileListContents(this);
        }
    }
}
