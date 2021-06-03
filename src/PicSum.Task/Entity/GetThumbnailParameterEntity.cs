using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PicSum.Core.Task.Base;

namespace PicSum.Task.Entity
{
    public class GetThumbnailParameterEntity : IEntity
    {
        public IList<string> FilePathList { get; set; }
        public int FirstIndex { get; set; }
        public int LastIndex { get; set; }
        public int ThumbnailWidth { get; set; }
        public int ThumbnailHeight { get; set; }
    }
}
