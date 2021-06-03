using System;
using SWF.UIComponent.TabOperation;
using PicSum.UIComponent.Contents.FileListContents;

namespace PicSum.UIComponent.Contents.ContentsParameter
{
    /// <summary>
    /// ファイル表示履歴ファイルリストコンテンツパラメータ
    /// </summary>
    public class FileHistoryFileListContentsParameter : IContentsParameter
    {
        private DateTime _viewDate;
        private string _selectedFilePath;

        public DateTime ViewDate
        {
            get
            {
                return _viewDate;
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

        public FileHistoryFileListContentsParameter(DateTime viewDate)
        {
            _viewDate = viewDate;
            _selectedFilePath = string.Empty;
        }

        public ContentsPanel CreateContents()
        {
            return new FileHistoryFileListContents(this);
        }
    }
}
