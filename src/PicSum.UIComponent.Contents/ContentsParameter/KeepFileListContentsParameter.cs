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
        private string _selectedFilePath;

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
            _selectedFilePath = string.Empty;
        }

        public ContentsPanel CreateContents()
        {
            return new KeepFileListContents(this);
        }
    }
}
