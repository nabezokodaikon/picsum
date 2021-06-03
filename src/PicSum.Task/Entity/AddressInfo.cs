using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PicSum.Core.Task.Base;

namespace PicSum.Task.Entity
{
    public class AddressInfo : IEntity
    {
        public string FolderPath { get; set; }
        public IList<FileShallowInfoEntity> FolderList { get; set; }
        public bool HasSubFolder { get; set; }
    }
}
