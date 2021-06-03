using System;
using PicSum.Core.Base.Conf;

namespace PicSum.UIComponent.AddressBar
{
    public class SelectedFolderEventArgs : EventArgs
    {
        private ContentsOpenType _openType = ContentsOpenType.Default;
        private string _folderPath = string.Empty;
        private string _subFolderPath = string.Empty;

        public ContentsOpenType OpenType
        {
            get
            {
                return _openType;
            }
        }

        public string FolderPath
        {
            get
            {
                return _folderPath;
            }
        }

        public string SubFolderPath
        {
            get
            {
                return _subFolderPath;
            }
        }

        public SelectedFolderEventArgs(ContentsOpenType openType, string folderPath)
        {
            if (folderPath == null)
            {
                throw new ArgumentNullException("folderPath");
            }

            _openType = openType;
            _folderPath = folderPath;
        }

        public SelectedFolderEventArgs(ContentsOpenType openType, string folderPath, string subFolderPath)
        {
            if (folderPath == null)
            {
                throw new ArgumentNullException("folderPath");
            }

            if (subFolderPath == null)
            {
                throw new ArgumentNullException("subFolderPath");
            }

            _openType = openType;
            _folderPath = folderPath;
            _subFolderPath = subFolderPath;
        }
    }
}
