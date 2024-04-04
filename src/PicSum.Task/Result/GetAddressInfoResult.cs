using PicSum.Core.Task.Base;
using PicSum.Task.Entity;
using SWF.Common;
using System.Collections.Generic;

namespace PicSum.Task.Result
{
    public sealed class GetAddressInfoResult
        : AbstractResultEntity
    {
        public string DirectoryPath { get; set; }
        public IList<FileShallowInfoEntity> DirectoryList { get; set; }
        public bool HasSubDirectory { get; set; }
    }
}
