using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PicSum.Core.Data.FileAccessor;
using PicSum.Core.Task.Base;
using SWF.Common;

namespace PicSum.Task.Entity
{
    public class AddressInfo : IEntity
    {
        public string FolderPath { get; set; }
        public IList<FileShallowInfoEntity> FolderList { get; set; }
        public bool HasSubFolder { get; set; }
        public FileException GetAddressInfoException { get; set; }
    }
}
