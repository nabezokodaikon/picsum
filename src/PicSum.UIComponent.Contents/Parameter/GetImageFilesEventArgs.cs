using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSum.UIComponent.Contents.Parameter
{
    public sealed class GetImageFilesEventArgs
        : EventArgs
    {
        public IList<string> FilePathList { get; private set; }
        public string SelectedFilePath { get; private set; }

        public GetImageFilesEventArgs(IList<string> filePathList, string selectedFilePath)
        {
            this.FilePathList = filePathList ?? throw new ArgumentNullException(nameof(filePathList));
            this.SelectedFilePath = selectedFilePath ?? throw new ArgumentNullException(nameof(selectedFilePath));
        }
    }
}
