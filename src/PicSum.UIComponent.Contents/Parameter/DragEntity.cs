using System;
using System.Drawing;

namespace PicSum.UIComponent.Contents.Parameter
{
    public class DragEntity
    {
        public string PageSources { get; private set; }
        public string SourcesKey { get; private set; }
        public string CurrentFilePath { get; private set; }
        public Func<ImageViewerPageParameter, Action> GetImageFilesAction { get; private set; }
        public string PageTitle { get; private set; }
        public Image PageIcon { get; private set; }

        public DragEntity(
            string pageSources,
            string sourcesKey,
            string currentFilePath,
            Func<ImageViewerPageParameter, Action> getImageFilesAction,
            string pageTitle,
            Image pageIcon)
        {
            this.PageSources = pageSources ?? throw new ArgumentNullException(nameof(pageSources));
            this.SourcesKey = sourcesKey ?? throw new ArgumentNullException(nameof(sourcesKey));
            this.CurrentFilePath = currentFilePath ?? throw new ArgumentNullException(nameof(currentFilePath));
            this.GetImageFilesAction = getImageFilesAction ?? throw new ArgumentNullException(nameof(getImageFilesAction));
            this.PageTitle = pageTitle ?? throw new ArgumentNullException(nameof(pageTitle));
            this.PageIcon = pageIcon ?? throw new ArgumentNullException(nameof(pageIcon));
        }
    }
}
