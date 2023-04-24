using System.Collections.Generic;
using System.IO;
using PicSum.Core.Base.Conf;
using PicSum.Core.Task.Base;

namespace PicSum.Task.Result
{
    public class GetImageFileByDirectoryResult : IEntity
    {
        public string DirectoryPath { get; set; }
        public IList<string> FilePathList { get; set; }
        public string SelectedFilePath { get; set; }
        public DirectoryNotFoundException DirectoryNotFoundException { get; set; }
        public ContentsOpenType FileOpenType { get; set; }
        public int TabIndex { get; set; }
    }
}
