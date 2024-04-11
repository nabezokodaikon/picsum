using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.ImageViewer;
using SWF.UIComponent.TabOperation;
using System;
using System.Drawing;
using System.Runtime.Versioning;

namespace PicSum.UIComponent.Contents.Parameter
{
    /// <summary>
    /// 画像表示コンテンツパラメータ
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class ImageViewerPageParameter
        : IPageParameter
    {
        public event EventHandler<GetImageFilesEventArgs> GetImageFiles;

        public string PageSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string Key { get; private set; }
        public Func<ImageViewerPageParameter, Action> GetImageFilesAction { get; private set; }
        public string PageTitle { get; private set; }
        public Image PageIcon { get; private set; }
        public string SelectedFilePath { get; set; }

        public ImageViewerPageParameter(
            string pageSources,
            string sourcesKey,
            Func<ImageViewerPageParameter, Action> getImageFilesAction,
            string selectedFilePath,
            string pageTitle,
            Image pageIcon)
        {
            this.PageSources = pageSources ?? throw new ArgumentNullException(nameof(pageSources));
            this.SourcesKey = sourcesKey ?? throw new ArgumentNullException(nameof(sourcesKey));
            this.Key = string.Format("{0}ImageViewerPage:{1}", this.PageSources, this.SourcesKey);
            this.GetImageFilesAction = getImageFilesAction ?? throw new ArgumentNullException(nameof(getImageFilesAction));
            this.PageTitle = pageTitle ?? throw new ArgumentNullException(nameof(pageTitle));
            this.PageIcon = pageIcon ?? throw new ArgumentNullException(nameof(pageIcon));
            this.SelectedFilePath = selectedFilePath ?? throw new ArgumentNullException(nameof(selectedFilePath));
        }

        public PagePanel CreatePage()
        {
            return new ImageViewerPage(this);
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
