using System.Collections.Generic;
using PicSum.Core.Task.Base;

namespace PicSum.Task.Entity
{
    public class ExportFileParameterEntity : IEntity
    {
        public string ExportFolderPath { get; set; }
        public IList<string> FilePathList { get; set; }
    }
}
