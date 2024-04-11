using System;
using System.Drawing;

namespace PicSum.UIComponent.Contents.Parameter
{
    public class DragEntity(
        string pageSources,
        string sourcesKey,
        string currentFilePath,
        Func<ImageViewerPageParameter, Action> getImageFilesAction,
        string pageTitle,
        Image pageIcon)
    {
        public string PageSources { get; private set; }
            = pageSources ?? throw new ArgumentNullException(nameof(pageSources));
        public string SourcesKey { get; private set; }
            = sourcesKey ?? throw new ArgumentNullException(nameof(sourcesKey));
        public string CurrentFilePath { get; private set; }
            = currentFilePath ?? throw new ArgumentNullException(nameof(currentFilePath));
        public Func<ImageViewerPageParameter, Action> GetImageFilesAction { get; private set; }
            = getImageFilesAction ?? throw new ArgumentNullException(nameof(getImageFilesAction));
        public string PageTitle { get; private set; }
            = pageTitle ?? throw new ArgumentNullException(nameof(pageTitle));
        public Image PageIcon { get; private set; }
            = pageIcon ?? throw new ArgumentNullException(nameof(pageIcon));
    }
}
