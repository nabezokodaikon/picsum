using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.ImageViewer;
using SWF.Core.Base;
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
        public Func<ImageViewerPageParameter, Action> ImageFilesGetAction { get; private set; }
        public string PageTitle { get; private set; }
        public Image PageIcon { get; private set; }
        public string SelectedFilePath { get; set; }
        public SortInfo SortInfo { get; set; }

        public ImageViewerPageParameter(
            string pageSources,
            string sourcesKey,
            Func<ImageViewerPageParameter, Action> imageFilesGetAction,
            string selectedFilePath,
            SortInfo sortInfo,
            string pageTitle,
            Image pageIcon)
        {
            this.PageSources = pageSources ?? throw new ArgumentNullException(nameof(pageSources));
            this.SourcesKey = sourcesKey ?? throw new ArgumentNullException(nameof(sourcesKey));
            this.Key = $"{this.PageSources}ImageViewerPage: {this.SourcesKey}";
            this.ImageFilesGetAction = imageFilesGetAction ?? throw new ArgumentNullException(nameof(imageFilesGetAction));
            this.PageTitle = pageTitle ?? throw new ArgumentNullException(nameof(pageTitle));
            this.PageIcon = pageIcon ?? throw new ArgumentNullException(nameof(pageIcon));
            this.SortInfo = sortInfo ?? throw new ArgumentNullException(nameof(sortInfo));
            this.SelectedFilePath = selectedFilePath ?? throw new ArgumentNullException(nameof(selectedFilePath));
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
