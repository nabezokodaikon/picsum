using System;
using System.Collections.Generic;

namespace PicSum.UIComponent.Contents.Common
{
    public sealed class SelectedFileChangeEventArgs
        : EventArgs
    {
        public IList<string> FilePathList { get; private set; }

        public SelectedFileChangeEventArgs()
        {
            this.FilePathList = [];
        }

        public SelectedFileChangeEventArgs(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this.FilePathList = [filePath];
        }

        public SelectedFileChangeEventArgs(IList<string> filePathList)
        {
            ArgumentNullException.ThrowIfNull(filePathList, nameof(filePathList));

            this.FilePathList = filePathList;
        }
    }
}
