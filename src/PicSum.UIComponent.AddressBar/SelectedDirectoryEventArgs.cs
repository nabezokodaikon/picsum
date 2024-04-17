using PicSum.Core.Base.Conf;
using System;

namespace PicSum.UIComponent.AddressBar
{
    public sealed class SelectedDirectoryEventArgs
        : EventArgs
    {
        public PageOpenType OpenType { get; private set; } = PageOpenType.Default;
        public string DirectoryPath { get; private set; } = string.Empty;
        public string SubDirectoryPath { get; private set; } = string.Empty;

        public SelectedDirectoryEventArgs(PageOpenType openType, string directoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));

            this.OpenType = openType;
            this.DirectoryPath = directoryPath;
        }

        public SelectedDirectoryEventArgs(PageOpenType openType, string directoryPath, string subDirectoryPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));
            ArgumentException.ThrowIfNullOrEmpty(subDirectoryPath, nameof(subDirectoryPath));

            this.OpenType = openType;
            this.DirectoryPath = directoryPath;
            this.SubDirectoryPath = subDirectoryPath;
        }
    }
}
