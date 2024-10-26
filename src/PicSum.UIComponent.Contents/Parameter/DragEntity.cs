using SWF.Core.Base;
using System;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.Parameter
{
    public class DragEntity(
        object sender,
        string pageSources,
        string sourcesKey,
        string currentFilePath,
        SortInfo sortInfo,
        Func<ImageViewerPageParameter, Action<Control>> getImageFilesAction,
        string pageTitle,
        System.Drawing.Image pageIcon)
    {
        public object Sender { get; private set; }
            = sender ?? throw new ArgumentNullException(nameof(sender));

        public string PageSources { get; private set; }
            = pageSources ?? throw new ArgumentNullException(nameof(pageSources));

        public string SourcesKey { get; private set; }
            = sourcesKey ?? throw new ArgumentNullException(nameof(sourcesKey));

        public string CurrentFilePath { get; private set; }
            = currentFilePath ?? throw new ArgumentNullException(nameof(currentFilePath));

        public SortInfo SortInfo { get; private set; }
            = sortInfo ?? throw new ArgumentNullException(nameof(sortInfo));

        public Func<ImageViewerPageParameter, Action<Control>> GetImageFilesAction { get; private set; }
            = getImageFilesAction ?? throw new ArgumentNullException(nameof(getImageFilesAction));

        public string PageTitle { get; private set; }
            = pageTitle ?? throw new ArgumentNullException(nameof(pageTitle));

        public System.Drawing.Image PageIcon { get; private set; }
            = pageIcon ?? throw new ArgumentNullException(nameof(pageIcon));
    }
}
