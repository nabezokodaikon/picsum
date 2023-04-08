using System;
using System.Collections.Generic;

namespace PicSum.UIComponent.Contents
{
    public class SelectedFileChangeEventArgs : EventArgs
    {
        public IList<string> FilePathList { get; private set; }

        public SelectedFileChangeEventArgs() 
        {
            this.FilePathList = new List<string>();
        }

        public SelectedFileChangeEventArgs(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            this.FilePathList = new List<string>(new string[] { filePath });
        }

        public SelectedFileChangeEventArgs(IList<string> filePathList)
        {
            if (filePathList == null)
            {
                throw new ArgumentNullException("filePathList");
            }

            this.FilePathList = filePathList;
        }
    }
}
