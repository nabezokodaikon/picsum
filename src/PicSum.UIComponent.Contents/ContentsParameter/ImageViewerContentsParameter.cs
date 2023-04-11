using System;
using System.Collections.Generic;
using System.Drawing;
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

        public string ContentsSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string Key { get; private set; }

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

        public string ContentsTitle { get; private set; }

        public Image ContentsIcon { get; private set; }

        public ImageViewerContentsParameter(string contentsSources, string sourcesKey, IList<string> filePathList, string selectedFilePath, string contentsTitle, Image contentsIcon)
        {
            if (filePathList == null)
            {
                throw new ArgumentNullException("filePathList");
            }

            if (selectedFilePath == null)
            {
                throw new ArgumentNullException("selectedFilePath");
            }

            this.ContentsSources = contentsSources ?? throw new ArgumentNullException(nameof(contentsSources));
            this.SourcesKey = sourcesKey ?? throw new ArgumentNullException(nameof(sourcesKey));
            this.Key = string.Format("{0}ImageContents:{1}", this.ContentsSources, this.SourcesKey);

            _filePathList = filePathList;
            _selectedFilePath = selectedFilePath;
            this.ContentsTitle = contentsTitle ?? throw new ArgumentNullException(nameof(contentsTitle));
            this.ContentsIcon = contentsIcon ?? throw new ArgumentNullException(nameof(contentsIcon));
        }

        public ContentsPanel CreateContents()
        {
            return new ImageViewerContents.ImageViewerContents(this);
        }
    }
}
