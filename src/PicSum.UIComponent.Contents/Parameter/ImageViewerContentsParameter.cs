using PicSum.UIComponent.Contents.ImageViewer;
using SWF.UIComponent.TabOperation;
using System;
using System.Drawing;

namespace PicSum.UIComponent.Contents.Parameter
{
    /// <summary>
    /// 画像表示コンテンツパラメータ
    /// </summary>
    public sealed class ImageViewerContentsParameter
        : IContentsParameter
    {
        public event EventHandler<GetImageFilesEventArgs> GetImageFiles;

        public string ContentsSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string Key { get; private set; }   
        public Func<ImageViewerContentsParameter, Action> GetImageFilesAction { get; private set; }
        public string ContentsTitle { get; private set; }
        public Image ContentsIcon { get; private set; }
        public string SelectedFilePath { get; set; }

        public ImageViewerContentsParameter(
            string contentsSources,
            string sourcesKey,
            Func<ImageViewerContentsParameter, Action> getImageFilesAction,
            string selectedFilePath,
            string contentsTitle,
            Image contentsIcon)
        {
            this.ContentsSources = contentsSources ?? throw new ArgumentNullException(nameof(contentsSources));
            this.SourcesKey = sourcesKey ?? throw new ArgumentNullException(nameof(sourcesKey));
            this.Key = string.Format("{0}ImageContents:{1}", this.ContentsSources, this.SourcesKey);
            this.GetImageFilesAction = getImageFilesAction ?? throw new ArgumentNullException(nameof(getImageFilesAction));
            this.ContentsTitle = contentsTitle ?? throw new ArgumentNullException(nameof(contentsTitle));
            this.ContentsIcon = contentsIcon ?? throw new ArgumentNullException(nameof(contentsIcon));
            this.SelectedFilePath = selectedFilePath ?? throw new ArgumentNullException(nameof(selectedFilePath));
        }

        public ContentsPanel CreateContents()
        {
            return new ImageViewerContents(this);
        }

        public void OnGetImageFiles(GetImageFilesEventArgs e)
        {
            if (this.GetImageFiles != null) 
            {
                this.GetImageFiles(this, e);
            }             
        }
    }
}
