using System;
using System.Collections.Generic;
using System.Drawing;

namespace PicSum.UIComponent.Contents
{
    public sealed class GetImageFilesEventArgs
        : EventArgs
    {
        public IList<string> FilePathList { get; private set; }
        public string SelectedFilePath { get; private set; }
        public string ContentsTitle { get; private set; }
        public Image ContentsIcon { get; private set; }

        public GetImageFilesEventArgs(
            IList<string> filePathList,
            string selectedFilePath,
            string contentsTitle,
            Image contentsIcon)
        {
            this.FilePathList = filePathList ?? throw new ArgumentNullException(nameof(filePathList));
            this.SelectedFilePath = selectedFilePath ?? throw new ArgumentNullException(nameof(selectedFilePath));
            this.ContentsTitle = contentsTitle ?? throw new ArgumentNullException(nameof(contentsTitle));
            this.ContentsIcon = contentsIcon ?? throw new ArgumentNullException(nameof(contentsIcon));
        }
    }
}
