using System.Collections.Generic;
using PicSum.Core.Task.Base;

namespace PicSum.Task.Paramter
{
    public class ExportFileParameter : IEntity
    {
        public string ExportDirectoryPath { get; set; }
        public IList<string> FilePathList { get; set; }
    }
}
