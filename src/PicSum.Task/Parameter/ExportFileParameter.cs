using PicSum.Core.Task.Base;
using System.Collections.Generic;

namespace PicSum.Task.Paramter
{
    public sealed class ExportFileParameter
        : IEntity
    {
        public string SrcFilePath { get; set; }
        public string ExportFilePath { get; set; }
    }
}
