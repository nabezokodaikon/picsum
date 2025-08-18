using SWF.Core.Base;
using System;

namespace PicSum.UIComponent.AddressBar
{

    public sealed class SelectedDirectoryEventArgs
        : EventArgs
    {
        public PageOpenMode OpenMode { get; private set; } = PageOpenMode.Default;
        public string DirectoryPath { get; private set; } = string.Empty;
        public string SubDirectoryPath { get; private set; } = string.Empty;

        public SelectedDirectoryEventArgs(PageOpenMode openMode, string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            this.OpenMode = openMode;
            this.DirectoryPath = directoryPath;
        }

        public SelectedDirectoryEventArgs(PageOpenMode openMode, string directoryPath, string subDirectoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));
            ArgumentException.ThrowIfNullOrEmpty(subDirectoryPath, nameof(subDirectoryPath));

            this.OpenMode = openMode;
            this.DirectoryPath = directoryPath;
            this.SubDirectoryPath = subDirectoryPath;
        }
    }
}
