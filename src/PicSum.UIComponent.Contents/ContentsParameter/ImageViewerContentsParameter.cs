using System;
using System.Collections.Generic;
using SWF.UIComponent.TabOperation;

namespace PicSum.UIComponent.Contents.ContentsParameter
{
    /// <summary>
    /// 画像表示コンテンツパラメータ
    /// </summary>
    public class ImageViewerContentsParameter : IContentsParameter
    {
        private IList<string> _filePathList;
        private string _selectedFilePath;

        public IList<string> FilePathList
        {
            get
            {
                return _filePathList;
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

        public ImageViewerContentsParameter(IList<string> filePathList, string selectedFilePath)
        {
            if (filePathList == null)
            {
                throw new ArgumentNullException("filePathList");
            }

            if (selectedFilePath == null)
            {
                throw new ArgumentNullException("selectedFilePath");
            }

            _filePathList = filePathList;
            _selectedFilePath = selectedFilePath;
        }

        public ContentsPanel CreateContents()
        {
            return new ImageViewerContents.ImageViewerContents(this);
        }
    }
}
