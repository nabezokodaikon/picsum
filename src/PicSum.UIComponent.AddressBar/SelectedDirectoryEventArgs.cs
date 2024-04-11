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
            if (directoryPath == null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            this.OpenType = openType;
            this.DirectoryPath = directoryPath;
        }

        public SelectedDirectoryEventArgs(PageOpenType openType, string directoryPath, string subDirectoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            if (subDirectoryPath == null)
            {
                throw new ArgumentNullException(nameof(subDirectoryPath));
            }

            this.OpenType = openType;
            this.DirectoryPath = directoryPath;
            this.SubDirectoryPath = subDirectoryPath;
        }
    }
}
