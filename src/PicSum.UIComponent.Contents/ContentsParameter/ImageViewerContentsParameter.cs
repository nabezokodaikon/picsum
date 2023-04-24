using SWF.UIComponent.TabOperation;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace PicSum.UIComponent.Contents.ContentsParameter
{
    /// <summary>
    /// 画像表示コンテンツパラメータ
    /// </summary>
    public class ImageViewerContentsParameter
        : IContentsParameter
    {
        private string selectedFilePath;

        public string ContentsSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string Key { get; private set; }
        public IList<string> FilePathList { get; private set; }

        public string SelectedFilePath
        {
            get
            {
                return this.selectedFilePath;
            }
            set
            {
                this.selectedFilePath = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public string ContentsTitle { get; private set; }

        public Image ContentsIcon { get; private set; }

        public ImageViewerContentsParameter(
            string contentsSources,
            string sourcesKey,
            IList<string> filePathList,
            string selectedFilePath,
            string contentsTitle,
            Image contentsIcon)
        {
            this.ContentsSources = contentsSources ?? throw new ArgumentNullException(nameof(contentsSources));
            this.SourcesKey = sourcesKey ?? throw new ArgumentNullException(nameof(sourcesKey));
            this.Key = string.Format("{0}ImageContents:{1}", this.ContentsSources, this.SourcesKey);
            this.FilePathList = filePathList ?? throw new ArgumentNullException(nameof(filePathList));
            this.selectedFilePath = selectedFilePath ?? throw new ArgumentNullException(nameof(selectedFilePath));
            this.ContentsTitle = contentsTitle ?? throw new ArgumentNullException(nameof(contentsTitle));
            this.ContentsIcon = contentsIcon ?? throw new ArgumentNullException(nameof(contentsIcon));
        }

        public ContentsPanel CreateContents()
        {
            return new ImageViewerContents.ImageViewerContents(this);
        }
    }
}
