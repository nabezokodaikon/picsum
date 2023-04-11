using System;
using PicSum.UIComponent.Contents.FileListContents;
using SWF.UIComponent.TabOperation;

namespace PicSum.UIComponent.Contents.ContentsParameter
{
    /// <summary>
    /// クリップボードファイルリストコンテンツパラメータ
    /// </summary>
    public class KeepFileListContentsParameter : IContentsParameter
    {
        public const string CONTENTS_SOURCES = "Keep";

        private string _selectedFilePath;

        public string ContentsSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string Key { get; private set; }

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

        public KeepFileListContentsParameter()
        {
            this.ContentsSources = CONTENTS_SOURCES;
            this.SourcesKey = string.Empty;
            this.Key = string.Format("{0}ListContents", this.ContentsSources);
            _selectedFilePath = string.Empty;
        }

        public ContentsPanel CreateContents()
        {
            return new KeepFileListContents(this);
        }
    }
}
