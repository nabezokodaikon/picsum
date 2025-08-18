using PicSum.UIComponent.Contents.Common;
using PicSum.UIComponent.Contents.ImageView;
using SWF.Core.Base;
using SWF.Core.Job;
using SWF.UIComponent.TabOperation;
using System;
using System.Drawing;

namespace PicSum.UIComponent.Contents.Parameter
{
    /// <summary>
    /// 画像表示コンテンツパラメータ
    /// </summary>

    public sealed class ImageViewPageParameter
        : AbstractPageParameter
    {
        public event EventHandler<GetImageFilesEventArgs> GetImageFiles;

        public Func<ImageViewPageParameter, Action<ISender>> ImageFilesGetAction { get; private set; }
        public string PageTitle { get; private set; }
        public Image PageIcon { get; private set; }

        public ImageViewPageParameter(
            string pageSources,
            string sourcesKey,
            Func<ImageViewPageParameter, Action<ISender>> imageFilesGetAction,
            string selectedFilePath,
            SortParameter sortInfo,
            string pageTitle,
            Image pageIcon,
            bool visibleBookmarkMenuItem)
        {
            this.PageSources = pageSources ?? throw new ArgumentNullException(nameof(pageSources));
            this.SourcesKey = sourcesKey ?? throw new ArgumentNullException(nameof(sourcesKey));
            this.Key = $"{this.PageSources}ImageViewPage: {this.SourcesKey}";
            this.ImageFilesGetAction = imageFilesGetAction ?? throw new ArgumentNullException(nameof(imageFilesGetAction));
            this.PageTitle = pageTitle ?? throw new ArgumentNullException(nameof(pageTitle));
            this.PageIcon = pageIcon ?? throw new ArgumentNullException(nameof(pageIcon));
            this.SortInfo = sortInfo ?? throw new ArgumentNullException(nameof(sortInfo));
            this.SelectedFilePath = selectedFilePath ?? throw new ArgumentNullException(nameof(selectedFilePath));
            this.VisibleBookmarkMenuItem = visibleBookmarkMenuItem;
        }

        public override PagePanel CreatePage()
        {
            return new ImageViewPage(this);
        }

        public void OnGetImageFiles(GetImageFilesEventArgs e)
        {
            this.GetImageFiles?.Invoke(this, e);
        }
    }
}
