using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PicSum.Core.Task.Base;
using SWF.Common;

namespace PicSum.Task.Entity
{
    public class AddressInfo : IEntity
    {
        public string DirectoryPath { get; set; }
        public IList<FileShallowInfoEntity> DirectoryList { get; set; }
        public bool HasSubDirectory { get; set; }
        public FileUtilException GetAddressInfoException { get; set; }
    }
}
