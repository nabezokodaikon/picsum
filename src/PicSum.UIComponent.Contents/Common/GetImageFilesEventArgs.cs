using System;
using System.Collections.Generic;
using System.Drawing;

namespace PicSum.UIComponent.Contents.Common
{
    public sealed class GetImageFilesEventArgs
        : EventArgs
    {
        public IList<string> FilePathList { get; private set; }
        public string SelectedFilePath { get; private set; }
        public string PageTitle { get; private set; }
        public Image PageIcon { get; private set; }

        public GetImageFilesEventArgs(
            IList<string> filePathList,
            string selectedFilePath,
            string pageTitle,
            Image pageIcon)
        {
            this.FilePathList = filePathList ?? throw new ArgumentNullException(nameof(filePathList));
            this.SelectedFilePath = selectedFilePath ?? throw new ArgumentNullException(nameof(selectedFilePath));
            this.PageTitle = pageTitle ?? throw new ArgumentNullException(nameof(pageTitle));
            this.PageIcon = pageIcon ?? throw new ArgumentNullException(nameof(pageIcon));
        }
    }
}
