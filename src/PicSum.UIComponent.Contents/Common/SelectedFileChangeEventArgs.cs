using System;

namespace PicSum.UIComponent.Contents.Common
{
    public sealed class SelectedFileChangeEventArgs
        : EventArgs
    {
        public string[] FilePathList { get; private set; }

        public SelectedFileChangeEventArgs()
        {
            this.FilePathList = [];
        }

        public SelectedFileChangeEventArgs(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            this.FilePathList = [filePath];
        }

        public SelectedFileChangeEventArgs(string[] filePathList)
        {
            ArgumentNullException.ThrowIfNull(filePathList, nameof(filePathList));

            this.FilePathList = filePathList;
        }
    }
}
