using System;
using PicSum.Core.Base.Conf;

namespace PicSum.UIComponent.AddressBar
{
    public class SelectedDirectoryEventArgs : EventArgs
    {
        private ContentsOpenType _openType = ContentsOpenType.Default;
        private string _directoryPath = string.Empty;
        private string _subDirectoryPath = string.Empty;

        public ContentsOpenType OpenType
        {
            get
            {
                return _openType;
            }
        }

        public string DirectoryPath
        {
            get
            {
                return _directoryPath;
            }
        }

        public string SubDirectoryPath
        {
            get
            {
                return _subDirectoryPath;
            }
        }

        public SelectedDirectoryEventArgs(ContentsOpenType openType, string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException("directoryPath");
            }

            _openType = openType;
            _directoryPath = directoryPath;
        }

        public SelectedDirectoryEventArgs(ContentsOpenType openType, string directoryPath, string subDirectoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException("directoryPath");
            }

            if (subDirectoryPath == null)
            {
                throw new ArgumentNullException("subDirectoryPath");
            }

            _openType = openType;
            _directoryPath = directoryPath;
            _subDirectoryPath = subDirectoryPath;
        }
    }
}
