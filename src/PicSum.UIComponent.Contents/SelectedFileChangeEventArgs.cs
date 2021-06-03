using System;
using System.Collections.Generic;

namespace PicSum.UIComponent.Contents
{
    public class SelectedFileChangeEventArgs : EventArgs
    {
        private IList<string> _filePathList = null;

        public IList<string> FilePathList
        {
            get
            {
                return _filePathList;
            }
        }

        public SelectedFileChangeEventArgs(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            _filePathList = new List<string>(new string[] { filePath });
        }

        public SelectedFileChangeEventArgs(IList<string> filePathList)
        {
            if (filePathList == null)
            {
                throw new ArgumentNullException("filePathList");
            }

            _filePathList = filePathList;
        }
    }
}
