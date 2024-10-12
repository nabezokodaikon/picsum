using System;
using System.Collections.Generic;
using System.Drawing;

namespace PicSum.UIComponent.Contents.Common
{
    public sealed class GetImageFilesEventArgs(
        IList<string> filePathList,
        string selectedFilePath,
        string pageTitle,
        Image pageIcon)
                : EventArgs
    {
        public IList<string> FilePathList { get; private set; }
            = filePathList ?? throw new ArgumentNullException(nameof(filePathList));
        public string SelectedFilePath { get; private set; }
            = selectedFilePath ?? throw new ArgumentNullException(nameof(selectedFilePath));
        public string PageTitle { get; private set; }
            = pageTitle ?? throw new ArgumentNullException(nameof(pageTitle));
        public Image PageIcon { get; private set; }
            = pageIcon ?? throw new ArgumentNullException(nameof(pageIcon));
    }
}
