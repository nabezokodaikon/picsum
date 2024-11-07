using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.ImageViewer;
using SWF.Core.Base;
using SWF.Core.Job;
using SWF.UIComponent.TabOperation;
using System;
using System.Runtime.Versioning;

namespace PicSum.UIComponent.Contents.Parameter
{
    /// <summary>
    /// 画像表示コンテンツパラメータ
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class ImageViewerPageParameter
        : IPageParameter
    {
        public event EventHandler<GetImageFilesEventArgs> GetImageFiles;

        public string PageSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string Key { get; private set; }
        public Func<ImageViewerPageParameter, Action<ISender>> ImageFilesGetAction { get; private set; }
        public string PageTitle { get; private set; }
        public System.Drawing.Image PageIcon { get; private set; }
        public string SelectedFilePath { get; set; }
        public SortInfo SortInfo { get; set; }
        public bool VisibleBookmarkMenuItem { get; private set; }
        public bool VisibleClipMenuItem { get; private set; }

        public ImageViewerPageParameter(
            string pageSources,
            string sourcesKey,
            Func<ImageViewerPageParameter, Action<ISender>> imageFilesGetAction,
            string selectedFilePath,
            SortInfo sortInfo,
            string pageTitle,
            System.Drawing.Image pageIcon,
            bool visibleBookmarkMenuItem,
            bool visibleClipMenuItem)
        {
            this.PageSources = pageSources ?? throw new ArgumentNullException(nameof(pageSources));
            this.SourcesKey = sourcesKey ?? throw new ArgumentNullException(nameof(sourcesKey));
            this.Key = $"{this.PageSources}ImageViewerPage: {this.SourcesKey}";
            this.ImageFilesGetAction = imageFilesGetAction ?? throw new ArgumentNullException(nameof(imageFilesGetAction));
            this.PageTitle = pageTitle ?? throw new ArgumentNullException(nameof(pageTitle));
            this.PageIcon = pageIcon ?? throw new ArgumentNullException(nameof(pageIcon));
            this.SortInfo = sortInfo ?? throw new ArgumentNullException(nameof(sortInfo));
            this.SelectedFilePath = selectedFilePath ?? throw new ArgumentNullException(nameof(selectedFilePath));
            this.VisibleBookmarkMenuItem = visibleBookmarkMenuItem;
            this.VisibleClipMenuItem = visibleClipMenuItem;
        }

        public PagePanel CreatePage()
        {
            return new ImageViewerPage(this);
        }

        public void OnGetImageFiles(GetImageFilesEventArgs e)
        {
            this.GetImageFiles?.Invoke(this, e);
        }
    }
}
